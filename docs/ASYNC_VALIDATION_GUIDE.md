# Async/Await Validation Guide

**Audience:** Validation / QA Engineers  
**Feature:** Phase 3 – Workstream C: Async/Await patterns for I/O-bound operations  
**Branch:** `copilot/implement-async-await-patterns`

---

## Overview

This guide describes how to validate the async/await work introduced in the ACAT codebase.  The changes add non-blocking async equivalents for every synchronous file-I/O path and for the ConvAssist named-pipe communication layer.  The existing synchronous APIs are **unchanged** — no existing functionality is broken; the async methods are additive.

The validation strategy has three layers:

1. **Automated unit tests** (fast, no ACAT runtime needed)
2. **Manual smoke tests** (require a working ACAT build)
3. **UI-thread deadlock proof** (WinForms-specific regression)

---

## 1 · Prerequisites

| Requirement | Notes |
|---|---|
| .NET Framework 4.8.1 SDK | Required to build and run the test project |
| Visual Studio 2022 or `dotnet` CLI | Either works for the automated tests |
| ACAT installed / built from source | Required for manual smoke tests only |
| ConvAssist word predictor configured | Required for pipe tests only |

Clone or pull the branch:

```cmd
git checkout copilot/implement-async-await-patterns
```

---

## 2 · Automated Unit Tests

All async paths have automated coverage.  Run the test suite first — every test must pass before proceeding to manual tests.

### 2.1 Run all async-related tests

```cmd
cd src\Libraries\ACATCore.Tests

dotnet test --filter "FullyQualifiedName~RepositoryTests" -v normal
dotnet test --filter "FullyQualifiedName~PreferencesBaseAsyncTests" -v normal
```

### 2.2 Expected results

All tests in both suites must report **Passed**.  The key scenarios covered are:

| Test class | What is validated |
|---|---|
| `RepositoryTests` | `ConfigurationRepository<T>.LoadAsync/SaveAsync` round-trip; `PreferencesRepository<T>.LoadAsync/SaveAsync` XML round-trip; `ThemeRepository` async null/missing-file guards; `IAsyncRepository<T>` interface assignment |
| `PreferencesBaseAsyncTests` | `PreferencesBase.LoadAsync<T>` — null path, missing file, round-trip, auto-save; `PreferencesBase.ReloadAsync<T>`; `PreferencesBase.SaveAsync<T>` cancellation; `GlobalPreferences.LoadAsync/SaveAsync` round-trip |

### 2.3 Verify cancellation is honoured

The `ConfigurationRepository_LoadAsync_RespectsCancellation` and `PreferencesBase_SaveAsync_RespectsCancellation` tests prove that passing a pre-cancelled `CancellationToken` raises `OperationCanceledException`.  Confirm these tests are present and passing — they are the primary guard against fire-and-forget regressions.

---

## 3 · Manual Smoke Tests

These tests exercise the async paths end-to-end in a running ACAT instance.  They require a complete debug build:

```cmd
msbuild src\ACAT.sln /p:Configuration=Debug
```

### 3.1 Preferences load on startup (PreferencesBase / GlobalPreferences)

**Goal:** Confirm the application starts without freezing and that user settings are loaded correctly.

1. Launch ACAT normally.
2. Open **Settings → User Preferences**.
3. Change at least one value (e.g., scan time) and click **Save**.
4. Restart ACAT.
5. **Expected:** The changed value is still present after restart, confirming `SaveAsync` persisted the file and `LoadAsync` read it back on startup.

**Regression indicator:** If the UI freezes for more than ~2 seconds on startup or on save, a deadlock in the async path is likely.

---

### 3.2 Theme switching (SetActiveThemeAsync)

**Goal:** Confirm that switching themes does not block the UI thread.

1. Open **Settings → Themes**.
2. Switch to a theme that is different from the currently active one.
3. **Expected:** The UI updates immediately without a visible freeze.  The new theme colours are applied to all scanners and dialogs.
4. Restart ACAT.
5. **Expected:** The newly selected theme is still active after restart.

**What to look for:** A brief visual "lag" when switching was acceptable under the synchronous `SetActiveTheme`; the async path (`SetActiveThemeAsync`) should feel at least as responsive.

---

### 3.3 Abbreviations load / save (Abbreviations.LoadAsync / SaveAsync)

**Goal:** Confirm abbreviations are loaded and saved without blocking.

1. Open **Settings → Abbreviations**.
2. Add a new abbreviation (e.g., `ty` → `thank you`).
3. Click **Save** and close the dialog.
4. Type `ty` in the text area.
5. **Expected:** ACAT expands `ty` to `thank you`, proving the save was persisted correctly and the abbreviation list was reloaded.

---

### 3.4 Pronunciations load / save (Pronunciations.LoadAsync / SaveAsync)

**Goal:** Confirm the TTS pronunciation list is loaded and saved without blocking.

1. Open **Settings → Pronunciations**.
2. Add an override (e.g., pronounce `acat` as `ay cat`).
3. Click **Save** and close.
4. Use **Speak** on text containing `acat`.
5. **Expected:** The TTS engine uses the overridden pronunciation.

---

### 3.5 ConvAssist word predictor initialisation (CreatePipeServerAsync)

**Goal:** Confirm ConvAssist starts without a UI-thread deadlock.

> This test only applies if ConvAssist is configured.

1. Enable the ConvAssist word predictor in Settings.
2. Restart ACAT.
3. **Expected:** The word predictor panel appears and suggestions populate within the normal startup time (~5–10 seconds).  The UI is responsive throughout startup.

**Regression indicator:** If ACAT hangs on startup with ConvAssist enabled, suspect the `CreatePipeServerAsync` path.  The old `.Result` call was the known deadlock risk — the async path eliminates this.

---

### 3.6 ConvAssist word prediction requests (WriteAsync path)

**Goal:** Confirm that word and sentence predictions arrive without blocking the main thread.

1. Open the typing scanner.
2. Type a few words.
3. **Expected:** Word predictions update after each word, and the scanner remains responsive between keystrokes.
4. Enable sentence prediction mode if available.
5. **Expected:** Sentence suggestions appear without any visible UI stutter.

---

## 4 · UI-Thread Deadlock Proof (Regression Test)

This is the highest-priority regression to check.  All async methods in library code use `ConfigureAwait(false)`.  To confirm this, perform the following while using a debug build with the debugger attached:

1. Set a breakpoint inside `PreferencesBase.LoadAsync<T>` (line: `await repo.LoadAsync(...).ConfigureAwait(false)`).
2. Trigger a preferences reload from the UI (e.g., by switching profiles).
3. **Expected:** The breakpoint is hit on a **thread-pool thread**, not on the WinForms UI thread (`Thread.CurrentThread.IsBackground == true`; `SynchronizationContext.Current == null` at the `await` continuation point).

If the continuation runs on the UI thread with `SynchronizationContext.Current != null`, a missing `ConfigureAwait(false)` is the cause — file a defect.

---

## 5 · What Is Not Changed (Regression Boundary)

The following behaviours are unchanged and should pass existing regression suites without modification:

- All synchronous `Load()` / `Save()` calls are still present and untouched.
- The existing XML serialization format for all preferences files is unchanged.
- The existing JSON format for all configuration files is unchanged.
- CQRS command/query routing is unchanged (new async handler samples are additive only).
- The ConvAssist `WriteSync` method is still present; only async alternatives were added alongside it.

---

## 6 · Known Limitations

| Limitation | Detail |
|---|---|
| XML serialization is not truly async | `XmlUtils.XmlFileLoadAsync` / `XmlFileSaveAsync` use `Task.Run` because `XmlSerializer` has no native async API on .NET Framework 4.8.1.  This avoids blocking the calling thread but does consume a thread-pool thread. |
| `NamedPipeServerConvAssist.WriteAsync` uses APM | The `BeginWrite`/`EndWrite` APM pattern is used because `Stream.WriteAsync()` on named pipes behaves differently on .NET Framework 4.8.1.  This is correct and intentional. |
| No `IAsyncDisposable` | .NET Framework 4.8.1 does not include `IAsyncDisposable`.  Cleanup uses standard `IDisposable`. |

---

## 7 · Test Coverage Summary

| Area | Automated tests | Manual smoke test |
|---|---|---|
| `ConfigurationRepository<T>` async | ✅ `RepositoryTests` | N/A (covered by file I/O paths) |
| `PreferencesRepository<T>` async | ✅ `RepositoryTests` | ✅ 3.1 Preferences load on startup |
| `PreferencesBase` static async helpers | ✅ `PreferencesBaseAsyncTests` | ✅ 3.1 |
| `GlobalPreferences` async | ✅ `PreferencesBaseAsyncTests` | ✅ 3.1 |
| `Abbreviations.LoadAsync/SaveAsync` | — | ✅ 3.3 |
| `Pronunciations.LoadAsync/SaveAsync` | — | ✅ 3.4 |
| `Theme.CreateAsync` / `SetActiveThemeAsync` | — | ✅ 3.2 |
| `CreatePipeServerAsync` | — | ✅ 3.5 |
| `WriteAsync` pipe variants | — | ✅ 3.6 |
| Cancellation token propagation | ✅ `RepositoryTests`, `PreferencesBaseAsyncTests` | — |
| UI-thread deadlock proof | — | ✅ Section 4 |
