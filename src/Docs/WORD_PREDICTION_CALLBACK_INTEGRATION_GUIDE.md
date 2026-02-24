# Integrating Word Prediction Callbacks into PerformanceMonitor

## Quick Integration Guide

To enable performance monitoring of word prediction operations in ACATTalk and ACATApp, wire up the new callbacks.

## Step 1: Update PerformanceMonitor.cs (if needed)

Add new metric categories if they don't exist:

```csharp
public enum MetricCategory
{
    Startup,
    Shutdown,
    Memory,
    WordPrediction,    // ← Add if missing
    UserInteraction    // ← Add if missing
}
```

## Step 2: Wire Up Callbacks in Program.cs

### For ACATTalk:

Add after `PerformanceMonitor.Initialize()`:

```csharp
[STAThread]
public static void Main(string[] args)
{
#if PERFORMANCE
    PerformanceMonitor.Initialize();
    PerformanceMonitor.StartTimer("TotalStartupTime");
    PerformanceMonitor.LogEvent("Application", "Main entry point");
    
    // Wire up word prediction performance callbacks
    UserControlWordPredictionCommon.OnPredictionLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("PredictionCall", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
    };
    
    UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("AutoComplete", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.UserInteraction);
    };
    
    UserControlWordPredictionCommon.OnRefreshLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("PredictionRefresh", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
    };
#endif

    // ... rest of Main()
}
```

### For ACATApp:

Add in `InitializeGlobals()` or at the beginning of `Main()`:

```csharp
private static void InitializeGlobals()
{
    CoreGlobals.AppId = "ACATApp";
    CoreGlobals.ACATUserGuideFileName = "ACAT User Guide.pdf";
    FatalErrorHandler.EvtFatalError += CoreGlobals_EvtFatalError;

#if PERFORMANCE
    PerformanceMonitor.Initialize();
    
    // Wire up word prediction callbacks
    UserControlWordPredictionCommon.OnPredictionLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("PredictionCall", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
    };
    
    UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("AutoComplete", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.UserInteraction);
    };
    
    UserControlWordPredictionCommon.OnRefreshLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("PredictionRefresh", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
    };
#endif
}
```

## Step 3: Add Using Statement

Ensure this is at the top of Program.cs:

```csharp
using ACAT.Extension.UI;  // For UserControlWordPredictionCommon
```

## Metrics Collected

Once integrated, you'll capture:

### 1. Prediction Call Latency
**What:** Time spent in `IWordPredictor.Predict()` call  
**Typical:** 10-50ms for local predictors, 100-500ms for ML models  
**Use:** Identify slow word prediction engines

### 2. AutoComplete Latency
**What:** Time to insert selected word into target application  
**Typical:** 5-20ms  
**Use:** Identify UI thread blocking or slow text insertion

### 3. Prediction Refresh Latency
**What:** Full refresh cycle (get caret, extract context, predict, update UI)  
**Typical:** 20-100ms  
**Use:** Identify overall prediction performance bottlenecks

## Example Output

With PerformanceMonitor integration, you'll see metrics like:

```
[Metric] WordPrediction/PredictionCall: 23.45ms (avg: 28.12ms, min: 15.23ms, max: 45.67ms)
[Metric] UserInteraction/AutoComplete: 12.34ms (avg: 14.56ms, min: 8.90ms, max: 21.23ms)
[Metric] WordPrediction/PredictionRefresh: 35.67ms (avg: 42.34ms, min: 25.45ms, max: 67.89ms)
```

## Verification

### Test That Callbacks Are Invoked:

```csharp
// In a test or debug session
int callCount = 0;
UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (ms) =>
{
    callCount++;
    Console.WriteLine($"AutoComplete #{callCount}: {ms:F2}ms");
};

// Type in ACATTalk and select predicted words
// Console should show measurements
```

### Check Performance Report:

After running ACATTalk, check:
- `ACAT_Performance_Report_*.txt`
- `ACAT_Performance_Metrics_*.json`

Look for the new metrics:
- PredictionCall
- AutoComplete
- PredictionRefresh

## Advanced Usage

### Statistical Analysis:

```csharp
#if PERFORMANCE
    var predictionTimes = new List<double>();
    
    UserControlWordPredictionCommon.OnPredictionLatencyMs = (ms) =>
    {
        predictionTimes.Add(ms);
        PerformanceMonitor.RecordMetric("PredictionCall", ms, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
        
        // Log slow predictions
        if (ms > 100)
        {
            _logger.LogWarning("Slow prediction: {Ms}ms", ms);
        }
    };
#endif
```

### Live Dashboard Updates:

```csharp
UserControlWordPredictionCommon.OnPredictionLatencyMs = (ms) =>
{
    PerformanceMonitor.RecordMetric("PredictionCall", ms, "ms", 
        PerformanceMonitor.MetricCategory.WordPrediction);
    
    // Update live dashboard
    _performanceDashboard?.Dispatcher.InvokeAsync(() =>
    {
        _performanceDashboard.UpdatePredictionLatency(ms);
    });
};
```

### Telemetry Integration:

```csharp
UserControlWordPredictionCommon.OnPredictionLatencyMs = (ms) =>
{
    // Send to Application Insights, DataDog, etc.
    telemetryClient.TrackMetric("WordPredictionLatency", ms);
    
    // Also record locally
    PerformanceMonitor.RecordMetric("PredictionCall", ms, "ms", 
        PerformanceMonitor.MetricCategory.WordPrediction);
};
```

## Troubleshooting

### Callbacks not firing?

**Check:**
1. Are the callbacks set before word prediction is used?
2. Is the code path actually executing? (add breakpoint)
3. Is word prediction enabled in preferences?

### Metrics not appearing in report?

**Check:**
1. Is PERFORMANCE symbol defined?
2. Is PerformanceMonitor.Initialize() called?
3. Is PerformanceMonitor.Shutdown() generating the report?

### Performance overhead concerns?

**Solution:**
- Callbacks have minimal overhead (~microseconds)
- Only active when `#if PERFORMANCE` is defined
- Null check (`?.`) is optimized by JIT
- Consider sampling: only record 1 in N measurements

```csharp
private static int _sampleCounter = 0;
UserControlWordPredictionCommon.OnPredictionLatencyMs = (ms) =>
{
    if (Interlocked.Increment(ref _sampleCounter) % 10 == 0)  // Sample 10%
    {
        PerformanceMonitor.RecordMetric("PredictionCall", ms, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
    }
};
```

## Files Involved

- ✅ **Libraries\ACATExtension\UI\UserControlWordPredictionCommon.cs** - Callbacks defined
- ⏳ **Applications\ACATTalk\Program.cs** - Need to wire up callbacks
- ⏳ **Applications\ACATApp\Program.cs** - Need to wire up callbacks
- ⏳ **Applications\ACATTalk\PerformanceMonitor.cs** - May need metric categories

## Benefits Summary

✅ **Decoupled** - Metrics collection separate from business logic  
✅ **Flexible** - Any consumer can hook into metrics  
✅ **Thread-safe** - Local stopwatch instances  
✅ **Performance** - Zero overhead when callbacks not set  
✅ **Consistent** - Unified pattern across all metrics  
✅ **Testable** - Easy to mock and verify in tests
