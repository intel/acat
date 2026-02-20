# Fix: ACATTalk/ACATApp Hanging on Exit in DEBUG Mode

## Problem

**Symptom:** ACATTalk and ACATApp (ACAT Dashboard) hang on exit and never fully shut down when built in DEBUG mode.

**Root Cause:** The `PerformanceDashboard` WPF window is created and shown in DEBUG mode but never properly closed, causing the WPF Dispatcher to keep the application alive indefinitely.

## Investigation

### Code Flow in DEBUG Mode

1. **Application Starts:**
   ```csharp
   #if DEBUG
       var collector = new RuntimeMetricsCollector();
       var profiler = new MemoryProfiler();
       collector.Start(intervalMs: 5000);
       
       var dashboard = new PerformanceDashboard(collector, profiler);
       dashboard.Show();  // ← WPF Window is shown
   #endif
   ```

2. **Application Runs Normally**

3. **Exit Sequence Begins:**
   ```csharp
   Context.Dispose();          // ACAT cleanup
   Common.Uninit();            // More cleanup
   modernLoggingFactory?.Dispose();  // Logger disposal
   AppCommon.OnExit();         // Final cleanup
   ```

4. **Application Hangs:**
   - Main application thread completes
   - WPF Dispatcher thread is still running
   - PerformanceDashboard window is still open
   - **Application never terminates**

### Why WPF Prevents Exit

WPF creates a separate UI thread with a `Dispatcher` that manages the message pump. As long as any WPF window is open, the Dispatcher keeps running, preventing the application from exiting.

In this case:
- `PerformanceDashboard` is a WPF `Window`
- It has a `Timer` that refreshes every 2 seconds
- The window is never explicitly closed
- The Dispatcher never shuts down
- The application hangs waiting for the Dispatcher thread to terminate

## Solution

### Changes Made

#### 1. Track the PerformanceDashboard Instance

```csharp
// Add a static field to hold the dashboard reference
private static Splash splash = null;
private static ILoggerFactory modernLoggingFactory = null;
private static ILogger _logger;
private static IServiceProvider _serviceProvider;
#if DEBUG
private static PerformanceDashboard _performanceDashboard = null;  // ← NEW
#endif
```

#### 2. Store Reference When Creating

```csharp
#if DEBUG
    var collector = new RuntimeMetricsCollector();
    var profiler = new MemoryProfiler();
    collector.Start(intervalMs: 5000);
    
    _performanceDashboard = new PerformanceDashboard(collector, profiler);  // ← Store it
    _performanceDashboard.Show();
#endif
```

#### 3. Explicitly Close and Shutdown Before Exit

**In ACATTalk (Program.cs):**
```csharp
splash = new Splash();
splash.Show(StringResources.ExitingACAT);
AuditLog.Audit(new AuditEvent("Application", "stop"));

#if DEBUG
// Close PerformanceDashboard before disposing Context
if (_performanceDashboard != null)
{
    _performanceDashboard.Dispatcher.InvokeShutdown();  // ← Shutdown dispatcher
    _performanceDashboard.Close();                       // ← Close window
    _performanceDashboard = null;
}
#endif

Context.Dispose();
Common.Uninit();
```

**In ACATApp (Program.cs):**
```csharp
private static void ShutdownApplication()
{
    AuditLog.Audit(new AuditEvent("Application", "stop"));

#if DEBUG
    // Close PerformanceDashboard before disposing Context
    if (_performanceDashboard != null)
    {
        _performanceDashboard.Dispatcher.InvokeShutdown();
        _performanceDashboard.Close();
        _performanceDashboard = null;
    }
#endif

    Context.Dispose();
    Common.Uninit();
    CloseSplashScreen();
    _logger.LogDebug("ACAT Dashboard Application shutdown");
    modernLoggingFactory?.Dispose();
    AppCommon.OnExit();
}
```

## Key Points

### Why Both `InvokeShutdown()` and `Close()` Are Needed

1. **`Dispatcher.InvokeShutdown()`** - Stops the WPF message pump
   - Prevents new messages from being processed
   - Signals the Dispatcher thread to terminate
   - Essential for proper WPF cleanup

2. **`Close()`** - Closes the window itself
   - Disposes the Timer (`Window_Closed` handler)
   - Releases window resources
   - Triggers cleanup logic

### Order Matters

The PerformanceDashboard must be closed **before** `Context.Dispose()` because:
- The dashboard may reference ACAT components
- Those components are cleaned up during `Context.Dispose()`
- Accessing disposed components could cause exceptions
- WPF cleanup should complete before final ACAT cleanup

### Only Affects DEBUG Builds

This fix only applies to DEBUG mode:
- RELEASE builds don't create the PerformanceDashboard
- No WPF window, no hang
- Production builds are unaffected

## Testing

### Before Fix (DEBUG Mode):
```
1. Launch ACATTalk
2. Exit the application
3. ❌ Application appears to close but process remains running
4. ❌ Must be forcefully terminated (Task Manager or Ctrl+C)
```

### After Fix (DEBUG Mode):
```
1. Launch ACATTalk
2. Exit the application
3. ✅ PerformanceDashboard closes automatically
4. ✅ Application terminates cleanly
5. ✅ Process exits immediately
```

### Verify Fix:
```powershell
# Build in DEBUG mode
dotnet build /p:Configuration=Debug

# Run ACATTalk
./build/bin/Debug/ACATTalk.exe

# Exit the application
# Check Task Manager - ACATTalk.exe should be gone
```

## Related Issues

This fix addresses:
- ✅ ACATTalk hanging on exit (DEBUG mode)
- ✅ ACATApp hanging on exit (DEBUG mode)
- ✅ WPF Dispatcher not shutting down
- ✅ Timer resources not being released

## Files Modified

1. **Applications\ACATTalk\Program.cs**
   - Added `_performanceDashboard` static field
   - Store dashboard reference on creation
   - Explicit shutdown and close before exit

2. **Applications\ACATApp\Program.cs**
   - Added `_performanceDashboard` static field
   - Store dashboard reference on creation
   - Explicit shutdown and close in `ShutdownApplication()`

## Prevention

To prevent similar issues in the future:

1. **Always dispose WPF windows** when the application exits
2. **Use `Dispatcher.InvokeShutdown()`** for proper WPF cleanup
3. **Track all UI windows** (WinForms or WPF) at the application level
4. **Test exit scenarios** in both DEBUG and RELEASE modes
5. **Check Task Manager** to verify processes actually terminate

## Additional Notes

### Why This Wasn't Caught Earlier

- RELEASE builds don't include the PerformanceDashboard
- Developers typically test in RELEASE mode for production
- DEBUG-only hanging is less visible in automated testing
- Manual testing often involves forcefully stopping the debugger

### Alternative Solutions Considered

1. ❌ **Call `Application.Exit()`** - Too aggressive, doesn't allow cleanup
2. ❌ **Let finalizers handle it** - Unreliable, may never be called
3. ✅ **Explicit shutdown** - Clean, predictable, proper resource disposal

## Conclusion

The fix ensures clean shutdown in DEBUG mode by:
1. Tracking the WPF PerformanceDashboard window
2. Explicitly shutting down its Dispatcher
3. Closing the window before final cleanup
4. Maintaining proper cleanup order

The application now exits cleanly in both DEBUG and RELEASE configurations.
