# Animation Engine Developer Guide

**Document Status**: Issue #274 Deliverable — Implementation Complete  
**Version**: 1.0  
**Date**: February 2026  
**Epic**: Animation System Modernization (Phase 3)  

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Component Diagram](#2-component-diagram)
3. [How to Create a New Scan Mode Strategy](#3-how-to-create-a-new-scan-mode-strategy)
4. [How to Create a Custom Highlight Renderer](#4-how-to-create-a-custom-highlight-renderer)
5. [How to Add JSON Animation Configuration for a New Panel](#5-how-to-add-json-animation-configuration-for-a-new-panel)
6. [Adapter Layer: PanelAnimationManager Integration](#6-adapter-layer-panelanimationmanager-integration)
7. [Migration Guide for Extension Authors](#7-migration-guide-for-extension-authors)
8. [BCI Extension Notes](#8-bci-extension-notes)
9. [Performance Targets](#9-performance-targets)
10. [Testing](#10-testing)

---

## 1. Architecture Overview

The ACAT animation engine uses a layered architecture designed for backward compatibility. The new engine coexists with the legacy `AnimationPlayer` through an adapter layer.

### Key Design Principles

- **Zero regression**: All existing callers of `PanelAnimationManager` and `UserControlAnimationManager` continue to work unchanged.
- **Property injection**: `IAnimationService` is injected via a public property, not a constructor parameter, so existing `new PanelAnimationManager(logger)` call sites are not broken.
- **Graceful fallback**: If the new engine session fails to create, `AnimationPlayerAdapter.TryCreate()` returns `null` and the manager falls back to `AnimationPlayer`.
- **Strategy pattern**: Scan algorithms (`auto`, `manual`, `step`) are pluggable via `IScanModeStrategy`.
- **Event bus integration**: State changes are published to `IEventBus` as `AnimationStateChangedEvent`, `AnimationTransitionEvent`, and `AnimationHighlightEvent`.

### Layer Summary

| Layer | Classes | Responsibility |
|-------|---------|----------------|
| **Adapter** | `AnimationPlayerAdapter` | Bridges managers to `IAnimationSession` |
| **Service** | `AnimationService` | Session factory + lifecycle registry |
| **Session** | `AnimationSession` | Per-panel scan loop, widget highlighting |
| **Strategy** | `AutoScanStrategy`, `ManualScanStrategy`, `StepScanStrategy` | Scan algorithm selection |
| **Config** | `AnimationConfig`, `XmlAnimationConfigAdapter` | XML-to-model bridge |
| **Rendering** | `WinFormsHighlightRenderer` | Visual highlight application |
| **Events** | `AnimationStateChangedEvent` etc. | Decoupled state change notifications |
| **Legacy** | `AnimationPlayer`, `AnimationManager` | Preserved for backward compatibility |

---

## 2. Component Diagram

```
Callers (PanelAnimationManager / UserControlAnimationManager)
    │
    │  AnimationService property (optional, property injection)
    ▼
AnimationPlayerAdapter.TryCreate()
    │  ┌────────────────────┐
    │  │  On failure:       │
    │  │  returns null →    │
    │  │  falls back to     │
    │  │  AnimationPlayer   │
    │  └────────────────────┘
    │
    ▼
IAnimationService.CreateSession(rootWidget, config, strategyName)
    │
    ▼
IAnimationSession (AnimationSession)
    │
    ├── IScanTimer (SystemScanTimer)          — fires Elapsed on interval
    ├── IScanModeStrategy (AutoScanStrategy)  — SelectNext, HandleInput
    ├── IHighlightRenderer                   — Render, ClearHighlight, ClearAll
    └── IEventBus                            — publishes state change events
```

---

## 3. How to Create a New Scan Mode Strategy

Implement `IScanModeStrategy` in `ACAT.Core.AnimationManagement.Strategies`:

```csharp
using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement.Strategies
{
    /// <summary>
    /// Example: a BCI-specific scan strategy that waits for a neural signal.
    /// </summary>
    public class BciScanStrategy : IScanModeStrategy
    {
        public string Name => "bci";

        public int SelectNext(IReadOnlyList<AnimationWidgetConfig> widgets,
            int currentIndex, IScanContext context)
        {
            // BCI: advance only when neural signal is received (handled via HandleInput).
            // Return currentIndex to hold position until input arrives.
            if (currentIndex < 0) return 0;
            return currentIndex; // hold
        }

        public int SelectPrevious(IReadOnlyList<AnimationWidgetConfig> widgets,
            int currentIndex, IScanContext context)
        {
            return currentIndex <= 0 ? 0 : currentIndex - 1;
        }

        public ScanInputAction HandleInput(ScanInputEvent inputEvent, IScanContext context)
        {
            return inputEvent.Type switch
            {
                ScanInputType.Switch1Activated => ScanInputAction.Advance,
                ScanInputType.Switch2Activated => ScanInputAction.Select,
                _ => ScanInputAction.None
            };
        }

        public void OnSequenceStart(IReadOnlyList<AnimationWidgetConfig> widgets, IScanContext context) { }
        public void OnSequenceEnd(IScanContext context) { }
    }
}
```

**Register the strategy** in the DI factory (`DefaultScanStrategyFactory`) or by extending `IScanStrategyFactory`:

```csharp
public class ExtendedScanStrategyFactory : IScanStrategyFactory
{
    public IScanModeStrategy Create(string strategyName)
    {
        if (strategyName == "bci") return new BciScanStrategy();
        return new DefaultScanStrategyFactory().Create(strategyName);
    }
}
```

Then register it in DI:
```csharp
services.AddSingleton<IScanStrategyFactory, ExtendedScanStrategyFactory>();
```

---

## 4. How to Create a Custom Highlight Renderer

Implement `IHighlightRenderer`:

```csharp
using ACAT.Core.AnimationManagement.Interfaces;
using ACAT.Core.AnimationManagement.Rendering;

public class DirectXHighlightRenderer : IHighlightRenderer
{
    public void Render(string widgetName, HighlightStyle style)
    {
        // Use SharpDX to draw a highlight overlay on the widget.
    }

    public void ClearHighlight(string widgetName)
    {
        // Remove overlay for this widget.
    }

    public void ClearAll()
    {
        // Remove all overlays.
    }
}
```

Register it in DI before calling `AddAnimationEngine()`:

```csharp
services.AddSingleton<IHighlightRenderer, DirectXHighlightRenderer>();
services.AddAnimationEngine();
```

> **Note**: `WinFormsHighlightRenderer` is the default production renderer. It accepts callback lambdas and is suitable for most WinForms panels.

---

## 5. How to Add JSON Animation Configuration for a New Panel

The engine supports both XML (legacy) and JSON (new) configuration.

### Option A: JSON file (preferred for new panels)

Create `{PanelName}.animation.json` in the panel's config directory:

```json
{
  "panelName": "MyNewPanel",
  "scanStrategy": "auto",
  "sequences": [
    {
      "name": "Row1",
      "isFirst": true,
      "autoStart": true,
      "iterations": "3",
      "scanTime": "@ScanTime",
      "firstPauseTime": "0",
      "onEnter": "",
      "onEnd": "",
      "widgets": [
        { "name": "Button1", "playBeep": false, "onSelected": "" },
        { "name": "Button2", "playBeep": false, "onSelected": "" },
        { "name": "Button3", "playBeep": false, "onSelected": "" }
      ]
    },
    {
      "name": "Row2",
      "isFirst": false,
      "autoStart": true,
      "iterations": "1",
      "scanTime": "@ScanTime",
      "widgets": [
        { "name": "Button4", "playBeep": true, "onSelected": "switchToPanel(MainPanel)" }
      ]
    }
  ]
}
```

Load it via `AnimationConfigProvider`:

```csharp
var provider = new AnimationConfigProvider();
var config = provider.LoadForPanel("MyNewPanel", configDirectory);
```

### Option B: XML (legacy, automatic via XmlAnimationConfigAdapter)

Existing XML panel configs use the `<Animations>` element. The adapter converts them automatically:

```xml
<ACAT>
  <Animations>
    <Animation name="Row1" start="true" autoStart="true" scanTime="600" iterations="3">
      <Widget name="Button1" onSelect="" />
      <Widget name="Button2" onSelect="" />
    </Animation>
  </Animations>
</ACAT>
```

The `AnimationPlayerAdapter.TryCreate()` call in `PanelAnimationManager.Start()` handles this automatically by loading the XML node from the config file and passing it to `XmlAnimationConfigAdapter.Convert()`.

### Schema Migration Constraints (C1–C5)

| Constraint | Description | Handling |
|-----------|-------------|---------|
| C1 | `Iterations` as `@VarName` runtime reference | Stored as string in `AnimationSequenceConfig.Iterations` |
| C2 | `ScanTime`/`FirstPauseTime` as variable names | Stored as string in config; resolved at session start |
| C3 | Wildcard widget names (`Box1/*`, `@SelectedWidget`) | Passed through as-is; expansion is at Start() time |
| C4 | Per-widget `OnSelected` PCode | Stored in `AnimationWidgetConfig.OnSelected` |
| C5 | Per-animation `OnEnter`/`OnEnd` PCode | Stored in `AnimationSequenceConfig.OnEnter`/`OnEnd` |

---

## 6. Adapter Layer: PanelAnimationManager Integration

### How the Adapter is Activated

`PanelAnimationManager` and `UserControlAnimationManager` both have an optional `IAnimationService` property:

```csharp
// Property injection — does not break existing callers
public IAnimationService AnimationService { get; set; }
```

When `AnimationService` is non-null, `Start()` calls `AnimationPlayerAdapter.TryCreate()`. If the adapter is created successfully, the new engine runs. If not (exception or null service), the legacy `AnimationPlayer` is used.

### Enabling the New Engine

Set the property after creating the manager:

```csharp
var animManager = new PanelAnimationManager(LogManager.GetLogger<PanelAnimationManager>());
animManager.AnimationService = container.GetService<IAnimationService>();
animManager.Init(panelConfigMapEntry);
```

Or when using DI-constructed objects, inject via the service locator:

```csharp
// In DialogCommon or ScannerCommon, after creating PanelAnimationManager:
_animationManager.AnimationService = ServiceLocator.GetService<IAnimationService>();
```

### Fallback Behavior

```
Start() called
    │
    ├── AnimationService is null?
    │     └── Use legacy AnimationPlayer ──────────────────────────────►
    │
    └── AnimationService is set
          │
          ├── TryCreate() succeeds?
          │     └── Use AnimationPlayerAdapter (new engine) ───────────►
          │
          └── TryCreate() returns null (exception during session creation)
                └── Use legacy AnimationPlayer ───────────────────────►
```

---

## 7. Migration Guide for Extension Authors

### For New Extensions

Use the new engine directly via `IAnimationService`:

```csharp
// 1. Resolve from DI
var animationService = container.GetRequiredService<IAnimationService>();

// 2. Create a config (from XML or JSON)
var xmlAdapter = new XmlAnimationConfigAdapter();
var config = xmlAdapter.Convert(panelName, animationsXmlNode);

// 3. Create and start a session
var session = animationService.CreateSession(rootWidget, config, "auto");
session.Start();

// 4. Handle actuator input
session.Interrupt(); // on switch press

// 5. Clean up
session.Stop();
session.Dispose();
```

### For Existing Extensions

If your extension extends `AnimationManager` directly (like `AnimationSharpManagerV2`):

1. **No immediate changes required** — `AnimationManager` continues to work with `AnimationPlayer`.
2. **Optional migration**: Add `IAnimationService AnimationService { get; set; }` to your manager and delegate to `AnimationPlayerAdapter.TryCreate()` in your initialization code.
3. **Future migration**: When ready to fully migrate, replace the `AnimationPlayer` fields with `IAnimationSession` and call `IAnimationService.CreateSession()` directly.

### Interface Changes (None)

All public interfaces (`IAnimationManager`, `IPanelAnimationManager`, `IUserControlAnimationManager`) are **unchanged**. Existing callers are not affected.

---

## 8. BCI Extension Notes

`AnimationSharpManagerV2.cs` (2,885 lines) extends `AnimationManager` with its own scan loop and SharpDX overlay rendering. It continues to function unchanged.

### BCI Migration Roadmap (Future Phase)

The following BCI components could be replaced by the new engine in a future phase:

| BCI Component | New Engine Equivalent | Lines Saved |
|--------------|----------------------|-------------|
| Internal scan timer loop (~150 lines) | `SystemScanTimer` + `AutoScanStrategy` | ~150 |
| Widget highlight logic (~400 lines) | `WinFormsHighlightRenderer` or `DirectXHighlightRenderer` | ~400 |
| State machine (~300 lines) | `AnimationSession` state machine | ~300 |
| Switch event routing (~200 lines) | `IScanModeStrategy.HandleInput()` | ~200 |
| **Total** | | **~1,050 lines** |

The BCI-specific SharpDX rendering (~850 lines) would be preserved as a `DirectXHighlightRenderer` implementation.

---

## 9. Performance Targets

From design spec §14 (validated in `AnimationPerformanceBenchmarks.cs`):

| Metric | Target | Test |
|--------|--------|------|
| Config load time (5 animations) | ≤20ms | BP01 |
| Config load time (25 animations, BCI worst-case) | ≤20ms | BP02 |
| `AnimationService.CreateSession()` | ≤5ms | BP03 |
| `AnimationSession.Start()` | ≤5ms | BP04 |
| `AnimationSession.Stop()` | ≤50ms | BP05 |
| Service shutdown (10 sessions) | ≤100ms | BP06 |
| 100 adapter lifecycle cycles | ≤2s | BP07 |

---

## 10. Testing

### Unit Tests (`ACATCore.Tests.Configuration/AnimationEngineTests.cs`)

Tests T01–T20 cover `TestScanTimer`, `AutoScanStrategy`, `ManualScanStrategy`, `StepScanStrategy`, `AnimationSession`, `XmlAnimationConfigAdapter`, `AnimationConfigProvider`, and DI registration.

### Integration Tests (`ACATCore.Tests.Integration/AnimationIntegrationTests.cs`)

Tests IT01–IT15 cover the full adapter layer:
- Adapter creation with/without service, with/without XML
- Start/Stop/Pause/Resume lifecycle
- Transition between animation sequences
- Multi-panel scenarios
- Event bus publication on state changes

### Performance Benchmarks (`ACATCore.Tests.Performance/AnimationPerformanceBenchmarks.cs`)

Tests BP01–BP08 validate that all performance targets from the design spec are met.

### Running the Tests

```bash
# From src/ directory:
dotnet test Libraries/ACATCore.Tests.Configuration/ACATCore.Tests.Configuration.csproj --configuration TestOnly
dotnet test Libraries/ACATCore.Tests.Integration/ACATCore.Tests.Integration.csproj --configuration TestOnly
dotnet test Libraries/ACATCore.Tests.Performance/ACATCore.Tests.Performance.csproj --configuration TestOnly
```

---

## Related Documents

- [`docs/ANIMATION_SYSTEM_DESIGN.md`](ANIMATION_SYSTEM_DESIGN.md) — Full design specification
- [`docs/ANIMATION_SYSTEM_ANALYSIS.md`](ANIMATION_SYSTEM_ANALYSIS.md) — Current system analysis
- [`docs/adr/ADR-001-animation-system-architecture.md`](adr/ADR-001-animation-system-architecture.md) — Architecture decisions
- [`ACAT_MODERNIZATION_PLAN.md`](../ACAT_MODERNIZATION_PLAN.md) — Phase 3 modernization roadmap
