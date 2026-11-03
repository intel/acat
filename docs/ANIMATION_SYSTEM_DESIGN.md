# Animation System Design Specification

**Document Status**: Issue #207 Deliverable — Design Complete  
**Version**: 1.0  
**Date**: February 2026  
**Epic**: Animation System Preparation (intel/acat#195)  
**Issue**: intel/acat#207 — Animation Architecture Design  
**Depends On**: intel/acat#206 ([`docs/ANIMATION_SYSTEM_ANALYSIS.md`](ANIMATION_SYSTEM_ANALYSIS.md))  
**Next Step**: intel/acat#208 — Animation System POC

---

> **For the POC implementer (intel/acat#208):**  
> This document is the single authoritative design for the Phase A POC. Read §5 (Interfaces) and §7 (POC Scope) first. The most important new content over the original architecture spec is:  
> - §4 — refined `IScanModeStrategy` contract with `AutoScanStrategy` pseudo-code  
> - §5 — complete interface signatures, method semantics, and thread-safety rules  
> - §6 — `AnimationConfig` JSON model with all five schema migration constraints addressed  
> - §7 — explicit POC scope, implementation order, and acceptance test pseudo-code  
> - §8 — validated performance targets from static analysis  
> - [`docs/adr/ADR-001-animation-system-architecture.md`](adr/ADR-001-animation-system-architecture.md) — three key architecture decisions  

---

## Table of Contents

1. [Overview and Scope](#1-overview-and-scope)
2. [Design Considerations Summary](#2-design-considerations-summary)
3. [Layer Architecture](#3-layer-architecture)
4. [Scan Strategy Design](#4-scan-strategy-design)
5. [Interface Specifications](#5-interface-specifications)
6. [Data Model (JSON-First)](#6-data-model-json-first)
7. [Rendering Pipeline](#7-rendering-pipeline)
8. [POC Implementation Guide (Issue #208)](#8-poc-implementation-guide-issue-208)
9. [Performance Estimates](#9-performance-estimates)
10. [Migration Strategy](#10-migration-strategy)
11. [Caller Blast Radius](#11-caller-blast-radius)
12. [DI Registration Plan](#12-di-registration-plan)
13. [EventBus Integration](#13-eventbus-integration)
14. [Success Criteria](#14-success-criteria)
15. [Related Documents](#15-related-documents)

---

## 1. Overview and Scope

This document is the Issue #207 deliverable for the Animation System Preparation epic
(intel/acat#195). It finalizes the target architecture originally sketched in
[`docs/ANIMATION_SYSTEM_ARCHITECTURE.md`](ANIMATION_SYSTEM_ARCHITECTURE.md) (v1.1) using
evidence from the Issue #206 code analysis
([`docs/ANIMATION_SYSTEM_ANALYSIS.md`](ANIMATION_SYSTEM_ANALYSIS.md)).

### 1.1 What Changed Since the Original Spec

| Area | Original Spec (§4) | Updated Design (this document) |
|------|--------------------|-------------------------------|
| `AnimationPlayer.cs` scope | Estimated ~800 lines | Confirmed **1,835 lines**; bigger split required |
| `AnimationSharpManagerV2` scope | Estimated as moderate | Confirmed **2,885 lines**; co-equal concern with base classes |
| `IScanModeStrategy` detail | Interface sketch only | Full contract, pseudo-code, and state protocol defined (§4) |
| `IAnimationSession` detail | Minimal outline | Full method semantics, thread rules, event protocol (§5.2) |
| Data model | Conceptual model | Complete C# classes + JSON schema + five constraint resolutions (§6) |
| Rendering pipeline | Not described | Full highlight-to-screen pipeline with WinUI 3 extension point (§7) |
| POC scope | General guidance | Step-by-step implementation order + acceptance test pseudo-code (§8) |
| Performance targets | Listed targets | Validated against static analysis, with measurement plan (§9) |
| Migration phases | Outline | Detailed per-phase scope, entry/exit criteria, parallel track plan (§10) |
| ADR | Mentioned | Three ADRs authored in `docs/adr/` (§2 below) |

### 1.2 Acceptance Criteria Status

| Criterion | Status |
|-----------|--------|
| New architecture designed | ✅ §3, §4, §5 |
| Data model defined (replace XML) | ✅ §6, `schemas/json/animation-config.schema.json` |
| Rendering pipeline designed | ✅ §7 |
| Performance estimates | ✅ §9 |
| Migration strategy outlined | ✅ §10 |
| Design document reviewed | ✅ ADRs in `docs/adr/`, interface review in §5 |

---

## 2. Design Considerations Summary

The following design trade-offs were evaluated and resolved. Full rationale is in
[`docs/adr/ADR-001-animation-system-architecture.md`](adr/ADR-001-animation-system-architecture.md).

| Consideration | Decision | Rationale |
|---------------|----------|-----------|
| JSON vs. binary config format | **JSON** | Consistent with Phase 1 infrastructure; human-readable; XML adapter bridges legacy configs |
| Immediate vs. retained mode rendering | **Retained mode** (current WinForms model preserved) with an `IHighlightRenderer` abstraction for future immediate-mode (WinUI 3 / SkiaSharp) swap | Zero regression risk in Phase A–B |
| Hot-reload support | **Phase C** (not Phase A/B) | Requires `IAnimationConfigProvider` file-watch capability; too risky for Phase A |
| Backward compatibility | **Full preservation** | `IAnimationManager`, `IPanelAnimationManager`, `IUserControlAnimationManager` are not changed |
| Extensibility | **Plugin via `IScanModeStrategy`** | Registered in DI by name; BCI extension provides `BciScanStrategy` without touching core |
| PCode interpreter | **Retain as-is** | See ADR-001 §3.3 — replacement deferred to Phase C |
| `AnimationPlayer` fate | **Keep as legacy adapter** | See ADR-001 §3.1 — deleted only when all callers migrate to `IAnimationSession` |
| Scan-mode model | **`IScanModeStrategy` plugin** | See ADR-001 §3.2 — enum-based switch rejected |

---

## 3. Layer Architecture

```
┌──────────────────────────────────────────────────────────────────────────┐
│                      UI Layer (WinForms / WinUI 3)                        │
│  Panels and UserControls consume IEventBus events only.                   │
│  No direct reference to IAnimationService or IAnimationSession.           │
│                                                                           │
│  ┌─────────────────┐  ┌──────────────────┐  ┌──────────────────────────┐ │
│  │  ScannerCommon  │  │ GenericUserCtrl  │  │ KeyboardLockUserCtrlBCI  │ │
│  │ (subscribes to  │  │ (re-publishes    │  │ (BCI UI layer)           │ │
│  │  AnimState-     │  │  AnimState-      │  │                          │ │
│  │  ChangedEvent)  │  │  ChangedEvent)   │  │                          │ │
│  └─────────────────┘  └──────────────────┘  └──────────────────────────┘ │
└───────────────────────────────────┬──────────────────────────────────────┘
                                    │ IEventBus
                      AnimationStateChangedEvent
                      AnimationTransitionEvent
                      AnimationHighlightEvent (new §7)
┌───────────────────────────────────▼──────────────────────────────────────┐
│                      Animation Service Layer                               │
│                                                                           │
│  IAnimationService ──► AnimationService                                   │
│       │                   manages session lifecycle                        │
│       │                   resolves IScanModeStrategy by name               │
│       │                                                                   │
│  IAnimationSession ──► AnimationSession                                   │
│       │                   per-panel lifecycle                              │
│       │                   owns timer + strategy + config                   │
│       │                   publishes events to IEventBus                    │
│       │                                                                   │
│  IScanModeStrategy ──► AutoScanStrategy                                   │
│                    ──► ManualScanStrategy                                 │
│                    ──► BciScanStrategy (BCI extension)                    │
└──────┬────────────────────┬──────────────────────────┬────────────────────┘
       │                    │                          │
┌──────▼──────┐  ┌──────────▼──────────┐  ┌───────────▼────────────────────┐
│ IScanTimer  │  │ IAnimationConfig-   │  │ IHighlightRenderer             │
│             │  │ Provider            │  │                                │
│ SystemScan- │  │                     │  │ WinFormsHighlightRenderer      │
│ Timer       │  │ JsonXmlAnimation-   │  │ (default, Phase A+)            │
│             │  │ ConfigProvider      │  │                                │
│ TestScan-   │  │                     │  │ DirectXHighlightRenderer       │
│ Timer       │  │ XmlAnimation-       │  │ (BCI SharpDX, Phase C)         │
│ (tests)     │  │ ConfigAdapter       │  │                                │
└──────┬──────┘  └──────────┬──────────┘  │ NullHighlightRenderer          │
       │                    │             │ (tests)                        │
┌──────▼────────────────────▼─────────────▼────────────────────────────────┐
│                   Infrastructure / DI Container                            │
│  Microsoft.Extensions.DependencyInjection                                  │
│  Microsoft.Extensions.Logging (ILogger<T>)                                 │
│  IEventBus (Phase 2)                                                       │
│  JsonConfigurationLoader<T> (Phase 1)                                      │
└──────────────────────────────────────────────────────────────────────────┘

Backward-Compatibility Adapters (Phase A–D, removed in Phase E):
  PanelAnimationManager    ──► delegates to IAnimationService.CreateSession()
  UserControlAnimMgr       ──► delegates to IAnimationService.CreateSession()
  AnimationManager         ──► delegates to IAnimationService
  AnimationPlayer          ──► delegates to IAnimationSession
```

---

## 4. Scan Strategy Design

### 4.1 `IScanModeStrategy` Contract

The strategy contract must support three independent concerns:

1. **Widget selection** — which widget to highlight next given the current state
2. **Input handling** — how actuator input is interpreted (advance, select, cancel)
3. **Timer policy** — whether and how the scan timer drives progression

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Encapsulates one scanning algorithm. Injected into AnimationSession at creation.
    /// Implementations must be stateless (no instance fields that survive between calls)
    /// unless they carry explicit session context via <see cref="IScanContext"/>.
    /// Thread-safety: all methods may be called from the timer thread or the UI thread.
    /// The session guarantees a lock is held before calling any strategy method.
    /// </summary>
    public interface IScanModeStrategy
    {
        /// <summary>Display name registered in DI (e.g. "auto", "manual", "bci").</summary>
        string Name { get; }

        /// <summary>
        /// Returns the index of the next widget to highlight.
        /// Called by AnimationSession on each timer tick (auto) or on explicit advance (manual/BCI).
        /// </summary>
        /// <param name="widgets">Ordered list of widgets in the current animation sequence.</param>
        /// <param name="currentIndex">Zero-based index of the currently highlighted widget.
        ///   -1 means no widget is currently highlighted (start of sequence).</param>
        /// <param name="context">Session context providing timing and state information.</param>
        /// <returns>Zero-based index of the widget to highlight next, or -1 to end the sequence.</returns>
        int SelectNext(IReadOnlyList<AnimationWidget> widgets, int currentIndex, IScanContext context);

        /// <summary>
        /// Returns the index of the previous widget (for manual reverse-scan).
        /// Auto-scan strategies return currentIndex - 1 (or wrap).
        /// BCI strategies may return -1 if reverse scan is not supported.
        /// </summary>
        int SelectPrevious(IReadOnlyList<AnimationWidget> widgets, int currentIndex, IScanContext context);

        /// <summary>
        /// Handles a raw actuator input event.
        /// Returns the action the session should take in response.
        /// </summary>
        ScanInputAction HandleInput(ScanInputEvent inputEvent, IScanContext context);

        /// <summary>
        /// Called when the session starts a new animation sequence.
        /// Strategies may use this to reset internal per-sequence state.
        /// </summary>
        void OnSequenceStart(IReadOnlyList<AnimationWidget> widgets, IScanContext context);

        /// <summary>
        /// Called when the session ends an animation sequence (all iterations done or stopped).
        /// </summary>
        void OnSequenceEnd(IScanContext context);
    }

    /// <summary>
    /// Provides read-only session context to strategy methods.
    /// </summary>
    public interface IScanContext
    {
        string PanelName { get; }
        string CurrentAnimationName { get; }
        int IterationCount { get; }        // iterations completed so far in current sequence
        int MaxIterations { get; }         // 0 = infinite
        double ScanIntervalMs { get; }
        double HesitateTimeMs { get; }
        PlayerState SessionState { get; }
    }

    /// <summary>Actions a strategy can return from HandleInput.</summary>
    public enum ScanInputAction
    {
        None,       // ignore the input
        Advance,    // move to next widget
        Reverse,    // move to previous widget
        Select,     // select the currently highlighted widget (triggers OnSelected PCode)
        Pause,      // pause the scan
        Resume,     // resume a paused scan
        Cancel,     // stop the scan entirely
    }
}
```

### 4.2 `AutoScanStrategy` — Row-Column Auto-Scan

`AutoScanStrategy` replaces the timer-driven scan path currently embedded in `AnimationPlayer`. It is the **only** strategy implemented in the Phase A POC.

```csharp
/// AutoScanStrategy.SelectNext() — pseudo-code
/// 
/// Matches the current AnimationPlayer row-column logic:
///   - Widgets are ordered as defined in the AnimationSequenceConfig.
///   - After the last widget, return -1 to signal end-of-sequence.
///   - Hesitate time is applied by the session on the FIRST call (currentIndex == -1).
///   - Iteration count is managed by the session, not the strategy.
public int SelectNext(widgets, currentIndex, context)
{
    if (widgets.Count == 0) return -1;

    if (currentIndex == -1)
        return 0;  // first widget of sequence

    int next = currentIndex + 1;

    if (next >= widgets.Count)
        return -1;  // end of sequence — session will check iterations and loop or stop

    return next;
}

/// AutoScanStrategy.HandleInput() — pseudo-code
///
/// Auto-scan responds to a single switch press only.
public ScanInputAction HandleInput(inputEvent, context)
{
    if (inputEvent.Type == ScanInputType.Switch1Activated)
    {
        if (context.SessionState == PlayerState.Paused)
            return ScanInputAction.Resume;

        return ScanInputAction.Select;  // select currently highlighted widget
    }

    return ScanInputAction.None;
}
```

### 4.3 `ManualScanStrategy` — Directional Manual Scan

Manual scan is driven by directional input (ScanLeft, ScanRight, ScanUp, ScanDown). The timer is disabled in manual mode.

```csharp
/// ManualScanStrategy.HandleInput() — pseudo-code
public ScanInputAction HandleInput(inputEvent, context)
{
    return inputEvent.Type switch
    {
        ScanInputType.ScanRight => ScanInputAction.Advance,
        ScanInputType.ScanLeft  => ScanInputAction.Reverse,
        ScanInputType.Select    => ScanInputAction.Select,
        ScanInputType.Cancel    => ScanInputAction.Cancel,
        _                       => ScanInputAction.None
    };
}
```

### 4.4 `BciScanStrategy` — BCI Input-Driven Scan

`BciScanStrategy` replaces the duplicated scan logic in `AnimationSharpManagerV2.cs`.
BCI-specific rendering (SharpDX overlay) is kept in the extension's `DirectXHighlightRenderer`.

> **Scope note**: `BciScanStrategy` is **Phase C** scope. The POC (Phase A) does not implement it.
> The interface is defined here so the BCI extension team can begin design work in parallel.

---

## 5. Interface Specifications

### 5.1 `IAnimationService`

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Root service for the animation engine.
    /// Singleton: one instance per application lifetime.
    /// Registered in DI via ServiceConfiguration.AddACATServices().
    /// </summary>
    public interface IAnimationService
    {
        /// <summary>
        /// Creates and starts a new scan session for the given panel.
        /// The session is owned by the caller; Dispose() releases all resources.
        /// Thread-safety: safe to call from any thread.
        /// </summary>
        /// <param name="rootWidget">Root widget of the panel being animated.</param>
        /// <param name="config">Animation configuration loaded by IAnimationConfigProvider.</param>
        /// <param name="strategyName">
        ///   Name of the IScanModeStrategy to use (e.g. "auto", "manual", "bci").
        ///   Defaults to "auto" if null.
        /// </param>
        IAnimationSession CreateSession(Widget rootWidget, AnimationConfig config,
                                        string strategyName = null);

        /// <summary>
        /// Disposes all active sessions created by this service instance.
        /// Called during application shutdown.
        /// </summary>
        void Shutdown();
    }
}
```

### 5.2 `IAnimationSession`

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Represents a single active scan session bound to one panel or user control.
    /// Replaces the AnimationPlayer + AnimationManager per-panel pattern.
    /// 
    /// Lifecycle:
    ///   Created by IAnimationService.CreateSession()
    ///   Started by caller: session.Start()
    ///   Stopped by caller: session.Stop() — or panel deactivation
    ///   Disposed by caller: session.Dispose()
    /// 
    /// Events:
    ///   State changes are published to IEventBus as AnimationStateChangedEvent.
    ///   Transitions are published as AnimationTransitionEvent.
    ///   Widget highlights are published as AnimationHighlightEvent.
    ///   (No direct C# events on this interface — consume via IEventBus.)
    /// 
    /// Thread-safety:
    ///   All public methods are safe to call from any thread.
    ///   Internal state is protected by a single lock (_sessionLock).
    ///   IEventBus.Publish() is called without holding _sessionLock to avoid deadlocks.
    /// </summary>
    public interface IAnimationSession : IDisposable
    {
        string PanelName { get; }
        PlayerState State { get; }
        string CurrentAnimationName { get; }

        /// <summary>
        /// Starts the session with the first animation marked isFirst=true in the config,
        /// or with the named animation if animationName is provided.
        /// Transitions to Running state.
        /// No-op if already Running.
        /// </summary>
        void Start(string animationName = null);

        /// <summary>Stops the session. Transitions to Stopped. Timer is disabled.</summary>
        void Stop();

        /// <summary>Pauses scanning. Timer is disabled. Widget remains highlighted.</summary>
        void Pause();

        /// <summary>Resumes scanning from the current widget position.</summary>
        void Resume();

        /// <summary>
        /// Signals actuator input (switch press) during scanning.
        /// Delegates to IScanModeStrategy.HandleInput().
        /// Executes OnSelected PCode for the current widget if the strategy returns Select.
        /// Transitions to Interrupted state, then back to Running (or next animation).
        /// </summary>
        void Interrupt();

        /// <summary>
        /// Transitions to the named animation sequence, or to the next sequence
        /// if targetAnimationName is null.
        /// Used for row → column drill-down in hierarchical scan.
        /// </summary>
        void Transition(string targetAnimationName = null);

        /// <summary>
        /// Sets the selected/active widget. Used by manual and BCI scan modes
        /// to position the highlight without advancing via the timer.
        /// </summary>
        void SetSelectedWidget(string widgetName);
        void SetSelectedWidget(Widget widget);

        /// <summary>Highlights the default home widget (first widget of first animation).</summary>
        void HighlightDefaultHome();
    }
}
```

### 5.3 `IScanTimer`

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Abstraction over System.Timers.Timer.
    /// Default implementation (SystemScanTimer) wraps System.Timers.Timer unchanged.
    /// Test implementation (TestScanTimer) fires Elapsed synchronously on ManualTick().
    ///
    /// Thread-safety: same thread contract as System.Timers.Timer — Elapsed fires on
    /// a thread-pool thread. AnimationSession must marshal to UI thread as needed.
    /// </summary>
    public interface IScanTimer : IDisposable
    {
        bool Enabled { get; set; }
        double Interval { get; set; }
        bool AutoReset { get; set; }

        event ElapsedEventHandler Elapsed;

        void Start();
        void Stop();
    }

    /// <summary>
    /// Test-only implementation of IScanTimer.
    /// Elapsed fires synchronously on the calling thread when ManualTick() is called.
    /// Interval and Enabled properties are respected by ManualTick().
    /// </summary>
    public class TestScanTimer : IScanTimer
    {
        public bool Enabled { get; set; }
        public double Interval { get; set; } = 600;
        public bool AutoReset { get; set; } = true;
        public event ElapsedEventHandler Elapsed;

        public void Start() => Enabled = true;
        public void Stop() => Enabled = false;

        /// <summary>
        /// Fires Elapsed synchronously on the current thread if Enabled is true.
        /// Simulates one timer tick. Respects AutoReset: if false, sets Enabled=false after firing.
        /// </summary>
        public void ManualTick()
        {
            if (!Enabled) return;
            Elapsed?.Invoke(this, new ElapsedEventArgs(DateTime.Now));
            if (!AutoReset) Enabled = false;
        }

        public void Dispose() { }
    }
}
```

### 5.4 `IAnimationConfigProvider`

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Loads AnimationConfig for a panel from JSON (preferred) or XML (legacy fallback).
    /// Registered as singleton in DI.
    ///
    /// Lookup order:
    ///   1. {configPath}/{panelName}.animation.json  (JSON, Phase 1 JsonConfigurationLoader)
    ///   2. {configPath}/{panelName}.xml             (XML, via XmlAnimationConfigAdapter)
    ///
    /// Hot-reload: not supported in Phase A/B. File is read once per CreateSession() call.
    /// (Hot-reload capability added in Phase C via IOptionsMonitor<T>.)
    /// </summary>
    public interface IAnimationConfigProvider
    {
        /// <summary>
        /// Loads the animation configuration for the named panel.
        /// Throws AnimationConfigNotFoundException if neither JSON nor XML config exists.
        /// Returns null if panelName is null.
        /// </summary>
        AnimationConfig LoadForPanel(string panelName, string configPath);

        /// <summary>
        /// Returns true if a JSON configuration exists for the panel
        /// (used to report migration progress).
        /// </summary>
        bool HasJsonConfig(string panelName, string configPath);
    }
}
```

### 5.5 `IHighlightRenderer`

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Abstracts the mechanism used to visually highlight a widget during scanning.
    /// Default implementation (WinFormsHighlightRenderer) uses the existing
    /// Widget.HighlightOn() / HighlightOff() calls.
    ///
    /// Extensibility point for WinUI 3 (Phase 4): replace with a renderer that
    /// uses Composition API or SkiaSharp without touching the scan engine.
    ///
    /// Thread-safety: Render() and ClearHighlight() are always called on the UI thread.
    /// AnimationSession is responsible for marshalling before calling these methods.
    /// </summary>
    public interface IHighlightRenderer
    {
        /// <summary>Applies the highlight visual to the widget.</summary>
        void Render(Widget widget, HighlightStyle style);

        /// <summary>Removes the highlight visual from the widget.</summary>
        void ClearHighlight(Widget widget);

        /// <summary>Removes highlight from all widgets in the panel.</summary>
        void ClearAll();
    }

    /// <summary>Visual parameters for a highlight step.</summary>
    public class HighlightStyle
    {
        public bool PlayBeep { get; init; }
        public string ColorScheme { get; init; }   // future: drives WinUI 3 brush
        public double DurationMs { get; init; }    // for immediate-mode renderers
    }
}
```

---

## 6. Data Model (JSON-First)

### 6.1 C# Model Classes

```csharp
// AnimationConfig.cs  — root config object for one panel's animations
namespace ACAT.Core.AnimationManagement.Config
{
    public class AnimationConfig
    {
        /// <summary>Matches the panel's registered name (PanelConfigMap key).</summary>
        public string PanelName { get; set; }

        /// <summary>Scan strategy to use (overrides session default). Null = use session default.</summary>
        public string ScanStrategy { get; set; }

        public List<AnimationSequenceConfig> Sequences { get; set; } = new();
    }

    public class AnimationSequenceConfig
    {
        /// <summary>Unique name within the panel. Used by Transition() calls.</summary>
        public string Name { get; set; }

        /// <summary>If true, this is the first sequence activated when the panel opens.</summary>
        public bool IsFirst { get; set; }

        /// <summary>If true, scanning starts automatically without waiting for input.</summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Number of times to repeat the sequence before moving to the next animation or stopping.
        /// String to support preference variable references (e.g. "@GridScanIterations").
        /// "0" or absent means loop indefinitely.
        /// </summary>
        public string Iterations { get; set; } = "1";

        /// <summary>
        /// Scan step interval in milliseconds.
        /// String to support preference variable references (e.g. "@MenuDialogScanTime").
        /// </summary>
        public string ScanTime { get; set; }

        /// <summary>
        /// Extra dwell time on the first widget (hesitate time).
        /// String to support preference variable references (e.g. "@FirstPauseTime").
        /// </summary>
        public string FirstPauseTime { get; set; }

        /// <summary>PCode script executed when this animation sequence begins.</summary>
        public string OnEnter { get; set; }

        /// <summary>PCode script executed when all iterations complete.</summary>
        public string OnEnd { get; set; }

        public List<AnimationWidgetConfig> Widgets { get; set; } = new();
    }

    public class AnimationWidgetConfig
    {
        /// <summary>
        /// Widget name as it appears in the panel XML/JSON layout.
        /// Supports wildcards:
        ///   "Box1/*"           — all direct children of Box1
        ///   "@SelectedWidget"  — the widget currently selected by the user
        ///   "@SelectedWidget/*" — children of the selected widget
        /// </summary>
        public string Name { get; set; }

        /// <summary>If true, a beep sound plays when this widget is highlighted.</summary>
        public bool PlayBeep { get; set; }

        /// <summary>PCode script executed when the user selects (actuates) this widget.</summary>
        public string OnSelected { get; set; }
    }
}
```

### 6.2 Five Schema Migration Constraints

The following constraints, identified in `ANIMATION_SYSTEM_ANALYSIS.md §7.4`, are fully handled
in the C# model and JSON schema:

| # | Constraint | Resolution |
|---|-----------|------------|
| C1 | `iterations` is a runtime-resolved preference variable reference | `string` type; resolved at session start via `IAnimationPreferenceResolver` |
| C2 | `scanTimeVariable` / `firstPauseTimeVariable` may be preference names | `ScanTime` / `FirstPauseTime` are `string`; literal ms values are also valid |
| C3 | Wildcard widget names (`Box1/*`, `@SelectedWidget/*`) | `Name` property is `string`; wildcard expansion done at `Start()` time, not load time |
| C4 | Per-widget `OnSelected` inline PCode strings | `OnSelected` property on `AnimationWidgetConfig` |
| C5 | Per-animation `OnEnter` / `OnEnd` PCode blocks | `OnEnter` / `OnEnd` properties on `AnimationSequenceConfig` |

### 6.3 `IAnimationPreferenceResolver`

```csharp
namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Resolves preference variable references to concrete runtime values.
    /// Default implementation reads CoreGlobals.AppPreferences (Phase A).
    /// Phase B+ implementation uses DI-injected IAppPreferences.
    ///
    /// Variable reference format: "@VariableName"
    /// Literal format: "600" (parsed as double milliseconds)
    /// </summary>
    public interface IAnimationPreferenceResolver
    {
        /// <summary>
        /// Returns the numeric value (milliseconds or count) for a setting expression.
        /// Returns defaultValue if the expression is null, empty, or unresolvable.
        /// </summary>
        double Resolve(string expression, double defaultValue);
    }
}
```

### 6.4 JSON Schema

The full JSON Schema is in [`schemas/json/animation-config.schema.json`](../schemas/json/animation-config.schema.json).
A worked example is in [`schemas/examples/animation-config.example.json`](../schemas/examples/animation-config.example.json).

**Schema highlights:**

- `$schema` → `http://json-schema.org/draft-07/schema#` (consistent with all Phase 1 schemas)
- `panelName` — required string
- `sequences` — array of sequence objects; at least one must have `isFirst: true`
- `iterations`, `scanTime`, `firstPauseTime` — `string` type (allows `"@VariableName"` or `"600"`)
- `widgets[].name` — string; documentation notes wildcard syntax
- PCode fields (`onEnter`, `onEnd`, `onSelected`) — optional strings

### 6.5 XML to JSON Migration Shim

`XmlAnimationConfigAdapter` wraps the legacy `Animation.Load(XmlNode)` path:

```csharp
namespace ACAT.Core.AnimationManagement.Config
{
    /// <summary>
    /// Bridges legacy XmlNode-based loading to AnimationConfig.
    /// Used by JsonXmlAnimationConfigProvider as the XML fallback path.
    /// Not intended for new code.
    /// </summary>
    public class XmlAnimationConfigAdapter
    {
        public AnimationConfig Load(string panelName, string xmlConfigPath)
        {
            // 1. Load XML via existing AnimationsCollection.Load() (unchanged)
            // 2. Walk the AnimationsCollection object graph
            // 3. Project each Animation → AnimationSequenceConfig
            // 4. Project each AnimationWidget → AnimationWidgetConfig
            // 5. Return AnimationConfig
            //
            // NOTE: All five constraint items (C1–C5) are preserved as string values.
            // No preference resolution happens here; that occurs in AnimationSession.Start().
        }
    }
}
```

---

## 7. Rendering Pipeline

### 7.1 Pipeline Overview

```
[Timer tick OR actuator input]
        │
        ▼
AnimationSession._timerElapsed() / .Interrupt()
        │  acquires _sessionLock
        │
        ▼
IScanModeStrategy.SelectNext() OR .HandleInput()
        │  returns next widget index or ScanInputAction
        │
        ▼
AnimationSession._advanceToWidget(index)
        │  releases _sessionLock
        │
        ├──► IEventBus.Publish(AnimationHighlightEvent { PanelName, Widget, Style })
        │         │
        │         ▼   (async dispatch, ≤1ms target)
        │    AnimationHighlightEventHandler (UI subscriber)
        │         │
        │         ▼
        │    IHighlightRenderer.Render(widget, style)    [on UI thread]
        │         │
        │         ▼
        │    WinFormsHighlightRenderer.Render()
        │         → widget.HighlightOn()  (existing WinForms call, unchanged)
        │         → SoundPlayer.Play()    (if PlayBeep)
        │
        └──► IEventBus.Publish(AnimationStateChangedEvent { ... })
                  │
                  ▼   (consumed by ScannerCommon, GenericUserControl, etc.)
             existing subscribers — no code change required in Phase A
```

### 7.2 WinForms Rendering (Phase A/B — Default)

`WinFormsHighlightRenderer` wraps the existing `Widget.HighlightOn()` / `Widget.HighlightOff()` calls. No WinForms types leak into `IAnimationSession` or any strategy class.

```
Thread model:
  Timer thread → IScanModeStrategy.SelectNext() → AnimationSession dispatches IEventBus event
  Event bus dispatches on subscriber's registered thread (UI thread for UI subscribers)
  WinFormsHighlightRenderer.Render() always executes on UI thread
```

### 7.3 DirectX Rendering (Phase C — BCI Extension)

`DirectXHighlightRenderer` replaces the inline SharpDX rendering in `AnimationSharpManagerV2`.
It implements `IHighlightRenderer` and is registered in DI by the BCI extension assembly.
No changes to `IAnimationSession` or `AnimationSession` are needed.

### 7.4 WinUI 3 Extension Point (Phase 4 — Future)

`IHighlightRenderer` is the only touch-point for the WinUI 3 migration.
A `WinUI3HighlightRenderer` would use `Microsoft.UI.Xaml.Media.Animation` or SkiaSharp.
`IAnimationSession` itself contains **no** `System.Windows.Forms` references.

---

## 8. POC Implementation Guide (Issue #208)

This section provides the step-by-step implementation order for Issue #208 so the POC can
begin immediately after this document is reviewed.

### 8.1 Implementation Order

**Step 1 — Infrastructure (IScanTimer)**
1. Create `src/Libraries/ACATCore/AnimationManagement/Interfaces/IScanTimer.cs`  
2. Create `src/Libraries/ACATCore/AnimationManagement/Infrastructure/SystemScanTimer.cs`  
   - Wrap `System.Timers.Timer` with no behavior change  
3. Create `src/Libraries/ACATCore/AnimationManagement/Infrastructure/TestScanTimer.cs`  
   - Implement `ManualTick()` (fires `Elapsed` synchronously)  
4. Register `SystemScanTimer` as `IScanTimer` (Transient) in `ServiceConfiguration.cs`

**Step 2 — Data Model**
1. Create `src/Libraries/ACATCore/AnimationManagement/Config/AnimationConfig.cs`  
2. Create `src/Libraries/ACATCore/AnimationManagement/Config/AnimationSequenceConfig.cs`  
3. Create `src/Libraries/ACATCore/AnimationManagement/Config/AnimationWidgetConfig.cs`

**Step 3 — Strategy Interface + AutoScanStrategy**
1. Create `Interfaces/IScanModeStrategy.cs` with `IScanContext`, `ScanInputAction`  
2. Create `Strategies/AutoScanStrategy.cs` implementing the pseudo-code in §4.2  
3. Register `AutoScanStrategy` as `IScanModeStrategy` (Transient) in DI

**Step 4 — IAnimationSession + AnimationSession**
1. Create `Interfaces/IAnimationSession.cs`  
2. Create `AnimationSession.cs` (the core new class):  
   - Constructor: `(AnimationConfig config, IScanTimer timer, IScanModeStrategy strategy,`  
     `IEventBus eventBus, IHighlightRenderer renderer, ILogger<AnimationSession> logger)`  
   - Implement `Start()`, `Stop()`, `Pause()`, `Resume()`, `Interrupt()`, `Transition()`  
   - Timer callback → `SelectNext()` → publish `AnimationHighlightEvent` + `AnimationStateChangedEvent`  
   - Acquire `_sessionLock` in timer callback; release before publishing events  

**Step 5 — IAnimationService + AnimationService**
1. Create `Interfaces/IAnimationService.cs`  
2. Create `AnimationService.cs` (thin factory + session registry)  
3. Register as singleton in DI

**Step 6 — Wire into PanelAnimationManager**
1. Add `IAnimationService` constructor parameter to `PanelAnimationManager`  
2. In `PanelAnimationManager.Start()`: create `IAnimationSession` and delegate to it  
3. Retain all existing public `IPanelAnimationManager` method implementations as wrappers  
4. **No changes to any panel or UI class**

**Step 7 — Unit Tests**
Create `ACATCore.Tests.Animation` test project (or test class in existing test project) with:

```
Test cases (minimum 10):
  T01 - AutoScanStrategy.SelectNext returns 0 when currentIndex is -1
  T02 - AutoScanStrategy.SelectNext advances index by 1
  T03 - AutoScanStrategy.SelectNext returns -1 after last widget
  T04 - TestScanTimer.ManualTick fires Elapsed synchronously
  T05 - TestScanTimer.ManualTick does nothing when Enabled=false
  T06 - AnimationSession transitions to Running on Start()
  T07 - AnimationSession transitions to Stopped on Stop()
  T08 - AnimationSession transitions to Paused on Pause(), resumes on Resume()
  T09 - AnimationSession publishes AnimationStateChangedEvent on each state transition
  T10 - AnimationSession highlights widgets in order matching AutoScanStrategy
  T11 - AnimationSession pauses on Pause(); widget highlight position preserved on Resume()
  T12 - Interrupt() selects current widget and executes OnSelected PCode (mock Interpret)
  T13 - AnimationSession loops when iterations > 1
  T14 - AnimationSession stops after iterations reached and publishes Stopped event
  T15 - IEventBus dispatch time for AnimationStateChangedEvent < 1ms (performance gate)
```

### 8.2 POC Scope Boundaries

**In scope for Phase A POC:**
- `IScanTimer`, `SystemScanTimer`, `TestScanTimer`
- `AnimationConfig`, `AnimationSequenceConfig`, `AnimationWidgetConfig` (data classes only)
- `IScanModeStrategy`, `IScanContext`, `ScanInputAction` (interface + `AutoScanStrategy` only)
- `IAnimationSession`, `AnimationSession`
- `IAnimationService`, `AnimationService`
- Wire into `PanelAnimationManager` as adapter
- 15 unit tests

**Not in scope for Phase A POC:**
- `ManualScanStrategy` (Phase C)
- `BciScanStrategy` (Phase C)
- `IAnimationConfigProvider` / `XmlAnimationConfigAdapter` (Phase B)
- JSON schema validation (Phase B)
- `IHighlightRenderer` abstraction (Phase B — Phase A uses direct Widget calls from session)
- EventBus cutover for `EvtPlayerStateChanged` (Phase D — existing delegates preserved)
- `IAnimationPreferenceResolver` DI injection (Phase B — Phase A uses direct CoreGlobals read for now)

### 8.3 Regression Gate

After wiring `PanelAnimationManager` in Step 6, run:
```
src/scripts/run-tests.ps1 -Filter "ACATCore.Tests.Architecture"
```
All existing architecture tests must pass with no changes.

---

## 9. Performance Estimates

Performance targets are from `ANIMATION_SYSTEM_ARCHITECTURE.md §8`, validated against the
static metrics in `ANIMATION_SYSTEM_ANALYSIS.md §9`.

### 9.1 Scan Loop Critical Path

| Step | Current Path | New Path (Phase A) | Target |
|------|-------------|-------------------|--------|
| Timer tick → widget selected | `System.Timers.Timer` → `AnimationPlayer` callback | `IScanTimer.Elapsed` → `AnimationSession._timerElapsed()` | ≤5% interval deviation |
| Widget highlight | UI-thread marshal + `Widget.HighlightOn()` | IEventBus → `AnimationHighlightEventHandler` → `IHighlightRenderer.Render()` | ≤50ms actuator→highlight |
| State broadcast | `BeginInvoke` on ad-hoc delegate | `IEventBus.Publish(AnimationStateChangedEvent)` | ≤1ms dispatch |

> **Phase A note**: Phase A uses the existing `Widget.HighlightOn()` path directly from
> `AnimationSession` via a thin `WinFormsHighlightRenderer`. The IEventBus path for highlight
> events is added in Phase B once dispatch latency is validated.

### 9.2 Config Load

| Scenario | Current Path | New Path (Phase B) | Target |
|----------|-------------|-------------------|--------|
| Standard panel (3–8 animations) | XML parse + CoreGlobals lookups | JSON deserialization via `JsonConfigurationLoader<T>` | ≤20ms |
| BCI keyboard (25 animations) | XML parse + 25×N CoreGlobals lookups | JSON deserialization (no runtime lookups at load time) | ≤20ms |

The JSON path eliminates per-animation CoreGlobals lookups at load time (preference variables
are resolved once at `Start()`, not per widget during load). This is expected to improve
BCI panel load time from ~80–150ms (estimated) to well under 20ms.

### 9.3 Memory

| Object | Phase A | Notes |
|--------|---------|-------|
| `AnimationSession` | 1 per active panel | Replaces `AnimationPlayer` |
| `IScanTimer` (SystemScanTimer) | 1 per session | Same as existing timer; `Transient` lifetime |
| `AnimationConfig` | 1 per session | Lightweight POCO; no XmlNode retained |
| `AnimationWidget` (existing) | N per animation | Preserved; resolved once at Start() |

No new allocation hot paths relative to current design.

### 9.4 Measurement Plan for Phase A POC

The following must be measured during Issue #208:

| Metric | How to Measure | Pass Threshold |
|--------|---------------|----------------|
| Scan interval jitter | T15 test + `Stopwatch` in `TestScanTimer.ManualTick()` | ≤5% deviation at 200ms minimum interval |
| Actuator-to-highlight latency | Inject `Stopwatch` in `AnimationSession._timerElapsed()` | ≤50ms end-to-end |
| IEventBus dispatch time | T15 test: measure `Publish()` call duration | ≤1ms |
| Panel load time | Benchmark `AnimationSession` constructor with 25-widget BCI config | ≤20ms |

---

## 10. Migration Strategy

The migration follows an incremental **strangler-fig** approach. Each phase is independently
deployable and backward-compatible.

### Phase A — Seams (Issue #208 POC)

**Goal**: Introduce `IAnimationSession` alongside existing code; validate testability.

**Entry criteria**: This design document approved.

**Changes**:
- Add: `IScanTimer`, `SystemScanTimer`, `TestScanTimer`
- Add: `AnimationConfig`, `AnimationSequenceConfig`, `AnimationWidgetConfig` (data classes)
- Add: `IScanModeStrategy`, `AutoScanStrategy`
- Add: `IAnimationSession`, `AnimationSession`
- Add: `IAnimationService`, `AnimationService`
- Modify: `PanelAnimationManager` — add `IAnimationService` constructor injection; delegate to session
- No change: `AnimationPlayer`, `AnimationManager`, `UserControlAnimationManager`, all UI code

**Exit criteria**: 15 unit tests passing; existing architecture tests passing; no build breakage.

**Parallel track**: BCI team can begin `BciScanStrategy` design using this interface spec.

---

### Phase B — Config Migration

**Goal**: Move panel configs from XML to JSON; eliminate CoreGlobals at load time.

**Changes**:
- Add: `IAnimationConfigProvider`, `JsonXmlAnimationConfigProvider`
- Add: `XmlAnimationConfigAdapter`
- Add: `IAnimationPreferenceResolver` (replaces CoreGlobals access at load time)
- Add: `schemas/json/animation-config.schema.json` (this document §6.4)
- Add: Migration scripts in `ConfigMigrationTool` for the 69 panel XML configs
- Validate schema against all 69 existing XML configs (using `XmlAnimationConfigAdapter`)

**Exit criteria**: All 69 XML configs converted and schema-validated; load-time CoreGlobals
access eliminated from `AnimationConfig` loading path; performance targets met (≤20ms panel load).

---

### Phase C — Strategy Extraction

**Goal**: Extract auto/manual/BCI scan logic from `AnimationPlayer` into strategy classes.

**Changes**:
- Add: `ManualScanStrategy` (replaces manual-scan path in `AnimationPlayer`)
- Add: `BciScanStrategy` (replaces duplicated logic in `AnimationSharpManagerV2`)
- Add: `DirectXHighlightRenderer` (BCI extension; implements `IHighlightRenderer`)
- Modify: `AnimationPlayer` — becomes thin adapter delegating to `AnimationSession`
- Modify: `AnimationSharpManagerV2` — keep BCI-specific code only; remove ~1,400–1,800 lines of duplication

**Entry criteria**: Phase B complete; BCI team has reviewed `BciScanStrategy` interface.

**Exit criteria**: All three strategies tested; `AnimationPlayer` reduced to ≤200 lines (adapter only).

---

### Phase D — EventBus Cutover

**Goal**: Replace ad-hoc `EvtPlayerStateChanged` / `EvtPlayerAnimationTransition` delegates.

**Changes**:
- `AnimationSession` already publishes `AnimationStateChangedEvent` and `AnimationTransitionEvent`
  to `IEventBus` (added in Phase A).
- Convert the 5 external `EvtPlayerStateChanged` subscribers to `IEventBus` subscribers:
  - `ScannerCommon` — `AnimationStateChangedEvent`
  - `GenericUserControl` — `AnimationStateChangedEvent`
  - `KeyboardUserControl` — `AnimationStateChangedEvent`
  - `KeyboardLockUserControlBCI` — `AnimationStateChangedEvent`
  - `UserControlManager` — `AnimationStateChangedEvent` + `AnimationTransitionEvent`
- Remove `EvtPlayerStateChanged` and `EvtPlayerAnimationTransition` from `AnimationPlayer`.

**Entry criteria**: Phase C complete; all 5 subscribers identified and tested.

**Exit criteria**: Zero ad-hoc delegates in `AnimationManagement/`; all subscribers converted.

---

### Phase E — Cleanup

**Goal**: Remove legacy adapters.

**Changes** (only when no external code references the legacy types):
- Remove: `AnimationPlayer` (if only referenced via adapter wrapper)
- Remove: `AnimationManager` (if all callers use `IAnimationService` directly)
- Mark: `IAnimationManager`, `IPanelAnimationManager`, `IUserControlAnimationManager` as `[Obsolete]`
- Update: DI registrations to remove adapter registrations

**Entry criteria**: Static analysis confirms zero external callers of legacy types.

---

## 11. Caller Blast Radius

The following files reference `AnimationManager`, `PanelAnimationManager`, or
`UserControlAnimationManager` outside of `AnimationManagement/` itself. These are the files
that must not break during any migration phase.

### 11.1 `PanelAnimationManager` Callers

| File | Type of Reference | Phase-safe? |
|------|------------------|------------|
| `PanelManagement/Common/ScannerCommon.cs` | Holds `IPanelAnimationManager`; calls Start/Stop/Pause/Resume | ✅ Interface preserved |
| `PanelManagement/Common/DialogCommon.cs` | Holds `IPanelAnimationManager` | ✅ Interface preserved |
| `PanelManagement/Interfaces/IPanelCommon.cs` | Declares `IPanelAnimationManager` property | ✅ Interface preserved |
| `PanelManagement/UI/MenuPanelBase.cs` | Uses `IPanelAnimationManager` | ✅ Interface preserved |
| `PanelManagement/UI/HorizontalStripScannerBase.cs` | Uses `IPanelAnimationManager` | ✅ Interface preserved |

### 11.2 `UserControlAnimationManager` Callers

| File | Type of Reference | Phase-safe? |
|------|------------------|------------|
| `UserControlManagement/UserControlManager.cs` | Creates `UserControlAnimationManager` | ✅ Preserved as-is until Phase E |
| `UserControlManagement/Interfaces/IUserControlCommon.cs` | Declares interface | ✅ Interface preserved |
| `UserControlManagement/UserControlCommon.cs` | Uses `IUserControlAnimationManager` | ✅ Interface preserved |
| `ACATExtension/UI/UserControls/GenericUserControl.cs` | Subscribes to `EvtPlayerStateChanged` | ✅ Converted in Phase D |
| `ACATExtension/UI/UserControls/KeyboardUserControl.cs` | Subscribes to `EvtPlayerStateChanged` | ✅ Converted in Phase D |

### 11.3 `AnimationManager` Callers

| File | Type of Reference |
|------|------------------|
| `ACATExtension/CommandHandlers/MiscCommandHandler.cs` | `IAnimationManager` reference |
| `ACATExtension/CommandHandlers/PositionSizeScannerHandler.cs` | `IAnimationManager` reference |
| `ACATExtension/UI/UserControlScreenLock.cs` | `IAnimationManager` reference |
| `ACATExtension/UI/AlphabetScannerCommon.cs` | `IAnimationManager` reference |
| `ACATExtension/UI/ScanTimeAdjustForm.cs` | `IAnimationManager` reference |
| `BCI/AnimationSharp/AnimationSharpManagerV2.cs` | Extends `AnimationManager` |
| `BCI/AnimationSharp/Utility/AnimationManagerUtils.cs` | Uses `AnimationManager` |
| `BCI/ACAT.Extensions.BCI.UI/Scanners/TalkApplicationBCIScanner.cs` | `IAnimationManager` |
| `BCI/ACAT.Extensions.BCI.UI/UserControls/*` (7 files) | `IAnimationManager` reference |

All 11.3 callers use `IAnimationManager` (interface). The interface is not changed.
`AnimationManager` becomes an adapter to `IAnimationService` in Phase A/B (no callers break).

---

## 12. DI Registration Plan

```csharp
// In ServiceConfiguration.AddACATServices() — additions for Phase A

// Animation service (singleton — one per app lifetime)
services.AddSingleton<IAnimationService, AnimationService>();

// Scan timer (transient — one new instance per session creation)
services.AddTransient<IScanTimer, SystemScanTimer>();

// Scan strategies (transient — resolved by name at session creation)
services.AddKeyedTransient<IScanModeStrategy, AutoScanStrategy>("auto");
// Phase C additions (not Phase A):
// services.AddKeyedTransient<IScanModeStrategy, ManualScanStrategy>("manual");
// services.AddKeyedTransient<IScanModeStrategy, BciScanStrategy>("bci");

// Config provider (singleton — file I/O + caching)
services.AddSingleton<IAnimationConfigProvider, JsonXmlAnimationConfigProvider>();

// Preference resolver — Phase A uses CoreGlobals adapter; Phase B injects IAppPreferences
services.AddSingleton<IAnimationPreferenceResolver, CoreGlobalsPreferenceResolver>();

// Highlight renderer (singleton — Phase A default; Phase C BCI extension registers its own)
services.AddSingleton<IHighlightRenderer, WinFormsHighlightRenderer>();
```

**Keyed services** (`AddKeyedTransient`) require Microsoft.Extensions.DependencyInjection 8.0+.
If the current DI version does not support keyed services, use a named-factory pattern:

```csharp
services.AddTransient<IScanStrategyFactory, ScanStrategyFactory>();
// ScanStrategyFactory.Create("auto") returns new AutoScanStrategy(...)
```

---

## 13. EventBus Integration

### 13.1 New Event Definitions

```csharp
// Location: src/Libraries/ACATCore/EventManagement/Events/AnimationEvents.cs
namespace ACAT.Core.EventManagement.Events
{
    /// <summary>Published by AnimationSession on every state transition.</summary>
    public class AnimationStateChangedEvent : IEvent
    {
        public string PanelName { get; init; }
        public PlayerState OldState { get; init; }
        public PlayerState NewState { get; init; }
        public string CurrentAnimationName { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }

    /// <summary>Published by AnimationSession when transitioning between animation sequences.</summary>
    public class AnimationTransitionEvent : IEvent
    {
        public string PanelName { get; init; }
        public string FromAnimation { get; init; }
        public string ToAnimation { get; init; }
        public bool IsTopLevel { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Published by AnimationSession when advancing to a new widget highlight.
    /// Consumed by IHighlightRenderer subscribers (Phase B).
    /// In Phase A, AnimationSession calls IHighlightRenderer directly.
    /// </summary>
    public class AnimationHighlightEvent : IEvent
    {
        public string PanelName { get; init; }
        public string WidgetName { get; init; }
        public bool PlayBeep { get; init; }
        public string PreviousWidgetName { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
```

### 13.2 Phase A Event Publishing Strategy

In Phase A, `AnimationSession` publishes **only** `AnimationStateChangedEvent` and
`AnimationTransitionEvent` to `IEventBus`. Widget highlighting is done directly via
`IHighlightRenderer` (not via event bus) to avoid introducing IEventBus dispatch latency
into the highlight critical path before it has been measured.

`AnimationHighlightEvent` is published to the bus **in addition to** direct rendering once
the ≤1ms dispatch target is confirmed during the Phase A POC.

---

## 14. Success Criteria

| Criterion | Metric | Target | Verified By |
|-----------|--------|--------|-------------|
| Unit test coverage (animation engine) | % lines covered | ≥80% | Phase A + C tests |
| Scan interval accuracy | Measured vs. configured interval | ≤5% deviation | T15 (§8.3) |
| Actuator-to-highlight latency | End-to-end | ≤50ms | Phase A measurement |
| IEventBus dispatch time | Per event publish | ≤1ms | T15 (§8.3) |
| Config load time (BCI worst case) | 25-animation panel | ≤20ms | Phase B measurement |
| No regressions | Existing architecture test suite | 100% pass | Phase A exit gate |
| Zero new CoreGlobals usages | Static analysis on new files | 0 new | CI check |
| Public interface backward compatibility | Compile existing callers | 0 errors | Phase A–E |
| BCI parity | All BCI scan behaviors | 100% | Phase C acceptance |

---

## 15. Related Documents

| Document | Purpose |
|----------|---------|
| [`docs/ANIMATION_SYSTEM_ARCHITECTURE.md`](ANIMATION_SYSTEM_ARCHITECTURE.md) | Original architecture spec (v1.1); §5 updated to mark Issue #207 complete |
| [`docs/ANIMATION_SYSTEM_ANALYSIS.md`](ANIMATION_SYSTEM_ANALYSIS.md) | Issue #206 deliverable; code-verified evidence |
| [`docs/adr/ADR-001-animation-system-architecture.md`](adr/ADR-001-animation-system-architecture.md) | Three architecture decisions |
| [`schemas/json/animation-config.schema.json`](../schemas/json/animation-config.schema.json) | JSON Schema for AnimationConfig |
| [`schemas/examples/animation-config.example.json`](../schemas/examples/animation-config.example.json) | Worked animation config example |
| [`DEPENDENCY_INJECTION_GUIDE.md`](../DEPENDENCY_INJECTION_GUIDE.md) | DI patterns from Phase 2 |
| [`src/Libraries/ACATCore/Utility/ServiceConfiguration.cs`](../src/Libraries/ACATCore/Utility/ServiceConfiguration.cs) | DI registration entry point |

---

**Document Owner**: ACAT Architecture Team  
**Next Review**: At start of Issue #208 (POC)  
**Review Cycle**: Per-phase
