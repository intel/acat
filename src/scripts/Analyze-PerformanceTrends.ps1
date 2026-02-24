# Analyze ACATTalk Performance Trends Across Multiple Reports
# This script analyzes trends across multiple performance report files

param(
    [Parameter(Mandatory=$false)]
    [string]$ReportsPath = "$env:USERPROFILE\ACATTalk_PerformanceReports",
    
    [Parameter(Mandatory=$false)]
    [int]$LastN = 10,
    
    [Parameter(Mandatory=$false)]
    [string[]]$Files,
    
    [Parameter(Mandatory=$false)]
    [string[]]$Metrics,
    
    [Parameter(Mandatory=$false)]
    [switch]$ShowChart,
    
    [Parameter(Mandatory=$false)]
    [switch]$ExportHtml,
    
    [Parameter(Mandatory=$false)]
    [switch]$ExportCsv,
    
    [Parameter(Mandatory=$false)]
    [switch]$IncludeStatistics,
    
    [Parameter(Mandatory=$false)]
    [int]$ChartWidth = 80,
    
    [Parameter(Mandatory=$false)]
    [int]$ChartHeight = 15
)

$ErrorActionPreference = "Stop"

# Helper function to calculate statistics
function Get-Statistics {
    param([double[]]$Values)
    
    if ($Values.Count -eq 0) {
        return $null
    }
    
    $sorted = $Values | Sort-Object
    $count = $sorted.Count
    $sum = ($sorted | Measure-Object -Sum).Sum
    $avg = $sum / $count
    $min = $sorted[0]
    $max = $sorted[-1]
    
    # Median
    if ($count % 2 -eq 0) {
        $median = ($sorted[$count/2 - 1] + $sorted[$count/2]) / 2
    } else {
        $median = $sorted[[Math]::Floor($count/2)]
    }
    
    # Standard deviation
    $variance = ($sorted | ForEach-Object { [Math]::Pow($_ - $avg, 2) } | Measure-Object -Average).Average
    $stdDev = [Math]::Sqrt($variance)
    
    return @{
        Count = $count
        Min = $min
        Max = $max
        Average = $avg
        Median = $median
        StdDev = $stdDev
        Sum = $sum
    }
}

# Helper function to draw ASCII chart
function Show-TrendChart {
    param(
        [string]$Title,
        [array]$DataPoints,
        [int]$Width = 80,
        [int]$Height = 15
    )
    
    if ($DataPoints.Count -eq 0) {
        Write-Host "No data to chart" -ForegroundColor Yellow
        return
    }
    
    Write-Host ""
    Write-Host "Chart: $Title" -ForegroundColor Cyan
    Write-Host ("-" * $Width) -ForegroundColor Gray
    
    $values = $DataPoints | ForEach-Object { [double]$_.Value }
    $min = ($values | Measure-Object -Minimum).Minimum
    $max = ($values | Measure-Object -Maximum).Maximum
    $range = $max - $min
    
    if ($range -eq 0) {
        Write-Host "All values are identical: $min" -ForegroundColor Yellow
        return
    }
    
    # Normalize values to chart height
    $normalized = $values | ForEach-Object {
        [int](($_ - $min) / $range * ($Height - 1))
    }
    
    # Draw chart from top to bottom
    for ($y = $Height - 1; $y -ge 0; $y--) {
        $line = ""
        $yValue = $min + ($y / ($Height - 1)) * $range
        
        # Y-axis label
        $line += ("{0,8:F1} |" -f $yValue)
        
        # Plot points
        for ($x = 0; $x -lt $normalized.Count; $x++) {
            $barWidth = [Math]::Max(1, [Math]::Floor($Width / $normalized.Count))
            
            if ($normalized[$x] -eq $y) {
                $line += "●" * $barWidth
            } elseif ($normalized[$x] -gt $y) {
                $line += "│" * $barWidth
            } else {
                $line += " " * $barWidth
            }
        }
        
        # Color code based on height
        $color = "White"
        if ($y -gt $Height * 0.66) { $color = "Red" }
        elseif ($y -gt $Height * 0.33) { $color = "Yellow" }
        else { $color = "Green" }
        
        Write-Host $line -ForegroundColor $color
    }
    
    # X-axis
    Write-Host (" " * 10 + ("-" * ($Width - 10))) -ForegroundColor Gray
    Write-Host (" " * 10 + "Oldest" + (" " * ($Width - 25)) + "Latest") -ForegroundColor Gray
    Write-Host ""
}

# Helper function to get trend indicator
function Get-TrendIndicator {
    param([double]$First, [double]$Last)
    
    $change = (($Last - $First) / $First) * 100
    
    if ($change -lt -5) {
        return @{ Symbol = "▼▼"; Color = "Green"; Text = "Improving" }
    } elseif ($change -lt -1) {
        return @{ Symbol = "▼"; Color = "Green"; Text = "Slight Improvement" }
    } elseif ($change -gt 5) {
        return @{ Symbol = "▲▲"; Color = "Red"; Text = "Degrading" }
    } elseif ($change -gt 1) {
        return @{ Symbol = "▲"; Color = "Yellow"; Text = "Slight Degradation" }
    } else {
        return @{ Symbol = "═"; Color = "White"; Text = "Stable" }
    }
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ACATTalk Performance Trend Analyzer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if reports directory exists
if (-not (Test-Path $ReportsPath)) {
    Write-Warning "Reports directory not found: $ReportsPath"
    Write-Host "No performance reports have been generated yet." -ForegroundColor Yellow
    exit 0
}

# Determine which files to analyze
$csvFiles = @()

if ($Files) {
    # Use explicitly specified files
    foreach ($file in $Files) {
        if (Test-Path $file) {
            $csvFiles += Get-Item $file
        } elseif (Test-Path (Join-Path $ReportsPath $file)) {
            $csvFiles += Get-Item (Join-Path $ReportsPath $file)
        } else {
            Write-Warning "File not found: $file"
        }
    }
} else {
    # Get last N reports
    $csvFiles = Get-ChildItem -Path $ReportsPath -Filter "*.csv" | 
        Sort-Object LastWriteTime -Descending | 
        Select-Object -First $LastN |
        Sort-Object LastWriteTime  # Re-sort chronologically for trend analysis
}

if ($csvFiles.Count -eq 0) {
    Write-Warning "No CSV reports found"
    exit 0
}

if ($csvFiles.Count -lt 2) {
    Write-Warning "Need at least 2 reports for trend analysis. Found: $($csvFiles.Count)"
    Write-Host "Use .\scripts\Analyze-Performance.ps1 for single report analysis" -ForegroundColor Yellow
    exit 0
}

Write-Host "Analyzing $($csvFiles.Count) report(s)" -ForegroundColor Green
Write-Host "Date Range: $($csvFiles[0].LastWriteTime.ToString('yyyy-MM-dd HH:mm')) to $($csvFiles[-1].LastWriteTime.ToString('yyyy-MM-dd HH:mm'))" -ForegroundColor Gray
Write-Host ""

# Import all reports
$allData = @()
$reportIndex = 0

foreach ($file in $csvFiles) {
    try {
        $reportData = Import-Csv $file.FullName
        foreach ($row in $reportData) {
            $row | Add-Member -NotePropertyName "ReportIndex" -NotePropertyValue $reportIndex
            $row | Add-Member -NotePropertyName "ReportFile" -NotePropertyValue $file.Name
            $row | Add-Member -NotePropertyName "ReportTime" -NotePropertyValue $file.LastWriteTime
            $allData += $row
        }
        $reportIndex++
    } catch {
        Write-Warning "Failed to import: $($file.Name) - $($_.Exception.Message)"
    }
}

Write-Host "Loaded $($allData.Count) total metrics from $reportIndex reports" -ForegroundColor Green
Write-Host ""

# Determine which metrics to analyze
if ($Metrics) {
    $metricsToAnalyze = $Metrics
} else {
    # Default key metrics
    $metricsToAnalyze = @(
        "TotalStartupTime",
        "ContextInit",
        "PeakMemoryUsage",
        "MemoryGrowth",
        "TotalApplicationLifetime"
    )
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Performance Trends" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$trendResults = @()

foreach ($metricName in $metricsToAnalyze) {
    $metricData = $allData | Where-Object { $_.Metric -eq $metricName } | Sort-Object ReportIndex
    
    if ($metricData.Count -eq 0) {
        Write-Host "$metricName" -ForegroundColor Yellow -NoNewline
        Write-Host " - Not found in reports" -ForegroundColor Gray
        continue
    }
    
    $values = $metricData | ForEach-Object { [double]$_.Value }
    $unit = $metricData[0].Unit
    $category = $metricData[0].Category
    
    $stats = Get-Statistics -Values $values
    $first = $values[0]
    $last = $values[-1]
    $trend = Get-TrendIndicator -First $first -Last $last
    
    # Display metric name and trend
    Write-Host ("{0,-35}" -f $metricName) -NoNewline
    Write-Host (" {0} " -f $trend.Symbol) -NoNewline -ForegroundColor $trend.Color
    Write-Host ("{0,10:F2} {1} → {2,10:F2} {3}" -f $first, $unit, $last, $unit) -NoNewline
    
    $change = $last - $first
    $changePercent = ($change / $first) * 100
    Write-Host ("  ({0:+0.00;-0.00}, {1:+0.0;-0.0}%)" -f $change, $changePercent) -ForegroundColor $trend.Color
    
    if ($IncludeStatistics) {
        Write-Host ("    Min: {0,8:F2} {1}  Max: {2,8:F2} {3}  Avg: {4,8:F2} {5}  StdDev: {6,8:F2}" -f `
            $stats.Min, $unit, $stats.Max, $unit, $stats.Average, $unit, $stats.StdDev) -ForegroundColor Gray
    }
    
    # Store for later use
    $trendResults += [PSCustomObject]@{
        Metric = $metricName
        Category = $category
        Unit = $unit
        First = $first
        Last = $last
        Change = $change
        ChangePercent = $changePercent
        Trend = $trend.Text
        Min = $stats.Min
        Max = $stats.Max
        Average = $stats.Average
        Median = $stats.Median
        StdDev = $stats.StdDev
        DataPoints = $metricData
    }
    
    # Show chart if requested
    if ($ShowChart) {
        Show-TrendChart -Title "$metricName ($unit)" -DataPoints $metricData -Width $ChartWidth -Height $ChartHeight
    }
}

Write-Host ""

# Overall health assessment
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Trend Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$improving = ($trendResults | Where-Object { $_.ChangePercent -lt -5 }).Count
$degrading = ($trendResults | Where-Object { $_.ChangePercent -gt 5 }).Count
$stable = ($trendResults | Where-Object { $_.ChangePercent -ge -5 -and $_.ChangePercent -le 5 }).Count

Write-Host "  Improving Metrics:  " -NoNewline
Write-Host ("{0,3}" -f $improving) -ForegroundColor Green
Write-Host "  Degrading Metrics:  " -NoNewline
Write-Host ("{0,3}" -f $degrading) -ForegroundColor Red
Write-Host "  Stable Metrics:     " -NoNewline
Write-Host ("{0,3}" -f $stable) -ForegroundColor White
Write-Host ""

# Show most improved and most degraded
if ($trendResults.Count -gt 0) {
    Write-Host "Most Improved:" -ForegroundColor Green
    $mostImproved = $trendResults | Sort-Object ChangePercent | Select-Object -First 3
    foreach ($item in $mostImproved) {
        Write-Host ("  {0,-35} {1,8:F1}% improvement" -f $item.Metric, [Math]::Abs($item.ChangePercent)) -ForegroundColor Green
    }
    Write-Host ""
    
    Write-Host "Most Degraded:" -ForegroundColor Red
    $mostDegraded = $trendResults | Sort-Object ChangePercent -Descending | Select-Object -First 3
    foreach ($item in $mostDegraded) {
        if ($item.ChangePercent -gt 0) {
            Write-Host ("  {0,-35} {1,8:F1}% degradation" -f $item.Metric, $item.ChangePercent) -ForegroundColor Red
        }
    }
    Write-Host ""
}

# Category-based trend analysis
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Trends by Category" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$categoryGroups = $trendResults | Group-Object Category

foreach ($catGroup in $categoryGroups) {
    Write-Host "[$($catGroup.Name)]" -ForegroundColor Yellow
    
    $avgChange = ($catGroup.Group | Measure-Object -Property ChangePercent -Average).Average
    $catTrend = Get-TrendIndicator -First 100 -Last (100 + $avgChange)
    
    Write-Host ("  Overall Trend: {0} {1} ({2:F1}% average change)" -f $catTrend.Symbol, $catTrend.Text, $avgChange) -ForegroundColor $catTrend.Color
    Write-Host ("  Metrics: {0}" -f $catGroup.Count) -ForegroundColor Gray
    Write-Host ""
}

# Memory trend analysis
$memoryMetrics = $trendResults | Where-Object { $_.Category -eq "Memory" }
if ($memoryMetrics.Count -gt 0) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Memory Trends" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    foreach ($mem in $memoryMetrics) {
        Write-Host ("{0,-30}" -f $mem.Metric) -NoNewline
        Write-Host (" First: {0,8:F2} MB  Last: {1,8:F2} MB  Change: {2,8:F2} MB ({3:+0.0;-0.0}%)" -f `
            $mem.First, $mem.Last, $mem.Change, $mem.ChangePercent) -ForegroundColor $(
                if ($mem.ChangePercent -gt 10) { "Red" } 
                elseif ($mem.ChangePercent -gt 5) { "Yellow" } 
                elseif ($mem.ChangePercent -lt -5) { "Green" }
                else { "White" }
            )
    }
    Write-Host ""
}

# Volatility analysis - identify unstable metrics
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Volatility Analysis" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$volatileMetrics = $trendResults | 
    Where-Object { $_.Average -gt 0 } |
    ForEach-Object {
        $_ | Add-Member -NotePropertyName "CV" -NotePropertyValue ($_.StdDev / $_.Average * 100) -PassThru
    } |
    Sort-Object CV -Descending |
    Select-Object -First 5

Write-Host "Top 5 Most Volatile Metrics (Coefficient of Variation):" -ForegroundColor Yellow
foreach ($metric in $volatileMetrics) {
    $color = if ($metric.CV -gt 30) { "Red" } elseif ($metric.CV -gt 15) { "Yellow" } else { "White" }
    Write-Host ("  {0,-35} CV: {1,6:F1}%  (StdDev: {2,8:F2} {3})" -f $metric.Metric, $metric.CV, $metric.StdDev, $metric.Unit) -ForegroundColor $color
}
Write-Host ""

# Export to HTML
if ($ExportHtml) {
    $htmlPath = Join-Path $ReportsPath "PerformanceTrends_$(Get-Date -Format 'yyyyMMdd_HHmmss').html"
    
    $html = @"
<!DOCTYPE html>
<html>
<head>
    <title>ACATTalk Performance Trends</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; background-color: #f5f5f5; }
        h1 { color: #0078d4; }
        h2 { color: #106ebe; margin-top: 30px; }
        table { border-collapse: collapse; width: 100%; background-color: white; margin: 10px 0; }
        th { background-color: #0078d4; color: white; padding: 10px; text-align: left; }
        td { padding: 8px; border-bottom: 1px solid #ddd; }
        tr:hover { background-color: #f0f0f0; }
        .improving { color: #107c10; font-weight: bold; }
        .degrading { color: #d13438; font-weight: bold; }
        .stable { color: #605e5c; }
        .summary { background-color: white; padding: 15px; margin: 10px 0; border-radius: 5px; }
        .metric-card { display: inline-block; background-color: white; padding: 15px; margin: 10px; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); min-width: 200px; }
    </style>
</head>
<body>
    <h1>ACATTalk Performance Trends</h1>
    <div class="summary">
        <p><strong>Analysis Date:</strong> $(Get-Date)</p>
        <p><strong>Reports Analyzed:</strong> $($csvFiles.Count)</p>
        <p><strong>Date Range:</strong> $($csvFiles[0].LastWriteTime.ToString('yyyy-MM-dd HH:mm')) to $($csvFiles[-1].LastWriteTime.ToString('yyyy-MM-dd HH:mm'))</p>
    </div>
    
    <div class="summary">
        <div class="metric-card">
            <h3 style="color: #107c10; margin: 0;">Improving</h3>
            <p style="font-size: 32px; margin: 10px 0;">$improving</p>
        </div>
        <div class="metric-card">
            <h3 style="color: #d13438; margin: 0;">Degrading</h3>
            <p style="font-size: 32px; margin: 10px 0;">$degrading</p>
        </div>
        <div class="metric-card">
            <h3 style="color: #605e5c; margin: 0;">Stable</h3>
            <p style="font-size: 32px; margin: 10px 0;">$stable</p>
        </div>
    </div>
    
    <h2>Detailed Trends</h2>
    <table>
        <tr>
            <th>Metric</th>
            <th>Category</th>
            <th>First Value</th>
            <th>Last Value</th>
            <th>Change</th>
            <th>Trend</th>
            <th>Min</th>
            <th>Max</th>
            <th>Average</th>
            <th>StdDev</th>
        </tr>
"@

    foreach ($result in ($trendResults | Sort-Object Category, Metric)) {
        $trendClass = if ($result.ChangePercent -lt -5) { "improving" } 
                      elseif ($result.ChangePercent -gt 5) { "degrading" } 
                      else { "stable" }
        
        $html += @"
        <tr>
            <td>$($result.Metric)</td>
            <td>$($result.Category)</td>
            <td>$($result.First.ToString("F2")) $($result.Unit)</td>
            <td>$($result.Last.ToString("F2")) $($result.Unit)</td>
            <td class="$trendClass">$($result.Change.ToString("+0.00;-0.00")) $($result.Unit) ($($result.ChangePercent.ToString("+0.0;-0.0"))%)</td>
            <td class="$trendClass">$($result.Trend)</td>
            <td>$($result.Min.ToString("F2")) $($result.Unit)</td>
            <td>$($result.Max.ToString("F2")) $($result.Unit)</td>
            <td>$($result.Average.ToString("F2")) $($result.Unit)</td>
            <td>$($result.StdDev.ToString("F2"))</td>
        </tr>
"@
    }
    
    $html += @"
    </table>
    
    <h2>Report Files Analyzed</h2>
    <ul>
"@

    foreach ($file in $csvFiles) {
        $html += "        <li>$($file.Name) - $($file.LastWriteTime.ToString('yyyy-MM-dd HH:mm:ss'))</li>`n"
    }
    
    $html += @"
    </ul>
</body>
</html>
"@
    
    $html | Out-File -FilePath $htmlPath -Encoding UTF8
    Write-Host "HTML report exported to: $htmlPath" -ForegroundColor Green
    Write-Host ""
}

# Export aggregated CSV
if ($ExportCsv) {
    $csvExportPath = Join-Path $ReportsPath "TrendSummary_$(Get-Date -Format 'yyyyMMdd_HHmmss').csv"
    
    $trendResults | Select-Object Metric, Category, Unit, First, Last, Change, ChangePercent, Trend, Min, Max, Average, Median, StdDev |
        Export-Csv -Path $csvExportPath -NoTypeInformation
    
    Write-Host "Trend summary CSV exported to: $csvExportPath" -ForegroundColor Green
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Analysis Complete" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Usage Examples:" -ForegroundColor Yellow
Write-Host "  Show charts:           .\scripts\Analyze-PerformanceTrends.ps1 -ShowChart" -ForegroundColor White
Write-Host "  Last 20 reports:       .\scripts\Analyze-PerformanceTrends.ps1 -LastN 20" -ForegroundColor White
Write-Host "  Specific metrics:      .\scripts\Analyze-PerformanceTrends.ps1 -Metrics @('TotalStartupTime','PeakMemoryUsage')" -ForegroundColor White
Write-Host "  With statistics:       .\scripts\Analyze-PerformanceTrends.ps1 -IncludeStatistics" -ForegroundColor White
Write-Host "  Export HTML:           .\scripts\Analyze-PerformanceTrends.ps1 -ExportHtml" -ForegroundColor White
Write-Host "  Export CSV:            .\scripts\Analyze-PerformanceTrends.ps1 -ExportCsv" -ForegroundColor White
Write-Host "  All features:          .\scripts\Analyze-PerformanceTrends.ps1 -ShowChart -IncludeStatistics -ExportHtml" -ForegroundColor White
Write-Host ""
