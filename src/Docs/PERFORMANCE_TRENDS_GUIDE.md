# Performance Trend Analysis - Quick Reference

## Overview

The performance trend analysis feature allows you to compare multiple performance report files over time to identify patterns, improvements, and regressions in ACATTalk's performance metrics.

## Key Capabilities

### 1. Multi-Report Comparison
- Analyze 2 to N reports simultaneously
- Automatic chronological ordering
- Support for specific file selection or "last N" mode

### 2. Trend Detection
- **Improving (▼▼)**: >5% improvement
- **Slight Improvement (▼)**: 1-5% improvement  
- **Stable (═)**: ±1% change
- **Slight Degradation (▲)**: 1-5% degradation
- **Degrading (▲▲)**: >5% degradation

### 3. Statistical Analysis
- **Count**: Number of measurements
- **Min/Max**: Range of values
- **Average**: Mean value across all reports
- **Median**: Middle value (resistant to outliers)
- **StdDev**: Standard deviation (measure of variability)
- **CV**: Coefficient of Variation (volatility indicator)

### 4. Visualizations
- ASCII charts showing trends over time
- Color-coded indicators (Red=degrading, Yellow=caution, Green=improving)
- HTML export with interactive tables
- CSV export for Excel/data tools

## Common Use Cases

### Use Case 1: Weekly Performance Review
Check overall application health trends:
```powershell
cd src
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 20 -ShowChart -ExportHtml
```
**What to look for:**
- Are startup times increasing?
- Is memory usage growing over time?
- Are there any highly volatile metrics?

### Use Case 2: Before/After Code Change
Measure impact of your optimization:
```powershell
# Before changes: Run 3-5 times to establish baseline
.\scripts\Build-Performance.ps1 -Run

# Make your code changes

# After changes: Run 3-5 times
.\scripts\Build-Performance.ps1 -Run

# Compare all runs
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 10 -ShowChart -IncludeStatistics
```
**What to look for:**
- Did your target metric improve?
- Did you accidentally degrade other metrics?
- Is the improvement consistent or variable?

### Use Case 3: Regression Investigation
Something seems slower - find out what changed:
```powershell
# Analyze longer history
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 30 -IncludeStatistics

# Focus on specific problem areas
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 30 -Metrics @('TotalStartupTime','ContextInit','UICreation') -ShowChart
```
**What to look for:**
- When did the degradation start? (correlate with report timestamps)
- Which specific metrics are affected?
- Is it a gradual trend or sudden spike?

### Use Case 4: Release Comparison
Compare performance across versions:
```powershell
# Get reports from different time periods
$oldVersionReports = Get-ChildItem "$env:USERPROFILE\ACATTalk_PerformanceReports\backup_v1.0" -Filter "*.csv"
$newVersionReports = Get-ChildItem "$env:USERPROFILE\ACATTalk_PerformanceReports\backup_v2.0" -Filter "*.csv"

# Analyze each version
.\scripts\Analyze-PerformanceTrends.ps1 -Files $oldVersionReports.FullName -ExportHtml
.\scripts\Analyze-PerformanceTrends.ps1 -Files $newVersionReports.FullName -ExportHtml
```
**What to look for:**
- Overall trend direction in each version
- Compare the exported HTML files side-by-side
- Focus on average values and stability (StdDev)

### Use Case 5: CI/CD Performance Tracking
Integrate into automated builds:
```powershell
# In your CI/CD pipeline, after running automated tests
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 50 -ExportCsv -ExportHtml

# Upload artifacts
# - PerformanceTrends_YYYYMMDD_HHmmss.html
# - TrendSummary_YYYYMMDD_HHmmss.csv

# Set build warning/failure based on trends
# (parse CSV and check for degradation thresholds)
```

## Understanding the Output

### Console Output

```
========================================
ACATTalk Performance Trend Analyzer
========================================

Analyzing 10 report(s)
Date Range: 2024-01-15 10:00 to 2024-01-20 15:30

========================================
Performance Trends
========================================

TotalStartupTime                    ▼   2500.00 ms →   2350.00 ms  (-150.00, -6.0%)
ContextInit                         ═    450.00 ms →    455.00 ms  (+5.00, +1.1%)
PeakMemoryUsage                     ▲▲   180.00 MB →    195.00 MB  (+15.00, +8.3%)
```

**Interpreting Results:**
- **Symbol**: Visual trend indicator
- **First Value**: Oldest report in selection
- **Last Value**: Newest report in selection  
- **Change**: Absolute difference
- **Percentage**: Relative change

### Trend Summary

Shows count of metrics by trend direction:
- **Improving Metrics**: Performance is getting better
- **Degrading Metrics**: Performance is getting worse (investigate these!)
- **Stable Metrics**: No significant change

### Volatility Analysis

Identifies metrics with high variability:
- **High CV (>30%)**: Very inconsistent, may indicate:
  - Environmental factors (load, background processes)
  - Non-deterministic code paths
  - Measurement issues
- **Medium CV (15-30%)**: Some variability, monitor
- **Low CV (<15%)**: Consistent and reliable

## Best Practices

### 1. Establish Baselines
- Run performance tests 5-10 times before making changes
- This establishes a reliable baseline with known variance

### 2. Consistent Test Conditions
- Close unnecessary applications
- Use same test scenarios each run
- Wait for system to stabilize (disk cache, etc.)

### 3. Regular Monitoring
- Weekly trend analysis to catch gradual degradation
- Before/after analysis for every performance-related PR
- Monthly deeper dives with full statistics

### 4. Focus on Meaningful Changes
- Changes <1% are often noise
- Changes 1-5% warrant monitoring
- Changes >5% require investigation

### 5. Context Matters
- Look at multiple metrics together
- Consider trade-offs (e.g., memory for speed)
- Correlate with code changes using timestamps

## Advanced Scenarios

### Compare Specific Metrics Only
```powershell
# Focus on startup metrics only
.\scripts\Analyze-PerformanceTrends.ps1 -Metrics @(
    'TotalStartupTime',
    'ContextInit', 
    'LoggingInit',
    'DependencyInjectionInit'
) -ShowChart
```

### Custom Chart Size
```powershell
# Larger charts for presentations
.\scripts\Analyze-PerformanceTrends.ps1 -ShowChart -ChartWidth 120 -ChartHeight 20
```

### Export Everything
```powershell
# Generate all outputs for documentation
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 30 -ShowChart -IncludeStatistics -ExportHtml -ExportCsv
```

### Analyze Specific Date Range
```powershell
# Get reports from specific dates
$reports = Get-ChildItem "$env:USERPROFILE\ACATTalk_PerformanceReports" -Filter "*.csv" | 
    Where-Object { $_.LastWriteTime -ge '2024-01-15' -and $_.LastWriteTime -le '2024-01-20' }

.\scripts\Analyze-PerformanceTrends.ps1 -Files $reports.FullName -ShowChart -ExportHtml
```

## Troubleshooting

**"Need at least 2 reports for trend analysis"**
- Generate more reports by running ACATTalk multiple times
- Each normal exit creates a new report

**"No data to chart"**
- Check that the metric name is spelled correctly
- Use `-Metrics` to see what's available in your reports

**Charts look distorted**
- Adjust `-ChartWidth` and `-ChartHeight` parameters
- Ensure console window is wide enough

**Missing metrics in trends**
- Older reports may not have newer metrics
- Script will skip metrics not found in any report

## Data Format

### CSV Report Format (Input)
```csv
Category,Metric,Value,Unit,Count,Min,Max,Timestamp
Startup,TotalStartupTime,2450.50,ms,1,2450.50,2450.50,2024-01-15T10:30:00
Memory,PeakMemoryUsage,185.23,MB,1,185.23,185.23,2024-01-15T10:30:00
```

### Trend Summary CSV Format (Output)
```csv
Metric,Category,Unit,First,Last,Change,ChangePercent,Trend,Min,Max,Average,Median,StdDev
TotalStartupTime,Startup,ms,2500.00,2350.00,-150.00,-6.0,Improving,2300.00,2600.00,2425.00,2410.00,85.50
```

## Next Steps

After running trend analysis:

1. **Identify Issues**: Look for degrading trends
2. **Investigate**: Use timestamps to correlate with code changes
3. **Optimize**: Focus on high-impact metrics
4. **Validate**: Re-run trends to confirm improvements
5. **Document**: Export HTML reports for team reviews

## Integration with Existing Scripts

The trend analyzer complements existing scripts:

- **Analyze-Performance.ps1**: Single report, detailed breakdown
- **Analyze-PerformanceTrends.ps1**: Multiple reports, trend analysis
- **Build-Performance.ps1**: Build and run with performance monitoring

Typical workflow:
```powershell
# Quick check after a single run
.\scripts\Analyze-Performance.ps1

# Deep analysis after multiple runs
.\scripts\Analyze-PerformanceTrends.ps1 -ShowChart -IncludeStatistics -ExportHtml
```
