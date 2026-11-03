# ACAT Async Migration Analysis

## Overview

This document records the audit of blocking I/O operations in ACAT as the first step
of the async/await workstream (Phase 3 – Workstream C). It identifies each blocking call,
categorises it by priority, and maps it to its async replacement.

## Phase 4: Call-site adoption inventory (#278)

The following inventory was collected **before code changes**, searching `src/**/*.cs`
for the synchronous APIs listed in the issue and following instance and inherited
preference calls. Definitions, comments, and tests are not production callers.
The existing synchronous implementations are compatibility boundaries, not candidates
for deletion.

| API family | Initial callers / locations | Adoption plan |
|---|---|---|
| JSON loader | 9 calls inside `Theme`, `Abbreviations`, `Pronunciations`, `ActuatorConfig`, `PreferredWordPredictors`; 5 in `ConfigurationExamples` | Preserve sync implementations; route startup to the existing async implementations. Examples are not startup paths. |
| Global/user preferences | `AppCommon.LoadGlobalSettings`, `LoadUserPreferences`, onboarding saves; `ACATPreferences.Load`, `LoadDefaultSettings` | Add async helpers alongside the public sync methods and await from WinForms startup events. |
| PreferencesBase | 19 explicitly qualified loads: 2 command tables, 8 default extension settings wrappers (LaunchApp, Chrome, Edge, Outlook, Explorer, Camera, SAPI, TTSClient), 9 EEG settings wrappers | Preserve synchronous plugin contracts; adopt async where a task-returning load path is available. |
| Abbreviations / pronunciations | `AbbreviationsManager.Load`, manager initialization and mutation saves; `TTSManager` pronunciation initialization and mutation saves | Async manager initialization; retain immediate-result mutation contracts where necessary. |
| Actuator configuration | 8 explicit `ActuatorConfig.Load` calls in core (`ActuatorManager`, `Actuators`, switch/keyboard forms), 1 in onboarding; instance saves | Async startup loading; document immediate-result accessors and UI configuration boundaries. |
| Preferred predictors | `WordPredictors` constructor (`PreferredWordPredictors.Load`) and instance saves | Move I/O out of construction into async initialization. |
| Themes | 2 `Theme.Create` calls in `ThemeManager` constructor, 1 in `SetActiveTheme`, 2 in `ThemeRepository`; 1 `SetActiveTheme` in `Context` | Async manager startup and repository loads; retain public synchronous compatibility paths. |
| ConvAssist | `ConvAssistWordPredictor.Init`: 1 pipe creation; 3 request methods call `WriteSync`; `NamedPipeServerConvAssist.Restart`: 1 pipe creation | Use existing async initialization and request methods in task-capable paths; preserve synchronous predictor interface methods. |
| XML helpers | `PreferencesRepository.Save`: 1 `XmlFileSave`; generic `XmlFileLoad` in synchronous preference/repository helpers | Preserve sync repository methods; use existing async repository overrides in async callers. |

Initialization is order-sensitive: user/profile paths precede preference loading;
themes precede panels; actuators and agents may construct WinForms controls. Do not
run the whole initialization sequence on a worker thread. Independent file loads
may overlap, but UI work must return to the owning STA thread.

Validation baseline in this Linux sandbox: the integration test command stops at
MSB3644 (missing .NET Framework 4.8.1 reference assemblies), before compilation or
test execution. Windows validation remains necessary; this is not a test failure.

### Reproducible expression census

This census counts explicitly qualified production calls, excluding comments,
string literals, tests, and `src/Docs` samples. It includes the bodies of retained
synchronous APIs. Inherited/instance preference calls and unqualified pipe restart
calls are followed separately in the path inventory above.

| Qualified sync expression | Before adoption | Retained compatibility/example expressions |
|---|---:|---:|
| JSON `loader` / `_loader` Load/Save | 14 | 14 |
| PreferencesBase Load/Save/Reload | 19 | 19 |
| GlobalPreferences Load/Save | 1 | 1 |
| ActuatorConfig Load/Save | 9 | 5 |
| Abbreviations Load/Save | 1 | 1 |
| PreferredWordPredictors Load/Save | 1 | 1 |
| Theme.Create | 5 | 5 |
| manager.SetActiveTheme | 1 | 1 |
| namedPipe.CreatePipeServer | 1 | 1 |
| namedPipe.WriteSync | 3 | 3 |
| XmlUtils.XmlFileLoad/XmlFileSave | 2 | 2 |
| **Total** | **57** | **53** |

Four dialog load expressions are replaced outright (and their two instance saves
are also asynchronous). Other migration paths add task-returning entry points and
route production callers to them, while retaining the original expressions in
synchronous compatibility methods. Therefore a raw sync grep count alone is not
the count of I/O operations still performed by async startup.

### Application adoption results

- **17 startup call-site replacements** across ACATApp, ACATTalk, ACATConfig,
  ACATConfigNext, and ACATWatch: 3 global loads, 4 user preference loads, 2 Context
  initializations, 3 username saves, 3 display-setting saves, and 2 onboarding
  calls. ConfigNext additionally bypasses its synchronous constructor reload using
  preloaded preferences, and its cancellation reload is asynchronous.
- Startup begins after the WinForms message loop starts. Existing `[STAThread]`
  `void Main` signatures remain unchanged; no async `Main` or blocking task bridges
  are used. Modeless forms keep the startup task alive until `FormClosed`.
- `GlobalPreferences.LoadAsync` now also awaits the factory-default file save
  before loading user settings. Previously this first write was synchronous even
  through the async API. Cancellation coverage verifies that a pre-cancelled load
  cannot write that file.
- Factory preference *object construction* (`LoadDefaults`/`LoadDefaultSettings`)
  has no file I/O and deliberately stays synchronous.

### Manager and extension adoption results

| Migrated path | Async I/O used | Compatibility retained |
|---|---|---|
| Context → themes | ThemeManager.InitAsync, SetActiveThemeAsync, ThemeRepository.LoadAsync → Theme.CreateAsync | Lazy synchronous theme access and Init/SetActiveTheme |
| Context → actuators | ActuatorManager.LoadExtensionsAsync → Actuators.LoadAsync → ActuatorConfig.LoadAsync/SaveAsync | Plugin discovery/construction, hardware Init and immediate-result mutation APIs |
| Context → word prediction | WordPredictionManager.InitAsync → preferred predictor LoadAsync; SetActiveWordPredictorAsync → optional IAsyncInitializer | Existing IWordPredictor implementations fall back to synchronous Init |
| Context → abbreviations | AbbreviationsManager.InitAsync → Abbreviations.LoadAsync | Existing Init/Load and synchronous mutations |
| Context → TTS | SetActiveEngineAsync → SAPI/TTSClient InitAsync → culture-aware Pronunciations.LoadAsync | Engine construction/settings, plugin preference saves, legacy engine Init |
| ConvAssist startup | Settings.LoadAsync, CreatePipeServerAsync | Synchronous Init and preference access |
| ConvAssist request processing | Async word/sentence handlers and learning → existing async sends | Existing synchronous request handlers and IWordPredictor methods |
| Switch/keyboard dialogs | Four config loads and two saves → ActuatorConfig async APIs | Public form constructors/signatures unchanged |

Preferred predictor configuration and abbreviations are independent reads and
are awaited with `Task.WhenAll` in Context startup. Predictor activation still
follows configuration loading; plugin constructors, hardware initialization,
panel/widget initialization, and engine selection are dispatched to the owning
STA. The pipe response slot is shared, so its requests are serialized, **not**
parallelized.

New regression coverage exercises theme async/sync parity and fallback,
abbreviation missing/valid/malformed files, async and legacy predictor
initialization/fallback, STA construction and dispatcher error propagation,
learning selection, pipe protocol behavior, global-default save cancellation,
and dialog save rejection when the reloaded actuator is absent.

### Validation status

- `git diff --check` passes.
- Core/application/ConvAssist builds and the requested integration/performance
  test commands were attempted. All stop before compilation at MSB3644 because
  this Linux environment lacks the .NET Framework 4.8.1 targeting pack.
- Focused new test commands were also attempted and hit the same prerequisite.
  WinForms execution, passing-test counts, and zero-new-warning semantic builds
  cannot be verified here; they require Windows with the net481 developer pack.
- Existing MSB3539 output-directory warnings appear on baseline test commands;
  output configuration is outside this migration's scope.
- Secret scanning of every changed/new file found no secrets. Post-commit CodeQL
  was requested, but C# analysis was skipped because the database is too large;
  this is not a completed clean security analysis.

### Deliberately synchronous compatibility boundaries

- The existing `Load`, `Save`, `Reload`, `Init`, theme creation/selection, and
  predictor request methods remain callable. Their implementations must not call
  an async method and block on its task. Existing synchronous tests continue to
  exercise these APIs rather than being rewritten merely to reduce grep counts.
- Chrome, Edge, Outlook, Explorer, and LaunchApp agents read preferences in their
  constructors. The type loader and external consumers expect fully initialized
  instances on return. An asynchronous constructor is not available in C#;
  returning an incompletely initialized agent would change that contract.
- Camera and EEG preference wrappers serve synchronous `GetPreferences`,
  calibration, and hardware configuration contracts. Their nine EEG and one
  Camera `PreferencesBase.Load` expressions are retained rather than starting
  unobserved asynchronous work or blocking on a task.
- `CmdDescriptorTable.Load` and `CmdScopeTable.Load` are immediate-result table
  accessors; `CommandManager.Init` currently performs no file I/O.
- `PreferencesRepository.Load/Save` and `ThemeRepository.Load/GetDefault` implement
  `IRepository<T>`. The async repository counterparts serve asynchronous callers;
  the synchronous interface is not changed.
- `ActuatorManager.GetActuatorConfig`, enable/disable mutations, and existing
  synchronous collection mutation APIs return immediate results. Configuration
  dialog loads and saves can instead await I/O before accepting their result.
- `ConfigurationExamples` contains five synchronous JSON calls in illustrative
  immediate-result loaders/constructors and reload callbacks, not application
  startup. These examples remain demonstrations of the synchronous API.
- Synchronous ConvAssist request methods remain for `IWordPredictor` consumers.
  Async request processing must serialize access to the shared response slot;
  parallel sends on a single pipe are not independent work.
  The synchronous compatibility sends use bounded semaphore acquisition, not
  `Task.Wait`; async sends await the semaphore with cancellation. No parameterless
  `.Wait()`, `.Result`, or `GetAwaiter().GetResult()` task bridges are added.

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

The infrastructure work described above makes the async API usable across the
codebase. Phase 4 call-site adoption and deliberately synchronous compatibility
boundaries are tracked separately at the top of this document.

Potential further optimisations:
- Replace the `Task.Run` fallback in `XmlUtils.XmlFileLoadAsync`/`XmlFileSaveAsync` with
  genuinely async XML serialization once the codebase is fully on .NET 6+.
- Replace the APM (`BeginWrite`/`EndWrite`) pattern in `NamedPipeServerConvAssist.WriteAsync`
  with `Stream.WriteAsync()` for cleaner code once targeting .NET 6+.
