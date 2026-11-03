# User Control Logger Initialization Fix

## Problem

The `_logger` field in `SentencePredictionUserControl` was null when the `textToSpeech()` method was called, causing logging to fail silently when using the null-conditional operator (`_logger?.LogDebug`).

## Root Cause

The `_logger` field was declared in the base class `GenericUserControl` but **never initialized**:

```csharp
// GenericUserControl.cs
protected ILogger _logger;  // Declared but never initialized!
```

### Why It Was Null

User controls in WPF/WinForms are instantiated through the designer/XAML, not through a DI container, so there's no automatic constructor injection. The `Initialize()` method was called after construction but didn't set up the logger.

## Solution

Added logger initialization to the `Initialize()` method in both base classes:

### 1. GenericUserControl.cs
```csharp
public virtual bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
{
    // Initialize logger for this user control
    _logger = LogManager.GetLogger(GetType());
    
    _userControlCommon = new UserControlCommon(this, mapEntry, scanner);
    // ... rest of initialization
}
```

### 2. KeyboardUserControl.cs
```csharp
public override bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
{
    // Initialize logger for this user control
    _logger = LogManager.GetLogger(GetType());
    
    _userControlCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);
    // ... rest of initialization
}
```

### 3. SentencePredictionUserControl.cs

Removed the null-conditional operators since logger is now guaranteed to be initialized:

```csharp
private void textToSpeech(String text)
{
    if (!String.IsNullOrEmpty(text))
    {
        _logger.LogDebug("*** TTS *** : {Text}", text);  // No more ?. operator
        TTSManager.Instance.ActiveEngine.Speak(text);
        _logger.LogDebug("*** TTS *** : sent text!");
        // ...
    }
}
```

## Impact

- ✅ **All user controls** inheriting from `GenericUserControl` or `KeyboardUserControl` now have properly initialized loggers
- ✅ Logging works correctly in all user control methods
- ✅ No more silent logging failures
- ✅ Type-specific loggers (e.g., `ILogger<SentencePredictionUserControl>`) for better log categorization

## Files Changed

1. `Libraries/ACATExtension/UI/UserControls/GenericUserControl.cs`
2. `Libraries/ACATExtension/UI/UserControls/KeyboardUserControl.cs`
3. `Extensions/ACAT.Extensions.UI/UserControls/SentencePredictionUserControl.cs`

## Testing Recommendations

1. Verify logging appears for all user control operations
2. Check that user control lifecycle (Initialize → OnLoad → OnPause → OnResume) logs correctly
3. Confirm no null reference exceptions in user control logging code
4. Verify TTS logging in `SentencePredictionUserControl.textToSpeech()`

## Related Work

This fix is part of Phase 2 DI infrastructure work, specifically improving logging consistency across the application. User controls present a unique challenge because they're not created through DI, so they need explicit logger initialization during their initialization lifecycle.
