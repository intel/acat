# Section 3.2.1 Implementation Complete: PanelManager EventBus Publishing

## Summary

Successfully implemented **Section 3.2.1** from ARCHITECTURE_IMPLEMENTATION_STATUS.md. PanelManager and PanelStack now publish panel lifecycle events to the EventBus alongside the legacy event delegates.

## Changes Made

### 1. PanelManager.cs

**Added:**
- `using ACAT.Core.EventManagement;` namespace
- `private readonly IEventBus _eventBus;` field
- IEventBus parameter to constructor (optional, may be null)
- IEventBus resolution in singleton lazy initialization
- Event publishing in `NotifyPanelPreShow()` method

**Modified Methods:**
- **Constructor:** Now accepts `ILogger<PanelManager> logger, IEventBus eventBus = null`
- **_instance (Lazy):** Resolves IEventBus from Context.ServiceProvider during initialization
- **NotifyPanelPreShow():** Publishes `PanelShowEvent` to EventBus after firing legacy event
- **createPanelStack():** Passes IEventBus to PanelStack constructor

**Code Added:**
```csharp
// In lazy initialization
IEventBus eventBus = Context.ServiceProvider?.GetService(typeof(IEventBus)) as IEventBus;
return new PanelManager(logger, eventBus);

// In NotifyPanelPreShow
if (_eventBus != null && arg.Panel is IScannerPanel scanner)
{
    var panelClass = scanner.PanelClass ?? arg.Panel.GetType().Name;
    _eventBus.Publish(new PanelShowEvent(panelClass));
    _logger?.LogTrace($"Published PanelShowEvent for {panelClass}");
}
```

### 2. PanelStack.cs

**Added:**
- `using ACAT.Core.EventManagement;` namespace
- `private readonly IEventBus _eventBus;` field
- IEventBus parameter to constructor (optional, may be null)
- `publishPanelShowEvent(IPanel panel)` private helper method
- `publishPanelHideEvent(IPanel panel)` private helper method
- Event publishing at 4 show points and 1 hide point

**Modified Methods:**
- **Constructor:** Now accepts `ILogger<PanelStack> logger = null, IEventBus eventBus = null`
- **show():** Publishes `PanelShowEvent` after Windows.ShowDialog/Show/ShowForm calls (4 locations)
- **panel_FormClosed():** Publishes `PanelHideEvent` when panel is closed

**Code Added:**
```csharp
// Helper methods
private void publishPanelShowEvent(IPanel panel)
{
    if (_eventBus != null && panel is IScannerPanel scanner)
    {
        var panelClass = scanner.PanelClass ?? panel.GetType().Name;
        _eventBus.Publish(new PanelShowEvent(panelClass));
        _logger?.LogTrace($"Published PanelShowEvent for {panelClass}");
    }
}

private void publishPanelHideEvent(IPanel panel)
{
    if (_eventBus != null && panel is IScannerPanel scanner)
    {
        var panelClass = scanner.PanelClass ?? panel.GetType().Name;
        _eventBus.Publish(new PanelHideEvent(panelClass));
        _logger?.LogTrace($"Published PanelHideEvent for {panelClass}");
    }
}

// Called after panel shows
publishPanelShowEvent(panel);

// Called when panel closes
publishPanelHideEvent(panel);
```

## Event Publishing Points

### PanelShowEvent Published:
1. **Normal Show with Parent** - After `Windows.Show(parentForm, panelForm)`
2. **Dialog with Parent** - After `Windows.ShowDialog(parentForm, panelForm)`
3. **Normal Show without Parent** - After `Windows.ShowForm(panelForm)`
4. **Dialog without Parent** - After `panelForm.ShowDialog()`

### PanelHideEvent Published:
1. **Panel FormClosed** - In `panel_FormClosed()` after audit log and before closing owned forms

## Backward Compatibility

✅ **100% Backward Compatible:**
- Legacy `EvtPanelPreShow` delegate still fires
- EventBus publishing is **optional** (works if IEventBus is null)
- No breaking changes to existing APIs
- All existing code continues to work

## Gradual Migration Path

The implementation supports gradual migration:

### Phase 1 (Current):
- ✅ Legacy events fire (existing subscribers work)
- ✅ EventBus events publish (new subscribers can use them)
- ✅ Both systems work side-by-side

### Phase 2 (Future):
- New code subscribes to EventBus events
- Old code continues using legacy delegates
- No breaking changes

### Phase 3 (Long-term):
- Gradually migrate legacy subscribers to EventBus
- Eventually deprecate legacy events

## How to Subscribe

### Using EventBus (New Code):

```csharp
public class MyComponent
{
    private readonly IEventBus _eventBus;
    
    public MyComponent(IEventBus eventBus)
    {
        _eventBus = eventBus;
        
        // Subscribe to panel show events
        _eventBus.Subscribe<PanelShowEvent>(OnPanelShow);
        
        // Subscribe to panel hide events
        _eventBus.Subscribe<PanelHideEvent>(OnPanelHide);
    }
    
    private void OnPanelShow(PanelShowEvent evt)
    {
        Console.WriteLine($"Panel shown: {evt.PanelClass} at {evt.Timestamp}");
    }
    
    private void OnPanelHide(PanelHideEvent evt)
    {
        Console.WriteLine($"Panel hidden: {evt.PanelClass} at {evt.Timestamp}");
    }
    
    public void Dispose()
    {
        _eventBus.Unsubscribe<PanelShowEvent>(OnPanelShow);
        _eventBus.Unsubscribe<PanelHideEvent>(OnPanelHide);
    }
}
```

### Using Legacy Events (Existing Code - Still Works):

```csharp
// Still works - no changes needed
Context.AppPanelManager.EvtPanelPreShow += OnPanelPreShow;

private void OnPanelPreShow(object sender, PanelPreShowEventArg e)
{
    // Existing code continues to work
}
```

## Benefits

### 1. Decoupling
- Subscribers don't need direct references to PanelManager
- Loosely coupled via EventBus interface
- Easy to add new subscribers without modifying PanelManager

### 2. Testability
- EventBus can be mocked in unit tests
- Easy to verify events are published
- No need for complex test setups

### 3. Flexibility
- Multiple subscribers can listen to same events
- Subscribers can be added/removed at runtime
- Weak references prevent memory leaks

### 4. Consistency
- Follows modern event-driven architecture
- Consistent with CQRS and Repository patterns
- Aligns with architectural modernization goals

## Testing

### Verify EventBus Integration:

```csharp
// In a test or at application startup
var eventBus = Context.ServiceProvider.GetRequiredService<IEventBus>();

// Subscribe to events
int showCount = 0;
int hideCount = 0;

eventBus.Subscribe<PanelShowEvent>(evt => {
    showCount++;
    Console.WriteLine($"Panel shown: {evt.PanelClass}");
});

eventBus.Subscribe<PanelHideEvent>(evt => {
    hideCount++;
    Console.WriteLine($"Panel hidden: {evt.PanelClass}");
});

// Show and close a panel
var panel = Context.AppPanelManager.CreatePanel("TestPanel");
Context.AppPanelManager.Show(panel);
// showCount should be 1

Context.AppPanelManager.CloseCurrentPanel();
// hideCount should be 1
```

### Verify Legacy Events Still Work:

```csharp
// Existing code should continue to work
bool legacyEventFired = false;

Context.AppPanelManager.EvtPanelPreShow += (sender, e) => {
    legacyEventFired = true;
};

var panel = Context.AppPanelManager.CreatePanel("TestPanel");
Context.AppPanelManager.Show(panel);

// legacyEventFired should be true
```

## Implementation Notes

### Why Both PanelManager and PanelStack?

- **PanelManager** is the public API (singleton pattern)
- **PanelStack** does the actual show/hide work (internal implementation)
- EventBus must be injected into both:
  - PanelManager receives IEventBus from DI
  - PanelManager passes IEventBus to PanelStack when creating it
  - PanelStack publishes events at the actual show/hide points

### Why Publish After Windows.Show()?

- Events are published **after** the panel is shown to ensure the operation succeeded
- If Windows.Show() throws, no event is published (correct behavior)
- Subscribers receive events only for successful operations

### Why Check for IScannerPanel?

- Only `IScannerPanel` instances have a `PanelClass` property
- PanelClass is the canonical identifier for panel types
- Falls back to `Type.Name` for non-scanner panels

### Thread Safety

- EventBus implementation is thread-safe (uses weak references with locks)
- Panel show/hide operations happen on UI thread
- No additional synchronization needed

## Next Steps (from Architecture Document)

1. ✅ **DONE:** Section 3.1 - Register CQRS handlers in DI
2. ✅ **DONE:** Section 3.2.1 - PanelManager EventBus publishing
3. **TODO:** Section 3.2.2 - ActuatorManager EventBus publishing (~0.5 day)
4. **TODO:** Section 3.2.3 - ConfigurationReloadService EventBus publishing (~0.5 day)
5. **TODO:** Section 3.4.1 - GlobalPreferences repository migration (~0.5 day)

## Files Modified

1. **Libraries\ACATCore\PanelManagement\PanelManager.cs**
   - Added IEventBus field and constructor parameter
   - Modified NotifyPanelPreShow to publish events
   - Modified createPanelStack to pass IEventBus

2. **Libraries\ACATCore\PanelManagement\PanelStack.cs**
   - Added IEventBus field and constructor parameter
   - Added publishPanelShowEvent helper method
   - Added publishPanelHideEvent helper method
   - Modified show() to publish PanelShowEvent (4 locations)
   - Modified panel_FormClosed() to publish PanelHideEvent

## Verification

✅ **Build Status:** Successful  
✅ **No Compilation Errors**  
✅ **Backward Compatible:** Legacy events still work  
✅ **EventBus Integration:** Events published when IEventBus available  
✅ **Null-Safe:** Works correctly when IEventBus is null

## Example Use Case: Performance Monitoring

```csharp
// Monitor panel lifecycle for performance tracking
public class PanelPerformanceMonitor
{
    private readonly Dictionary<string, DateTime> _showTimes = new();
    
    public PanelPerformanceMonitor(IEventBus eventBus)
    {
        eventBus.Subscribe<PanelShowEvent>(OnPanelShow);
        eventBus.Subscribe<PanelHideEvent>(OnPanelHide);
    }
    
    private void OnPanelShow(PanelShowEvent evt)
    {
        _showTimes[evt.PanelClass] = evt.Timestamp;
        Console.WriteLine($"Panel {evt.PanelClass} shown at {evt.Timestamp}");
    }
    
    private void OnPanelHide(PanelHideEvent evt)
    {
        if (_showTimes.TryGetValue(evt.PanelClass, out DateTime showTime))
        {
            var duration = evt.Timestamp - showTime;
            Console.WriteLine($"Panel {evt.PanelClass} was visible for {duration.TotalSeconds:F2}s");
            _showTimes.Remove(evt.PanelClass);
        }
    }
}
```

## Architecture Compliance

✅ **Follows Event-Driven Architecture:**
- Publishers (PanelManager/PanelStack) don't know about subscribers
- Subscribers don't need references to publishers
- Loose coupling via EventBus

✅ **Follows SOLID Principles:**
- Single Responsibility: Each class has one reason to change
- Open/Closed: Can add subscribers without modifying publishers
- Dependency Inversion: Depend on IEventBus abstraction

✅ **Gradual Migration:**
- No breaking changes
- Legacy and modern patterns coexist
- Migration can happen incrementally

---

**Status:** ✅ Section 3.2.1 Complete  
**Build:** ✅ Successful  
**Next:** Section 3.2.2 - ActuatorManager EventBus Publishing
