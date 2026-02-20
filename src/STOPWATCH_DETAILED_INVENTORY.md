# Detailed Stopwatch Usage Inventory

## Complete List of Performance Timing Instances

---

## 1. TextController.cs - AutoComplete Operation (COMPLEX)

**File:** `Libraries\ACATCore\PanelManagement\Common\TextController.cs`  
**Method:** `AutoCompleteWord(string wordSelected)`  
**Complexity:** ⭐⭐⭐⭐⭐ (5 measurements, most complex)

### Current Implementation:

```csharp
public void AutoCompleteWord(string wordSelected)
{
    using AgentContext context = Context.AppAgentMgr.ActiveContext();
    Context.AppAgentMgr.TextChangedNotifications.Hold();

    // === MEASUREMENT 1: Get previous word ===
    CoreGlobals.Stopwatch4.Reset();
    CoreGlobals.Stopwatch4.Start();
    
    int caretPos = context.TextAgent().GetCaretPos();
    context.TextAgent().GetPrevWordOffsetAutoComplete(out int offset, out int count);
    
    CoreGlobals.Stopwatch4.Stop();
    _logger?.LogDebug("AutoComplete TimeElapsed 1: {ElapsedMilliseconds}", 
        CoreGlobals.Stopwatch4.ElapsedMilliseconds);

    // === MEASUREMENT 2: Check insert vs replace ===
    CoreGlobals.Stopwatch4.Reset();
    CoreGlobals.Stopwatch4.Start();
    
    bool checkInsert = context.TextAgent().CheckInsertOrReplaceWord(
        out int insertOrReplaceOffset, out string wordToReplace);
    
    CoreGlobals.Stopwatch4.Stop();
    _logger?.LogDebug("AutoComplete TimeElapsed 2: {ElapsedMilliseconds}", 
        CoreGlobals.Stopwatch4.ElapsedMilliseconds);

    // === MEASUREMENT 3: Insert operation (conditional) ===
    CoreGlobals.Stopwatch4.Reset();
    CoreGlobals.Stopwatch4.Start();
    
    if (checkInsert)
    {
        CoreGlobals.Stopwatch5.Reset();
        CoreGlobals.Stopwatch5.Start();
        
        context.TextAgent().Insert(insertOrReplaceOffset, wordSelected);
        
        CoreGlobals.Stopwatch5.Stop();
        _logger?.LogDebug("AutoComplete Insert operation TimeElapsed: {ElapsedMilliseconds}", 
            CoreGlobals.Stopwatch5.ElapsedMilliseconds);
    }
    else
    {
        context.TextAgent().Replace(insertOrReplaceOffset, wordToReplaceLength, wordSelected);
    }
    
    CoreGlobals.Stopwatch4.Stop();
    _logger?.LogDebug("AutoComplete TimeElapsed 3: {ElapsedMilliseconds}", 
        CoreGlobals.Stopwatch4.ElapsedMilliseconds);

    // === MEASUREMENT 4: Post-autocomplete ===
    CoreGlobals.Stopwatch4.Reset();
    CoreGlobals.Stopwatch4.Start();
    
    postAutoCompleteWord();
    _autoCompleteCaretPos = context.TextAgent().GetCaretPos();
    
    CoreGlobals.Stopwatch4.Stop();
    _logger?.LogDebug("AutoComplete TimeElapsed 4: {ElapsedMilliseconds}", 
        CoreGlobals.Stopwatch4.ElapsedMilliseconds);
}
```

### Suggested Conversion:

```csharp
// Add to TextController class
public static Action<string, double> OnAutoCompletePhaseLatencyMs;

// Convert to:
var sw = Stopwatch.StartNew();
int caretPos = context.TextAgent().GetCaretPos();
context.TextAgent().GetPrevWordOffsetAutoComplete(out int offset, out int count);
sw.Stop();
OnAutoCompletePhaseLatencyMs?.Invoke("GetPrevWord", sw.Elapsed.TotalMilliseconds);

sw = Stopwatch.StartNew();
bool checkInsert = context.TextAgent().CheckInsertOrReplaceWord(...);
sw.Stop();
OnAutoCompletePhaseLatencyMs?.Invoke("CheckInsertReplace", sw.Elapsed.TotalMilliseconds);

// etc.
```

**Metrics Produced:**
- AutoComplete_GetPrevWord
- AutoComplete_CheckInsertReplace  
- AutoComplete_Insert (or AutoComplete_Replace)
- AutoComplete_PostCompletion

---

## 2. UserControlKeyboardCommon.cs - Key Actuation

**File:** `Libraries\ACATCore\UserControlManagement\UserControlKeyboardCommon.cs`  
**Method:** `buttonActuated(Widget widget, ref bool handled)`  
**Lines:** 69-86  
**Complexity:** ⭐⭐ (2 measurements)

### Current Implementation:

```csharp
if (widget.Value.Length > 1)
{
    CoreGlobals.Stopwatch1.Reset();
    CoreGlobals.Stopwatch1.Start();
    
    Context.AppAgentMgr.TextChangedNotifications.Hold();
    SendKeys.SendWait(widget.Value + " ");
    Context.AppAgentMgr.TextChangedNotifications.Release();
    
    CoreGlobals.Stopwatch1.Stop();
    _logger?.LogDebug("TimeElapsed 1: {ElapsedMs}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
}
else
{
    CoreGlobals.Stopwatch1.Reset();
    CoreGlobals.Stopwatch1.Start();
    
    actuateKey(button.GetWidgetAttribute(), widget.Value[0]);
    
    CoreGlobals.Stopwatch1.Stop();
    _logger?.LogDebug("TimeElapsed 2 : {ElapsedMs}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
}
```

### Suggested Conversion:

```csharp
// Add to UserControlKeyboardCommon class
public static Action<string, double> OnKeyActuationLatencyMs;

// Convert to:
if (widget.Value.Length > 1)
{
    var sw = Stopwatch.StartNew();
    
    Context.AppAgentMgr.TextChangedNotifications.Hold();
    SendKeys.SendWait(widget.Value + " ");
    Context.AppAgentMgr.TextChangedNotifications.Release();
    
    sw.Stop();
    OnKeyActuationLatencyMs?.Invoke("MultiChar", sw.Elapsed.TotalMilliseconds);
}
else
{
    var sw = Stopwatch.StartNew();
    
    actuateKey(button.GetWidgetAttribute(), widget.Value[0]);
    
    sw.Stop();
    OnKeyActuationLatencyMs?.Invoke("SingleKey", sw.Elapsed.TotalMilliseconds);
}
```

**Metrics Produced:**
- KeyActuation_MultiChar
- KeyActuation_SingleKey

---

## 3. ScannerCommon.cs - Key Actuation (DUPLICATE)

**File:** `Libraries\ACATCore\PanelManagement\Common\ScannerCommon.cs`  
**Method:** `buttonActuated(Widget widget, ref bool handled)`  
**Lines:** 878-894  
**Complexity:** ⭐⭐ (2 measurements - SAME as UserControlKeyboardCommon!)

### Current Implementation:

```csharp
if (widget.Value.Length > 1)
{
    CoreGlobals.Stopwatch1.Reset();
    CoreGlobals.Stopwatch1.Start();
    
    Context.AppAgentMgr.TextChangedNotifications.Hold();
    SendKeys.SendWait(widget.Value + " ");
    Context.AppAgentMgr.TextChangedNotifications.Release();
    
    CoreGlobals.Stopwatch1.Stop();
    _logger?.LogTrace("TimeElapsed 1: " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);
}
else
{
    CoreGlobals.Stopwatch1.Reset();
    CoreGlobals.Stopwatch1.Start();
    
    actuateKey(button.GetWidgetAttribute(), widget.Value[0]);
    
    CoreGlobals.Stopwatch1.Stop();
    _logger?.LogTrace("TimeElapsed 2 : " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);
}
```

### Suggested Conversion:

**Same as UserControlKeyboardCommon** - use the same callback!

```csharp
// Use UserControlKeyboardCommon.OnKeyActuationLatencyMs callback
// Or create a shared utility class
```

**Note:** This is **duplicate code**. Consider:
1. Extracting to shared helper class
2. Using the same callback from UserControlKeyboardCommon
3. Investigating why both exist and if one can be removed

---

## 4. EditTextControlAgent.cs - Text Change Event

**File:** `Libraries\ACATCore\AgentManagement\TextControlAgents\EditTextControlAgent.cs`  
**Method:** `onTextChanged(object sender, AutomationEventArgs e)`  
**Lines:** 209-215  
**Complexity:** ⭐ (1 measurement)

### Current Implementation:

```csharp
private void onTextChanged(object sender, AutomationEventArgs e)
{
    CoreGlobals.Stopwatch2.Reset();
    CoreGlobals.Stopwatch2.Start();
    
    triggerTextChanged(this);
    
    CoreGlobals.Stopwatch2.Stop();
    _logger?.LogDebug("onTextChanged() TimeElapsed: {ElapsedMs}ms", 
        CoreGlobals.Stopwatch2.ElapsedMilliseconds);
}
```

### Suggested Conversion:

```csharp
// Add to EditTextControlAgent class
public static Action<double> OnTextChangeEventLatencyMs;

// Convert to:
private void onTextChanged(object sender, AutomationEventArgs e)
{
    var sw = Stopwatch.StartNew();
    
    triggerTextChanged(this);
    
    sw.Stop();
    OnTextChangeEventLatencyMs?.Invoke(sw.Elapsed.TotalMilliseconds);
}
```

**Metrics Produced:**
- TextChangeEvent

---

## 5. AlphabetScannerCommon.cs - Word Prediction (DUPLICATE)

**File:** `Libraries\ACATExtension\UI\AlphabetScannerCommon.cs`  
**Methods:** `OnWidgetActuated()`, `refreshWordPredictionsAndSetCurrentWord()`  
**Lines:** 364-399, 588-599  
**Complexity:** ⭐⭐⭐ (3 measurements - DUPLICATES UserControlWordPredictionCommon!)

### Current Implementation:

```csharp
// Word autocomplete
if (e.SourceWidget is WordListItemWidget)
{
    CoreGlobals.Stopwatch1.Reset();
    CoreGlobals.Stopwatch1.Start();
    
    _form.Invoke(new MethodInvoker(delegate
    {
        autoComplete(e.SourceWidget as WordListItemWidget);
    }));
    
    CoreGlobals.Stopwatch1.Stop();
    _logger.LogDebug("TimeElapsed 3 : {ElapsedMs}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
}

// Letter autocomplete
if (e.SourceWidget is LetterListItemWidget)
{
    CoreGlobals.Stopwatch1.Reset();
    CoreGlobals.Stopwatch1.Start();
    
    _form.Invoke(new MethodInvoker(delegate
    {
        autoComplete(e.SourceWidget as LetterListItemWidget);
    }));
    
    CoreGlobals.Stopwatch1.Stop();
    _logger.LogDebug("TimeElapsed 3 : {ElapsedMs}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
}

// Refresh
private void refreshWordPredictionsAndSetCurrentWord()
{
    CoreGlobals.Stopwatch3.Reset();
    CoreGlobals.Stopwatch3.Start();
    
    if (!tryRefreshWordPredictionsAndSetCurrentWord())
    {
        tryRefreshWordPredictionsAndSetCurrentWord();
    }
    
    CoreGlobals.Stopwatch3.Stop();
    _logger.LogDebug("TimeElapsed for tryRefreshWordPredictionsAndSetCurrentWord: " + 
        CoreGlobals.Stopwatch3.ElapsedMilliseconds);
}
```

### Recommended Action:

**Option 1:** Use UserControlWordPredictionCommon instead (preferred)  
**Option 2:** If must keep, use same callbacks:
```csharp
// Use existing callbacks from UserControlWordPredictionCommon:
UserControlWordPredictionCommon.OnAutoCompleteLatencyMs
UserControlWordPredictionCommon.OnRefreshLatencyMs
```

---

## 6. AnimationPlayer.cs - Manual Scan Research (AUDIT)

**File:** `Libraries\ACATCore\AnimationManagement\AnimationPlayer.cs`  
**Lines:** 593-600  
**Complexity:** ⭐ (1 measurement - AUDIT PURPOSE)

### Current Implementation:

```csharp
// Start stopwatch (will stop when actuates)
_stopwatch.Reset();
_stopwatch.Start();

// ...later when user actuates...

// Save past iteration
AuditLog.Audit(new AuditEventManualScanExperiments(
    _prevManualScanMode.ToString(), 
    _highlightedWidget.Panel.ToString(),
    _highlightedWidget.Name, 
    _highlightedWidget.Value, 
    _highlightedWidget.Command, 
    _stopwatch.ElapsedMilliseconds));  // ← Audit data
```

### Recommendation:

**Leave as-is.** This is for research/audit purposes, not performance monitoring.

The elapsed time is logged to `AuditLog` for scientific research on manual scanning behavior, not for application performance optimization.

---

## Supporting Evidence: CoreGlobals Stopwatch Declarations

**File:** `Libraries\ACATCore\Utility\CoreGlobals.cs`  
**Lines:** 22-30

```csharp
public static Stopwatch Stopwatch1 = new();  // Used in 5+ places
public static Stopwatch Stopwatch2 = new();  // Used in EditTextControlAgent
public static Stopwatch Stopwatch3 = new();  // Used in AlphabetScannerCommon, UserControlWordPredictionCommon (converted)
public static Stopwatch Stopwatch4 = new();  // Used extensively in TextController
public static Stopwatch Stopwatch5 = new();  // Used in TextController.Insert
```

**Problem:**
- ❌ Shared static instances across all threads
- ❌ Race conditions possible
- ❌ One operation can overwrite another's timing
- ❌ Not scalable

**Solution:**
- ✅ Convert to local stopwatch instances
- ✅ Use callbacks for metrics collection
- ✅ Thread-safe by design

---

## Conversion Impact Analysis

### Files Using CoreGlobals.Stopwatch:

| File | Stopwatches | Measurements | Complexity | Priority |
|------|-------------|--------------|------------|----------|
| TextController.cs | 4, 5 | 5 | ⭐⭐⭐⭐⭐ | 🔴 HIGH |
| UserControlKeyboardCommon.cs | 1 | 2 | ⭐⭐ | 🔴 HIGH |
| ScannerCommon.cs | 1 | 2 | ⭐⭐ | 🔴 HIGH |
| AlphabetScannerCommon.cs | 1, 3 | 3 | ⭐⭐⭐ | 🟡 MEDIUM |
| EditTextControlAgent.cs | 2 | 1 | ⭐ | 🟡 MEDIUM |

**Total:** 5 files, ~13 measurements

### Conversion Effort Estimate:

| Task | Time | Difficulty |
|------|------|------------|
| TextController (5 measurements) | 45-60 min | Hard |
| UserControlKeyboardCommon (2) | 15-20 min | Easy |
| ScannerCommon (2) | 15-20 min | Easy |
| EditTextControlAgent (1) | 10-15 min | Easy |
| AlphabetScannerCommon (3) | 30-45 min | Medium |
| **Total** | **~2-3 hours** | Medium |

Plus integration/testing: +1 hour

---

## Suggested Callback Architecture

### Shared Callbacks (Multiple Files):

```csharp
// For key actuation (UserControlKeyboardCommon + ScannerCommon)
public static class KeyboardMetrics
{
    public static Action<string, double> OnKeyActuationLatencyMs;
}

// Usage in both files:
KeyboardMetrics.OnKeyActuationLatencyMs?.Invoke("SingleKey", ms);
```

### Per-Component Callbacks:

```csharp
// TextController.cs
public static Action<string, double> OnAutoCompletePhaseLatencyMs;

// EditTextControlAgent.cs
public static Action<double> OnTextChangeEventLatencyMs;

// AlphabetScannerCommon.cs (if kept separate)
// Use UserControlWordPredictionCommon callbacks
```

---

## Thread Safety Analysis

### Current State (CoreGlobals.Stopwatch):

```
Thread 1: Stopwatch1.Start() → Operation A → Stopwatch1.Stop()
Thread 2:                   Stopwatch1.Start() → Operation B → Stopwatch1.Stop()
                                   ↑
                            RACE CONDITION! ❌
                   Thread 2 resets while Thread 1 is measuring
```

### After Conversion (Local Stopwatch):

```
Thread 1: sw1.Start() → Operation A → sw1.Stop()
Thread 2: sw2.Start() → Operation B → sw2.Stop()
                ↑
         NO INTERFERENCE ✅
      Each thread has its own stopwatch
```

---

## Integration Example

After converting all files:

```csharp
// In ACATTalk/Program.cs Main()
#if PERFORMANCE
    // Text input metrics
    KeyboardMetrics.OnKeyActuationLatencyMs = (type, ms) =>
        PerformanceMonitor.RecordMetric($"KeyActuation_{type}", ms, "ms", 
            PerformanceMonitor.MetricCategory.Interaction);
    
    // Autocomplete phases
    TextController.OnAutoCompletePhaseLatencyMs = (phase, ms) =>
        PerformanceMonitor.RecordMetric($"AutoComplete_{phase}", ms, "ms", 
            PerformanceMonitor.MetricCategory.TextPrediction);
    
    // Text change events
    EditTextControlAgent.OnTextChangeEventLatencyMs = (ms) =>
        PerformanceMonitor.RecordMetric("TextChangeEvent", ms, "ms", 
            PerformanceMonitor.MetricCategory.Interaction);
    
    // Word prediction (already done)
    UserControlWordPredictionCommon.OnPredictionLatencyMs = (ms) =>
        PerformanceMonitor.RecordMetric("WordPredictorCall", ms, "ms", 
            PerformanceMonitor.MetricCategory.TextPrediction);
#endif
```

---

## Metrics That Will Be Available

After full conversion:

### TextPrediction Category:
- `WordPredictorCall` - AI/ML engine call ✅ (DONE)
- `PredictionRefresh` - Full refresh cycle ✅ (DONE)
- `AutoComplete_GetPrevWord` - Get word to replace
- `AutoComplete_CheckInsertReplace` - Determine operation type
- `AutoComplete_Insert` - Insert new word
- `AutoComplete_Replace` - Replace existing word
- `AutoComplete_PostCompletion` - Finalization

### Interaction Category:
- `AutoCompleteInsert` - Word selection ✅ (DONE)
- `KeyActuation_SingleKey` - Single key press
- `KeyActuation_MultiChar` - Multi-char string
- `TextChangeEvent` - Windows automation callback

---

## Files NOT Needing Conversion

### Utilities and Infrastructure:
- `MicroSecondTimer.cs` - High-precision timer utility
- `PerfMon.cs` - System-level PerformanceCounter monitoring
- `PerformanceMonitor.cs` - The monitoring system itself

### Tests:
- `BaseTest.cs` - Test timing infrastructure
- `AssertHelper.cs` - Test assertions with timeouts

### BCI/Research:
- `DimReductRDA.cs` - ML algorithm timing (domain-specific)
- `gTecDeviceTester.cs` - Hardware testing
- `OpenBCIDeviceTester.cs` - Hardware testing
- `UserControlBCISignalCheck.cs` - Signal quality checks

### Audit/Research:
- `AnimationPlayer.cs` - Manual scan research data
- Timers used for audit logging, not performance

---

## Code Smells Identified

### 1. Duplicate Code
- UserControlKeyboardCommon.cs ↔ ScannerCommon.cs (IDENTICAL)
- AlphabetScannerCommon.cs ↔ UserControlWordPredictionCommon.cs (SIMILAR)

### 2. Overuse of Shared Stopwatches
- 5 static stopwatches when local instances are safer
- Thread safety concerns
- Confusing naming (Stopwatch1-5)

### 3. Inconsistent Patterns
- Some use callbacks (modern)
- Some use direct logging (legacy)
- Some use both shared and local stopwatches

---

## Recommended Conversion Order

### Phase 1 (High Value):
1. ✅ **UserControlWordPredictionCommon** - DONE
2. **TextController.AutoCompleteWord** - Complex but high-impact
3. **UserControlKeyboardCommon + ScannerCommon** - Deduplicate first

### Phase 2 (Cleanup):
4. **EditTextControlAgent** - Simple conversion
5. **AlphabetScannerCommon** - Investigate if it can use UserControlWordPredictionCommon

### Phase 3 (Optional):
6. Consider deprecating CoreGlobals.Stopwatch1-5 once all conversions complete
7. Add compiler warnings for CoreGlobals.Stopwatch usage

---

## Benefits of Full Conversion

After converting all instances:

✅ **Thread Safety** - No more race conditions  
✅ **Flexibility** - Metrics can go to logs, monitors, telemetry, dashboards  
✅ **Consistency** - Unified pattern across codebase  
✅ **Performance** - Zero overhead when callbacks not set  
✅ **Testability** - Easy to mock and verify  
✅ **Code Quality** - Remove duplicate code  
✅ **Maintainability** - Clear, modern patterns

---

## Total Impact

**Lines of code affected:** ~50-70 lines across 5 files  
**Callbacks to create:** ~3-4 new callbacks  
**Duplicate code to consolidate:** 2-3 instances  
**Estimated effort:** 3-4 hours for full conversion + testing  
**Risk:** Low (similar pattern already proven in UserControlWordPredictionCommon)

---

## Next Action Items

1. **Review this inventory** and prioritize
2. **Decide on callback architecture** (shared vs per-component)
3. **Convert TextController.AutoCompleteWord** (highest complexity)
4. **Consolidate duplicate code** (UserControlKeyboardCommon/ScannerCommon)
5. **Investigate AlphabetScannerCommon** redundancy
6. **Wire up all callbacks** in Program.cs
7. **Test thoroughly** to ensure no functional regressions
8. **Update documentation** with new metrics available
