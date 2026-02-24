# Example: Performance Trend Analysis Demo
# This script demonstrates common trend analysis scenarios

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Performance Trend Analysis Examples" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$reportsPath = "$env:USERPROFILE\ACATTalk_PerformanceReports"

# Check if reports exist
if (-not (Test-Path $reportsPath)) {
    Write-Host "⚠ No reports found. Generate some reports first:" -ForegroundColor Yellow
    Write-Host "  1. Run: .\scripts\Build-Performance.ps1 -Run" -ForegroundColor White
    Write-Host "  2. Use ACATTalk for a few minutes" -ForegroundColor White
    Write-Host "  3. Exit ACATTalk normally" -ForegroundColor White
    Write-Host "  4. Repeat 3-5 times to generate multiple reports" -ForegroundColor White
    exit 0
}

$reportCount = (Get-ChildItem -Path $reportsPath -Filter "*.csv").Count

if ($reportCount -lt 2) {
    Write-Host "⚠ Found only $reportCount report(s). Need at least 2 for trend analysis." -ForegroundColor Yellow
    Write-Host "  Generate more reports by running ACATTalk multiple times." -ForegroundColor White
    exit 0
}

Write-Host "✓ Found $reportCount reports in $reportsPath" -ForegroundColor Green
Write-Host ""

# Scenario 1: Basic trend analysis
Write-Host "Example 1: Basic Trend Analysis (Last 5 Reports)" -ForegroundColor Cyan
Write-Host "-" * 80 -ForegroundColor Gray
Write-Host "Command: .\scripts\Analyze-PerformanceTrends.ps1 -LastN 5" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press Enter to run..." -ForegroundColor Gray
$null = Read-Host

.\scripts\Analyze-PerformanceTrends.ps1 -LastN 5

Write-Host ""
Write-Host "Press Enter for next example..." -ForegroundColor Gray
$null = Read-Host

# Scenario 2: With statistics
Write-Host ""
Write-Host "Example 2: Detailed Statistics" -ForegroundColor Cyan
Write-Host "-" * 80 -ForegroundColor Gray
Write-Host "Command: .\scripts\Analyze-PerformanceTrends.ps1 -LastN 5 -IncludeStatistics" -ForegroundColor Yellow
Write-Host ""
Write-Host "Shows min, max, average, and standard deviation for each metric" -ForegroundColor Gray
Write-Host "Press Enter to run..." -ForegroundColor Gray
$null = Read-Host

.\scripts\Analyze-PerformanceTrends.ps1 -LastN 5 -IncludeStatistics

Write-Host ""
Write-Host "Press Enter for next example..." -ForegroundColor Gray
$null = Read-Host

# Scenario 3: Visual charts
Write-Host ""
Write-Host "Example 3: Visual Trend Charts" -ForegroundColor Cyan
Write-Host "-" * 80 -ForegroundColor Gray
Write-Host "Command: .\scripts\Analyze-PerformanceTrends.ps1 -LastN 5 -ShowChart" -ForegroundColor Yellow
Write-Host ""
Write-Host "Displays ASCII charts showing trends over time" -ForegroundColor Gray
Write-Host "Press Enter to run..." -ForegroundColor Gray
$null = Read-Host

.\scripts\Analyze-PerformanceTrends.ps1 -LastN 5 -ShowChart

Write-Host ""
Write-Host "Press Enter for next example..." -ForegroundColor Gray
$null = Read-Host

# Scenario 4: Specific metrics
Write-Host ""
Write-Host "Example 4: Focus on Specific Metrics" -ForegroundColor Cyan
Write-Host "-" * 80 -ForegroundColor Gray
Write-Host "Command: .\scripts\Analyze-PerformanceTrends.ps1 -Metrics @('TotalStartupTime','PeakMemoryUsage') -ShowChart" -ForegroundColor Yellow
Write-Host ""
Write-Host "Analyze only the metrics you care about" -ForegroundColor Gray
Write-Host "Press Enter to run..." -ForegroundColor Gray
$null = Read-Host

.\scripts\Analyze-PerformanceTrends.ps1 -Metrics @('TotalStartupTime','PeakMemoryUsage') -ShowChart

Write-Host ""
Write-Host "Press Enter for final example..." -ForegroundColor Gray
$null = Read-Host

# Scenario 5: Full export
Write-Host ""
Write-Host "Example 5: Export to HTML and CSV" -ForegroundColor Cyan
Write-Host "-" * 80 -ForegroundColor Gray
Write-Host "Command: .\scripts\Analyze-PerformanceTrends.ps1 -LastN 10 -ExportHtml -ExportCsv" -ForegroundColor Yellow
Write-Host ""
Write-Host "Creates shareable HTML report and CSV for further analysis" -ForegroundColor Gray
Write-Host "Press Enter to run..." -ForegroundColor Gray
$null = Read-Host

.\scripts\Analyze-PerformanceTrends.ps1 -LastN 10 -ExportHtml -ExportCsv

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Demo Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  • Review the exported HTML report" -ForegroundColor White
Write-Host "  • Try running with different parameters" -ForegroundColor White
Write-Host "  • See PERFORMANCE_TRENDS_GUIDE.md for more scenarios" -ForegroundColor White
Write-Host ""
