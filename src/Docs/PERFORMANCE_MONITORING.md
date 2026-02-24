# ACATTalk Performance Monitoring

This document describes how to enable and use performance monitoring for establishing baseline performance metrics in ACATTalk.

## Overview

Performance monitoring is built into ACATTalk using conditional compilation. When enabled, it tracks:

- **Startup Performance**: Application launch time, initialization phases
- **Memory Usage**: Startup, peak, and growth over time
- **Component Initialization**: Logging, DI, Context, TTS, Word Prediction
- **UI Performance**: Panel creation times
- **Shutdown Performance**: Cleanup and disposal times

## Enabling Performance Monitoring

### Option 1: Define PERFORMANCE Symbol in Project Properties

1. Right-click on **ACATTalk** project in Solution Explorer
2. Select **Properties**
3. Go to **Build** tab
4. In **Conditional compilation symbols**, add `PERFORMANCE`
5. Choose your configuration (Debug or Release)
6. Save and rebuild

### Option 2: Create a New Build Configuration

1. In Visual Studio, go to **Build** > **Configuration Manager**
2. Click **<New...>** under Active solution configuration
3. Name it `Performance`
4. Copy settings from `Release`
5. Click OK
6. Right-click **ACATTalk** project > **Properties** > **Build** tab
7. Select **Performance** configuration
8. Add `PERFORMANCE` to **Conditional compilation symbols**
9. Save and rebuild

### Option 3: Command Line Build

```powershell
# Build entire solution first (ensures all dependencies are current)
msbuild ACAT.sln /p:Configuration=Release

# Rebuild ACATTalk with PERFORMANCE symbol
msbuild Applications\ACATTalk\ACATTalk.csproj /t:Rebuild /p:Configuration=Release /p:DefineConstants="TRACE;PERFORMANCE"
```

**Note**: ACATTalk depends on other projects in the ACAT solution (ACATCore, ACATExtension, etc.). The build scripts automatically build the entire solution first to ensure all dependencies are up-to-date, then rebuild ACATTalk with performance monitoring enabled.

## Running with Performance Monitoring

1. **Build** the project with PERFORMANCE symbol defined
2. **Run** ACATTalk normally
3. **Use** the application as you would for typical workflows
4. **Exit** the application (performance reports are generated on shutdown)

## Performance Reports

Reports are automatically generated when ACATTalk exits and saved to:

```
%USERPROFILE%\ACATTalk_PerformanceReports\
```

Two files are created per session:

### Text Report (`ACATTalk_Performance_YYYYMMDD_HHmmss.txt`)
Human-readable report organized by categories:
- Startup metrics
- Initialization metrics
- Memory usage
- UI performance
- Shutdown metrics

Example:
```
================================================================================
ACATTalk Performance Baseline Report
Generated: 2024-01-15 10:30:45
================================================================================

[Startup]
--------------------------------------------------------------------------------
  TotalStartupTime                         2450.23 ms
  FileUtilsLogAssemblyInfo                   45.12 ms
  SetUserNameAndProfile                     125.67 ms
  LoadUserPreferences                       234.89 ms

[Initialization]
--------------------------------------------------------------------------------
  LoggingInitialization                      89.34 ms
  DependencyInjectionSetup                   12.56 ms
  ContextPreInit                            345.78 ms
  ContextInit                              1234.56 ms
  ContextPostInit                           156.23 ms

[Memory]
--------------------------------------------------------------------------------
  StartMemoryUsage                           45.67 MB
  PeakMemoryUsage                           128.34 MB
  EndMemoryUsage                             98.45 MB
  MemoryGrowth                               52.78 MB

[UI]
--------------------------------------------------------------------------------
  CreateTalkApplicationScanner               234.12 ms

[Shutdown]
--------------------------------------------------------------------------------
  TotalApplicationLifetime                   300.45 s
  ShutdownTime                               456.78 ms
```

### CSV Report (`ACATTalk_Performance_YYYYMMDD_HHmmss.csv`)
Machine-readable format for analysis in Excel, Python, or other tools:
```csv
Category,Metric,Value,Unit,Count,Min,Max,Timestamp
Startup,TotalStartupTime,2450.23,ms,1,2450.23,2450.23,2024-01-15T10:30:45
Initialization,ContextInit,1234.56,ms,1,1234.56,1234.56,2024-01-15T10:30:45
...
```

## Metrics Collected

### Startup Category
- **TotalStartupTime**: Complete startup from Main() to UI shown
- **FileUtilsLogAssemblyInfo**: Assembly info logging time
- **SetUserNameAndProfile**: User/profile setup time
- **LoadUserPreferences**: Preferences loading time

### Initialization Category
- **LoggingInitialization**: Modern logging setup
- **DependencyInjectionSetup**: DI container configuration
- **ContextPreInit**: Context pre-initialization
- **ContextInit**: Main context initialization (includes TTS, Word Prediction, Agent Manager, etc.)
- **ContextPostInit**: Post-initialization phase

### Memory Category
- **StartMemoryUsage**: Working set at application start (MB)
- **CurrentMemoryUsage**: Sampled every 5 seconds during runtime (MB)
- **PeakMemoryUsage**: Maximum memory usage during session (MB)
- **EndMemoryUsage**: Working set at application exit (MB)
- **MemoryGrowth**: Net memory change during session (MB)

### UI Category
- **CreateTalkApplicationScanner**: Main UI panel creation time

### Shutdown Category
- **TotalApplicationLifetime**: Total runtime in seconds
- **ShutdownTime**: Cleanup and disposal time

## Debugging

Performance events are also logged to **Debug Output** window in Visual Studio:

```
[0.000s] Application: Main entry point
[0.125s] Startup: FileUtilsLogAssemblyInfo complete
[2.450s] Startup: Initialization complete
[300.450s] Shutdown: Starting application shutdown
[300.906s] Shutdown: Generating performance report
```

## Performance Impact

When `PERFORMANCE` symbol is **NOT** defined:
- All performance monitoring code is **completely excluded** from compilation
- **Zero runtime overhead**
- **Zero memory overhead**

When `PERFORMANCE` symbol **IS** defined:
- Minimal overhead (< 0.1% typical)
- Memory monitoring timer runs every 5 seconds
- Small memory overhead for metric storage

## Analyzing Results

### Baseline Comparison
Run multiple sessions and compare:
```powershell
# From solution root
.\scripts\Analyze-Performance.ps1 -Compare
```

Or manually:
```powershell
# Example PowerShell to compare multiple runs
$reports = Get-ChildItem "$env:USERPROFILE\ACATTalk_PerformanceReports\*.csv"
Import-Csv $reports[0] | Where-Object Metric -eq "TotalStartupTime"
Import-Csv $reports[1] | Where-Object Metric -eq "TotalStartupTime"
```

### Performance Regression Detection
1. Establish baseline with current build
2. Make code changes
3. Run with performance monitoring
4. Compare new metrics to baseline
5. Identify regressions (slower operations)

### Key Metrics to Watch
- **TotalStartupTime** should be < 3 seconds
- **ContextInit** is typically the longest phase
- **MemoryGrowth** should be stable (not continuously increasing)
- **PeakMemoryUsage** should be reasonable for your workload

## Extending Performance Monitoring

To add custom metrics in your code:

```csharp
#if PERFORMANCE
PerformanceMonitor.StartTimer("MyOperation");
#endif

// Your code here

#if PERFORMANCE
PerformanceMonitor.StopTimer("MyOperation", PerformanceMonitor.MetricCategory.Interaction);
#endif
```

Or record a direct metric:
```csharp
#if PERFORMANCE
PerformanceMonitor.RecordMetric("CustomMetric", 123.45, "items/sec", PerformanceMonitor.MetricCategory.Interaction);
#endif
```

## Troubleshooting

**Problem**: Build fails with dependency errors  
**Solution**: Ensure the entire solution is built and NuGet packages are restored. The scripts handle this automatically. If building manually, run:
```powershell
msbuild ACAT.sln /t:Restore,Build
```

**Problem**: NuGet restore fails  
**Solution**: Try restoring manually:
```powershell
# Using NuGet.exe
nuget restore ACAT.sln

# Or using MSBuild
msbuild ACAT.sln /t:Restore
```

**Problem**: No performance reports generated  
**Solution**: Ensure PERFORMANCE symbol is defined and application exits normally (not crashed)

**Problem**: Reports folder not found  
**Solution**: Check `%USERPROFILE%\ACATTalk_PerformanceReports\` exists; folder is created automatically

**Problem**: Can't see debug output  
**Solution**: In Visual Studio, go to **View** > **Output**, select "Debug" from dropdown

## Best Practices

1. **Consistent Environment**: Run baselines on the same machine with same configuration
2. **Multiple Runs**: Take average of 3-5 runs for reliable baseline
3. **Typical Workflow**: Use realistic workflows when measuring
4. **Clean State**: Restart machine before important baseline measurements
5. **Document Changes**: Note any configuration or code changes between baselines

## Next Steps

After establishing baseline:
- Add more granular instrumentation to specific components
- Create automated performance tests
- Set up CI/CD performance gates
- Monitor trends over time
