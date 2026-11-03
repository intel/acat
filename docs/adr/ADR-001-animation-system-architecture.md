# ADR-001: Animation System Architecture Decisions

**Status**: Accepted  
**Date**: February 2026  
**Issue**: intel/acat#207 — Animation Architecture Design  
**Epic**: Animation System Preparation (intel/acat#195)  
**Authors**: ACAT Architecture Team  

---

## Context

During Issue #207 (Animation Architecture Design), three significant architecture decisions
were deferred from the original specification
([`docs/ANIMATION_SYSTEM_ARCHITECTURE.md`](../ANIMATION_SYSTEM_ARCHITECTURE.md)) because
they required code evidence from Issue #206 (Animation System Analysis) before they could
be made with confidence.

This ADR documents those decisions and their rationale. Each decision is in its own section.
All decisions are grounded in findings from
[`docs/ANIMATION_SYSTEM_ANALYSIS.md`](../ANIMATION_SYSTEM_ANALYSIS.md).

---

## Decision 1: Keep `AnimationPlayer` as a Legacy Adapter

### Question

Should `AnimationPlayer` be:

**(A) Kept as a legacy adapter** that delegates to the new `IAnimationSession`, or  
**(B) Deleted immediately** and replaced wholesale by `AnimationSession`?

### Decision: **Option A — Keep as legacy adapter**

### Rationale

1. **`AnimationPlayer` is `internal`** (`AnimationPlayer.cs:68`), but it is indirectly
   referenced through the public `AnimationManager` → `PanelAnimationManager` →
   `UserControlAnimationManager` chain. Deleting it requires auditing all callers of all
   three manager classes (48 source files — see
   `ANIMATION_SYSTEM_DESIGN.md §11`).

2. **1,835 lines** (confirmed in Issue #206) — significantly larger than the original ~800-line
   estimate. A big-bang replacement of a 1,835-line class in a single PR is a high-regression-risk
   change for an accessibility-critical path. The strangler-fig approach is safer.

3. **Zero test coverage** for `AnimationPlayer` (pain point P8). Without tests, any
   replacement is unverifiable. The POC introduces `TestScanTimer` + `IAnimationSession` tests
   *first*, then shrinks `AnimationPlayer` incrementally as covered tests replace each path.

4. **Phase A scope constraint**: The POC (Issue #208) must not change any UI or panel code.
   Keeping `AnimationPlayer` as a forwarding adapter achieves this — `PanelAnimationManager`
   delegates to `IAnimationSession` internally while its public surface (called by 48 files)
   is unchanged.

### Consequences

- `AnimationPlayer` is marked `// LEGACY ADAPTER — Phase E removal target` in Phase A.
- It is reduced to ≤200 lines in Phase C after scan strategies are extracted.
- It is deleted in Phase E only after static analysis confirms no external references.
- **Not** marked `[Obsolete]` until Phase D (to avoid compiler noise for the 48 callers).

---

## Decision 2: `IScanModeStrategy` Plugin vs. Enum-Based Scan-Mode Switch

### Question

Should the animation engine support multiple scan modes via:

**(A) `IScanModeStrategy` — a pluggable interface resolved from DI**, or  
**(B) An enum-based switch** (`ManualScanModes` / scan mode flag) inside `AnimationPlayer`?

### Decision: **Option A — `IScanModeStrategy` plugin interface**

### Rationale

1. **`AnimationPlayer` is already 1,835 lines** precisely because it contains an enum-based
   switch between auto-scan and manual-scan modes (pain point P6). Adding another mode
   (e.g., BCI, eye-gaze, step-scan) requires modifying the class again. This is the exact
   anti-pattern the rewrite must eliminate.

2. **`AnimationSharpManagerV2` is 2,885 lines** and contains a third parallel implementation
   of the same enum-based switch (P10). The duplication has already created a 2,885-line
   maintenance burden. Option B would reproduce this pattern.

3. **Testability**: Each `IScanModeStrategy` implementation can be unit-tested independently
   with a simple widget list and mock `IScanContext`. Testing an enum-switch variant requires
   the entire `AnimationPlayer` to be instantiated.

4. **Extensibility**: Third-party BCI hardware vendors or research teams can provide new
   scan strategies as DI-registered plugins without modifying core classes. This is consistent
   with the Phase 2 pattern used for `IWordPredictionManager` and `IActuatorManager`.

5. **Phase A constraint**: Only `AutoScanStrategy` needs to be implemented in Phase A (POC).
   The interface design does not constrain the timeline for `ManualScanStrategy` or
   `BciScanStrategy` in Phase C.

### Consequences

- `AutoScanStrategy`, `ManualScanStrategy`, `BciScanStrategy` are separate classes
  registered in DI under named keys (see `ANIMATION_SYSTEM_DESIGN.md §12`).
- `AnimationSession` receives the strategy via constructor injection; the strategy name
  is passed at `IAnimationService.CreateSession()` call time.
- Adding a new scan mode in Phase 4+ requires only: (a) implementing `IScanModeStrategy`,
  (b) registering in DI, and (c) updating config files to reference the strategy name.

### Rejected Alternative

The enum-based approach was considered viable only if scan modes were a closed, fixed set.
Given the BCI extension already represents a third mode and the roadmap includes future
eye-gaze and step-scan modes, a closed set is not a valid assumption.

---

## Decision 3: PCode Interpreter — Retain, Replace, or Expose as Config-Side Scripting

### Question

Should the PCode interpreter (`Interpret.cs`, 441 lines) be:

**(A) Retained as-is** — no changes in Phase 3, injected via interface  
**(B) Replaced with an existing expression library** (e.g. NCalc, Jint, or Roslyn scripting)  
**(C) Exposed as config-side scripting** — replace PCode inline strings with a structured
      action model in the `AnimationConfig` JSON

### Decision: **Option A — Retain as-is, inject via interface**

### Rationale

1. **Existing PCode scripts work reliably.** The 69 panel XML configs and their PCode
   `onEnter`/`onEnd`/`onSelected` scripts represent a known-working corpus. Replacing
   the interpreter risks introducing behavioral regressions in scripts that are difficult
   to test end-to-end.

2. **PCode is opaque but self-contained.** `Interpret.cs` (441 lines) + `Parser.cs`
   (277 lines) is a small, isolated system. Pain point P3 (no test harness) is a
   maintenance concern but not a functional regression risk for Phase A–C.

3. **Replacement scope is large.** The 69 panel configs + BCI extension configs contain
   hundreds of PCode strings. Migrating them to a new format is a multi-day project
   independent of the scan-engine rewrite. Conflating the two increases Phase 3 risk.

4. **Injection via `IScriptInterpreter` is sufficient for testability.** The POC can mock
   `IScriptInterpreter.Execute()` to verify that PCode hooks are called at the correct
   points (`onEnter`, `onSelected`, `onEnd`) without requiring the interpreter to execute
   real PCode in unit tests.

### How Option A Works

An `IScriptInterpreter` interface is introduced in Phase A:

```csharp
namespace ACAT.Core.Interpreter.Interfaces
{
    /// <summary>
    /// Executes a PCode script string. Default implementation delegates to Interpret.Execute().
    /// Test implementation records calls without executing.
    /// </summary>
    public interface IScriptInterpreter
    {
        void Execute(string pcode);
    }

    public class PCodeInterpreter : IScriptInterpreter
    {
        public void Execute(string pcode) => Interpret.Execute(pcode);
    }

    public class NullScriptInterpreter : IScriptInterpreter
    {
        public void Execute(string pcode) { /* no-op for tests */ }
    }
}
```

`AnimationSession` receives `IScriptInterpreter` via constructor injection. The DI
registration maps `IScriptInterpreter` → `PCodeInterpreter`. Test code substitutes
`NullScriptInterpreter` or a recording mock.

### Deferred Work

Option C (structured action model) remains a valuable long-term direction and should be
evaluated in Phase 4 once the scan engine is stable. The `AnimationWidgetConfig.onSelected`
and `AnimationSequenceConfig.onEnter`/`onEnd` fields are already `string` type, which is
backward-compatible with either PCode strings or a future structured format.

Option B (expression library replacement) is deferred to Phase 4. If it is pursued,
the `IScriptInterpreter` interface introduced in Phase A is the correct replacement point.

### Consequences

- `IScriptInterpreter` is added to `Interfaces/` in Phase A alongside `IScanTimer`.
- `Interpret.cs` and `Parser.cs` are **not modified** in Phase A, B, or C.
- Unit tests use `NullScriptInterpreter` or a test-double that records PCode calls.
- `AnimationSession.Start()` / `Transition()` / `_timerElapsed()` call
  `IScriptInterpreter.Execute(sequence.OnEnter)` / `Execute(widget.OnSelected)` /
  `Execute(sequence.OnEnd)` at the appropriate lifecycle points.

---

## Summary

| Decision | Choice | Phase | Key Reason |
|----------|--------|-------|-----------|
| `AnimationPlayer` fate | Keep as legacy adapter (Phase E removal) | Phase A–E | 48 callers; no test coverage; incremental safer |
| Scan mode model | `IScanModeStrategy` plugin interface | Phase A | Closed-set assumption invalid; extensibility required |
| PCode interpreter | Retain via `IScriptInterpreter` injection | Phase A | Working corpus; replacement is separate large project |

---

**Related Documents**:

- [`docs/ANIMATION_SYSTEM_DESIGN.md`](../ANIMATION_SYSTEM_DESIGN.md) — Full design specification (Issue #207)
- [`docs/ANIMATION_SYSTEM_ANALYSIS.md`](../ANIMATION_SYSTEM_ANALYSIS.md) — Evidence base (Issue #206)
- [`docs/ANIMATION_SYSTEM_ARCHITECTURE.md`](../ANIMATION_SYSTEM_ARCHITECTURE.md) — Original spec (v1.1)
