# Analyze ACATTalk Performance Reports
# This script analyzes and compares performance reports

param(
    [Parameter(Mandatory=$false)]
    [string]$ReportsPath = "$env:USERPROFILE\ACATTalk_PerformanceReports",
    
    [Parameter(Mandatory=$false)]
    [int]$TopN = 5,
    
    [Parameter(Mandatory=$false)]
    [switch]$Compare,
    
    [Parameter(Mandatory=$false)]
    [switch]$Export
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ACATTalk Performance Report Analyzer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if reports directory exists
if (-not (Test-Path $ReportsPath)) {
    Write-Warning "Reports directory not found: $ReportsPath"
    Write-Host "No performance reports have been generated yet." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To generate reports:" -ForegroundColor Yellow
    Write-Host "  1. Build ACATTalk with PERFORMANCE symbol defined" -ForegroundColor White
    Write-Host "  2. Run ACATTalk" -ForegroundColor White
    Write-Host "  3. Exit ACATTalk normally" -ForegroundColor White
    Write-Host ""
    Write-Host "Or use: .\scripts\Build-Performance.ps1 -Run" -ForegroundColor Yellow
    exit 0
}

# Get all CSV reports
$csvFiles = Get-ChildItem -Path $ReportsPath -Filter "*.csv" | Sort-Object LastWriteTime -Descending

if ($csvFiles.Count -eq 0) {
    Write-Warning "No CSV reports found in: $ReportsPath"
    exit 0
}

Write-Host "Found $($csvFiles.Count) report(s)" -ForegroundColor Green
Write-Host ""

# Show latest report
$latestReport = $csvFiles[0]
Write-Host "Latest Report: $($latestReport.Name)" -ForegroundColor Cyan
Write-Host "Generated: $($latestReport.LastWriteTime)" -ForegroundColor Gray
Write-Host ""

# Import and analyze latest report
$data = Import-Csv $latestReport.FullName

# Group by category
$categories = $data | Group-Object Category

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Performance Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

foreach ($category in $categories) {
    Write-Host "[$($category.Name)]" -ForegroundColor Yellow
    
    $metrics = $category.Group | Sort-Object { [double]$_.Value } -Descending
    
    foreach ($metric in $metrics) {
        $value = [double]$metric.Value
        $name = $metric.Metric.PadRight(40)
        
        # Color code based on value ranges (customize as needed)
        $color = "White"
        if ($metric.Category -eq "Startup" -and $value -gt 1000) { $color = "Red" }
        elseif ($metric.Category -eq "Startup" -and $value -gt 500) { $color = "Yellow" }
        elseif ($metric.Category -eq "Memory" -and $value -gt 200) { $color = "Yellow" }
        
        Write-Host "  $name" -NoNewline
        Write-Host ("{0,10:F2} {1}" -f $value, $metric.Unit) -ForegroundColor $color
    }
    Write-Host ""
}

# Show top slowest operations
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Top $TopN Slowest Operations" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$slowest = $data | 
    Where-Object { $_.Unit -eq "ms" } | 
    Sort-Object { [double]$_.Value } -Descending | 
    Select-Object -First $TopN

foreach ($item in $slowest) {
    $value = [double]$item.Value
    Write-Host ("{0,-40} {1,10:F2} ms" -f $item.Metric, $value) -ForegroundColor $(if ($value -gt 1000) { "Red" } elseif ($value -gt 500) { "Yellow" } else { "White" })
}
Write-Host ""

# Memory analysis
$memoryMetrics = $data | Where-Object { $_.Category -eq "Memory" }
if ($memoryMetrics) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Memory Analysis" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    $startMem = [double]($memoryMetrics | Where-Object Metric -eq "StartMemoryUsage" | Select-Object -ExpandProperty Value)
    $peakMem = [double]($memoryMetrics | Where-Object Metric -eq "PeakMemoryUsage" | Select-Object -ExpandProperty Value)
    $endMem = [double]($memoryMetrics | Where-Object Metric -eq "EndMemoryUsage" | Select-Object -ExpandProperty Value)
    $growth = [double]($memoryMetrics | Where-Object Metric -eq "MemoryGrowth" | Select-Object -ExpandProperty Value)
    
    Write-Host ("  Start Memory:    {0,10:F2} MB" -f $startMem) -ForegroundColor White
    Write-Host ("  Peak Memory:     {0,10:F2} MB" -f $peakMem) -ForegroundColor $(if ($peakMem -gt 500) { "Red" } elseif ($peakMem -gt 200) { "Yellow" } else { "Green" })
    Write-Host ("  End Memory:      {0,10:F2} MB" -f $endMem) -ForegroundColor White
    Write-Host ("  Memory Growth:   {0,10:F2} MB" -f $growth) -ForegroundColor $(if ($growth -gt 100) { "Red" } elseif ($growth -gt 50) { "Yellow" } else { "Green" })
    
    $growthPercent = ($growth / $startMem) * 100
    Write-Host ("  Growth Percent:  {0,10:F2} %" -f $growthPercent) -ForegroundColor $(if ($growthPercent -gt 200) { "Red" } elseif ($growthPercent -gt 100) { "Yellow" } else { "Green" })
    Write-Host ""
}

# Compare mode
if ($Compare -and $csvFiles.Count -ge 2) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Comparison with Previous Run" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    $previousReport = $csvFiles[1]
    Write-Host "Comparing to: $($previousReport.Name)" -ForegroundColor Gray
    Write-Host ""
    
    $previousData = Import-Csv $previousReport.FullName
    
    # Compare key metrics
    $keyMetrics = @("TotalStartupTime", "ContextInit", "PeakMemoryUsage", "MemoryGrowth")
    
    foreach ($metricName in $keyMetrics) {
        $current = [double]($data | Where-Object Metric -eq $metricName | Select-Object -ExpandProperty Value)
        $previous = [double]($previousData | Where-Object Metric -eq $metricName | Select-Object -ExpandProperty Value)
        $unit = ($data | Where-Object Metric -eq $metricName | Select-Object -ExpandProperty Unit)
        
        if ($current -and $previous) {
            $diff = $current - $previous
            $diffPercent = ($diff / $previous) * 100
            
            $color = "White"
            $indicator = "="
            if ($diff -lt -1) { 
                $color = "Green"
                $indicator = "▼"
            } 
            elseif ($diff -gt 1) { 
                $color = "Red"
                $indicator = "▲"
            }
            
            Write-Host ("{0,-30}" -f $metricName) -NoNewline
            Write-Host ("{0} " -f $indicator) -NoNewline -ForegroundColor $color
            Write-Host ("{0,10:F2} {1} " -f $current, $unit) -NoNewline
            Write-Host ("({0:+0.00;-0.00} {1}, {2:+0.0;-0.0}%)" -f $diff, $unit, $diffPercent) -ForegroundColor $color
        }
    }
    Write-Host ""
}

# Export summary
if ($Export) {
    $exportPath = Join-Path $ReportsPath "Summary_$(Get-Date -Format 'yyyyMMdd_HHmmss').txt"
    
    $summary = @"
ACATTalk Performance Summary
Generated: $(Get-Date)
Report: $($latestReport.Name)

KEY METRICS
-----------
Total Startup Time: $(($data | Where-Object Metric -eq "TotalStartupTime" | Select-Object -ExpandProperty Value)) ms
Context Init Time: $(($data | Where-Object Metric -eq "ContextInit" | Select-Object -ExpandProperty Value)) ms
Peak Memory Usage: $(($data | Where-Object Metric -eq "PeakMemoryUsage" | Select-Object -ExpandProperty Value)) MB
Memory Growth: $(($data | Where-Object Metric -eq "MemoryGrowth" | Select-Object -ExpandProperty Value)) MB

TOP 5 SLOWEST OPERATIONS
-------------------------
$($slowest | ForEach-Object { "$($_.Metric): $($_.Value) $($_.Unit)" } | Out-String)

FULL REPORT
-----------
$($data | Format-Table -AutoSize | Out-String)
"@
    
    $summary | Out-File -FilePath $exportPath -Encoding UTF8
    Write-Host "Summary exported to: $exportPath" -ForegroundColor Green
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Analysis Complete" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Options:" -ForegroundColor Yellow
Write-Host "  View all reports:  Get-ChildItem '$ReportsPath'" -ForegroundColor White
Write-Host "  Compare runs:      .\scripts\Analyze-Performance.ps1 -Compare" -ForegroundColor White
Write-Host "  Export summary:    .\scripts\Analyze-Performance.ps1 -Export" -ForegroundColor White
