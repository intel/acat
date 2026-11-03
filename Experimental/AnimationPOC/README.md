# Animation System POC — README

**Issue**: intel/acat#208 — Animation System POC  
**Depends on**: intel/acat#207 ([Design Specification](../docs/ANIMATION_SYSTEM_DESIGN.md))  
**Phase**: Phase A (Seams)  
**Status**: POC Complete — All acceptance criteria met

---

## Purpose

This directory contains the Phase A POC deliverable for the Animation System Preparation epic
(intel/acat#195). It validates the architecture proposed in
[`docs/ANIMATION_SYSTEM_DESIGN.md`](../docs/ANIMATION_SYSTEM_DESIGN.md) by implementing a
runnable proof of concept with unit tests and a WinForms scanner demo.

## Contents

```
Experimental/
├── AnimationPOC.sln                   Standalone solution for the POC
│
├── AnimationPOC/                      New animation engine (class library)
│   ├── PlayerState.cs                 PlayerState enum (mirrors ACATCore)
│   ├── Interfaces/
│   │   ├── IScanTimer.cs              Timer abstraction (replaces direct System.Timers.Timer use)
│   │   ├── IScanModeStrategy.cs       Scan algorithm interface + IScanContext + ScanInputAction
│   │   ├── IAnimationSession.cs       Per-panel scan session interface
│   │   ├── IAnimationService.cs       Root service interface (factory + registry)
│   │   ├── IHighlightRenderer.cs      Widget highlight abstraction
│   │   └── IEventBus.cs               Minimal pub/sub event bus interface
│   ├── Config/
│   │   └── AnimationConfig.cs         JSON-first data model (AnimationConfig, Sequence, Widget)
│   ├── Infrastructure/
│   │   ├── SystemScanTimer.cs         Production timer (wraps System.Timers.Timer)
│   │   ├── TestScanTimer.cs           Test timer (ManualTick() fires synchronously)
│   │   └── SimpleEventBus.cs          Simple IEventBus implementation for POC
│   ├── Strategies/
│   │   └── AutoScanStrategy.cs        Auto-scan strategy (Phase A only)
│   ├── Core/
│   │   ├── AnimationSession.cs        IAnimationSession implementation
│   │   └── AnimationService.cs        IAnimationService + IScanStrategyFactory
│   └── Events/
│       └── AnimationEvents.cs         AnimationStateChangedEvent, TransitionEvent, HighlightEvent
│
├── AnimationPOCDemo/                   WinForms scanner demo application
│   ├── Program.cs                     Entry point
│   └── ScannerDemoForm.cs             Simple 6-button scanner with event log + perf metrics
│
├── AnimationPOC.Tests/                 xUnit unit tests (T01–T15)
│   ├── AutoScanStrategyTests.cs        T01–T03 + extras: SelectNext, SelectPrevious, HandleInput
│   ├── TestScanTimerTests.cs           T04–T05 + extras: AutoReset, Start/Stop, defaults
│   ├── AnimationSessionTests.cs        T06–T14: Start, Stop, Pause, Resume, events, loop, select
│   └── PerformanceTests.cs             T15: EventBus dispatch <1ms, session construction <20ms
│
└── PERFORMANCE_RESULTS.md              Performance comparison results
```

---

## POC Scope (Phase A)

This POC implements exactly the Phase A scope defined in `ANIMATION_SYSTEM_DESIGN.md §8.2`:

| Component | Status |
|-----------|--------|
| `IScanTimer` | ✅ Implemented |
| `SystemScanTimer` | ✅ Implemented |
| `TestScanTimer` + `ManualTick()` | ✅ Implemented |
| `AnimationConfig`, `AnimationSequenceConfig`, `AnimationWidgetConfig` | ✅ Implemented |
| `IScanModeStrategy`, `IScanContext`, `ScanInputAction` | ✅ Implemented |
| `AutoScanStrategy` | ✅ Implemented |
| `IAnimationSession`, `AnimationSession` | ✅ Implemented |
| `IAnimationService`, `AnimationService` | ✅ Implemented |
| `IEventBus`, `SimpleEventBus` | ✅ Implemented |
| `IHighlightRenderer` | ✅ Implemented |
| 15 unit tests (T01–T15) | ✅ Implemented |
| WinForms scanner demo | ✅ Implemented |
| Performance results | ✅ See `PERFORMANCE_RESULTS.md` |

**Out of scope for Phase A (confirmed):**
- `ManualScanStrategy` (Phase C)
- `BciScanStrategy` (Phase C)
- `IAnimationConfigProvider` / `XmlAnimationConfigAdapter` (Phase B)
- JSON schema validation (Phase B)
- EventBus cutover for `EvtPlayerStateChanged` (Phase D)
- `IAnimationPreferenceResolver` DI injection (Phase B)
- Wire into `PanelAnimationManager` (Phase A production step — not modified in POC)

---

## Building and Running

### Prerequisites
- .NET 8 SDK (or Visual Studio 2022+ with .NET 4.8.1 targeting)
- Windows 10/11 (or Windows Server for demo; tests build on Linux but require Mono to run)

### Build

```powershell
cd Experimental
dotnet build AnimationPOC.sln -c Debug /p:Platform=x64
```

### Run Demo

```powershell
cd Experimental/AnimationPOCDemo/bin/Debug
AnimationPOCDemo.exe
```

The demo shows:
- A 6-button row scanner (A B C D E F)
- Start/Stop/Pause/Resume controls
- "Press Switch" button to simulate actuator input (selects current widget)
- Scan speed slider (200ms–2000ms)
- Real-time event log showing all AnimationStateChangedEvent and AnimationHighlightEvent deliveries
- Performance metrics (EventBus dispatch latency)

### Run Tests

```powershell
cd Experimental
dotnet test AnimationPOC.Tests/ -c Debug /p:Platform=x64
```

---

## Architecture Highlights

### Thread Safety
`AnimationSession` uses a single `_sessionLock` to protect all state fields.
`IEventBus.Publish()` and `IHighlightRenderer` calls are made **without** holding the lock
to prevent deadlocks with UI-thread subscribers.

### Testability
`TestScanTimer.ManualTick()` fires `Elapsed` synchronously on the calling thread,
making all timer-driven scan logic 100% deterministic in unit tests.
No `Thread.Sleep` required anywhere in the test suite.

### Backward Compatibility
No existing `AnimationManagement/` source files are modified.
The new interfaces are additive only. Integration into `PanelAnimationManager`
is the next Phase A production step (not part of this POC).

### JSON Configuration
`AnimationConfig` maps directly to `schemas/json/animation-config.schema.json`.
All five schema migration constraints (C1–C5) are handled:
- Preference variable references (`@VariableName`) parsed as strings, resolved at `Start()` time
- Wildcard widget names (`Box1/*`) stored as strings, expanded at `Start()` time
- PCode hooks (`onEnter`, `onEnd`, `onSelected`) stored as strings on config objects

---

## Design Decisions (ADR Summary)

See [`docs/adr/ADR-001-animation-system-architecture.md`](../docs/adr/ADR-001-animation-system-architecture.md) for full rationale.

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Config format | JSON (string model for dynamic values) | Consistent with Phase 1 infrastructure |
| Timer abstraction | `IScanTimer` with `TestScanTimer.ManualTick()` | Enables deterministic tests |
| Scan strategy | `IScanModeStrategy` plugin (not enum switch) | BCI extension registers `BciScanStrategy` without touching core |
| Rendering | `IHighlightRenderer` abstraction | WinUI 3 swap-in without changing scan engine |
| `AnimationPlayer` fate | Keep as-is in this POC | Deleted only after Phase E confirms no external callers |

---

## Next Steps (After POC Review)

1. **Wire `PanelAnimationManager`** (Phase A production) — add `IAnimationService` constructor param; delegate to session
2. **Phase B**: `IAnimationConfigProvider` + `XmlAnimationConfigAdapter` + JSON schema validation
3. **Phase C**: `ManualScanStrategy` + `BciScanStrategy` + `DirectXHighlightRenderer`
4. **Phase D**: Convert 5 `EvtPlayerStateChanged` subscribers to `IEventBus`
5. **Phase E**: Remove `AnimationPlayer` legacy adapter

---

**Acceptance Criteria for Issue #208:**

- [x] Basic POC implemented (all Phase A interfaces + implementations)
- [x] Simple scanner rendered (`AnimationPOCDemo` WinForms application)
- [x] Performance compared with existing (see `PERFORMANCE_RESULTS.md`)
- [x] Design validated (15 unit tests passing; all T01–T15 test cases covered)
- [x] POC code committed (this directory)
