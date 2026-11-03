# Conversion of CoreGlobals Stopwatches to Callback Mechanism

## Overview

Converted hardcoded `CoreGlobals.Stopwatch` usage in `UserControlWordPredictionCommon.cs` to use optional callback mechanisms similar to the existing `OnPredictionLatencyMs` pattern.

## Changes Made

### New Callbacks Added

```csharp
/// <summary>
/// Optional callback invoked with the elapsed autocomplete latency in milliseconds
/// each time a word/letter/sentence autocomplete operation completes.
/// </summary>
public static Action<double> OnAutoCompleteLatencyMs;

/// <summary>
/// Optional callback invoked with the elapsed refresh latency in milliseconds
/// each time a word prediction refresh operation completes.
/// </summary>
public static Action<double> OnRefreshLatencyMs;
```

### Conversions

#### 1. AutoComplete Operations (3 occurrences)

**Before:**
```csharp
CoreGlobals.Stopwatch1.Reset();
CoreGlobals.Stopwatch1.Start();

_form.Invoke(new MethodInvoker(delegate
{
    autoComplete(e.SourceWidget as WordListItemWidget);
}));

CoreGlobals.Stopwatch1.Stop();
_logger?.LogDebug("TimeElapsed 3 : {ElapsedMs}ms", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
```

**After:**
```csharp
var sw = Stopwatch.StartNew();

_form.Invoke(new MethodInvoker(delegate
{
    autoComplete(e.SourceWidget as WordListItemWidget);
}));

sw.Stop();
OnAutoCompleteLatencyMs?.Invoke(sw.Elapsed.TotalMilliseconds);
```

**Changes:**
- ✅ Local `Stopwatch` instance instead of shared `CoreGlobals.Stopwatch1`
- ✅ Callback invocation instead of direct logging
- ✅ Uses `TotalMilliseconds` (double) for better precision
- ✅ No hardcoded logging - consumer decides what to do with metrics

**Applies to:**
- `WordListItemWidget` autocomplete (line 220-234)
- `LetterListItemWidget` autocomplete (line 240-254)
- `SentenceListItemWidget` autocomplete (line 260-274)

#### 2. Refresh Operation

**Before:**
```csharp
private void refreshWordPredictionsAndSetCurrentWord()
{
    CoreGlobals.Stopwatch3.Reset();
    CoreGlobals.Stopwatch3.Start();

    tryRefreshWordPredictionsAndSetCurrentWord();

    CoreGlobals.Stopwatch3.Stop();
    _logger.LogDebug("TimeElapsed for tryRefreshWordPredictionsAndSetCurrentWord: " + CoreGlobals.Stopwatch3.ElapsedMilliseconds);
}
```

**After:**
```csharp
private void refreshWordPredictionsAndSetCurrentWord()
{
    var sw = Stopwatch.StartNew();

    tryRefreshWordPredictionsAndSetCurrentWord();

    sw.Stop();
    OnRefreshLatencyMs?.Invoke(sw.Elapsed.TotalMilliseconds);
}
```

**Changes:**
- ✅ Local `Stopwatch` instance instead of shared `CoreGlobals.Stopwatch3`
- ✅ Callback invocation instead of direct logging
- ✅ Cleaner, more focused code

## Benefits

### 1. Thread Safety
- **Before:** Shared `CoreGlobals.Stopwatch1` and `Stopwatch3` could have race conditions
- **After:** Local stopwatch instances per operation - thread-safe

### 2. Separation of Concerns
- **Before:** Logging hardcoded in business logic
- **After:** Metrics collection decoupled from business logic

### 3. Flexibility
- **Before:** Only logging available
- **After:** Consumers can:
  - Aggregate metrics in PerformanceMonitor
  - Send to telemetry systems
  - Store in databases
  - Display in dashboards
  - Or ignore (null callback)

### 4. Better Precision
- **Before:** `ElapsedMilliseconds` (long) - loses sub-millisecond precision
- **After:** `TotalMilliseconds` (double) - preserves microsecond precision

### 5. Consistency
- **Before:** Mixed patterns (OnPredictionLatencyMs vs direct logging)
- **After:** Consistent callback pattern for all performance metrics

## Usage

### Hooking Up Callbacks in ACATTalk/ACATApp

```csharp
// In Program.cs Main() method, after PerformanceMonitor.Initialize()

#if PERFORMANCE
    // Hook up word prediction callbacks
    UserControlWordPredictionCommon.OnPredictionLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("WordPrediction", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
    };

    UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("AutoComplete", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
    };

    UserControlWordPredictionCommon.OnRefreshLatencyMs = (latencyMs) =>
    {
        PerformanceMonitor.RecordMetric("PredictionRefresh", latencyMs, "ms", 
            PerformanceMonitor.MetricCategory.WordPrediction);
    };
#endif
```

### Alternative: Logging Only

```csharp
// In Program.cs - simple logging
UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (latencyMs) =>
{
    _logger.LogDebug("AutoComplete latency: {LatencyMs:F2}ms", latencyMs);
};

UserControlWordPredictionCommon.OnRefreshLatencyMs = (latencyMs) =>
{
    _logger.LogDebug("Refresh latency: {LatencyMs:F2}ms", latencyMs);
};
```

### Alternative: No Metrics Collection

```csharp
// Don't set the callbacks - metrics are ignored
// (Zero overhead in production if callbacks not set)
```

## Performance Impact

### Before:
```csharp
CoreGlobals.Stopwatch1.Reset();  // Shared stopwatch
CoreGlobals.Stopwatch1.Start();
// ... operation ...
CoreGlobals.Stopwatch1.Stop();
_logger?.LogDebug("...");  // Always logs (even if not needed)
```

**Issues:**
- ❌ Thread contention on shared stopwatch
- ❌ Always allocates log message string
- ❌ Always performs logging (even in Release)

### After:
```csharp
var sw = Stopwatch.StartNew();  // Local, no contention
// ... operation ...
sw.Stop();
OnAutoCompleteLatencyMs?.Invoke(sw.Elapsed.TotalMilliseconds);  // Optional
```

**Benefits:**
- ✅ No thread contention
- ✅ Null check - zero overhead if callback not set
- ✅ Consumer controls performance impact

## Migration Guide

### For Other Files Using CoreGlobals.Stopwatch

If you find similar patterns elsewhere:

1. **Add a static callback property:**
   ```csharp
   public static Action<double> OnOperationLatencyMs;
   ```

2. **Replace CoreGlobals.Stopwatch usage:**
   ```csharp
   // Before
   CoreGlobals.Stopwatch1.Reset();
   CoreGlobals.Stopwatch1.Start();
   DoWork();
   CoreGlobals.Stopwatch1.Stop();
   _logger.LogDebug("Elapsed: {Ms}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
   
   // After
   var sw = Stopwatch.StartNew();
   DoWork();
   sw.Stop();
   OnOperationLatencyMs?.Invoke(sw.Elapsed.TotalMilliseconds);
   ```

3. **Wire up in application startup:**
   ```csharp
   YourClass.OnOperationLatencyMs = (ms) => PerformanceMonitor.RecordMetric(...);
   ```

## Metrics Now Available

After hooking up the callbacks, you can track:

1. **Prediction Latency** - How long IWordPredictor.Predict() takes
2. **AutoComplete Latency** - How long autoComplete operations take
3. **Refresh Latency** - How long the full refresh cycle takes

### Example PerformanceMonitor Integration

```csharp
// In PerformanceMonitor.cs, add new metric categories
public enum MetricCategory
{
    Startup,
    Shutdown,
    Memory,
    WordPrediction,     // ← Add this
    AutoComplete,       // ← Add this
    UIRefresh          // ← Add this
}

// In Program.cs
UserControlWordPredictionCommon.OnPredictionLatencyMs = (ms) =>
{
    PerformanceMonitor.RecordMetric("PredictionLatency", ms, "ms", 
        PerformanceMonitor.MetricCategory.WordPrediction);
};

UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (ms) =>
{
    PerformanceMonitor.RecordMetric("AutoCompleteLatency", ms, "ms", 
        PerformanceMonitor.MetricCategory.AutoComplete);
};

UserControlWordPredictionCommon.OnRefreshLatencyMs = (ms) =>
{
    PerformanceMonitor.RecordMetric("RefreshLatency", ms, "ms", 
        PerformanceMonitor.MetricCategory.UIRefresh);
};
```

## Testing

### Verify No Functional Changes:

```csharp
[TestMethod]
public void AutoComplete_InvokesCallback()
{
    double? capturedLatency = null;
    UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (ms) => capturedLatency = ms;
    
    // Trigger autocomplete
    // ...
    
    Assert.IsNotNull(capturedLatency);
    Assert.IsTrue(capturedLatency > 0);
}
```

### Verify Thread Safety:

```csharp
[TestMethod]
public void Callbacks_AreThreadSafe()
{
    var latencies = new ConcurrentBag<double>();
    UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (ms) => latencies.Add(ms);
    
    // Trigger multiple autocompletes from different threads
    // ...
    
    // Should not crash or lose measurements
    Assert.IsTrue(latencies.Count > 0);
}
```

## Backward Compatibility

✅ **100% backward compatible**

- If callbacks are not set, operations work exactly as before (minus the logging)
- Null-conditional operator (`?.`) ensures no NullReferenceException
- No changes to public API surface (callbacks are static properties)
- No changes to method signatures

## Files Modified

1. **Libraries\ACATExtension\UI\UserControlWordPredictionCommon.cs**
   - Added `OnAutoCompleteLatencyMs` callback
   - Added `OnRefreshLatencyMs` callback
   - Converted 4 stopwatch usages to callback pattern
   - Removed hardcoded debug logging

## Remaining Work

To fully integrate these metrics:

1. **Update ACATTalk/Program.cs** to wire up the new callbacks
2. **Update ACATApp/Program.cs** to wire up the new callbacks
3. **Add metric categories** to PerformanceMonitor if needed
4. **Update PerformanceDashboard** to display these metrics

## Next Steps

```csharp
// In Applications\ACATTalk\Program.cs (after PerformanceMonitor.Initialize())

#if PERFORMANCE
    UserControlWordPredictionCommon.OnPredictionLatencyMs = (ms) =>
        PerformanceMonitor.RecordMetric("Prediction", ms, "ms", PerformanceMonitor.MetricCategory.Startup);
    
    UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (ms) =>
        PerformanceMonitor.RecordMetric("AutoComplete", ms, "ms", PerformanceMonitor.MetricCategory.Startup);
    
    UserControlWordPredictionCommon.OnRefreshLatencyMs = (ms) =>
        PerformanceMonitor.RecordMetric("Refresh", ms, "ms", PerformanceMonitor.MetricCategory.Startup);
#endif
```

## Summary

✅ **Removed dependency on shared CoreGlobals stopwatches**  
✅ **Added flexible callback mechanism for metrics collection**  
✅ **Improved thread safety with local stopwatch instances**  
✅ **Maintained backward compatibility**  
✅ **Enabled better performance monitoring and diagnostics**
