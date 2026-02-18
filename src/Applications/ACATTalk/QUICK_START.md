# ACATTalk Performance Baseline - Quick Start Guide

This guide will help you quickly establish performance baselines for ACATTalk.

## Quick Start (3 Steps)

### Step 1: Build with Performance Monitoring

**Option A - Using PowerShell Script (Easiest)**
```powershell
cd Applications\ACATTalk
.\Build-Performance.ps1 -Run
```
*Note: This builds the entire ACAT solution to ensure all dependencies are up-to-date, then rebuilds ACATTalk with performance monitoring enabled. This may take a few minutes on first run.*

**Option B - Using Visual Studio**
1. Open ACATTalk project properties
2. Go to Build tab
3. Add `PERFORMANCE` to "Conditional compilation symbols"
4. Build Solution (Ctrl+Shift+B)
5. Run ACATTalk (F5)

**Option C - Using MSBuild**
```powershell
# Build solution first
msbuild ACAT.sln /p:Configuration=Release

# Rebuild ACATTalk with performance monitoring
msbuild Applications\ACATTalk\ACATTalk.csproj /t:Rebuild /p:Configuration=Release /p:DefineConstants="TRACE;PERFORMANCE"
```

### Step 2: Use ACATTalk Normally

- Launch the application
- Perform typical operations (type text, use predictions, text-to-speech, etc.)
- Use it for a representative session (5-10 minutes)
- Exit normally (close the application)

### Step 3: View Performance Report

**Option A - Using PowerShell Script (Recommended)**
```powershell
# From solution root
.\scripts\Analyze-Performance.ps1
```

**Option B - Manual**
1. Navigate to: `%USERPROFILE%\ACATTalk_PerformanceReports\`
2. Open the latest `.txt` file for human-readable report
3. Open the latest `.csv` file in Excel for data analysis

## What Gets Measured

✅ **Startup Time** - How long it takes to launch  
✅ **Component Initialization** - Logging, DI, Context, TTS, Word Prediction  
✅ **Memory Usage** - Start, peak, end, and growth  
✅ **UI Performance** - Panel creation times  
✅ **Shutdown Time** - Cleanup performance  

## Example Output

```
[Startup]
  TotalStartupTime                         2450.23 ms
  FileUtilsLogAssemblyInfo                   45.12 ms
  LoadUserPreferences                       234.89 ms

[Initialization]
  ContextInit                              1234.56 ms
  ContextPostInit                           156.23 ms

[Memory]
  StartMemoryUsage                           45.67 MB
  PeakMemoryUsage                           128.34 MB
  MemoryGrowth                               52.78 MB
```

## Comparing Performance Between Runs

```powershell
# Run 1 - Establish baseline
.\Build-Performance.ps1 -Run
# ... use application and exit ...

# Run 2 - After code changes
.\Build-Performance.ps1 -Clean -Run
# ... use application and exit ...

# Compare results
.\Analyze-Performance.ps1 -Compare
```

## Building WITHOUT Performance Monitoring

Simply remove the `PERFORMANCE` symbol:
- No performance code is compiled
- Zero overhead
- Normal production build

## Troubleshooting

**Q: The build takes a long time**  
A: The script builds the entire ACAT solution to ensure all dependencies are current. Subsequent builds will be faster as only changed files are rebuilt.

**Q: I don't see any reports**  
A: Make sure you exit ACATTalk normally (not crash). Reports are generated on shutdown.

**Q: Where are the reports saved?**  
A: `%USERPROFILE%\ACATTalk_PerformanceReports\` (e.g., `C:\Users\YourName\ACATTalk_PerformanceReports\`)

**Q: How do I see debug output in Visual Studio?**  
A: View → Output, then select "Debug" from the dropdown

**Q: Can I run this in production?**  
A: Only with PERFORMANCE symbol defined. Remove it for production builds to eliminate any overhead.

## Advanced Usage

### Set Performance Thresholds
Edit `Analyze-Performance.ps1` to customize thresholds:
```powershell
# Line ~70: Startup time warning threshold
if ($metric.Category -eq "Startup" -and $value -gt 1000) { $color = "Red" }

# Line ~100: Memory usage warning threshold  
if ($peakMem -gt 500) { "Red" } elseif ($peakMem -gt 200) { "Yellow" }
```

### Export for CI/CD
```powershell
.\Analyze-Performance.ps1 -Export
# Creates summary file for automated analysis
```

### Add Custom Metrics
In your code:
```csharp
#if PERFORMANCE
PerformanceMonitor.StartTimer("MyCustomOperation");
#endif

// Your code here

#if PERFORMANCE
PerformanceMonitor.StopTimer("MyCustomOperation", PerformanceMonitor.MetricCategory.Interaction);
#endif
```

## Files Created

```
scripts/
├── Build-Performance.ps1          # Build script (PowerShell)
├── Build-Performance.bat          # Build script (Batch)
├── Analyze-Performance.ps1        # Analysis script
└── README.md                      # Scripts documentation

Applications/ACATTalk/
├── PerformanceMonitor.cs          # Core performance monitoring class
├── Program.cs                     # Instrumented with performance timers
├── PERFORMANCE_MONITORING.md      # Detailed documentation
└── QUICK_START.md                 # This file
```

## Next Steps

1. **Establish Baseline**: Run 3-5 times and average the results
2. **Document Baseline**: Save reports as your performance baseline
3. **Monitor Changes**: Run after significant code changes to detect regressions
4. **Optimize**: Use reports to identify bottlenecks
5. **Track Over Time**: Compare reports weekly/monthly to spot trends

## Support

For detailed documentation, see `PERFORMANCE_MONITORING.md`

For questions or issues:
- Check debug output (View → Output in Visual Studio)
- Verify PERFORMANCE symbol is defined
- Ensure application exits normally
