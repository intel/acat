# ACAT Architecture Modernization — Implementation Status Report

**Date:** February 2026  
**Feature:** Architecture Modernization (Issue #194, Sub-tasks #202–#205)  
**Status:** Infrastructure Complete — Production Integration Pending

---

## Executive Summary

The Phase 2 Architecture Modernization effort has **successfully delivered all four infrastructure layers** (interfaces, event system, CQRS, repository) in `ACATCore`. However, **none of the production application code has been migrated to use these new patterns**. The entire production codebase (applications, extensions, UI handlers) continues to use the old coupling patterns: direct singleton access (`Manager.Instance`), tight event coupling via C# delegates, and inline XML serialization.

### Completion by Area

| Deliverable | Infrastructure | DI Registration | Production Wiring | Status |
|-------------|:--------------:|:---------------:|:-----------------:|--------|
| Interface Extraction Guide | ✅ | n/a | n/a | **Done** |
| Event System (`IEventBus`) | ✅ | ✅ | ❌ | **Infra only** |
| CQRS (`ICommand`/`IQuery`) | ✅ | ❌ | ❌ | **Infra only** |
| Repository Pattern | ✅ | ❌ | ❌ | **Infra only** |

---

## 1. What Has Been Built

### 1.1 Interface Extraction (Issue #202)
**File:** `docs/INTERFACE_EXTRACTION_GUIDE.md`

A comprehensive guide covering naming conventions, directory layout, migration checklist, and component priority lists. The following manager interfaces already exist in the codebase:

| Interface | Concrete | Location | DI Registered? |
|-----------|----------|----------|:--------------:|
| `IPanelManager` | `PanelManager` | `PanelManagement/` | ✅ |
| `IAgentManager` | `AgentManager` | `AgentManagement/` | ✅ |
| `IActuatorManager` | `ActuatorManager` | `ActuatorManagement/` | ✅ |
| `IWordPredictionManager` | `WordPredictionManager` | `WordPredictorManagement/` | ✅ |
| `ITTSManager` | `TTSManager` | `TTSManagement/` | ✅ |
| `ISpellCheckManager` | `SpellCheckManager` | `SpellCheckManagement/` | ✅ |
| `IAbbreviationsManager` | `AbbreviationsManager` | `AbbreviationsManagement/` | ✅ |
| `ICommandManager` | `CommandManager` | `CommandManagement/` | ✅ |
| `IThemeManager` | `ThemeManager` | `ThemeManagement/` | ✅ |

**What's missing:** These interfaces are *defined and registered in DI* but production code **does not use them via injection**. Call sites access singletons directly (e.g., `PanelManager.Instance`, `Context.AppAgentMgr`, `ThemeManager.Instance.ActiveTheme`).

### 1.2 Event System (Issue #203)
**Files:** `Libraries/ACATCore/EventManagement/`

| File | Purpose |
|------|---------|
| `IEvent.cs` | Marker interface all events must implement |
| `IEventBus.cs` | Pub/sub contract: `Subscribe<T>`, `Unsubscribe<T>`, `Publish<T>` |
| `EventBase.cs` | Abstract base capturing `Timestamp` (UTC) |
| `EventBus.cs` | Thread-safe implementation using weak-reference delegates; dead subscriptions pruned on publish |
| `PanelEvents.cs` | `PanelShowEvent`, `PanelHideEvent`, `PanelActivateEvent` |
| `ActuatorEvents.cs` | `ActuatorSwitchActivatedEvent` |
| `ConfigurationEvents.cs` | `ConfigurationReloadEvent`, `ConfigurationChangedEvent` |
| `AgentEvents.cs` | `AgentContextChangedEvent` |

**DI Registration:** `IEventBus` → `EventBus` registered as singleton in `ServiceConfiguration.AddACATServices()`.

**Production usage:** Zero. No production code calls `_eventBus.Publish(...)` or `_eventBus.Subscribe<T>(...)`. The 35+ legacy `EventHandler` delegates on the manager interfaces (e.g., `EvtAppQuit`, `EvtCalibrationEndNotify`, `EvtDisplaySettingsChanged`) remain in use.

### 1.3 CQRS — Command/Query Separation (Issue #204)
**Files:** `Libraries/ACATCore/Patterns/CQRS/`

| File | Purpose |
|------|---------|
| `ICommand.cs` | Marker for void commands and result-returning commands |
| `IQuery<TResult>.cs` | Marker for read-only queries |
| `ICommandHandler<TCommand>.cs` | Contract for command handlers |
| `IQueryHandler<TQuery,TResult>.cs` | Contract for query handlers |
| `PanelCommands.cs` | `ShowPanelCommand`, `HidePanelCommand`, `GetActivePanelQuery`, `GetAllPanelNamesQuery` |
| `Samples/CreatePanelCommand.cs` | Full sample wrapping `IPanelManager.CreatePanel` |
| `Samples/CreatePanelCommandHandler.cs` | Full sample using constructor-injected `IPanelManager` |
| `Samples/HandleActuatorSwitchCommand.cs` | Pause/resume actuator via enum |
| `Samples/HandleActuatorSwitchCommandHandler.cs` | Delegates to `IActuatorManager.Pause()/Resume()` |
| `Samples/GetConfigurationValueQuery.cs` | Read config key from `EnvironmentConfiguration` |
| `Samples/GetConfigurationValueQueryHandler.cs` | Returns value from injected `EnvironmentConfiguration` |
| `Samples/GetActiveAgentNameQuery.cs` | Read active agent name |
| `Samples/GetActiveAgentNameQueryHandler.cs` | Delegates to `IAgentManager.GetCurrentAgentName()` |

**DI Registration:** None. CQRS handlers are not registered in `ServiceConfiguration`.

**Production usage:** Zero. The 9 call sites that call `PanelManager.Instance.CreatePanel(...)` directly are unchanged. The 65 call sites using `Context.AppActuatorManager.Pause()/Resume()` are unchanged.

### 1.4 Repository Pattern (Issue #205)
**Files:** `Libraries/ACATCore/DataAccess/`

| File | Purpose |
|------|---------|
| `IRepository<T>.cs` | `Load(key)`, `Save(entity, key)`, `GetDefault()` |
| `RepositoryBase<T>.cs` | Abstract base providing logger and null-guards |
| `PreferencesRepository<T>.cs` | XML-based; delegates to `XmlUtils.XmlFileLoad<T>` / `XmlUtils.XmlFileSave` |
| `ConfigurationRepository<T>.cs` | JSON-based; delegates to `System.Text.Json` |
| `ThemeRepository.cs` | Loads `Theme` objects; save intentionally unsupported |

**DI Registration:** None. Repositories are not registered in `ServiceConfiguration`.

**Production usage:** Zero. The codebase has:
- **6 direct calls** to `XmlUtils.XmlFileLoad` / `XmlUtils.XmlFileSave` in `GlobalPreferences.cs` and `PreferencesBase.cs` (these are the exact call sites `PreferencesRepository<T>` should replace)
- **20+ calls** to `prefs.Save()` on `PreferencesBase` subclasses throughout `ActuatorManagement`, `TTSManagement`, `WordPredictorManagement`, `SpellCheckManagement`, `CommandManagement`
- All `ThemeManager.Instance.ActiveTheme` accesses (23 call sites across `Widgets`, `WidgetManagement`, `Extensions`) bypass `ThemeRepository` entirely

---

## 2. What Is Sample / Test Code Only

### 2.1 Sample Handlers (not wired)
All files under `Libraries/ACATCore/Patterns/CQRS/Samples/` are **sample/demo code** that demonstrate the correct pattern but are not integrated:
- `CreatePanelCommandHandler` — shows how to wrap `IPanelManager.CreatePanel`; real code in `ACATApp/Program.cs` still calls `PanelManager.Instance.CreatePanel` directly
- `HandleActuatorSwitchCommandHandler` — shows how to route through `IActuatorManager`; real code calls `Context.AppActuatorManager.Pause()`
- `GetActiveAgentNameQueryHandler` — shows how to query `IAgentManager`; not called anywhere
- `GetConfigurationValueQueryHandler` — shows how to read config; not called anywhere

### 2.2 Tests (ACATCore.Tests.Architecture)
All tests in `Libraries/ACATCore.Tests.Architecture/` validate the infrastructure objects in isolation:
- `EventBusTests.cs` — tests `EventBus` publish/subscribe directly
- `CqrsTests.cs` — tests command/query object construction
- `RepositoryTests.cs` — tests `PreferencesRepository`/`ConfigurationRepository` round-trips

Existing test coverage in `ACATCore.Tests.Configuration/CQRSPatternTests.cs` validates handler behavior with test fakes — but these fakes are not used in production code.

---

## 3. Detailed TODO List — Production Integration

> **How to read this list:** Each item is marked with the subsystem it affects and the specific file where the change should be made. Items are ordered from highest to lowest value/impact.

---

### 3.1 Register CQRS Handlers and Repositories in DI

**File:** `src/Libraries/ACATCore/Utility/ServiceConfiguration.cs`  
**Section:** `AddACATServices()`

Add the following registrations to `AddACATServices()`. These are concrete code changes — paste them after the existing `EventBus` registration line:

```csharp
// CQRS command handlers (transient — stateless, created per request)
services.AddTransient<ICommandHandler<CreatePanelCommand>, CreatePanelCommandHandler>();
services.AddTransient<ICommandHandler<HandleActuatorSwitchCommand>, HandleActuatorSwitchCommandHandler>();

// CQRS query handlers (transient — stateless, created per request)
services.AddTransient<IQueryHandler<GetActiveAgentNameQuery, string>, GetActiveAgentNameQueryHandler>();
services.AddTransient<IQueryHandler<GetConfigurationValueQuery, string>, GetConfigurationValueQueryHandler>();

// Repositories (singleton — stateless file-access helpers)
// Note: generic open registration requires a ServiceCollection extension; 
// use closed registrations for now:
services.AddSingleton<IRepository<Theme>, ThemeRepository>();
// For generic preferences/config, callers can resolve: serviceProvider.GetRequiredService<PreferencesRepository<MyPrefs>>()
```

---

### 3.2 Publish EventBus Events from Managers

Each manager currently fires legacy `EventHandler` delegates. These should also fire the corresponding `IEventBus` events so that consumers can migrate gradually. Add `IEventBus` constructor injection to each manager.

#### 3.2.1 PanelManager — Panel lifecycle events

**File:** `src/Libraries/ACATCore/PanelManagement/PanelManager.cs`

- Inject `IEventBus` in constructor (already DI-resolved via `Context.ServiceProvider`).
- After a panel is shown: `_eventBus.Publish(new PanelShowEvent(panelClass))`
- After a panel is hidden: `_eventBus.Publish(new PanelHideEvent(panelClass))`
- After a panel is activated: `_eventBus.Publish(new PanelActivateEvent(panelClass))`

#### 3.2.2 ActuatorManager — Switch activation events

**File:** `src/Libraries/ACATCore/ActuatorManagement/ActuatorManager.cs`

- Inject `IEventBus`.
- In the switch activation handler: `_eventBus.Publish(new ActuatorSwitchActivatedEvent(switchName))`

#### 3.2.3 Configuration system — Reload events

**File:** `src/Libraries/ACATCore/Configuration/ConfigurationReloadService.cs`

- Inject `IEventBus`.
- In `OnConfigurationReloaded`: `_eventBus.Publish(new ConfigurationReloadEvent(filePath))`
- In `OnConfigurationReloadFailed`: publish a failure event (requires adding `ConfigurationReloadFailedEvent` to `ConfigurationEvents.cs`)

#### 3.2.4 AgentManager — Context change events

**File:** `src/Libraries/ACATCore/AgentManagement/AgentManager.cs`

- Inject `IEventBus`.
- When the active agent changes: `_eventBus.Publish(new AgentContextChangedEvent(agentName, context))`

---

### 3.3 Wire CQRS Commands at Call Sites

Replace direct singleton access with injected command handlers. Prioritize by frequency and risk:

#### 3.3.1 Panel creation (9 call sites, high value)

| File | Current | Target |
|------|---------|--------|
| `ACATApp/Program.cs:244` | `PanelManager.Instance.CreatePanel(...)` | `_commandHandler.Handle(new CreatePanelCommand(...))` |
| `ACATApp/Program.cs:325` | `PanelManager.Instance.CreatePanel(...)` | Same |
| `ACATTalk/Program.cs:234` | `PanelManager.Instance.CreatePanel(...)` | Same |
| `ACATTalk/Program.cs:346` | `PanelManager.Instance.CreatePanel(...)` | Same |
| `CommandHandlers/TalkWindowHandler.cs:44` | `PanelManager.Instance.CreatePanel(...)` | Same |
| `CommandHandlers/ShowScreenLockHandler.cs:49` | `PanelManager.Instance.CreatePanel(...)` | Same |
| `Scanners/DashboardAppScanner.cs:210` | `PanelManager.Instance.CreatePanel(...)` | Same |
| `ActuatorBase.cs:661` | `PanelManager.Instance.CreatePanel(...)` | Same |
| `ActuatorBase.cs:678` | `PanelManager.Instance.CreatePanel(...)` | Same |

**Note:** Each of these sites needs to receive an `ICommandHandler<CreatePanelCommand>` via constructor injection. The entry-point programs already have access to `_serviceProvider`.

#### 3.3.2 Actuator pause/resume (65 call sites)

Replace `Context.AppActuatorManager.Pause()` / `Context.AppActuatorManager.Resume()` with the CQRS command pattern. Classes should receive `ICommandHandler<HandleActuatorSwitchCommand>` via constructor injection (field `_actuatorHandler`):

```csharp
// Constructor injection (field: private readonly ICommandHandler<HandleActuatorSwitchCommand> _actuatorHandler)
_actuatorHandler.Handle(new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause));
```
Priority files: `ScannerCommon.cs`, `DialogCommon.cs`, `AnimationManager.cs`

#### 3.3.3 Active agent queries (122 call sites)

Replace `Context.AppAgentMgr.GetCurrentAgentName()` with:
```csharp
var name = _agentQueryHandler.Handle(new GetActiveAgentNameQuery());
```

---

### 3.4 Migrate Data Access to Repository Pattern

#### 3.4.1 GlobalPreferences.cs (6 direct XmlUtils calls)

**File:** `src/Libraries/ACATCore/Utility/GlobalPreferences.cs`

Replace:
```csharp
GlobalPreferences retVal = XmlUtils.XmlFileLoad<GlobalPreferences>(prefFile);
XmlUtils.XmlFileSave(retVal, prefFile);
```
With:
```csharp
var repo = new PreferencesRepository<GlobalPreferences>(_logger);
GlobalPreferences retVal = repo.Load(prefFile) ?? new GlobalPreferences();
repo.Save(retVal, prefFile);
```

#### 3.4.2 PreferencesBase.cs (static Load/Save helpers)

**File:** `src/Libraries/ACATCore/PreferencesManagement/PreferencesBase.cs`

The static `Load<T>` and `Save` methods call `XmlUtils` directly. Refactor to delegate to `PreferencesRepository<T>`. The repository should be resolved from DI where a container is available, or instantiated directly in static/legacy contexts:

```csharp
// In DI-aware callers — inject IRepository<T> or PreferencesRepository<T>
public static T Load<T>(string preferencesFile, ...) where T : new()
{
    // Option A: DI-resolved (preferred in new code)
    // var repo = serviceProvider.GetRequiredService<PreferencesRepository<T>>();
    
    // Option B: Direct instantiation for static/legacy code paths
    var repo = new PreferencesRepository<T>(logger: null);  // null → NullLogger used internally
    return repo.Load(preferencesFile) ?? new T();
}
```
This single change migrates all 20+ call sites that use `PreferencesBase.Load<T>` and `prefs.Save()` throughout `ActuatorManagement`, `TTSManagement`, `CommandManagement`, etc.

#### 3.4.3 ThemeManagement (23 ThemeManager.Instance.ActiveTheme call sites)

**File:** `src/Libraries/ACATCore/ThemeManagement/ThemeManager.cs`

Internally use `ThemeRepository` for loading `Theme` objects, replacing direct file system access. The `ThemeManager.Instance.ActiveTheme` property can remain as the consumer-facing API for now (no breaking change).

---

### 3.5 Subscribe to EventBus Instead of Legacy Delegates

Once events are published (§3.2), migrate subscribers away from `+=`/`-=` delegate patterns toward `IEventBus.Subscribe<T>`. Prioritize integration points:

| Legacy event | EventBus replacement | Primary subscriber |
|---|---|---|
| `IPanelManager.EvtAppQuit` | Subscribe to application lifecycle event (TBD — add `AppQuitEvent`) | `ActuatorManager.cs:284` |
| `IPanelManager.EvtDisplaySettingsChanged` | Subscribe to `DisplaySettingsChangedEvent` (TBD) | `PanelManager` subscribers |
| `ConfigurationReloadService.ConfigurationReloaded` | Subscribe to `ConfigurationReloadEvent` | `JsonConfigurationLoader.cs:333` |
| `IActuatorManager.EvtCalibrationEndNotify` | Subscribe to `CalibrationEndEvent` (TBD) | `PanelManager`, dialogs |

**Note:** Some of these event types do not exist yet — they need to be added to the appropriate `*Events.cs` files.

---

### 3.6 Missing Event Types (Add to EventManagement)

The following domain events are raised in the codebase via legacy C# delegates but have no `IEventBus` equivalent:

| Missing Event | Where to create | Trigger |
|---|---|---|
| `AppQuitEvent` | `PanelEvents.cs` | `PanelManager.EvtAppQuit` |
| `CalibrationEndEvent` | `ActuatorEvents.cs` | `ActuatorManager.EvtCalibrationEndNotify` |
| `DisplaySettingsChangedEvent` | `PanelEvents.cs` | `PanelManager.EvtDisplaySettingsChanged` |
| `ConfigurationReloadFailedEvent` | `ConfigurationEvents.cs` | `ConfigurationReloadService.ConfigurationReloadFailed` |
| `WordPredictionContextChangedEvent` | new `WordPredictionEvents.cs` | Word predictor context switch |

---

### 3.7 Missing DI Wiring in Non-ACATWatch Applications

**ACATWatch/Program.cs** correctly calls `services.AddACATInfrastructure()`. The two main applications do not:

| Application | Current | Required |
|---|---|---|
| `ACATApp/Program.cs` | calls `AddACATServices()` directly | also call `AddACATLogging()` or use `AddACATInfrastructure()` |
| `ACATTalk/Program.cs` | calls `AddACATServices()` directly | same |

Add CQRS handler registrations and repository registrations to `AddACATServices()` per §3.1, and all applications will pick them up automatically.

---

### 3.8 Extension Projects — Not Using DI

The following extension projects create objects directly and do not inject interfaces:

- `src/Libraries/ACATExtension/CommandHandlers/` — all 8 command handler files use `PanelManager.Instance.CreatePanel` and `Context.App*Manager` directly
- `src/Extensions/ACAT.Extensions.UI/Scanners/` — `DashboardAppScanner.cs`, `TalkApplicationScanner.cs`
- `src/Extensions/Default/FunctionalAgents/` — `SwitchWindowsAgent.cs`, `LaunchAppAgent.cs`

These need constructor injection, but first the hosting mechanism for extensions (`AgentsCache`, `LayoutAttribute`) must be updated to pass the `IServiceProvider` — which is partially done (`AgentsCache.cs:354` already uses `ActivatorUtilities.CreateInstance`).

---

## 4. Architecture Quick Reference (for future agents / Copilot)

```
src/Libraries/ACATCore/
│
├── EventManagement/          ← Event bus infrastructure
│   ├── IEvent.cs             ← Marker interface; all events implement this
│   ├── IEventBus.cs          ← Subscribe<T>/Unsubscribe<T>/Publish<T>
│   ├── EventBus.cs           ← Thread-safe weak-ref implementation; DI singleton
│   ├── EventBase.cs          ← Abstract base; sets Timestamp
│   ├── PanelEvents.cs        ← PanelShowEvent, PanelHideEvent, PanelActivateEvent
│   ├── ActuatorEvents.cs     ← ActuatorSwitchActivatedEvent
│   ├── ConfigurationEvents.cs← ConfigurationReloadEvent, ConfigurationChangedEvent
│   └── AgentEvents.cs        ← AgentContextChangedEvent
│
├── Patterns/CQRS/            ← Command/Query separation infrastructure
│   ├── ICommand.cs           ← Marker for commands (void and result-returning)
│   ├── IQuery<TResult>.cs    ← Marker for read-only queries
│   ├── ICommandHandler.cs    ← void Handle(TCommand)
│   ├── IQueryHandler.cs      ← TResult Handle(TQuery)
│   ├── PanelCommands.cs      ← ShowPanelCommand, HidePanelCommand, GetActivePanelQuery
│   └── Samples/              ← REFERENCE IMPLEMENTATIONS — not called by production code
│       ├── CreatePanelCommand[Handler].cs       → replaces PanelManager.Instance.CreatePanel
│       ├── HandleActuatorSwitchCommand[Handler].cs → replaces Actuator.Pause()/Resume()
│       ├── GetActiveAgentNameQuery[Handler].cs  → replaces Context.AppAgentMgr.*
│       └── GetConfigurationValueQuery[Handler].cs → reads EnvironmentConfiguration
│
├── DataAccess/               ← Repository pattern infrastructure
│   ├── IRepository<T>.cs     ← Load(key), Save(entity,key), GetDefault()
│   ├── RepositoryBase<T>.cs  ← Abstract base with logger + null-guards
│   ├── PreferencesRepository<T>.cs ← XML via XmlUtils (replaces PreferencesBase.Load<T>)
│   ├── ConfigurationRepository<T>.cs ← JSON via System.Text.Json
│   └── ThemeRepository.cs    ← Loads Theme objects; save unsupported
│
└── Utility/ServiceConfiguration.cs ← DI root
    ├── AddACATServices()      ← Registers all managers + EventBus singleton
    └── AddACATInfrastructure()← AddACATLogging() + AddACATServices()

KEY MISSING WIRING:
  • CQRS handlers NOT registered in ServiceConfiguration
  • Repository types NOT registered in ServiceConfiguration
  • EventBus.Publish() NEVER called in production code
  • 9 CreatePanel call sites still use PanelManager.Instance directly
  • 65 Actuator Pause/Resume call sites bypass CQRS command
  • 256 AgentMgr call sites bypass CQRS query
  • 23 ThemeManager.Instance.ActiveTheme calls bypass ThemeRepository
  • 6 XmlUtils.XmlFileLoad/Save calls in GlobalPreferences bypass PreferencesRepository
```

---

## 5. Recommended Implementation Order

1. **Register CQRS handlers + repositories in DI** (`ServiceConfiguration.cs`) — 1 day
2. **Publish EventBus events from PanelManager** (`PanelManager.cs`) — 1 day  
3. **Publish EventBus events from ActuatorManager** (`ActuatorManager.cs`) — 0.5 day
4. **Publish EventBus events from ConfigurationReloadService** — 0.5 day
5. **Migrate `GlobalPreferences` to `PreferencesRepository`** — 0.5 day
6. **Migrate `PreferencesBase.Load<T>/Save` to `PreferencesRepository`** — 1 day (high leverage: fixes 20+ call sites)
7. **Wire CQRS at application entry points** (ACATApp, ACATTalk entry-point panel creation) — 1 day
8. **Migrate ACATExtension CommandHandlers to use injected interfaces** — 2 days
9. **Add missing event types** (AppQuitEvent, CalibrationEndEvent, etc.) — 0.5 day
10. **Migrate legacy `+=` event subscriptions to EventBus** — 2–3 days (gradual)

**Estimated total remaining effort:** 10–12 developer-days to complete full production integration.
