# Performance Trend Analysis - Quick Start

## What Is This?

A new PowerShell script that analyzes performance trends across **multiple** ACATTalk performance reports to help you:
- ✅ Identify performance regressions over time
- ✅ Validate that your optimizations are working
- ✅ Track application health across releases
- ✅ Make data-driven performance decisions

## The Problem It Solves

**Before:** You could only compare 2 reports at a time using `Analyze-Performance.ps1 -Compare`
**Now:** You can analyze trends across 2 to N reports with statistical analysis and visualizations

## Quick Start (3 Steps)

### Step 1: Generate Multiple Reports
```powershell
# Build and run ACATTalk 3-5 times
.\scripts\Build-Performance.ps1 -Run
# Use ACATTalk for a few minutes, then exit normally
# Repeat 2-4 more times
```

### Step 2: Analyze Trends
```powershell
# Basic analysis
.\scripts\Analyze-PerformanceTrends.ps1

# With visualizations
.\scripts\Analyze-PerformanceTrends.ps1 -ShowChart -IncludeStatistics
```

### Step 3: Review Results
Look for trend indicators:
- ▼▼ / ▼ = **Improving** (good!)
- ═ = **Stable** (expected)
- ▲ / ▲▲ = **Degrading** (investigate!)

## Common Commands

```powershell
# Analyze last 20 reports
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 20

# Show visual trend charts
.\scripts\Analyze-PerformanceTrends.ps1 -ShowChart

# Detailed statistics
.\scripts\Analyze-PerformanceTrends.ps1 -IncludeStatistics

# Export HTML report for sharing
.\scripts\Analyze-PerformanceTrends.ps1 -ExportHtml

# Focus on specific metrics
.\scripts\Analyze-PerformanceTrends.ps1 -Metrics @('TotalStartupTime','PeakMemoryUsage')

# Everything at once
.\scripts\Analyze-PerformanceTrends.ps1 -ShowChart -IncludeStatistics -ExportHtml -ExportCsv
```

## Example Output

```
========================================
Performance Trends
========================================

TotalStartupTime                    ▼   2500.00 ms →   2350.00 ms  (-150.00, -6.0%)
ContextInit                         ═    450.00 ms →    455.00 ms  (+5.00, +1.1%)
PeakMemoryUsage                     ▲▲   180.00 MB →    195.00 MB  (+15.00, +8.3%)
MemoryGrowth                        ▼    45.00 MB →     42.00 MB   (-3.00, -6.7%)

========================================
Trend Summary
========================================

  Improving Metrics:      2
  Degrading Metrics:      1
  Stable Metrics:         1
```

## When To Use

### Use Case 1: After Making Code Changes
```powershell
# Before: Run 3 times to establish baseline
.\scripts\Build-Performance.ps1 -Run

# Make your changes

# After: Run 3 times to test
.\scripts\Build-Performance.ps1 -Run

# Compare
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 6 -ShowChart
```

### Use Case 2: Weekly Health Check
```powershell
# Review last week's performance
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 20 -ExportHtml
# Open the generated HTML file
```

### Use Case 3: Investigating Regressions
```powershell
# Look at longer history to find when it started
.\scripts\Analyze-PerformanceTrends.ps1 -LastN 30 -ShowChart -IncludeStatistics
```

## Interactive Demo

Want to see it in action? Run the demo:
```powershell
.\scripts\Demo-PerformanceTrends.ps1
```

## Full Documentation

For complete documentation, see:
- **[PERFORMANCE_TRENDS_GUIDE.md](PERFORMANCE_TRENDS_GUIDE.md)** - Comprehensive guide
- **[TREND_ANALYSIS_IMPLEMENTATION.md](TREND_ANALYSIS_IMPLEMENTATION.md)** - Implementation details
- **[README.md](README.md)** - All performance scripts

## Files Created

This feature consists of:
1. **`Analyze-PerformanceTrends.ps1`** - Main analysis script (~350 lines)
2. **`PERFORMANCE_TRENDS_GUIDE.md`** - User guide (~280 lines)
3. **`Demo-PerformanceTrends.ps1`** - Interactive demo (~140 lines)
4. **`README.md`** (updated) - Added documentation

## Support

Questions? Check the guides above or run:
```powershell
Get-Help .\scripts\Analyze-PerformanceTrends.ps1 -Detailed
```

---

**Pro Tip:** Combine with CI/CD to automatically track performance across builds! 🚀
