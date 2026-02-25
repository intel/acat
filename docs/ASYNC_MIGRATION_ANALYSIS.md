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

- **`NamedPipeServerConvAssist.CreatePipeServer`**: The `.Result` call is a
  deadlock risk if ever invoked on the UI thread. A future change should expose an
  `await CreatePipeServerAsync()` method and update callers accordingly.
- **`NamedPipeServerConvAssist.WriteSync`**: Should be deprecated in favour of
  `WriteAsync`; callers need updating.
- **`XmlUtils`**: Adding `XmlFileLoadAsync` / `XmlFileSaveAsync` helpers would allow
  `PreferencesRepository<T>` to use fully-async XML I/O instead of `Task.Run`.
- **CQRS handlers**: Concrete async command/query handler implementations should be
  added for any handler that performs I/O (e.g., reading configuration).
