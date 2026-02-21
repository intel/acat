# Phase 2 – Core Infrastructure Modernization: Final Status Report

**Epic Issue:** [#225](https://github.com/intel/acat/issues/225)  
**Report Date:** February 21, 2026  
**Status:** 🟡 **Substantially Complete – Final Wiring Tasks Outstanding**  
**Prepared By:** @Copilot  

---

## Executive Summary

Phase 2 set out to build on the Phase 1 logging/configuration foundation and deliver five major feature areas: Dependency Injection infrastructure, Configuration System Enhancement, Testing Infrastructure, Performance Monitoring, and Architecture Modernization (event bus, CQRS, repository pattern). A sixth area, Animation System Preparation, was scoped as Phase 3 investigation only.

**All six Feature issues (#190–#195) have been closed as completed.** The underlying infrastructure – interfaces, service container, event bus, CQRS scaffolding, repository layer, test projects, performance tooling, and documentation – is fully in place. However, **13 Task-level issues (#209–#221) remain formally open**, and a review of the source code reveals that some production call-site migration work (CQRS wiring, EventBus subscriber migration, schema-validation auto-wiring) is still pending before every Phase 2 success criterion can be claimed as fully met.

The sections below cover each Feature in detail, measure it against the original success criteria, and call out the specific remaining tasks that should be turned into new work items.

---

## Phase 2 Feature Review

### Feature #190 – Dependency Injection Infrastructure
**Status:** ✅ Closed / Completed (closed 2026-02-19)  
**Dependent Tasks:** #209, #212, #213, #214, #215, #216 – All **OPEN**

#### What Was Delivered
- `ServiceConfiguration` class (`Utility/ServiceConfiguration.cs`) with `AddACATServices()` and `AddACATInfrastructure()` convenience methods
- Ten core manager interfaces extracted and registered in the DI container:
  `IActuatorManager`, `IAgentManager`, `ITTSManager`, `IPanelManager`, `IThemeManager`, `IWordPredictionManager`, `ISpellCheckManager`, `IAbbreviationsManager`, `ICommandManager`, `IAutomationEventManager`
- Ten matching factory interfaces and implementations (e.g., `IActuatorManagerFactory`)
- `Context` class enhanced with `ServiceProvider` static property and `GetManager<T>()` method for DI-aware resolution
- `ExtensionInstantiator` verified working with `IServiceProvider`
- All five application entry points (ACATApp, ACATTalk, ACATConfigNext, ACATWatch, ACATConfig) updated to initialise the service container
- 82 DI-specific tests in `ACATCore.Tests.Configuration`; test coverage improved from ~40% to ~72% for DI infrastructure

#### What Is Still Outstanding
- All task issues (#209–#216) are still open; they need to be formally reviewed and closed (or their remaining sub-scope extracted into new tasks).
- The DI pattern used is **service-locator** (`Context.GetManager<T>()`), not **constructor injection**. Production classes still access managers via `Context.AppXxxMgr` static properties or `XxxManager.Instance`. True constructor injection (passing interface dependencies through constructors) has not been rolled out to existing classes.
- CQRS handlers are registered in DI (Section 3.1 complete) but call sites that *invoke* those handlers still use the old direct singleton pattern (see Feature #194 for details).

---

### Feature #191 – Configuration System Enhancement
**Status:** ✅ Closed / Completed (closed 2026-02-19)  
**Dependent Tasks:** #211, #217, #218, #219, #220, #221 – All **OPEN**

#### What Was Delivered
- **JSON Schema Definition (Task #217):** Six JSON schemas created and stored in `schemas/json/`:  
  `actuator-settings`, `theme`, `panel-config`, `abbreviations`, `pronunciations`, `animation-config`
- **Schema Validation (Tasks #211 / #218):** `JsonSchemaValidator` class in `Libraries/ACATCore/Configuration/` provides schema loading, caching, and field-level validation with detailed error messages
- **Configuration Hot-Reload (Task #219):** `ConfigurationReloadService` class implements `FileSystemWatcher`-based monitoring with 500 ms debounce; `JsonConfigurationLoader` exposes `EnableHotReload()` / `DisableHotReload()` methods and a `ConfigurationReloaded` event
- **Environment-Specific Configuration (Task #220):** `EnvironmentConfiguration` class supports `Development / Testing / Staging / Production` environments, file layering (`config.Development.json` over `config.json`), and `ACAT_*` environment-variable overrides
- **Configuration Migration Utilities (Task #221):** `ConfigurationVersionManager` class supports semantic-version detection, backup-before-migrate, and pluggable `IConfigurationMigration` handlers

#### What Is Still Outstanding
- All task issues (#211, #217–#221) are still open; they need formal closure review.
- `JsonSchemaValidator` is **not automatically invoked** by `JsonConfigurationLoader`. Callers must create a validator, load the schema, and call `Validate()` explicitly before loading a configuration file. There is no out-of-the-box enforcement that all configurations are schema-validated on load. Achieving the Phase 2 success criterion ("all configuration files have schema validation") requires either auto-wiring schema validation into `JsonConfigurationLoader` or documenting a clear caller-side convention.
- JSON schemas currently exist for six configuration types. Schemas for additional types (e.g., `AppPreferences`, word-predictor settings, TTS-engine settings, agent configurations) have not been created.
- Automatic migration on load (`ConfigurationVersionManager`) is not wired into `JsonConfigurationLoader`. The caller must check and migrate manually.

---

### Feature #192 – Testing Infrastructure
**Status:** ✅ Closed / Completed (closed 2026-02-19)  
**Dependent Tasks:** #196, #197, #222, #223, #224 – All **CLOSED**

#### What Was Delivered
- MSTest 3.7.0 as the primary test framework across all test projects
- Moq 4.20.72 for mocking; FluentValidation 11.9.0 for configuration validation testing
- `ACATCore.Tests.Shared` library with `BaseTest`, `MockHelper`, `TestDataBuilder`, `TestWorkspace`, `AssertHelper`, `TestDataGenerator`
- Five dedicated test projects:  
  - `ACATCore.Tests.Configuration` (217 tests)
  - `ACATCore.Tests.Architecture` (29 tests)
  - `ACATCore.Tests.Integration` (28 tests)
  - `ACATCore.Tests.Performance` (45 tests)
  - `ACATCore.Tests.Logging` (25 tests)
- Total: **355 tests**, all passing
- CI/CD test automation integrated into `build.yml` / `test.yml` workflows with test-result publishing

#### Assessment vs. Success Criteria
The testing infrastructure goal ("Test coverage > 60% for core libraries") is **met**. Coverage is approximately 72% for the DI infrastructure and 65% for configuration classes. The architecture and performance test suites add further coverage.

---

### Feature #193 – Performance Monitoring Enhancement
**Status:** ✅ Closed / Completed (closed 2026-02-20)  
**Dependent Tasks:** #198, #199, #200, #201 – All **CLOSED**

#### What Was Delivered
- `RuntimeMetricsCollector` in `Utility/Metrics/` – periodic sampling of CPU, memory, thread count, OS handles, categorised by `RuntimeMetricCategory`
- `MemoryProfiler` in `Utility/Diagnostics/` – labelled memory snapshot capture with CSV/JSON export
- `PerformanceRegressionDetector` in `Utility/Diagnostics/` – compares live metrics against a stored `PerformanceBaselineData` JSON file; baseline committed to `ACATCore.Tests.Performance/baselines/`
- `PerformanceDashboard` (WPF window) in `Diagnostics/` – real-time display of memory, runtime metrics, regression status, and a 60-sample working-set sparkline; supports `Ctrl+E` (CSV export), `Ctrl+J` (JSON export), F5 refresh; full keyboard accessibility
- `PanelActivityMonitor` registered in DI and activated in ACATApp – first production EventBus subscriber, demonstrating the event pattern in action
- 45 performance-specific tests in `ACATCore.Tests.Performance`

#### Assessment vs. Success Criteria
The performance monitoring goal ("Performance baseline established and monitored") is **fully met**.

---

### Feature #194 – Architecture Modernization
**Status:** ✅ Closed / Completed (closed 2026-02-21)  
**Dependent Tasks:** #202, #203, #204, #205 – All **CLOSED**

#### What Was Delivered

**Interface Extraction (Task #202)**  
- `docs/INTERFACE_EXTRACTION_GUIDE.md` published with naming conventions, directory layout, priority list (Tier 1 – 10 managers; Tier 2 – AnimationManager, UserControlManager, PreferencesManager, ExtensionInstantiator, ConfigurationManager), and a reusable extraction checklist

**Event System (Task #203)**  
`Libraries/ACATCore/EventManagement/` contains:  
`IEvent`, `IEventBus`, `EventBase`, `EventBus` (thread-safe, weak-reference subscriptions),  
`PanelEvents` (PanelShowEvent, PanelHideEvent, PanelActivateEvent),  
`ActuatorEvents` (ActuatorSwitchActivatedEvent),  
`ConfigurationEvents` (ConfigurationReloadEvent, ConfigurationChangedEvent, ConfigurationReloadFailedEvent),  
`AgentEvents` (AgentContextChangedEvent)

`IEventBus` is registered as a singleton. PanelManager, ActuatorManager, ConfigurationReloadService, and AgentManager all publish events. `PanelActivityMonitor` is the first production subscriber.

**CQRS (Task #204)**  
`Libraries/ACATCore/Patterns/CQRS/` contains:  
`ICommand`, `ICommand<TResult>`, `IQuery<TResult>`, `ICommandHandler<T>`, `IQueryHandler<T,R>`,  
Sample handlers: `CreatePanelCommandHandler`, `HandleActuatorSwitchCommandHandler`, `GetActiveAgentNameQueryHandler`, `GetConfigurationValueQueryHandler`  
All four handlers are registered in `ServiceConfiguration`.

**Repository Pattern (Task #205)**  
`Libraries/ACATCore/DataAccess/` contains:  
`IRepository<T>`, `RepositoryBase<T>`, `PreferencesRepository<T>` (XML via XmlUtils), `ConfigurationRepository<T>` (JSON), `ThemeRepository`  
`GlobalPreferences` and `PreferencesBase` have been migrated to use `PreferencesRepository<T>`, covering 26+ call sites. `ThemeManager` uses `ThemeRepository` internally.

**Architecture Unit Tests**  
`ACATCore.Tests.Architecture` has 29 tests covering EventBus, CQRS objects, and repository CRUD.

#### What Is Still Outstanding
- **CQRS call-site migration (Section 3.3 of ARCHITECTURE_IMPLEMENTATION_STATUS.md):** The CQRS sample handlers are registered in DI and functionally correct, but **production code still invokes them via the legacy singleton path**. Per `SECTION_3_3_IMPLEMENTATION_GUIDE.md`, 196 call sites require migration:
  - 9 `PanelManager.Instance.CreatePanel(...)` calls (ACATApp, ACATTalk, CommandHandlers, Scanners, ActuatorBase)
  - 65 `Context.AppActuatorManager.Pause()` / `Resume()` calls (ScannerCommon, DialogCommon, AnimationManager, etc.)
  - 122 `Context.AppAgentMgr.*` calls throughout Extensions
- **EventBus subscriber migration (Section 3.5):** ~35 legacy `+=`/`-=` C# delegate subscriptions in production code have not been migrated to `IEventBus.Subscribe<T>()`.
- **Missing event types (Section 3.6):** `AppQuitEvent`, `CalibrationEndEvent`, `DisplaySettingsChangedEvent`, `WordPredictionContextChangedEvent` have no `IEventBus` equivalents yet.
- **Extension project DI wiring (Section 3.8):** `ACATExtension/CommandHandlers/` (8 files) and `Extensions/ACAT.Extensions.UI/Scanners/` still use `PanelManager.Instance` and `Context.App*Manager` directly.

---

### Feature #195 – Animation System Preparation (Phase 3 Investigation)
**Status:** ✅ Closed / Completed (closed 2026-02-21) – **Deferred to Phase 3**  
**Dependent Tasks:** #206, #207, #208 – All **CLOSED**

#### What Was Delivered
- Current animation system analysis (`docs/ANIMATION_SYSTEM_ANALYSIS.md`)
- Architecture design proposal (`docs/ANIMATION_SYSTEM_ARCHITECTURE.md`)
- Architecture design document (`docs/ANIMATION_SYSTEM_DESIGN.md`)
- Animation System POC was closed as an investigation only; no production code changes

This feature was explicitly scoped as investigation-only for Phase 2, with full implementation deferred to Phase 3.

---

## Phase 2 Success Criteria Assessment

| Criterion | Target | Actual Status | Gap |
|-----------|--------|--------------|-----|
| Components use dependency injection | 90%+ | ~35–40% (service-locator pattern; constructor injection not rolled out) | 🔴 Not Met |
| Configuration files have schema validation | All | 6 of ~12 config types have schemas; validator not auto-wired | 🟡 Partial |
| Test coverage > 60% for core libraries | > 60% | ~72% DI / ~65% config / 355 total tests | ✅ Met |
| Performance baseline established & monitored | Yes | PerformanceDashboard, baselines, regression detection complete | ✅ Met |
| Zero critical architecture violations | Yes | All major patterns in place; no critical violations detected | ✅ Met |

---

## Open GitHub Issues Summary

The following 13 task-level issues remain open. For most, the code artefacts exist but the formal acceptance criteria (checkboxes in the issue description) have not been validated and the issues have not been closed.

| Issue | Title | Feature | Code Status | Recommended Action |
|-------|-------|---------|-------------|-------------------|
| #209 | Setup Service Container | #190 DI | ✅ Done (duplicate of #212) | Close as duplicate |
| #210 | Refactor Context Class | #190 DI | 🟡 Partial (DI-aware accessors added; constructor injection not completed) | Close with note on remaining constructor-injection work |
| #211 | Schema Validation Implementation | #191 Config | 🟡 Partial (validator created; not auto-wired) | Create follow-up task for auto-wiring |
| #212 | Setup Service Container | #190 DI | ✅ Done | Close |
| #213 | Extract Core Interfaces | #190 DI | ✅ Done (10 interfaces extracted and registered) | Close |
| #214 | Implement Factory Patterns | #190 DI | ✅ Done (10 factory interfaces and implementations) | Close |
| #215 | Refactor Context Class | #190 DI | 🟡 Partial (see #210) | Close as duplicate of #210 |
| #216 | Extension Loading with DI | #190 DI | ✅ Done (ExtensionInstantiator with IServiceProvider) | Close |
| #217 | JSON Schema Definition | #191 Config | ✅ Done (6 schemas in schemas/json/) | Close |
| #218 | Schema Validation Implementation | #191 Config | 🟡 Partial (see #211) | Close as duplicate of #211 |
| #219 | Configuration Hot-Reload | #191 Config | ✅ Done (ConfigurationReloadService, enableHotReload option) | Close |
| #220 | Environment-Specific Configuration | #191 Config | ✅ Done (EnvironmentConfiguration class) | Close |
| #221 | Configuration Migration Utilities | #191 Config | ✅ Done (ConfigurationVersionManager, IConfigurationMigration) | Close |

---

## Remaining Development Work

The items below represent work that is **required to fully meet Phase 2 success criteria** or to bring the infrastructure that was built in Phase 2 into active production use. Each item is described with enough detail to be turned directly into a GitHub Task issue.

---

### Task P2-R1: CQRS Wiring – Application Entry Points
**Estimated Effort:** 1 day  
**Feature Parent:** #194 Architecture Modernization  
**Priority:** High

Replace 4 direct `PanelManager.Instance.CreatePanel(...)` calls in the main application entry points with the registered `ICommandHandler<CreatePanelCommand>`:

- `Applications/ACATApp/Program.cs` (2 call sites in `ShowMainPanel()`)
- `Applications/ACATTalk/Program.cs` (2 call sites)

Both programs already hold `_serviceProvider`; the handler can be resolved with `GetRequiredService<ICommandHandler<CreatePanelCommand>>()`.

**Acceptance Criteria:**
- [ ] `PanelManager.Instance.CreatePanel` not called from ACATApp/Program.cs
- [ ] `PanelManager.Instance.CreatePanel` not called from ACATTalk/Program.cs
- [ ] Existing behaviour preserved (panels open correctly)
- [ ] Unit/integration tests updated or added

---

### Task P2-R2: CQRS Wiring – ACATExtension Command Handlers
**Estimated Effort:** 1 day  
**Feature Parent:** #194 Architecture Modernization  
**Priority:** High

Replace 5 `PanelManager.Instance.CreatePanel(...)` calls in extension command handlers and scanners with the CQRS command pattern:

- `Libraries/ACATExtension/CommandHandlers/TalkWindowHandler.cs`
- `Libraries/ACATExtension/CommandHandlers/ShowScreenLockHandler.cs`
- `Extensions/ACAT.Extensions.UI/Scanners/DashboardAppScanner.cs`
- `Libraries/ACATCore/ActuatorManagement/BaseActuators/ActuatorBase.cs` (2 sites)

**Acceptance Criteria:**
- [ ] All 5 call sites migrated
- [ ] Constructor injection or service-locator resolution used (no `Instance` access)
- [ ] Tests added for each handler

---

### Task P2-R3: CQRS Wiring – Actuator Pause/Resume (High-Leverage)
**Estimated Effort:** 1.5 days  
**Feature Parent:** #194 Architecture Modernization  
**Priority:** Medium

Replace 65 `Context.AppActuatorManager.Pause()` / `Context.AppActuatorManager.Resume()` calls with `ICommandHandler<HandleActuatorSwitchCommand>`. The highest-leverage targets are base classes that cascade to many derived classes:

- `Libraries/ACATExtension/UI/ScannerCommon.cs` (~30 call sites via inheritance)
- `Libraries/ACATExtension/UI/DialogCommon.cs` (~20 call sites via inheritance)
- `Libraries/ACATCore/AnimationManagement/AnimationManager.cs`

**Acceptance Criteria:**
- [ ] Base class changes fix the majority of derived-class call sites
- [ ] `Context.AppActuatorManager.Pause()` / `.Resume()` not called from ScannerCommon or DialogCommon
- [ ] Backward-compatible fallback maintained during migration

---

### Task P2-R4: CQRS Wiring – Agent Manager Queries
**Estimated Effort:** 1.5 days  
**Feature Parent:** #194 Architecture Modernization  
**Priority:** Medium

Replace 122 `Context.AppAgentMgr.*` query calls with the registered `IQueryHandler<GetActiveAgentNameQuery, string>` (and similar queries for other agent properties). An extension-method approach (wrapping the query handler) can reduce the number of individual changes.

**Acceptance Criteria:**
- [ ] `Context.AppAgentMgr.GetCurrentAgentName()` not called from Extension projects
- [ ] Extension method or helper wrapping the query handler is provided
- [ ] Tests demonstrate query resolution via DI

---

### Task P2-R5: EventBus Subscriber Migration
**Estimated Effort:** 1 day  
**Feature Parent:** #194 Architecture Modernization  
**Priority:** Medium

Migrate legacy C# `EventHandler` delegate subscriptions to `IEventBus.Subscribe<T>()` for the four event types that are now published by managers:

- Replace `ConfigurationReloadService.ConfigurationReloaded +=` → `Subscribe<ConfigurationReloadEvent>`
- Replace `PanelManager.EvtPanelChanged +=` (where applicable) → `Subscribe<PanelShowEvent>` / `Subscribe<PanelHideEvent>`
- Migrate subscribers in `JsonConfigurationLoader.cs:333`

**Acceptance Criteria:**
- [ ] At least 4 legacy delegate subscriptions replaced with EventBus equivalents
- [ ] Subscription tokens stored and disposed correctly (no memory leaks)
- [ ] Existing behaviour is preserved

---

### Task P2-R6: Add Missing EventBus Event Types
**Estimated Effort:** 0.5 day  
**Feature Parent:** #194 Architecture Modernization  
**Priority:** Low

Add the following event types to complete the event model and unblock future subscriber migration (Task P2-R5):

- `AppQuitEvent` → `PanelEvents.cs` (maps to `PanelManager.EvtAppQuit`)
- `CalibrationEndEvent` → `ActuatorEvents.cs` (maps to `ActuatorManager.EvtCalibrationEndNotify`)
- `DisplaySettingsChangedEvent` → `PanelEvents.cs` (maps to `PanelManager.EvtDisplaySettingsChanged`)
- `WordPredictionContextChangedEvent` → new `WordPredictionEvents.cs`

**Acceptance Criteria:**
- [ ] All 4 event types created, implementing `IEvent`
- [ ] Event types included in `ACATCore.Tests.Architecture` tests

---

### Task P2-R7: Schema Validation Auto-Wiring in JsonConfigurationLoader
**Estimated Effort:** 1 day  
**Feature Parent:** #191 Configuration System Enhancement  
**Priority:** High

`JsonSchemaValidator` exists but is not automatically invoked when `JsonConfigurationLoader<T>` loads a configuration file. This means callers can easily skip schema validation. To meet the Phase 2 criterion "all configuration files have schema validation":

1. Accept an optional `JsonSchemaValidator` (or a schema file path) in the `JsonConfigurationLoader<T>` constructor
2. Automatically call `validator.Validate(schemaName, filePath, out errors)` before deserialising
3. Log warnings (or throw in strict mode) if validation fails
4. Wire the six existing schemas to their corresponding loader usages in `ActuatorConfig`, `ThemeManager`, etc.

**Acceptance Criteria:**
- [ ] `JsonConfigurationLoader<T>` accepts optional schema validator
- [ ] Validation is called automatically on `Load()`
- [ ] Failure in non-strict mode logs a warning and continues
- [ ] Failure in strict mode throws a `ConfigurationValidationException`
- [ ] Tests cover both modes

---

### Task P2-R8: Expand JSON Schemas to All Configuration Types
**Estimated Effort:** 1.5 days  
**Feature Parent:** #191 Configuration System Enhancement  
**Priority:** Medium

Six schemas have been created. The following additional configuration types need schemas to fully satisfy "all configuration files have schema validation":

- `AppPreferences` / `GlobalPreferences`
- `WordPredictorSettings`
- `TTSEngineSettings`
- `AgentConfiguration`
- `UserProfile`

Create schemas in `schemas/json/` following the same pattern as the existing six. Document the schema versioning strategy in `schemas/README.md`.

**Acceptance Criteria:**
- [ ] Schemas created for all listed types
- [ ] Each schema has a `$schema`, `title`, `type`, `properties`, and `required` section
- [ ] `schemas/README.md` documents the versioning strategy

---

### Task P2-R9: Constructor Injection Rollout (Tier 2 Components)
**Estimated Effort:** 2 days  
**Feature Parent:** #190 Dependency Injection Infrastructure  
**Priority:** Low–Medium

The current DI adoption uses the service-locator pattern (`Context.GetManager<T>()`). To progress toward the "90%+ of components use DI" success criterion, constructor injection needs to be adopted in the Tier 2 components identified in the Interface Extraction Guide:

- Extract `IAnimationManager` from `AnimationManager` and register in DI
- Extract `IPreferencesManager` (if a manager-level coordinator exists) and register
- Extract `IConfigurationManager` and register
- Update `ExtensionInstantiator` to fully implement `IExtensionInstantiator` contract

**Acceptance Criteria:**
- [ ] Tier 2 components implement their interfaces
- [ ] Registered in `ServiceConfiguration.AddACATServices()`
- [ ] At least one consumer class per component uses constructor injection
- [ ] Interface Extraction Guide Tier 1 table updated

---

### Task P2-R10: Close and Reconcile Open Task Issues #209–#221
**Estimated Effort:** 0.5 day  
**Priority:** Administrative

Review each of the 13 open task issues against the code that was delivered:

- Close issues where work is complete (#212, #213, #214, #216, #217, #219, #220, #221)
- Close as duplicates (#209 → #212; #215 → #210; #218 → #211)
- Update #210 and #211 with the remaining scope identified in tasks P2-R7 and P2-R9 above, or link them to the new follow-up tasks

**Acceptance Criteria:**
- [ ] All 13 issues reviewed and either closed or updated with accurate remaining scope
- [ ] Epic issue #225 updated with a summary of remaining work

---

## Test Coverage Summary

| Test Project | Tests | Primary Coverage Area |
|---|---|---|
| ACATCore.Tests.Configuration | 217 | DI infrastructure, configuration loading, CQRS fakes |
| ACATCore.Tests.Performance | 45 | RuntimeMetrics, MemoryProfiler, RegressionDetector |
| ACATCore.Tests.Architecture | 29 | EventBus, CQRS objects, Repository pattern |
| ACATCore.Tests.Integration | 28 | Fresh install, XML migration, logging |
| ACATCore.Tests.Logging | 25 | Modern logging, legacy logger, performance |
| ACATCore.Tests.Shared | 11 | Shared utilities |
| **Total** | **355** | |

---

## Documentation Inventory

All major Phase 2 documentation is in place. Key documents:

| Document | Location |
|----------|----------|
| ACAT Modernization Plan | `ACAT_MODERNIZATION_PLAN.md` |
| Dependency Injection Guide | `DEPENDENCY_INJECTION_GUIDE.md` |
| Interface Extraction Guide | `docs/INTERFACE_EXTRACTION_GUIDE.md` |
| Configuration Enhancement Guide | `docs/CONFIGURATION_ENHANCEMENT_GUIDE.md` |
| Testing Infrastructure Guide | `TESTING_INFRASTRUCTURE.md` |
| Performance Dashboard | `docs/PERFORMANCE_DASHBOARD.md` |
| Architecture Implementation Status | `src/ARCHITECTURE_IMPLEMENTATION_STATUS.md` |
| CQRS Call-Site Wiring Guide | `src/SECTION_3_3_IMPLEMENTATION_GUIDE.md` |
| EventBus Quick Start | `src/EVENTBUS_QUICKSTART.md` |
| JSON Configuration Implementation | `JSON_CONFIGURATION_IMPLEMENTATION.md` |

---

## Risk & Dependency Notes

1. **CQRS migration is the highest-risk remaining item.** Migrating 196 call sites carries regression risk. A phased approach (entry points → base classes → extensions) as documented in `SECTION_3_3_IMPLEMENTATION_GUIDE.md` is strongly recommended.

2. **Extension DI wiring depends on hosting mechanism.** `ACATExtension` command handlers and scanner classes are loaded via `LayoutAttribute` reflection. The `AgentsCache` class already uses `ActivatorUtilities.CreateInstance`, but this needs to be consistently applied. Any extension DI work (P2-R2, P2-R3) should verify that the hosting mechanism passes the `IServiceProvider` to all loaded types.

3. **Phase 3 dependency.** Phase 3 (Async/Await Patterns) depends on Phase 2 DI infrastructure. The infrastructure is complete, but the incomplete call-site migration (P2-R1 through P2-R4) means Phase 3 components that need injected interfaces may still encounter mixed singleton/DI patterns.

---

## Recommended Next Steps

1. **Immediately:** Execute Task P2-R10 (close/reconcile open issues) to keep the issue tracker accurate.
2. **Sprint 1:** Deliver P2-R7 (schema validation auto-wiring) and P2-R1 (entry-point CQRS wiring) – these are the highest-value, lowest-risk items.
3. **Sprint 2:** Deliver P2-R2, P2-R3 (extension and actuator CQRS wiring) using the phased base-class strategy.
4. **Sprint 3:** Deliver P2-R4 (agent queries), P2-R5 (EventBus subscribers), P2-R6 (missing event types).
5. **Backlog:** P2-R8 (schema expansion) and P2-R9 (constructor injection rollout) as capacity allows.

Once P2-R1 through P2-R7 are complete, all Phase 2 success criteria will be met and Epic #225 can be formally closed.

---

**Document Owner:** ACAT Development Team  
**Review Cycle:** Per sprint  
**Related Documents:** `ACAT_MODERNIZATION_PLAN.md`, `src/ARCHITECTURE_IMPLEMENTATION_STATUS.md`
