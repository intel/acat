# Section 3.3 Implementation Guide: CQRS Wiring at Call Sites

## Overview

This guide provides step-by-step instructions for migrating **196 call sites** to use CQRS command/query handlers instead of direct singleton access. This is the final major task in the architecture modernization effort.

**Estimated Effort:** ~3 days (systematic, with testing)  
**Complexity:** High (requires DI wiring in many classes)  
**Value:** Very High (completes CQRS pattern implementation)

---

## Implementation Strategy

### Phased Approach (Recommended):

**Phase 1: Application Entry Points (Day 1)**
- ACATApp/Program.cs (2 call sites)
- ACATTalk/Program.cs (2 call sites)
- **4 sites, highest value, already have _serviceProvider**

**Phase 2: Extension Handlers (Day 2)**
- CommandHandlers (4 call sites)
- Scanners (1 call site)
- **5 sites, medium complexity**

**Phase 3: Base Classes (Day 2-3)**
- ActuatorBase.cs (2 call sites)
- ScannerCommon.cs, DialogCommon.cs (65 actuator sites)
- **67+ sites, highest leverage**

**Phase 4: Query Migration (Day 3)**
- Agent queries (122 sites)
- **Complex but systematic**

---

## Section 3.3.1: Panel Creation (9 Call Sites)

### Prerequisites

Ensure CQRS handlers are registered (Section 3.1 complete):
```csharp
// In ServiceConfiguration.cs - Already done ✅
services.AddTransient<ICommandHandler<CreatePanelCommand>, CreatePanelCommandHandler>();
```

### Pattern Overview

**Before (Direct Singleton Access):**
```csharp
Form form = PanelManager.Instance.CreatePanel("PanelClass", startupArg);
```

**After (CQRS Command):**
```csharp
// Resolve handler from DI
var createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();

// Execute command
var result = createPanelHandler.Handle(new CreatePanelCommand("PanelClass", startupArg));
Form form = result.Panel as Form;
```

---

## Phase 1: Application Entry Points

### 1.1 ACATApp/Program.cs - Call Site 1 (Line 244)

**Location:** `ShowMainPanel()` method

**Current Code:**
```csharp
private static void ShowMainPanel()
{
    var startupArg = new StartupArg("DashboardAppScanner")
    {
        QuitAppOnFormClose = false
    };

    Form form = PanelManager.Instance.CreatePanel("DashboardAppScanner", startupArg);
    if (form == null)
    {
        MessageBox.Show(string.Format(StringResources.InvalidFormName, startupArg.ToString()));
        return;
    }
    
    // ... rest of method
}
```

**Updated Code:**
```csharp
private static void ShowMainPanel()
{
    var startupArg = new StartupArg("DashboardAppScanner")
    {
        QuitAppOnFormClose = false
    };

    // CQRS: Use command handler instead of direct singleton
    var createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();
    var command = new CreatePanelCommand("DashboardAppScanner", startupArg);
    var result = createPanelHandler.Handle(command);
    
    Form form = result.Panel as Form;
    if (form == null)
    {
        MessageBox.Show(string.Format(StringResources.InvalidFormName, startupArg.ToString()));
        return;
    }
    
    // ... rest of method unchanged
}
```

**Additional Usings Needed:**
```csharp
using ACAT.Core.Patterns.CQRS;
using ACAT.Core.Patterns.CQRS.Samples;
```

---

### 1.2 ACATApp/Program.cs - Call Site 2 (Line 325)

**Location:** `showTalkInterfaceDescription()` method

**Current Code:**
```csharp
private static void showTalkInterfaceDescription()
{
    if (!Common.AppPreferences.ShowTalkInterfaceDescOnStartup)
    {
        return;
    }

    Form form = PanelManager.Instance.CreatePanel("DefaultInterfaceScanner", "ACAT Talk Description");
    if (form != null)
    {
        Context.AppPanelManager.ShowDialog(form as IPanel);
    }
}
```

**Updated Code:**
```csharp
private static void showTalkInterfaceDescription()
{
    if (!Common.AppPreferences.ShowTalkInterfaceDescOnStartup)
    {
        return;
    }

    // CQRS: Use command handler
    var createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();
    var command = new CreatePanelCommand("DefaultInterfaceScanner", "ACAT Talk Description");
    var result = createPanelHandler.Handle(command);
    
    if (result.Panel != null)
    {
        Context.AppPanelManager.ShowDialog(result.Panel);
    }
}
```

---

### 1.3 ACATTalk/Program.cs - Call Site 3 (Line 234)

**Location:** `PostInitialization()` method

**Current Code:**
```csharp
Form form = PanelManager.Instance.CreatePanel("TalkApplicationScanner", startupArg);
if (form != null)
{
    // Add ad-hoc agent that will handle the form
    IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("Talk Application Agent");
    // ... rest
}
```

**Updated Code:**
```csharp
// CQRS: Use command handler
var createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();
var command = new CreatePanelCommand("TalkApplicationScanner", startupArg);
var result = createPanelHandler.Handle(command);

Form form = result.Panel as Form;
if (form != null)
{
    // Add ad-hoc agent that will handle the form
    IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("Talk Application Agent");
    // ... rest unchanged
}
```

**Note:** ACATTalk also needs the same additional usings.

---

### 1.4 ACATTalk/Program.cs - Call Site 4 (Line 346)

**Location:** `showTalkInterfaceDescription()` method

**Current Code:**
```csharp
Form form = PanelManager.Instance.CreatePanel("DefaultInterfaceScanner", "ACAT Talk Description");
```

**Updated Code:**
```csharp
var createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();
var command = new CreatePanelCommand("DefaultInterfaceScanner", "ACAT Talk Description");
var result = createPanelHandler.Handle(command);
Form form = result.Panel as Form;
```

---

## Helper: Extract Handler Resolution to Field

To avoid repeating `GetRequiredService` in every method, extract to a field:

**At Class Level:**
```csharp
internal static class Program
{
    private static Splash splash = null;
    private static ILoggerFactory modernLoggingFactory = null;
    private static ILogger _logger;
    private static IServiceProvider _serviceProvider;
    
    // Add this:
    private static ICommandHandler<CreatePanelCommand> _createPanelHandler;
    
    // ... rest
}
```

**In InitializeDependencyInjection() or after Context initialization:**
```csharp
private static void InitializeDependencyInjection()
{
    // ... existing code ...
    
    // Resolve CQRS handlers once
    _createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();
}
```

**Then simplify calls:**
```csharp
// Instead of:
var createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();
var result = createPanelHandler.Handle(command);

// Use:
var result = _createPanelHandler.Handle(command);
```

---

## Phase 2: Extension Handlers

### 2.1 CommandHandlers Files

**Files to Update:**
- `Libraries\ACATExtension\CommandHandlers\TalkWindowHandler.cs` (line ~44)
- `Libraries\ACATExtension\CommandHandlers\ShowScreenLockHandler.cs` (line ~49)
- Plus 2 more in CommandHandlers directory

**Current Pattern (TalkWindowHandler example):**
```csharp
public class TalkWindowHandler
{
    public void Execute(object arg)
    {
        Form panel = PanelManager.Instance.CreatePanel("TalkApplicationScanner");
        // ...
    }
}
```

**Updated Pattern:**
```csharp
public class TalkWindowHandler
{
    private readonly ICommandHandler<CreatePanelCommand> _createPanelHandler;
    
    // Add constructor injection
    public TalkWindowHandler(ICommandHandler<CreatePanelCommand> createPanelHandler)
    {
        _createPanelHandler = createPanelHandler ?? throw new ArgumentNullException(nameof(createPanelHandler));
    }
    
    public void Execute(object arg)
    {
        var command = new CreatePanelCommand("TalkApplicationScanner");
        var result = _createPanelHandler.Handle(command);
        Form panel = result.Panel as Form;
        // ... rest unchanged
    }
}
```

**Challenge:** These handlers are instantiated via `AgentsCache` which needs DI support.

**Solution:** Update AgentsCache to use ActivatorUtilities (partially done already):

```csharp
// In AgentsCache.cs, when creating handler:
var handler = ActivatorUtilities.CreateInstance<TalkWindowHandler>(Context.ServiceProvider);
```

---

### 2.2 Scanner Files

**File:** `Extensions\ACAT.Extensions.UI\Scanners\DashboardAppScanner.cs` (line ~210)

**Same pattern as CommandHandlers:**
1. Add constructor with `ICommandHandler<CreatePanelCommand>` injection
2. Replace `PanelManager.Instance.CreatePanel` with handler call
3. Ensure scanner is created via ActivatorUtilities

---

## Phase 3: Base Classes (High Leverage)

### 3.1 ActuatorBase.cs (2 Call Sites)

**Files:** `Libraries\ACATCore\ActuatorManagement\BaseActuators\ActuatorBase.cs` (lines ~661, 678)

**Challenge:** ActuatorBase is a base class instantiated by derived actuators. Adding constructor parameters would break all derived classes.

**Solution 1: Property Injection (Recommended for base classes)**
```csharp
public abstract class ActuatorBase : IActuator
{
    // Add property injection point
    protected ICommandHandler<CreatePanelCommand> CreatePanelHandler { get; set; }
    
    protected virtual void InitializeHandlers()
    {
        // Resolve from DI if available
        CreatePanelHandler = Context.ServiceProvider?.GetService(typeof(ICommandHandler<CreatePanelCommand>)) 
            as ICommandHandler<CreatePanelCommand>;
    }
    
    // Then in methods that create panels:
    private void ShowCalibrationPanel()
    {
        if (CreatePanelHandler != null)
        {
            // Use CQRS
            var result = CreatePanelHandler.Handle(new CreatePanelCommand("CalibrationPanel"));
            var form = result.Panel as Form;
        }
        else
        {
            // Fallback to old pattern if DI not available
            var form = PanelManager.Instance.CreatePanel("CalibrationPanel");
        }
    }
}
```

**Solution 2: Service Locator Pattern (Pragmatic for legacy code)**
```csharp
private void ShowCalibrationPanel()
{
    // Try CQRS first, fall back to singleton
    var createPanelHandler = Context.ServiceProvider?.GetService(typeof(ICommandHandler<CreatePanelCommand>)) 
        as ICommandHandler<CreatePanelCommand>;
    
    Form form;
    if (createPanelHandler != null)
    {
        var result = createPanelHandler.Handle(new CreatePanelCommand("CalibrationPanel"));
        form = result.Panel as Form;
    }
    else
    {
        form = PanelManager.Instance.CreatePanel("CalibrationPanel");
    }
}
```

---

## Section 3.3.2: Actuator Pause/Resume (65 Call Sites)

### Pattern Overview

**Before:**
```csharp
Context.AppActuatorManager.Pause();
Context.AppActuatorManager.Resume();
```

**After:**
```csharp
// Using CQRS command
_actuatorSwitchHandler.Handle(new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause));
_actuatorSwitchHandler.Handle(new HandleActuatorSwitchCommand(ActuatorSwitchAction.Resume));
```

### High-Leverage Files

**ScannerCommon.cs** - Base class used by all scanners

**Location:** `Libraries\ACATCore\PanelManagement\Common\ScannerCommon.cs`

**Strategy:**
1. Add property injection (like ActuatorBase solution)
2. Update all Pause/Resume calls
3. **Automatically fixes ~30+ derived scanner classes**

**Implementation:**
```csharp
public class ScannerCommon
{
    // Add handler property
    protected ICommandHandler<HandleActuatorSwitchCommand> ActuatorSwitchHandler { get; set; }
    
    // Initialize in constructor or Initialize method
    protected virtual void InitializeHandlers()
    {
        ActuatorSwitchHandler = Context.ServiceProvider?.GetService(typeof(ICommandHandler<HandleActuatorSwitchCommand>)) 
            as ICommandHandler<HandleActuatorSwitchCommand>;
    }
    
    // Update all methods that pause/resume
    protected void PauseActuator()
    {
        if (ActuatorSwitchHandler != null)
        {
            ActuatorSwitchHandler.Handle(new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause));
        }
        else
        {
            Context.AppActuatorManager.Pause(); // Fallback
        }
    }
    
    protected void ResumeActuator()
    {
        if (ActuatorSwitchHandler != null)
        {
            ActuatorSwitchHandler.Handle(new HandleActuatorSwitchCommand(ActuatorSwitchAction.Resume));
        }
        else
        {
            Context.AppActuatorManager.Resume(); // Fallback
        }
    }
}
```

**Then update all call sites:**
```csharp
// Find and replace in ScannerCommon.cs:
// OLD: Context.AppActuatorManager.Pause()
// NEW: PauseActuator()

// OLD: Context.AppActuatorManager.Resume()
// NEW: ResumeActuator()
```

**DialogCommon.cs** - Similar approach for dialogs (~20 call sites)

**AnimationManager.cs** - Direct injection possible (~15 call sites)

---

## Section 3.3.3: Agent Queries (122 Call Sites)

### Pattern Overview

**Before:**
```csharp
string agentName = Context.AppAgentMgr.GetCurrentAgentName();
```

**After:**
```csharp
var query = new GetActiveAgentNameQuery();
string agentName = _agentQueryHandler.Handle(query);
```

### Strategy

**Option 1: Extension Method (Least Disruptive)**

Create extension method that uses CQRS under the hood:

```csharp
// In new file: Core/AgentManagement/AgentManagerExtensions.cs
public static class AgentManagerExtensions
{
    public static string GetCurrentAgentNameViaQuery(this IAgentManager agentManager)
    {
        var handler = Context.ServiceProvider?.GetService(typeof(IQueryHandler<GetActiveAgentNameQuery, string>)) 
            as IQueryHandler<GetActiveAgentNameQuery, string>;
        
        if (handler != null)
        {
            return handler.Handle(new GetActiveAgentNameQuery());
        }
        
        // Fallback to existing method
        return agentManager.GetCurrentAgentName();
    }
}
```

**Then gradually replace:**
```csharp
// Phase 1: Add new calls as .GetCurrentAgentNameViaQuery()
// Phase 2: Later, rename GetCurrentAgentName to GetCurrentAgentNameLegacy
// Phase 3: Rename GetCurrentAgentNameViaQuery to GetCurrentAgentName
```

**Option 2: Direct Replacement (More invasive)**

Find all 122 call sites and replace directly. Best done with IDE refactoring tools.

---

## Testing Strategy

### After Each Phase:

1. **Build Successfully**
```powershell
dotnet build
```

2. **Run Unit Tests**
```powershell
dotnet test --filter Category=CQRS
```

3. **Manual Testing**
- Start application
- Verify panels open correctly
- Verify actuator pause/resume works
- Verify agent context switching works

4. **Regression Testing**
- Test legacy code paths (when DI unavailable)
- Verify backward compatibility
- Check error handling

---

## Fallback Pattern

For ALL migrations, use this safe fallback pattern:

```csharp
// Try CQRS first
var handler = Context.ServiceProvider?.GetService(typeof(ICommandHandler<SomeCommand>)) 
    as ICommandHandler<SomeCommand>;

if (handler != null)
{
    // Modern path
    var result = handler.Handle(new SomeCommand(...));
}
else
{
    // Fallback to legacy singleton
    var result = SomeManager.Instance.SomeMethod(...);
}
```

This ensures:
- ✅ No breaking changes
- ✅ Works with or without DI
- ✅ Gradual migration possible
- ✅ Safe rollback

---

## Benefits After Completion

### ✅ Decoupling
- No direct singleton dependencies
- Testable command/query handlers
- Loosely coupled architecture

### ✅ Testability
```csharp
// Easy to test with mocks
var mockHandler = new Mock<ICommandHandler<CreatePanelCommand>>();
mockHandler.Setup(h => h.Handle(It.IsAny<CreatePanelCommand>()))
           .Returns(new CreatePanelResult { Panel = mockPanel });
```

### ✅ Extensibility
- Easy to add validation
- Easy to add logging/metrics
- Easy to add caching

### ✅ Consistency
- Same pattern across entire codebase
- Follows CQRS architectural pattern
- Modern .NET practices

---

## Progress Tracking

Use this checklist to track progress:

### Panel Creation (9 sites)
- [ ] ACATApp/Program.cs:244
- [ ] ACATApp/Program.cs:325
- [ ] ACATTalk/Program.cs:234
- [ ] ACATTalk/Program.cs:346
- [ ] TalkWindowHandler.cs:44
- [ ] ShowScreenLockHandler.cs:49
- [ ] DashboardAppScanner.cs:210
- [ ] ActuatorBase.cs:661
- [ ] ActuatorBase.cs:678

### Actuator Commands (65 sites)
- [ ] ScannerCommon.cs (~30 sites)
- [ ] DialogCommon.cs (~20 sites)
- [ ] AnimationManager.cs (~15 sites)

### Agent Queries (122 sites)
- [ ] Extension method approach
- [ ] Or direct replacement

---

## Recommended Order

**Day 1 Morning:** Phase 1 - Application entry points (4 sites)
**Day 1 Afternoon:** Test Phase 1, start Phase 2 - Handlers (5 sites)
**Day 2 Morning:** Complete Phase 2, start ScannerCommon.cs
**Day 2 Afternoon:** Complete ScannerCommon.cs, test thoroughly
**Day 3 Morning:** DialogCommon.cs and AnimationManager.cs
**Day 3 Afternoon:** Agent queries extension method, final testing

---

## Summary

**Total Call Sites:** 196
- Panel Creation: 9 sites
- Actuator Commands: 65 sites
- Agent Queries: 122 sites

**Estimated Time:** 3 days with testing
**Complexity:** High (requires DI wiring)
**Value:** Very High (completes CQRS modernization)

**Status After Completion:**
- ✅ EventBus: 100% Complete
- ✅ Repository: 100% Complete
- ✅ CQRS: **100% Complete**
- ✅ Architecture Modernization: **DONE!**

---

## Quick Start Commands

```powershell
# 1. Find all CreatePanel call sites
Get-ChildItem -Recurse -Include *.cs | Select-String "PanelManager.Instance.CreatePanel"

# 2. Find all Pause/Resume call sites
Get-ChildItem -Recurse -Include *.cs | Select-String "Context.AppActuatorManager.Pause|Context.AppActuatorManager.Resume"

# 3. Find all agent query call sites
Get-ChildItem -Recurse -Include *.cs | Select-String "Context.AppAgentMgr.GetCurrentAgentName"

# 4. Build after changes
dotnet build

# 5. Run tests
dotnet test
```

---

**Next Steps:**
1. Review this guide
2. Start with Phase 1 (easiest, highest value)
3. Test after each phase
4. Use fallback pattern for safety
5. Track progress with checklist

This systematic approach will complete the CQRS wiring safely and efficiently!
