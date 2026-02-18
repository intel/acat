# ACATTalk Performance Monitoring - Getting Started

This is a quick reference for the ACATTalk performance monitoring system. For complete documentation, see the `scripts/` directory.

## Quick Start (3 Commands)

```powershell
# 1. Build with performance monitoring
.\scripts\Build-Performance.ps1 -Run

# 2. Use ACATTalk normally for 5-10 minutes, then exit

# 3. Analyze performance
.\scripts\Analyze-Performance.ps1
```

That's it! Reports are saved to `%USERPROFILE%\ACATTalk_PerformanceReports\`

## What It Does

- ✅ Measures startup time, component initialization, memory usage
- ✅ Zero overhead when performance monitoring is disabled  
- ✅ Automatic NuGet package restore
- ✅ Builds entire solution with dependencies
- ✅ Generates professional reports
- ✅ Compares runs to detect regressions

## Documentation

- **`scripts/README.md`** - Complete script reference
- **`scripts/IMPLEMENTATION_SUMMARY.md`** - Full implementation details
- **`scripts/MIGRATION_GUIDE.md`** - Migration from previous location
- **`Applications/ACATTalk/QUICK_START.md`** - Quick start guide
- **`Applications/ACATTalk/PERFORMANCE_MONITORING.md`** - Detailed monitoring guide

## Common Commands

```powershell
# Build and run
.\scripts\Build-Performance.ps1 -Run

# Clean build
.\scripts\Build-Performance.ps1 -Clean -Run

# Fast build (skip package restore)
.\scripts\Build-Performance.ps1 -SkipRestore

# Analyze latest report
.\scripts\Analyze-Performance.ps1

# Compare with previous run
.\scripts\Analyze-Performance.ps1 -Compare

# Export summary
.\scripts\Analyze-Performance.ps1 -Export
```

## How It Works

1. **Build Script** defines `PERFORMANCE` symbol for ACATTalk
2. **Performance Monitor** collects metrics during runtime
3. **On Exit** generates reports automatically
4. **Analysis Script** provides insights and comparisons

## Example Output

```
[Startup]
  TotalStartupTime                         2450.23 ms
  ContextInit                              1234.56 ms

[Memory]
  PeakMemoryUsage                           128.34 MB
  MemoryGrowth                               52.78 MB
```

## Requirements

- Visual Studio 2019 or later (for MSBuild)
- PowerShell 5.1 or later
- .NET Framework 4.8.1

## Support

- See `scripts/README.md` for troubleshooting
- Check documentation in `Applications/ACATTalk/`
- Review example output in documentation

---

**For complete documentation, start with:** `scripts/README.md`
