# ACAT Performance Dashboard

The **Performance Dashboard** is a real-time WPF window that monitors application
health by surfacing memory, runtime metrics, baseline regression status, and a live
working-set trend sparkline.

---

## Table of Contents

- [Quick Start](#quick-start)
- [Shared Collectors](#shared-collectors)
- [Custom Baseline](#custom-baseline)
- [Dashboard Panels](#dashboard-panels)
- [Historical Trend Graph](#historical-trend-graph)
- [Export](#export)
- [Keyboard Accessibility](#keyboard-accessibility)
- [Architecture](#architecture)

---

## Quick Start

Open a self-contained dashboard that creates its own collectors:

```csharp
var dashboard = new PerformanceDashboard();
dashboard.Show();
```

The window auto-refreshes every **2 seconds** while open.

---

## Shared Collectors

Pass existing `RuntimeMetricsCollector` and `MemoryProfiler` instances so the
dashboard shows data already gathered by the application:

```csharp
// Application startup
var collector = new RuntimeMetricsCollector();
var profiler  = new MemoryProfiler();
collector.Start(intervalMs: 5000);
profiler.CaptureSnapshot("Startup");

// Open the dashboard sharing those instances
var dashboard = new PerformanceDashboard(
    collector: collector,
    profiler:  profiler);
dashboard.Show();
```

---

## Custom Baseline

Supply a `PerformanceBaselineData` to override the default regression thresholds
(startup < 3 s, input lag < 100 ms, etc.):

```csharp
string baselinePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "ACAT", "performance_baseline.json");

PerformanceBaselineData baseline = PerformanceBaseline.Load(baselinePath);

var dashboard = new PerformanceDashboard(
    collector: collector,
    profiler:  profiler,
    baseline:  baseline);
dashboard.Show();
```

The committed baseline used by CI lives at
`src/Libraries/ACATCore.Tests.Performance/baselines/performance-baseline.json`.

---

## Dashboard Panels

| Panel | Metrics |
|-------|---------|
| **Memory** | Working set (MB), managed heap (MB), total GC collection count |
| **Runtime** | Process uptime, active thread count, OS handle count |
| **Category Status** | One row per `RuntimeMetricCategory` (UI, Prediction, I/O, Memory, CPU, General). Each row has a toggle checkbox. Green ✓ = within baseline; orange ⚠ = threshold exceeded; grey = no data or hidden. |
| **Working Set Trend** | Sparkline of the last 60 working-set samples with axis labels. |

---

## Historical Trend Graph

The **Working Set Trend** panel displays a live sparkline of the last 60
working-set readings captured since the dashboard was opened (or since the
last "Clear History" action).

- The Y-axis auto-scales to the min/max values in the current history window.
- Axis labels (max at top, min at bottom) are shown in the top-left corner.
- The sparkline is rendered using WPF's built-in `Polyline` — no additional
  charting libraries are required.

The `PerformanceDashboardViewModel.WorkingSetHistory` observable collection
drives the sparkline; the code-behind calls `UpdateSparkline()` after each
metrics refresh.

---

## Export

Use the toolbar buttons (or keyboard shortcuts) to save captured data:

| Action | Button | Shortcut |
|--------|--------|----------|
| Export all memory snapshots as CSV | **Export CSV** | `Ctrl+E` |
| Export snapshots + runtime metrics as JSON | **Export JSON** | `Ctrl+J` |
| Refresh metrics immediately | **Refresh** | `F5` |
| Clear snapshot history | **Clear History** | — |

Exported CSV columns:
`Timestamp, Label, WorkingSetMB, PrivateMemoryMB, ManagedHeapMB, ThreadCount, HandleCount`

---

## Keyboard Accessibility

| Key | Action |
|-----|--------|
| `F5` | Refresh metrics immediately |
| `Ctrl+E` | Open Export CSV dialog |
| `Ctrl+J` | Open Export JSON dialog |
| `Tab` | Move focus between action buttons |
| `Space` / `Enter` | Activate focused button |

Category toggle checkboxes are reachable via `Tab` and can be toggled with
`Space`. All interactive controls have `AutomationProperties.Name` values for
screen-reader compatibility.

---

## Architecture

```
PerformanceDashboard.xaml          – WPF UI layout
PerformanceDashboard.xaml.cs       – Code-behind: timer, refresh loop, sparkline rendering
PerformanceDashboardViewModel.cs   – MVVM ViewModel: observable metric properties,
                                     WorkingSetHistory collection, sparkline range
```

The code-behind sets `DataContext = _viewModel` so that future XAML bindings
can target the ViewModel directly. Today the code-behind still writes to named
controls for the existing panels and calls
`_viewModel.UpdateFromSnapshot(...)` to keep the sparkline history up-to-date.

### Supporting classes (in `ACATCore`)

| Class | Location | Purpose |
|-------|----------|---------|
| `MemoryProfiler` | `Utility/Diagnostics/` | Captures and stores `MemorySnapshot` objects |
| `RuntimeMetricsCollector` | `Utility/Metrics/` | Periodic sampling; exposes `RuntimeMetricEntry` aggregates |
| `PerformanceBaseline` | `Utility/Diagnostics/` | Loads/saves baseline thresholds from JSON |
| `PerformanceRegressionDetector` | `Utility/Diagnostics/` | Compares observed values against baseline thresholds |
