# ACAT Async Migration Analysis

## Overview

This document records the audit of blocking I/O operations in ACAT as the first step
of the async/await workstream (Phase 3 – Workstream C). It identifies each blocking call,
categorises it by priority, and maps it to its async replacement.

---

## Blocking I/O Audit

### File I/O

| Location | Blocking Call | Priority | Async Replacement |
|---|---|---|---|
| `DataAccess/PreferencesRepository<T>.Load` | `XmlUtils.XmlFileLoad<T>` (wraps `XmlDocument.Load`) | High – may be called on startup path | `Task.Run(() => Load(...))` via `RepositoryBase<T>.LoadAsync` |
| `DataAccess/PreferencesRepository<T>.Save` | `XmlUtils.XmlFileSave` (wraps `XmlDocument.Save`) | Medium – save is typically on user action | `Task.Run(() => Save(...))` via `RepositoryBase<T>.SaveAsync` |
| `DataAccess/ConfigurationRepository<T>.Load` | `File.ReadAllText` | High – used during startup | `StreamReader.ReadToEndAsync()` in `ConfigurationRepository<T>.LoadAsync` |
| `DataAccess/ConfigurationRepository<T>.Save` | `File.WriteAllText` | Medium | `StreamWriter.WriteAsync()` in `ConfigurationRepository<T>.SaveAsync` |
| `Utility/JsonConfigurationLoader<T>.Load` | `File.ReadAllText` | High – used for configuration loading | `StreamReader.ReadToEndAsync()` in `JsonConfigurationLoader<T>.LoadAsync` |
| `Utility/JsonConfigurationLoader<T>.Save` | `File.WriteAllText` | Medium | `StreamWriter.WriteAsync()` in `JsonConfigurationLoader<T>.SaveAsync` |
| `Configuration/ConfigurationReloadService` | `Thread.Sleep(100)` (waits for file unlock) | Low – runs on background thread | Already on background thread via `Timer` callback; acceptable as-is |
| `DataAccess/ThemeRepository.Load` | `Theme.Create(...)` (wraps file reads) | Medium – theme loading at startup | `Task.Run(() => Load(...))` via `RepositoryBase<T>.LoadAsync` |

### Named Pipe Communication (ConvAssist)

| Location | Blocking Call | Priority | Status |
|---|---|---|---|
| `NamedPipeServerConvAssist.Write` | `NamedPipeServerStream.Write(...)` | Medium – already called from background thread | Blocking write; superseded by `WriteAsync` |
| `NamedPipeServerConvAssist.WriteSync` | `Thread.Sleep(10)` in polling loop | High – blocks caller thread while waiting for response | Blocking; use `WriteAsync` instead |
| `NamedPipeServerConvAssist.WriteAsync` | Uses `BeginWrite`/`EndWrite` APM pattern | — | Already async; use this in preference to `WriteSync` |
| `NamedPipeServerConvAssist.CreatePipeServer` | `.Result` on `StartNamedPipeServer(...)` | High – `.Result` can deadlock on UI thread | Should be awaited; callers must be updated |
| `NamedPipeServerConvAssist.ReadCallback` | `BeginRead` APM callback | — | Already async; acceptable |

### Network

| Location | Call | Priority | Status |
|---|---|---|---|
| Serilog Seq sink | Internal async batching | — | Already async internally; no action required |

---

## Implemented Changes (Phase 3 – Workstream C)

### New Interfaces

- **`DataAccess/IAsyncRepository<T>`** – Async variant of `IRepository<T>` with
  `LoadAsync`, `SaveAsync`, and `GetDefaultAsync`.
- **`Patterns/CQRS/IAsyncCommandHandler<TCommand>`** – Async CQRS command handler
  interface for I/O-bound commands.
- **`Patterns/CQRS/IAsyncQueryHandler<TQuery, TResult>`** – Async CQRS query handler
  interface for I/O-bound queries.

### Updated Classes

- **`DataAccess/RepositoryBase<T>`** – Now implements both `IRepository<T>` and
  `IAsyncRepository<T>`. Provides default `Task.Run`-based async implementations
  that derived classes may override.
- **`DataAccess/ConfigurationRepository<T>`** – Overrides `LoadAsync`/`SaveAsync`
  with fully-async `StreamReader`/`StreamWriter` implementations, avoiding `Task.Run`
  thread overhead for JSON configuration files.
- **`Utility/JsonConfigurationLoader<T>`** – New `LoadAsync` and `SaveAsync` methods
  using `StreamReader.ReadToEndAsync()` and `StreamWriter.WriteAsync()`.
- **`DependencyInjection/ServiceCollectionExtensions.cs`** – `AddRepositories()` now
  registers `IAsyncRepository<Theme>` as a singleton, forwarding to `ThemeRepository`.

---

## Blockers Analysis: Switching to Async

After the initial async infrastructure was put in place, a review of the codebase
identified the following blockers that prevented callers from switching to the async
API.  All **blockers** listed below have been resolved in this workstream.

### Resolved Blockers

| # | Blocker | Fix |
|---|---------|-----|
| 1 | `PreferencesBase` had no async static helpers (`LoadAsync`, `ReloadAsync`, `SaveAsync`) | Added to `PreferencesManagement/PreferencesBase.cs` |
| 2 | `GlobalPreferences` had no async entry points (`LoadAsync`, `SaveAsync`) | Added to `Utility/GlobalPreferences.cs` |

### Remaining (Incremental) Work

These items are **not blockers** for adopting the async API — the async infrastructure
exists and callers *can* switch.  They represent further incremental migration
opportunities:

| # | Item | Status |
|---|------|--------|
| 3 | `JsonConfigurationLoader<T>` callers (`Abbreviations`, `ActuatorConfig`, `Pronunciations`, `PreferredWordPredictors`, `Theme.cs`) still call `.Load()`/`.Save()` synchronously | ✅ Resolved: async `LoadAsync`/`SaveAsync` public methods added to all five classes; `Theme.CreateAsync()` added |
| 4 | `ThemeManager.SetActiveTheme()` calls `Theme.Create()` directly instead of `_themeRepository.LoadAsync()` | ✅ Resolved: `SetActiveThemeAsync()` added to both `IThemeManager` and `ThemeManager` |
| 5 | `NamedPipeServerConvAssist.CreatePipeServer` uses `.Result` (deadlock risk on UI thread) | ✅ Resolved: `CreatePipeServerAsync()` added; `ConvAssistWordPredictor.InitAsync()` uses it |
| 6 | `NamedPipeServerConvAssist.WriteSync` polls with `Thread.Sleep` | ✅ Resolved: async variants (`ConvAssistLearnAsync`, `SendMessageConvAssistSentencePredictionAsync`, `SendMessageConvAssistWordPredictionAsync`) added alongside sync methods |
| 7 | `XmlUtils` has no async variants | ✅ Resolved: `XmlFileLoadAsync`/`XmlFileSaveAsync` added to `XmlUtils`; `PreferencesRepository<T>` now overrides `LoadAsync`/`SaveAsync` using these helpers |
| 8 | CQRS handlers that perform I/O have no `IAsyncCommandHandler`/`IAsyncQueryHandler` implementations | ✅ Resolved: `LoadPreferencesQuery<T>` + `LoadPreferencesQueryHandler<T>`, `SavePreferencesCommand<T>` + `SavePreferencesCommandHandler<T>` added to `Patterns/CQRS/Samples/` |

---

## .NET Framework 4.8.1 Async Notes

- `File.ReadAllTextAsync` / `File.WriteAllTextAsync` are **not available** on
  .NET Framework 4.8.1 (introduced in .NET Core 2.0 / .NET Standard 2.1).
  Use `StreamReader.ReadToEndAsync()` and `StreamWriter.WriteAsync()` instead.
- `ConfigureAwait(false)` is applied on every `await` in library code to prevent
  `SynchronizationContext` deadlocks when called from WinForms UI threads.
- `IAsyncDisposable` is **not available** on .NET Framework 4.8.1. Cleanup uses
  `Task`-returning patterns or regular `IDisposable`.
- `Task.Run()` is used as a fallback for operations backed by blocking APIs
  (e.g., `XmlUtils`) that do not expose async variants.

---

## Remaining Work / Future Recommendations

All planned async migration items have been completed. The async API is now fully
usable across the entire codebase.

Potential further optimisations:
- Replace the `Task.Run` fallback in `XmlUtils.XmlFileLoadAsync`/`XmlFileSaveAsync` with
  genuinely async XML serialization once the codebase is fully on .NET 6+.
- Replace the APM (`BeginWrite`/`EndWrite`) pattern in `NamedPipeServerConvAssist.WriteAsync`
  with `Stream.WriteAsync()` for cleaner code once targeting .NET 6+.
