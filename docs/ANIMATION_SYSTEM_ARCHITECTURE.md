# Animation System Architecture Specification

**Document Status**: Phase 3 Planning — Issue #206 Analysis Complete  
**Version**: 1.1  
**Last Updated**: February 2026  
**Epic**: Phase 2 – Core Infrastructure Modernization (Investigation)  
**Tracked By**: intel/acat#195

> **Agent/Developer Note (intel/acat#195):** Issue #206 (Current Animation System Analysis) is **complete**.
> The full evidence-based analysis — with verified line counts, exact pain-point locations, event subscriber map,
> XML config audit, and BCI extension breakdown — is in [`docs/ANIMATION_SYSTEM_ANALYSIS.md`](ANIMATION_SYSTEM_ANALYSIS.md).
> Two important corrections from that analysis:
> - `AnimationPlayer.cs` is **1,835 lines** (not ~800 as estimated here in §3.1).
> - `AnimationSharpManagerV2.cs` is **2,885 lines** with significant duplication.
>
> **Next step**: Issue #207 – Animation Architecture Design.
> Start by reviewing `ANIMATION_SYSTEM_ANALYSIS.md §11` (Prioritized Recommendations for Issue #207)
> before finalizing the interface definitions in §4.2 of this document.  

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Goals and Design Principles](#2-goals-and-design-principles)
3. [Current System Analysis](#3-current-system-analysis)
4. [Target Architecture Design](#4-target-architecture-design)
5. [Task Breakdown](#5-task-breakdown)
   - [Issue #206 – Current Animation System Analysis](#issue-206--current-animation-system-analysis)
   - [Issue #207 – Animation Architecture Design](#issue-207--animation-architecture-design)
   - [Issue #208 – Animation System POC](#issue-208--animation-system-poc)
6. [Migration Strategy](#6-migration-strategy)
7. [Risks and Mitigations](#7-risks-and-mitigations)
8. [Success Criteria](#8-success-criteria)
9. [Dependencies](#9-dependencies)
10. [Glossary](#10-glossary)

---

## 1. Executive Summary

ACAT's animation system is the **heartbeat** of the application. It drives the scanning highlight that allows users with motor disabilities to interact with the application using actuators (switches, cameras, eye-gaze devices, BCI hardware). Because the animation system is so central to the user experience, it must be reliable, testable, and adaptable.

The current implementation – built around a `System.Timers.Timer` loop and XML-configured scan sequences – was designed before the project's shift to modern .NET patterns. As a result, it carries several structural limitations: tight coupling to XML, a custom PCode scripting interpreter, direct UI-thread marshalling, and no unit-testable seams. These make it difficult to add new scan modes, improve accessibility timing, or validate behavior in automated tests.

This document captures the **investigation deliverables** described in intel/acat#195 (Animation System Preparation – Phase 3 deferred). It provides:

- A thorough analysis of the current system's responsibilities, pain points, and constraints.
- A target architecture that aligns with Phase 2's DI/EventBus/Repository patterns while remaining backward-compatible.
- A concrete task breakdown that any developer can pick up and execute in Phase 3.

The rewrite is **deferred to Phase 3** but this specification is the single authoritative source of truth for that effort.

---

## 2. Goals and Design Principles

### 2.1 Goals

| # | Goal | Rationale |
|---|------|-----------|
| G1 | Make the animation engine fully unit-testable | Eliminate the gap in automated test coverage for the most critical subsystem |
| G2 | Decouple scan timing from UI framework primitives | Allow timing to be mocked, injected, and verified in tests |
| G3 | Replace implicit XML parsing with a typed, validated model | Leverage Phase 1 JSON configuration infrastructure and JSON Schema |
| G4 | Adopt the Phase 2 EventBus for state change notifications | Remove ad-hoc C# events and delegates from the public surface area |
| G5 | Support async/await for I/O operations (Phase 3 readiness) | Prepare for Phase 3's async patterns without requiring immediate rewrite |
| G6 | Preserve all existing scanning behaviors and BCI integration | Zero regression for end users; backward compatibility non-negotiable |
| G7 | Maintain accessibility timing guarantees | Scan interval accuracy directly impacts user success rates |

### 2.2 Design Principles

1. **Interface-first** – Every public dependency is expressed as an interface. Concrete classes are never referenced across subsystem boundaries.
2. **Dependency injection** – All services are registered in the DI container (Phase 2 infrastructure). No static singletons or `CoreGlobals` access inside the engine.
3. **Event-driven state** – State changes are broadcast via `IEventBus`. Consumers subscribe; the engine does not call them directly.
4. **Single responsibility** – The *timer*, the *scan sequence*, the *highlight renderer*, and the *input handler* are separate classes with separate tests.
5. **Configuration parity** – JSON configuration is first-class; XML remains supported via the existing migration path.
6. **Backward compatibility** – Public contracts (`IAnimationManager`, `IPanelAnimationManager`, `IUserControlAnimationManager`) are preserved. New interfaces are added alongside, not replacing, existing ones during the transition period.

---

## 3. Current System Analysis

> This section is the deliverable for **Issue #206**.

### 3.1 Component Inventory

> **Note**: Line counts below are verified actual values from `ANIMATION_SYSTEM_ANALYSIS.md §2`.
> The original estimates for `AnimationPlayer.cs` (~800) and `AnimationSharpManagerV2.cs` were significantly understated.

| File | Role | Lines |
|------|------|-------|
| `AnimationPlayer.cs` | Timer loop, widget highlight, state machine | **1,835** |
| `AnimationManager.cs` | Actuator event routing, player lifecycle | **976** |
| `PanelAnimationManager.cs` | Per-panel player management, config loading | **395** |
| `UserControlAnimationManager.cs` | UserControl variant of panel manager | **402** |
| `Animation.cs` | Single scan sequence (widget list + PCode hooks) | **485** |
| `AnimationsCollection.cs` | Collection of `Animations` keyed by panel name | **164** |
| `Animations.cs` | Collection of `Animation` objects for one panel | **156** |
| `AnimationWidget.cs` | Per-widget highlight settings (color, sound, etc.) | **148** |
| `Variables.cs` | Key/value store for animation-scoped variables | **155** |
| `Interfaces/IAnimationManager.cs` | Public interface for `AnimationManager` | **27** |
| `Interfaces/IPanelAnimationManager.cs` | Public interface for `PanelAnimationManager` | **13** |
| `Interfaces/IUserControlAnimationManager.cs` | Public interface for `UserControlAnimationManager` | **15** |

**BCI extension:** `AnimationSharpManagerV2.cs` in `src/Extensions/BCI/Common/AnimationSharp/` is **2,885 lines** — larger than all 12 core animation files combined. It provides an alternate `AnimationManager` implementation for Brain-Computer Interface devices. See `ANIMATION_SYSTEM_ANALYSIS.md §8` for the BCI-specific vs. duplicated behavior breakdown.

### 3.2 Data Flow

```
[Config File (XML)]
        │
        ▼
AnimationsCollection.Load()          ← PanelAnimationManager calls this at panel init
        │  loads N × Animations
        ▼
Animations.Load()                    ← one per panel name
        │  loads N × Animation
        ▼
Animation.Load() / ResolveUIWidgetsReferences()
        │  builds AnimationWidget list; parses PCode (OnEnter/OnSelect/OnEnd)
        ▼
AnimationPlayer                      ← one per panel
        │  System.Timers.Timer tick
        │  → highlights next AnimationWidget
        │  → fires EvtPlayerStateChanged (delegates)
        │
        ├── Actuator input (ActuatorManager events)
        │        ↓
        │   AnimationManager.handleActuatorEvent()
        │        ↓ calls Player.Transition / SetSelectedWidget / Pause / Resume
        │
        └── PCode execution via Interpret
                 ↓ OnEnter / OnSelect / OnEnd hooks
                 ↓ triggers panel transitions, speech, etc.
```

### 3.3 State Machine

```
            ┌─────────────────────┐
  Start()   │                     │  Stop()
──────────► │       Running       │ ──────────►  Stopped
            │                     │
            └──────┬──────┬───────┘
                   │      │
          Pause()  │      │  Timeout / all iterations done
                   ▼      ▼
               Paused   Timeout ──► (OnEnd PCode executed)
                   │
          Resume() │
                   ▼
               Running
                   │
      User switch  │
    (during scan)  ▼
               Interrupted ──► (OnSelect PCode executed)
```

`PlayerState` enum values: `Unknown`, `Stopped`, `Paused`, `Running`, `Timeout`, `Interrupted`.

### 3.4 Identified Pain Points

| ID | Pain Point | Impact |
|----|-----------|--------|
| P1 | **`System.Timers.Timer` is non-injectable** – tests cannot control time | No unit tests for timing-sensitive logic |
| P2 | **Direct `CoreGlobals.AppPreferences` access** inside `Animation` and `AnimationPlayer` | Breaks DI; makes preferences changes unpredictable in tests |
| P3 | **Custom PCode interpreter** is a separate, opaque runtime – no formal grammar or test harness | Bugs in scan scripts are hard to diagnose and reproduce |
| P4 | **Ad-hoc delegate events** (`EvtPlayerStateChanged`, `EvtPlayerAnimationTransition`) bypass EventBus | Inconsistent with Phase 2 event infrastructure; hard to trace event flow |
| P5 | **XML-only configuration** – `Animation.Load(XmlNode)` parses raw XML | Blocks JSON migration; no schema validation; brittle to malformed input |
| P6 | **Manual scan state is entangled with auto-scan state** in `AnimationPlayer` – same class handles both modes | High cyclomatic complexity; difficult to add new scan modes |
| P7 | **`SyncLock` threading primitive is app-specific** – not standard .NET synchronization | Obscures thread-safety guarantees; blocks async adoption |
| P8 | **`AnimationPlayer` is `internal`** – cannot be tested directly, only via integration | Forces all timing tests through the full stack |
| P9 | **Sound playback (`SoundPlayer`) is inline** in `AnimationManager` | Prevents silent/mocked playback in tests; couples audio to scan timing |
| P10 | **BCI extension duplicates** large portions of `AnimationManager` | Maintenance burden; divergence risk |

### 3.5 What Works Well (Preserve)

- The **hierarchical configuration model** (`AnimationsCollection → Animations → Animation`) is logical and maps cleanly to panels/screens.
- **Wildcard widget resolution** (`Box1/*`, `@SelectedWidget/*`) is a powerful runtime mechanism that should be retained.
- **Hesitate/step timing** per animation sequence (not global) enables fine-grained accessibility tuning.
- **`PlayerState` enum** cleanly models the lifecycle; the concept is worth keeping.
- The **PCode callback model** (OnEnter/OnSelect/OnEnd) is flexible; the scripting *language* is custom but the *hook structure* is sound.

---

## 4. Target Architecture Design

> This section is the deliverable for **Issue #207**.

### 4.1 Layer Overview

```
┌──────────────────────────────────────────────────────────────────┐
│                         UI Layer (Forms / WinUI 3)               │
│  Panels / UserControls subscribe to IEventBus for highlight      │
│  events; they do NOT hold a reference to the animation engine.   │
└─────────────────────────────┬────────────────────────────────────┘
                              │ IEventBus events
┌─────────────────────────────▼────────────────────────────────────┐
│                    Animation Service Layer                        │
│  IAnimationService  (new, replaces IAnimationManager)            │
│  IAnimationSession  (per-panel lifecycle handle)                 │
│  IScanModeStrategy  (pluggable auto/manual/BCI strategies)       │
└──────┬───────────────────┬────────────────────────────┬──────────┘
       │                   │                            │
┌──────▼──────┐  ┌─────────▼──────────┐  ┌────────────▼──────────┐
│ IScanTimer  │  │ IAnimationConfig-   │  │  IScanInputHandler    │
│ (injectable │  │ Provider (JSON/XML  │  │  (actuator adapter;   │
│  timer)     │  │  config loading)    │  │   publishes to bus)   │
└──────┬──────┘  └─────────┬──────────┘  └────────────┬──────────┘
       │                   │                            │
┌──────▼───────────────────▼────────────────────────────▼─────────┐
│                     Infrastructure / DI Container                │
│  Microsoft.Extensions.DependencyInjection                        │
│  Microsoft.Extensions.Logging (ILogger<T>)                       │
│  IEventBus (Phase 2 EventBus)                                    │
└──────────────────────────────────────────────────────────────────┘
```

### 4.2 New Interfaces

#### `IAnimationService` (replaces `IAnimationManager`)

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Root service for the animation engine. Registered as a singleton
    /// DI service. One instance per application lifetime.
    /// </summary>
    public interface IAnimationService
    {
        /// <summary>Creates a new scan session for the given panel root widget.</summary>
        IAnimationSession CreateSession(Widget rootWidget, AnimationConfig config);

        /// <summary>Disposes all active sessions and releases resources.</summary>
        void Shutdown();
    }
}
```

#### `IAnimationSession` (per-panel lifecycle)

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Represents a single active scan session bound to one panel.
    /// Replaces the direct use of AnimationPlayer + AnimationManager per panel.
    /// </summary>
    public interface IAnimationSession : IDisposable
    {
        PlayerState State { get; }

        void Start(string animationName = null);
        void Stop();
        void Pause();
        void Resume();
        void Interrupt();
        void Transition(string targetAnimationName = null);
        void SetSelectedWidget(string widgetName);
        void SetSelectedWidget(Widget widget);
        void HighlightDefaultHome();
    }
}
```

#### `IScanTimer` (injectable timer abstraction)

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Abstraction over System.Timers.Timer to allow test injection.
    /// Default implementation wraps System.Timers.Timer.
    /// Test implementation exposes ManualTick() for deterministic tests.
    /// </summary>
    public interface IScanTimer : IDisposable
    {
        bool Enabled { get; set; }
        double Interval { get; set; }
        event ElapsedEventHandler Elapsed;
        void Start();
        void Stop();
    }
}
```

#### `IScanModeStrategy` (pluggable scan strategies)

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Encapsulates a scanning strategy (automatic row-column, manual,
    /// BCI-driven, etc.). Injected into the scan session at creation time.
    /// </summary>
    public interface IScanModeStrategy
    {
        string Name { get; }
        AnimationWidget SelectNext(IReadOnlyList<AnimationWidget> widgets, int currentIndex);
        AnimationWidget SelectPrevious(IReadOnlyList<AnimationWidget> widgets, int currentIndex);
        bool HandleInput(ScanInputEvent inputEvent);
    }
}
```

#### `IAnimationConfigProvider` (configuration abstraction)

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Loads animation configuration for a given panel from JSON or XML.
    /// Replaces direct XmlNode parsing inside Animation.Load().
    /// </summary>
    public interface IAnimationConfigProvider
    {
        AnimationConfig LoadForPanel(string panelName, string configPath);
    }
}
```

### 4.3 EventBus Integration

Replace the current ad-hoc delegates with typed events on the Phase 2 `IEventBus`:

| Current Delegate | New EventBus Event | Published By |
|---|---|---|
| `EvtPlayerStateChanged` | `AnimationStateChangedEvent` | `AnimationSession` |
| `EvtPlayerAnimationTransition` | `AnimationTransitionEvent` | `AnimationSession` |
| `EvtAnimationWidgetAdded` | *(internal only; not on bus)* | `Animation` |
| `EvtResolveWidgetChildren` | *(internal only; not on bus)* | `Animation` |

```csharp
// Example event definitions (in EventManagement/Events/)
public class AnimationStateChangedEvent : IEvent
{
    public string PanelName { get; init; }
    public PlayerState OldState { get; init; }
    public PlayerState NewState { get; init; }
}

public class AnimationTransitionEvent : IEvent
{
    public string PanelName { get; init; }
    public string FromAnimation { get; init; }
    public string ToAnimation { get; init; }
    public bool IsTopLevel { get; init; }
}
```

### 4.4 Configuration Model (JSON-first)

Replace `Animation.Load(XmlNode)` with a typed C# model that can be deserialized from JSON (via Phase 1 `JsonConfigurationLoader<T>`) or populated from XML through a migration shim.

```csharp
// AnimationConfig.cs  – the root configuration object for one panel
public class AnimationConfig
{
    public string PanelName { get; set; }
    public List<AnimationSequenceConfig> Sequences { get; set; } = new();
}

// AnimationSequenceConfig.cs
public class AnimationSequenceConfig
{
    public string Name { get; set; }
    public bool IsFirst { get; set; }
    public bool AutoStart { get; set; }
    public string Iterations { get; set; } = "1";
    public string ScanTimeVariable { get; set; }
    public string FirstPauseTimeVariable { get; set; }
    public string OnEnter { get; set; }
    public string OnSelect { get; set; }
    public string OnEnd { get; set; }
    public List<AnimationWidgetConfig> Widgets { get; set; } = new();
}

// AnimationWidgetConfig.cs
public class AnimationWidgetConfig
{
    public string Name { get; set; }              // may be wildcard, e.g. "Box1/*"
    public bool PlayBeep { get; set; }
    public string OnSelected { get; set; }
}
```

A JSON Schema will be authored in `schemas/animation-config.schema.json` consistent with the patterns established in Phase 1 for `ActuatorSettings`, `Theme`, and `PanelConfig`.

### 4.5 Scan Strategy Plug-in Model

Split the two scan modes currently merged in `AnimationPlayer` into separate `IScanModeStrategy` implementations:

| Strategy Class | Scan Type | Replaces |
|---|---|---|
| `AutoScanStrategy` | Automatic row-column (timer-driven) | `AnimationPlayer` auto-scan path |
| `ManualScanStrategy` | Directional manual scanning | `AnimationPlayer` manual-scan path (ScanLeft/Right/Up/Down) |
| `BciScanStrategy` | BCI input-driven scan | `AnimationSharpManagerV2` logic |

Each strategy is registered in DI and resolved by name at session creation time.

### 4.6 Directory Structure (Post-Rewrite)

```
src/Libraries/ACATCore/AnimationManagement/
├── Interfaces/
│   ├── IAnimationService.cs       (new)
│   ├── IAnimationSession.cs       (new)
│   ├── IAnimationConfigProvider.cs (new)
│   ├── IScanTimer.cs              (new)
│   ├── IScanModeStrategy.cs       (new)
│   ├── IAnimationManager.cs       (preserved – backward compat)
│   ├── IPanelAnimationManager.cs  (preserved – backward compat)
│   └── IUserControlAnimationManager.cs (preserved – backward compat)
├── Events/
│   ├── AnimationStateChangedEvent.cs  (new)
│   └── AnimationTransitionEvent.cs    (new)
├── Config/
│   ├── AnimationConfig.cs         (new)
│   ├── AnimationSequenceConfig.cs (new)
│   ├── AnimationWidgetConfig.cs   (new)
│   └── XmlAnimationConfigAdapter.cs (new – wraps legacy XML loading)
├── Strategies/
│   ├── AutoScanStrategy.cs        (new)
│   ├── ManualScanStrategy.cs      (new)
│   └── BciScanStrategy.cs         (new – replaces AnimationSharpManagerV2 logic)
├── Infrastructure/
│   ├── SystemScanTimer.cs         (new – wraps System.Timers.Timer)
│   └── TestScanTimer.cs           (new – manual tick for tests)
├── AnimationService.cs            (new – implements IAnimationService)
├── AnimationSession.cs            (new – implements IAnimationSession)
├── Animation.cs                   (preserved, gradually refactored)
├── AnimationManager.cs            (preserved – adapter to new engine)
├── AnimationPlayer.cs             (preserved – adapter to new engine)
├── AnimationWidget.cs             (preserved)
├── AnimationsCollection.cs        (preserved)
├── Animations.cs                  (preserved)
├── PanelAnimationManager.cs       (preserved – delegates to IAnimationService)
├── UserControlAnimationManager.cs (preserved – delegates to IAnimationService)
└── Variables.cs                   (preserved)
```

### 4.7 Dependency Injection Registration

```csharp
// In ACATCore DI setup (ServiceConfiguration.cs)
services.AddSingleton<IAnimationService, AnimationService>();
services.AddTransient<IScanTimer, SystemScanTimer>();
services.AddTransient<IScanModeStrategy, AutoScanStrategy>();
services.AddTransient<IScanModeStrategy, ManualScanStrategy>();
services.AddTransient<IScanModeStrategy, BciScanStrategy>();
services.AddSingleton<IAnimationConfigProvider, JsonXmlAnimationConfigProvider>();
```

---

## 5. Task Breakdown

### Issue #206 – Current Animation System Analysis

> **STATUS: COMPLETE** — See [`docs/ANIMATION_SYSTEM_ANALYSIS.md`](ANIMATION_SYSTEM_ANALYSIS.md) for the full deliverable.

**Goal**: Produce a written, evidence-based analysis of the current animation system that identifies what must change, what must be preserved, and what risks exist.

**Acceptance Criteria**:

- [x] Complete the component inventory in §3.1 (verify line counts, add any missed files). — `AnimationPlayer.cs` is 1,835 lines (not ~800); `AnimationSharpManagerV2.cs` is 2,885 lines. See `ANIMATION_SYSTEM_ANALYSIS.md §2`.
- [x] Walk the full data flow (§3.2) and confirm it matches code reality; update if diverged. — Confirmed correct; method references added. See `ANIMATION_SYSTEM_ANALYSIS.md §3`.
- [x] Produce a state machine diagram (§3.3) validated against `AnimationPlayer.cs` transitions. — Confirmed; `Timeout` transience clarified. See `ANIMATION_SYSTEM_ANALYSIS.md §4`.
- [x] For each pain point in §3.4 (P1–P10): confirm it still exists in the current code, note its exact location (file + method), and assign a severity (High/Medium/Low). — All 10 confirmed with exact file:line references. See `ANIMATION_SYSTEM_ANALYSIS.md §5`.
- [x] Document all outbound event consumers – which classes subscribe to `EvtPlayerStateChanged` and `EvtPlayerAnimationTransition` – to understand migration scope. — 5 external subscribers for `EvtPlayerStateChanged`; 1 for `EvtPlayerAnimationTransition`. See `ANIMATION_SYSTEM_ANALYSIS.md §6`.
- [x] Review `AnimationSharpManagerV2.cs` and document which behaviors are BCI-specific vs. duplicated from the base manager. — BCI-specific vs. duplicated table provided. See `ANIMATION_SYSTEM_ANALYSIS.md §8`.
- [x] Review all panel XML config files under `src/ACATResources/` that contain `<Animation>` elements; count them and note any unusual patterns. — 69 files; 5 schema migration constraints identified. See `ANIMATION_SYSTEM_ANALYSIS.md §7`.
- [x] Write findings in a short analysis report appended to this document (§3) or in a linked `docs/ANIMATION_ANALYSIS_REPORT.md`. — Delivered as [`docs/ANIMATION_SYSTEM_ANALYSIS.md`](ANIMATION_SYSTEM_ANALYSIS.md).

**Estimated Effort**: 3–4 days  
**Output**: [`docs/ANIMATION_SYSTEM_ANALYSIS.md`](ANIMATION_SYSTEM_ANALYSIS.md) — full analysis report with verified metrics, subscriber map, XML audit, BCI analysis, and prioritized recommendations.

---

### Issue #207 – Animation Architecture Design

**Goal**: Finalize and validate the target architecture described in §4 before any implementation begins.

**Acceptance Criteria**:

- [ ] Review §4.2 interface definitions with the architecture team; adjust based on feedback.
- [ ] Confirm EventBus event schema (§4.3) with all known consumers (panels, agents, BCI extension).
- [ ] Finalize the `AnimationConfig` JSON model (§4.4) and author `schemas/json/animation-config.schema.json`.
  - Schema must validate against all existing XML panel configs when converted.
  - Use `schemas/json/` as the target directory, consistent with Phase 1 schemas.
- [ ] Define the `IScanModeStrategy` contract in detail (§4.5); write pseudo-code for `AutoScanStrategy.SelectNext()` that covers the current row-column logic.
- [ ] Confirm the DI registration plan (§4.7) with the DI service setup owner.
- [ ] Identify all callers of `AnimationManager`, `PanelAnimationManager`, and `UserControlAnimationManager` in the solution – these are the "blast radius" of the migration.
- [ ] Draft an ADR (Architecture Decision Record) covering:
  - Rationale for keeping `AnimationPlayer` as a legacy adapter vs. deleting it
  - Rationale for `IScanModeStrategy` vs. an enum-based scan-mode switch
  - Decision on PCode interpreter: retain, replace with expression library, or expose as config-side scripting?
- [ ] Update this document's §4 with any changes from the review.

**Estimated Effort**: 3–4 days  
**Output**: Updated §4, `schemas/animation-config.schema.json`, ADR document in `docs/adr/`.

---

### Issue #208 – Animation System POC

**Goal**: Build a minimal proof-of-concept that validates the key architectural decisions before committing to a full rewrite.

**Scope** (deliberately narrow – this is a POC, not production code):

1. Implement `IScanTimer` + `SystemScanTimer` + `TestScanTimer`.
2. Implement `IAnimationSession` as `AnimationSession`, backed by `IScanTimer` and a hardcoded `AutoScanStrategy`.
3. Write 10–15 unit tests that:
   - Control time via `TestScanTimer.ManualTick()`.
   - Assert that `AnimationStateChangedEvent` is published to `IEventBus` when state changes.
   - Assert correct widget highlight sequencing for a simple 3-widget scan.
   - Assert pause/resume preserves widget index.
4. Wire `AnimationSession` into `PanelAnimationManager` behind the existing `IPanelAnimationManager` interface **without** modifying any panel or UI code.
5. Run existing integration tests and verify no regressions.

**Acceptance Criteria**:

- [ ] `IScanTimer` interface exists and is documented.
- [ ] `SystemScanTimer` wraps `System.Timers.Timer` with no behavior change.
- [ ] `TestScanTimer` has a `ManualTick()` method; `Elapsed` fires synchronously when called.
- [ ] `AnimationSession` starts, stops, pauses, resumes, and advances the widget index correctly.
- [ ] `AnimationStateChangedEvent` is published to `IEventBus` on each state transition.
- [ ] Minimum 10 new unit tests; all passing.
- [ ] Existing architecture tests (`ACATCore.Tests.Architecture`) still pass.
- [ ] POC does not break any existing build targets.
- [ ] POC findings are documented in a brief write-up (1–2 pages) appended to this doc or in `docs/ANIMATION_POC_FINDINGS.md`.

**Estimated Effort**: 3–4 days  
**Output**: POC code in `src/Libraries/ACATCore/AnimationManagement/` (flagged with `// POC` comments), new test project or test class in `ACATCore.Tests`, `docs/ANIMATION_POC_FINDINGS.md`.

---

## 6. Migration Strategy

The rewrite will follow an **incremental strangler-fig approach** to avoid a big-bang cutover:

### Phase A – Seams (Issue #208 POC)
Introduce `IAnimationSession` and `IScanTimer` alongside existing code. `PanelAnimationManager` delegates to the new session internally while preserving its public interface. No UI or panel code changes.

### Phase B – Config Migration
- Author `XmlAnimationConfigAdapter` to bridge legacy `XmlNode` loading to `AnimationConfig`.
- Author `IAnimationConfigProvider` implementation that prefers JSON, falls back to XML.
- Add `animation-config.schema.json` and a migration path in `ConfigMigrationTool`.

### Phase C – Strategy Extraction
- Move auto-scan logic from `AnimationPlayer` into `AutoScanStrategy`.
- Move manual-scan logic into `ManualScanStrategy`.
- Move BCI-specific logic from `AnimationSharpManagerV2` into `BciScanStrategy`.
- `AnimationPlayer` becomes a thin adapter that delegates to the selected strategy.

### Phase D – EventBus Cutover
- Replace `EvtPlayerStateChanged` and `EvtPlayerAnimationTransition` delegates with `IEventBus` publications.
- Update all subscribers identified in Issue #206.

### Phase E – Cleanup
- Remove `AnimationPlayer` adapter (if no external code references it).
- Remove `AnimationManager` adapter (if no external code references it).
- Update DI registrations; mark legacy interfaces `[Obsolete]` where appropriate.

Each phase is independently releasable and testable. Phases A–B can run in parallel with Phase C.

---

## 7. Risks and Mitigations

| ID | Risk | Likelihood | Impact | Mitigation |
|----|------|-----------|--------|------------|
| R1 | Timing regressions break user experience | Medium | High | Keep `SystemScanTimer` as a thin wrapper; benchmark interval accuracy before and after |
| R2 | PCode interpreter compatibility breaks existing scan scripts | Low | High | Do not touch `Interpret` in Phase 3; the POC must prove the interpreter can be injected without modification |
| R3 | BCI extension divergence is larger than expected | Medium | Medium | Complete Issue #206 BCI analysis before starting Issue #208 |
| R4 | EventBus introduces latency in scan loop | Low | High | Profile event dispatch time; if > 1ms, use direct callbacks inside the session and only publish to bus from the session's public methods |
| R5 | XML panel configs do not all conform to the `AnimationConfig` schema | Medium | Medium | Run schema validation against all configs during Issue #207; fix gaps before migration |
| R6 | Thread-safety issues with new async-capable design | Medium | High | Keep `SyncLock` pattern in Phase A/B; introduce `lock`/`SemaphoreSlim` only in Phase C when threading model is fully understood |

---

## 8. Success Criteria

| Criterion | Metric | Target |
|-----------|--------|--------|
| Unit test coverage for animation engine | % lines covered | ≥ 80 % |
| Timing accuracy | Measured interval vs. configured interval | ≤ 5 % deviation |
| No regressions | Existing test suite pass rate | 100 % |
| Scan latency (actuator → highlight) | Measured end-to-end | ≤ 50 ms |
| Config load time | Per panel | ≤ 20 ms (consistent with Phase 1 target) |
| BCI parity | All BCI scan behaviors preserved | 100 % |
| Zero new `CoreGlobals` dependencies | Static analysis | 0 new usages in rewritten classes |
| Public interface backward compatibility | Compilation of existing callers | 0 compile errors |

---

## 9. Dependencies

| Dependency | Status | Notes |
|-----------|--------|-------|
| Phase 2 – DI infrastructure (`ServiceConfiguration.cs`) | ✅ Complete | Required for DI registration in Phase A |
| Phase 2 – `IEventBus` | ✅ Complete | Required for EventBus cutover in Phase D |
| Phase 1 – `JsonConfigurationLoader<T>` | ✅ Complete | Required for JSON config in Phase B |
| Phase 1 – JSON Schema patterns in `schemas/` | ✅ Complete | Required for `animation-config.schema.json` in Issue #207 |
| intel/acat#194 – Architecture Modernization | ✅ Complete | This spec depends on it |
| Phase 3 – Async/Await patterns | 📋 Future | Phase C/D should be designed to support async but need not implement it yet |
| WinUI 3 migration (Phase 4) | 📋 Future | Animation highlight rendering will change; ensure `IAnimationSession` does not reference `System.Windows.Forms` types |

---

## 10. Glossary

| Term | Definition |
|------|-----------|
| **Animation** | A named scan sequence: an ordered list of widgets the system highlights one at a time |
| **AnimationPlayer** | The class that owns the timer and advances through an animation's widget list |
| **AnimationWidget** | A wrapper that pairs a UI `Widget` with its per-step settings (beep, color, scripts) |
| **BCI** | Brain-Computer Interface – an input device that reads neural signals instead of physical switches |
| **Hesitate time** | Extra dwell time on the first widget of a new scan sequence, giving users time to orient |
| **PCode** | ACAT's proprietary bytecode script format used in `onEnter`, `onSelect`, `onEnd` hooks |
| **Scan interval / Stepping time** | How long (ms) each widget stays highlighted before advancing |
| **Scan mode** | The algorithm that determines which widget to highlight next (auto row-column vs. manual directional vs. BCI) |
| **Strangler-fig** | An incremental refactoring pattern: new code wraps old code, which is deleted once all consumers have migrated |

---

**Document Owner**: ACAT Architecture Team  
**Review Cycle**: Per-phase (update after each of Issue #206, #207, #208 completes)  
**Related Documents**:  
- [`ACAT_MODERNIZATION_PLAN.md`](../ACAT_MODERNIZATION_PLAN.md) – overall roadmap  
- [`docs/INTERFACE_EXTRACTION_GUIDE.md`](INTERFACE_EXTRACTION_GUIDE.md) – interface naming conventions  
- [`DEPENDENCY_INJECTION_GUIDE.md`](../DEPENDENCY_INJECTION_GUIDE.md) – DI patterns  
- [`src/ARCHITECTURE_IMPLEMENTATION_STATUS.md`](../src/ARCHITECTURE_IMPLEMENTATION_STATUS.md) – Phase 2 status  
