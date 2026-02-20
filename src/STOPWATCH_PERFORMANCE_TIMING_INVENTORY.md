# Inventory: Stopwatch Performance Timing Patterns in ACAT

## Summary

This document catalogs all instances of performance timing using stopwatches that could be converted to callback patterns similar to what was done in `UserControlWordPredictionCommon.cs`.

---

## Category 1: CoreGlobals.Stopwatch Usage (High Priority)

These use shared stopwatches from `CoreGlobals` which are NOT thread-safe and should be converted to local stopwatches with callbacks.

### 1.1 TextController.cs - AutoComplete Operations
**File:** `Libraries\ACATCore\PanelManagement\Common\TextController.cs`  
**Method:** `AutoCompleteWord(string wordSelected)`  
**Lines:** 117-214

**Stopwatches Used:**
- `CoreGlobals.Stopwatch4` - Multiple phases of autocomplete (4 measurements)
- `CoreGlobals.Stopwatch5` - Text insertion operation

**What's Measured:**
- **TimeElapsed 1** (lines 117-129): Getting previous word offset
- **TimeElapsed 2** (lines 131-149): Checking insert vs replace operation
- **TimeElapsed 3** (lines 199-200): Text replacement/insertion
- **Insert operation TimeElapsed** (lines 170-176): Specific to Insert path
- **TimeElapsed 4** (lines 213-214): Post-autocomplete operations

**Suggested Callback:**
```csharp
public static Action<string, double> OnAutoCompletePhaseLatencyMs;
// Usage: OnAutoCompletePhaseLatencyMs?.Invoke("GetPrevWord", ms);
```

---

### 1.2 UserControlKeyboardCommon.cs - Key Actuation
**File:** `Libraries\ACATCore\UserControlManagement\UserControlKeyboardCommon.cs`  
**Method:** `buttonActuated(Widget widget, ref bool handled)`  
**Lines:** 69-86

**Stopwatches Used:**
- `CoreGlobals.Stopwatch1` - Two separate measurements

**What's Measured:**
- **TimeElapsed 1** (lines 69-75): SendKeys.SendWait for multi-character strings
- **TimeElapsed 2** (lines 79-85): Single key actuation

**Suggested Callback:**
```csharp
public static Action<string, double> OnKeyActuationLatencyMs;
// string parameter: "MultiChar" or "SingleKey"
```

---

### 1.3 AlphabetScannerCommon.cs - Word/Letter Selection
**File:** `Libraries\ACATExtension\UI\AlphabetScannerCommon.cs`  
**Method:** `OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)`  
**Lines:** 364-399

**Stopwatches Used:**
- `CoreGlobals.Stopwatch1` - Two measurements (word and letter autocomplete)
- `CoreGlobals.Stopwatch3` - Refresh operations

**What's Measured:**
- **TimeElapsed 3** (lines 364-374): Word autocomplete invocation
- **TimeElapsed 3** (lines 384-394): Letter autocomplete invocation
- **tryRefreshWordPredictionsAndSetCurrentWord** (lines 588-599): Full refresh

**Note:** ⚠️ This appears to be **duplicate code** with `UserControlWordPredictionCommon.cs`!  
The newer `UserControlWordPredictionCommon` already has callbacks. This file may need to use that instead or be deprecated.

---

### 1.4 EditTextControlAgent.cs - Text Change Events
**File:** `Libraries\ACATCore\AgentManagement\TextControlAgents\EditTextControlAgent.cs`  
**Method:** `onTextChanged(object sender, AutomationEventArgs e)`  
**Lines:** 209-215

**Stopwatches Used:**
- `CoreGlobals.Stopwatch2`

**What's Measured:**
- **onTextChanged() TimeElapsed** (lines 209-215): Windows automation text change callback

**Suggested Callback:**
```csharp
public static Action<double> OnTextChangeEventLatencyMs;
```

---

### 1.5 ScannerCommon.cs - Key Actuation (Scanner Level)
**File:** `Libraries\ACATCore\PanelManagement\Common\ScannerCommon.cs`  
**Method:** `buttonActuated(Widget widget, ref bool handled)`  
**Lines:** 878-894

**Stopwatches Used:**
- `CoreGlobals.Stopwatch1` - Two measurements

**What's Measured:**
- **TimeElapsed 1** (lines 878-884): SendKeys.SendWait for multi-character
- **TimeElapsed 2** (lines 888-894): Single key actuation

**Note:** ⚠️ This appears to be **duplicate code** with `UserControlKeyboardCommon.cs`!  
Consider consolidating or using shared callback.

---

## Category 2: Local Stopwatch with Direct Logging (Medium Priority)

These use local stopwatch instances but log directly instead of using callbacks.

### 2.1 TalkApplicationScanner.cs - UI Key Press
**File:** `Extensions\ACAT.Extensions.UI\Scanners\TalkApplicationScanner.cs`  
**Method:** `TextBoxTalkWindowOnKeyPress(object sender, KeyPressEventArgs e)`  
**Lines:** 351-389

**Stopwatch:** Local `swUi`

**What's Measured:**
- Key press handling + TTS invocation

**Status:** ✅ **Already uses callback!**
```csharp
finally
{
    swUi.Stop();
    OnUiKeyPressLatencyMs?.Invoke(swUi.Elapsed.TotalMilliseconds);
}
```

---

### 2.2 AnimationPlayer.cs - Manual Scan Timing
**File:** `Libraries\ACATCore\AnimationManagement\AnimationPlayer.cs`  
**Lines:** 593-600

**Stopwatch:** Instance field `_stopwatch`

**What's Measured:**
- Time between manual scan iterations (for audit logging)

**Usage:**
```csharp
_stopwatch.Reset();
_stopwatch.Start();
// ...later...
AuditLog.Audit(new AuditEventManualScanExperiments(..., _stopwatch.ElapsedMilliseconds));
```

**Note:** This is for **audit/research purposes** (manual scan experiments), not performance monitoring.  
**Recommendation:** Leave as-is unless audit system needs callbacks.

---

## Category 3: Performance Counters (Different Pattern)

### 3.1 PerfMon.cs - System Performance Monitoring
**File:** `Libraries\ACATCore\Utility\PerfMon.cs`

**What's Measured:**
- Free memory, committed memory, private bytes, page file bytes
- Handle count, CPU utilization
- Uses `PerformanceCounter` class, not `Stopwatch`

**Purpose:** System-level performance monitoring (CSV logging)

**Recommendation:** This is a different monitoring system. Leave as-is or integrate with new PerformanceMonitor if desired.

---

## Category 4: Already Using Callbacks (Reference)

### 4.1 UserControlWordPredictionCommon.cs ✅
**File:** `Libraries\ACATExtension\UI\UserControlWordPredictionCommon.cs`

**Status:** ✅ **Already converted!**

**Callbacks:**
- `OnPredictionLatencyMs` - Word predictor call
- `OnAutoCompleteLatencyMs` - Autocomplete insertion  
- `OnRefreshLatencyMs` - Full prediction refresh

**Integration:** Already wired up in ACATTalk (PerformanceMonitor) and ACATApp (diagnostic logging)

---

## Recommended Conversion Priority

### 🔴 HIGH PRIORITY - Shared Stopwatches (Thread Safety Issues)

1. **TextController.AutoCompleteWord** - Most complex, 5 measurements
   - Suggested: `OnAutoCompletePhaseLatencyMs(phase, ms)`
   
2. **UserControlKeyboardCommon.buttonActuated** - Duplicate with ScannerCommon
   - Suggested: `OnKeyActuationLatencyMs(type, ms)`
   
3. **ScannerCommon.buttonActuated** - Duplicate with UserControlKeyboardCommon
   - Suggested: Use same callback as UserControlKeyboardCommon
   
4. **EditTextControlAgent.onTextChanged** - Text change event timing
   - Suggested: `OnTextChangeEventLatencyMs(ms)`

### 🟡 MEDIUM PRIORITY - Duplicate Code

5. **AlphabetScannerCommon** - Appears to duplicate UserControlWordPredictionCommon
   - **Recommendation:** Investigate if this can use UserControlWordPredictionCommon instead
   - If not, use same callbacks as UserControlWordPredictionCommon

### 🟢 LOW PRIORITY - Special Purpose

6. **AnimationPlayer** - Research/audit timing, not performance monitoring
   - Leave as-is unless audit system needs integration

7. **PerfMon** - System monitoring, different purpose
   - Consider integration with new PerformanceMonitor framework later

---

## Detailed Analysis: TextController.AutoCompleteWord

This is the **most complex case** with 5 different measurements:

```csharp
// Phase 1: Get previous word offset
CoreGlobals.Stopwatch4.Start();
context.TextAgent().GetPrevWordOffsetAutoComplete(out int offset, out int count);
CoreGlobals.Stopwatch4.Stop();
// Log: "AutoComplete TimeElapsed 1"

// Phase 2: Check insert vs replace
CoreGlobals.Stopwatch4.Start();
bool checkInsert = context.TextAgent().CheckInsertOrReplaceWord(out int insertOrReplaceOffset, out string wordToReplace);
CoreGlobals.Stopwatch4.Stop();
// Log: "AutoComplete TimeElapsed 2"

// Phase 3: Insert operation (conditional)
if (checkInsert)
{
    CoreGlobals.Stopwatch5.Start();
    context.TextAgent().Insert(insertOrReplaceOffset, wordSelected);
    CoreGlobals.Stopwatch5.Stop();
    // Log: "AutoComplete Insert operation TimeElapsed"
}
else
{
    // Replace operation (no timing)
}

// Phase 4: Overall completion
CoreGlobals.Stopwatch4.Start();
// ...post-autocomplete operations...
CoreGlobals.Stopwatch4.Stop();
// Log: "AutoComplete TimeElapsed 3"

// Phase 5: Get final caret position
CoreGlobals.Stopwatch4.Start();
_autoCompleteCaretPos = context.TextAgent().GetCaretPos();
CoreGlobals.Stopwatch4.Stop();
// Log: "AutoComplete TimeElapsed 4"
```

**Suggested Refactoring:**

```csharp
public static Action<string, double> OnAutoCompletePhaseLatencyMs;

// Usage:
var sw = Stopwatch.StartNew();
context.TextAgent().GetPrevWordOffsetAutoComplete(out int offset, out int count);
sw.Stop();
OnAutoCompletePhaseLatencyMs?.Invoke("GetPrevWord", sw.Elapsed.TotalMilliseconds);

sw = Stopwatch.StartNew();
bool checkInsert = context.TextAgent().CheckInsertOrReplaceWord(...);
sw.Stop();
OnAutoCompletePhaseLatencyMs?.Invoke("CheckInsertReplace", sw.Elapsed.TotalMilliseconds);

// etc.
```

---

## Code Duplication Issues

### Issue 1: AlphabetScannerCommon vs UserControlWordPredictionCommon

Both files have nearly identical autocomplete and refresh logic:

**AlphabetScannerCommon.cs:**
```csharp
if (e.SourceWidget is WordListItemWidget)
{
    CoreGlobals.Stopwatch1.Start();
    autoComplete(e.SourceWidget as WordListItemWidget);
    CoreGlobals.Stopwatch1.Stop();
    // Log elapsed
}

CoreGlobals.Stopwatch3.Start();
tryRefreshWordPredictionsAndSetCurrentWord();
CoreGlobals.Stopwatch3.Stop();
```

**UserControlWordPredictionCommon.cs:**
```csharp
if (e.SourceWidget is WordListItemWidget)
{
    var sw = Stopwatch.StartNew();
    autoComplete(e.SourceWidget as WordListItemWidget);
    sw.Stop();
    OnAutoCompleteLatencyMs?.Invoke(sw.Elapsed.TotalMilliseconds);
}

var sw = Stopwatch.StartNew();
tryRefreshWordPredictionsAndSetCurrentWord();
sw.Stop();
OnRefreshLatencyMs?.Invoke(sw.Elapsed.TotalMilliseconds);
```

**Recommendation:** Deprecate AlphabetScannerCommon or refactor it to use UserControlWordPredictionCommon.

### Issue 2: UserControlKeyboardCommon vs ScannerCommon

Both have identical key actuation timing logic:

```csharp
// Both files have this pattern:
CoreGlobals.Stopwatch1.Reset();
CoreGlobals.Stopwatch1.Start();
SendKeys.SendWait(widget.Value + " ");  // or actuateKey()
CoreGlobals.Stopwatch1.Stop();
_logger?.LogDebug("TimeElapsed: {Ms}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
```

**Recommendation:** Create shared callback in one location, have both use it.

---

## Statistics

| Category | Count | Thread-Safe? | Uses Callback? |
|----------|-------|--------------|----------------|
| CoreGlobals.Stopwatch | 5 files | ❌ No | ❌ No |
| Local Stopwatch + Log | 2 files | ✅ Yes | ❌ No |
| Local Stopwatch + Callback | 1 file | ✅ Yes | ✅ Yes |
| Audit/Research | 1 file | ✅ Yes | N/A |
| Performance Counters | 1 file | N/A | N/A |

**Total instances to convert:** ~4-5 files (depending on deduplication)

---

## Conversion Template

For each CoreGlobals.Stopwatch usage:

### Before:
```csharp
CoreGlobals.Stopwatch1.Reset();
CoreGlobals.Stopwatch1.Start();

// ... operation ...

CoreGlobals.Stopwatch1.Stop();
_logger?.LogDebug("TimeElapsed: {Ms}", CoreGlobals.Stopwatch1.ElapsedMilliseconds);
```

### After:
```csharp
var sw = Stopwatch.StartNew();

// ... operation ...

sw.Stop();
OnOperationLatencyMs?.Invoke(sw.Elapsed.TotalMilliseconds);
```

### With Phase Tracking:
```csharp
var sw = Stopwatch.StartNew();

// ... operation ...

sw.Stop();
OnOperationPhaseLatencyMs?.Invoke("PhaseName", sw.Elapsed.TotalMilliseconds);
```

---

## Integration with PerformanceMonitor

After conversion, wire up in `Program.cs`:

```csharp
#if PERFORMANCE
    // Text input performance
    UserControlKeyboardCommon.OnKeyActuationLatencyMs = (type, ms) =>
        PerformanceMonitor.RecordMetric($"KeyActuation_{type}", ms, "ms", 
            PerformanceMonitor.MetricCategory.Interaction);
    
    TextController.OnAutoCompletePhaseLatencyMs = (phase, ms) =>
        PerformanceMonitor.RecordMetric($"AutoComplete_{phase}", ms, "ms", 
            PerformanceMonitor.MetricCategory.TextPrediction);
    
    // Text agent performance
    EditTextControlAgent.OnTextChangeEventLatencyMs = (ms) =>
        PerformanceMonitor.RecordMetric("TextChangeEvent", ms, "ms", 
            PerformanceMonitor.MetricCategory.Interaction);
#endif
```

---

## Files Not Needing Conversion

These files mention "Stopwatch" or "Elapsed" but are NOT performance timing:

- **MicroSecondTimer.cs** - High-precision timer utility class
- **Test files** - Test infrastructure timing
- **PerformanceMonitor.cs** - Already part of new monitoring system
- **RuntimeMetricsCollector.cs** - Already part of new monitoring system
- **MemoryProfiler.cs** - Already part of new monitoring system
- **Timer callbacks** (timer_Elapsed) - Not stopwatch timing
- **BCI signal processing** - Domain-specific timing, not app performance

---

## Next Steps

1. ✅ **Done:** UserControlWordPredictionCommon converted
2. **Priority 1:** Convert TextController.AutoCompleteWord (5 measurements)
3. **Priority 2:** Consolidate UserControlKeyboardCommon + ScannerCommon (duplicate code)
4. **Priority 3:** Convert EditTextControlAgent.onTextChanged
5. **Investigation:** Determine if AlphabetScannerCommon can use UserControlWordPredictionCommon
6. **Wire up:** Integrate all callbacks with PerformanceMonitor in Program.cs

---

## Summary

**Total actionable items:** 4-5 files need conversion  
**Estimated effort:** 2-4 hours  
**Primary benefit:** Thread-safe, flexible performance monitoring  
**Secondary benefit:** Code deduplication opportunities

All instances follow similar patterns and can use the same conversion template established with UserControlWordPredictionCommon.cs.
