# ACAT Modernization Plan

**Last Updated**: February 21, 2026  
**Status**: Phase 2 DI Infrastructure In Progress 🔄  
**Version**: 3.1

---

## Executive Summary

The ACAT Modernization Plan is a comprehensive initiative to modernize the ACAT codebase by adopting modern .NET development practices. This multi-phase project aims to:

1. **Phase 1**: Logging Infrastructure & JSON Configuration (✅ **COMPLETE**)
2. **Phase 2**: Dependency Injection & Service Architecture (✅ **COMPLETE**)
3. **Phase 3**: Async/Await Patterns & Performance (📋 **FUTURE**)
4. **Phase 4**: UI Modernization & WinUI 3 Migration (📋 **FUTURE**)

### Current Status
- ✅ **Phase 1 Complete**: All 12 tickets delivered on schedule
- ✅ **Phase 2 Architecture Modernization Complete**: Interface extraction, event system, CQRS, and repository pattern delivered
- 🔄 **Phase 2 DI Infrastructure In Progress**: Service container, Context class refactor, interface extraction (Issues #209–#216)
- 📊 **Next**: Complete remaining DI infrastructure tasks, then Phase 3 planning
- 🎯 **Focus**: Dependency injection throughout ACAT core

---

## Phase 1: Foundation ✅ COMPLETE

**Duration**: 4 weeks  
**Timeline**: January 15 - February 11, 2026  
**Tickets**: Issues #1 through #12  
**Status**: ✅ **All objectives met**

### Objectives

1. ✅ **Modernize Logging Infrastructure**
   - Migrate from legacy `Log` class to `Microsoft.Extensions.Logging`
   - Implement structured logging with categories and levels
   - Add file-based logging with automatic rotation
   - Integrate `ILogger<T>` throughout codebase

2. ✅ **JSON Configuration System**
   - Create robust JSON configuration loading with validation
   - Implement JSON schemas for top configuration types
   - Maintain backward compatibility with XML
   - Provide migration tool for users

3. ✅ **Comprehensive Testing**
   - Unit tests for logging and configuration systems
   - Integration tests for end-to-end scenarios
   - Performance validation
   - No regressions in existing functionality

4. ✅ **Documentation & Knowledge Transfer**
   - User guides for logging and configuration
   - Developer documentation
   - Migration guides
   - Retrospective and lessons learned

### Tickets Completed

#### Week 1: Logging Infrastructure
- **Issue #1**: Set up modern logging infrastructure
  - Added `Microsoft.Extensions.Logging` packages
  - Created `LoggingConfiguration` helper class
  - Configured file-based logging with rotation
  - Status: ✅ Complete

- **Issue #2**: Sample log migration (Proof of Concept)
  - Converted 3 sample files as proof of concept
  - Validated patterns and approach
  - Created conversion guidelines
  - Status: ✅ Complete

- **Issue #3**: Update DI entry points
  - Updated 5 application entry points with modern logging
  - Integrated `ILoggerFactory` in startup code
  - Configured logging for each application
  - Status: ✅ Complete

#### Week 2: Logging Rollout & Testing
- **Issue #4**: Create logging tests
  - Created 32 unit tests for logging system
  - Tested legacy, cached, and modern logging
  - Performance and concurrency tests
  - Status: ✅ Complete (32/32 tests passing)

- **Issue #5**: Logging cleanup and optimization
  - Refactored logging code for consistency
  - Optimized performance (50-70ms for 10K messages)
  - Removed dead code
  - Status: ✅ Complete

#### Week 3: Configuration Analysis & Setup
- **Issue #6**: XML configuration analysis
  - Analyzed existing XML configuration files
  - Identified top 5 config types for migration
  - Documented configuration patterns
  - Status: ✅ Complete

- **Issue #7**: Create JSON schemas
  - Created JSON schemas for 5 configuration types:
    - ActuatorSettings
    - Theme (with ColorSchemes)
    - PanelConfig
    - Abbreviations
    - Pronunciations
  - Added FluentValidation validators
  - Status: ✅ Complete

- **Issue #8**: Build migration tool
  - Created `ConfigMigrationTool` console application
  - Supports XML-to-JSON migration
  - Batch processing with progress reporting
  - Backup creation for safety
  - Status: ✅ Complete

#### Week 4: Configuration Implementation & Testing
- **Issue #9**: Implement JSON configuration loading
  - Created `JsonConfigurationLoader<T>` generic loader
  - Integrated with existing configuration system
  - Maintained backward compatibility
  - Status: ✅ Complete

- **Issue #10**: Configuration documentation
  - User guide for JSON configuration
  - Developer documentation
  - Migration guide
  - Quick reference card
  - Status: ✅ Complete

- **Issue #11**: Integration testing
  - Created 31 integration tests
  - Tested fresh install, migration, logging, validation
  - All acceptance criteria met
  - Status: ✅ Complete (31/31 tests passing)

- **Issue #12**: Phase 1 documentation & handoff
  - Phase 1 completion report
  - Retrospective with lessons learned
  - This modernization plan
  - Phase 2 kickoff materials
  - Status: ✅ Complete

### Metrics Summary

#### Code Changes
- **Files modified**: 217+ files
- **Lines of code changed**: ~10,000 lines
- **Log calls converted**: 3,891 calls
- **Tests added**: 72 tests
  - 32 logging unit tests
  - 9 configuration unit tests
  - 31 integration tests
- **New projects created**: 3 test projects

#### Performance Results
- **Logging overhead**: 50-70ms for 10,000 messages (target: <100ms) ✅
- **Configuration load time**: 10-20ms per file ✅
- **Memory footprint**: <5 MB increase ✅
- **Startup time impact**: <100ms ✅

#### Quality Metrics
- **Test pass rate**: 100% (72/72 passing)
- **Regressions**: 0
- **Critical bugs**: 0
- **Performance targets**: All exceeded

### Key Deliverables

#### Code Artifacts
1. Modern logging infrastructure (`LoggingConfiguration.cs`)
2. JSON configuration loader (`JsonConfigurationLoader<T>`)
3. JSON schemas for 5 configuration types
4. FluentValidation validators
5. ConfigMigrationTool application
6. 3 comprehensive test projects

#### Documentation
1. LOGGING_MIGRATION_README.md (9.4 KB)
2. LOGGING_MIGRATION_GUIDE.md (6.5 KB)
3. JSON_CONFIGURATION_IMPLEMENTATION.md (9.6 KB)
4. docs/JSON_CONFIGURATION_GUIDE.md (User guide)
5. docs/JSON_CONFIGURATION_DEVELOPER_GUIDE.md
6. docs/JSON_CONFIGURATION_QUICK_REFERENCE.md
7. docs/JSON_CONFIGURATION_MIGRATION.md
8. TESTING_INFRASTRUCTURE.md
9. TESTING_MIGRATION_GUIDE.md
10. This modernization plan

### Success Criteria - All Met ✅

| Criterion | Target | Actual | Status |
|-----------|--------|--------|--------|
| All logging uses ILogger | 100% | 100% | ✅ |
| JSON configs loadable | Yes | Yes | ✅ |
| Migration tool functional | Yes | Yes | ✅ |
| Performance impact | <5% | <2% | ✅ |
| Test coverage | >80% | ~85% | ✅ |
| No regressions | 0 | 0 | ✅ |
| Documentation complete | Yes | Yes | ✅ |
| All tests passing | 100% | 100% | ✅ |

### Lessons Learned

#### What Worked Well
1. **Analysis-first approach** - Python analysis tool enabled accurate estimation
2. **Incremental rollout** - Phased conversion minimized risk
3. **Comprehensive testing** - Early tests caught issues before production
4. **Generic utilities** - Reusable components across config types
5. **Backward compatibility** - Maintained XML support eased migration

#### Challenges Overcome
1. **PowerShell dependency** - Required Windows-only CI workflow
2. **File locking on Windows** - Added retry logic in test helpers
3. **Multiple entry points** - Created reusable `LoggingConfiguration` helper
4. **Case sensitivity** - Created symbolic links for Designer.cs files

#### Best Practices Established
1. Always use `ILogger<T>` for type-specific loggers
2. Always validate configurations on load and save
3. Always provide fallback to defaults
4. Always include user-friendly error messages
5. Always create backups before migration

---

## Phase 2: Architecture Modernization ✅ COMPLETE

**Duration**: 2-3 weeks  
**Timeline**: February 2026  
**Status**: ✅ **Complete**

### Architecture Modernization Sub-tasks

All four sub-tasks (Issues #202–#205) have been delivered:

#### Issue #202: Interface Extraction Strategy ✅
- Interface extraction guidelines documented in `docs/INTERFACE_EXTRACTION_GUIDE.md`
- Naming conventions (I-prefix), directory layout, backward-compatibility strategy defined
- Component priority list (Tier 1 extracted, Tier 2 planned)
- Migration checklist and testing requirements established

#### Issue #203: Event System Implementation ✅
- `IEventBus` and `IEvent` interfaces defined in `Libraries/ACATCore/EventManagement/`
- `EventBus` implementation: thread-safe, weak-reference subscriptions, prevents memory leaks
- Subscription tokens (`ISubscriptionToken`) support `Dispose()` for automatic cleanup
- Built-in event types: `PanelShownEvent`, `PanelHiddenEvent`, `ActuatorSwitchEvent`, `ConfigurationReloadedEvent`, `AgentContextChangedEvent`

#### Issue #204: Command/Query Separation ✅
- CQRS marker interfaces in `Libraries/ACATCore/Patterns/CQRS/`:
  - `ICommand`, `ICommand<TResult>` – state-changing operations
  - `IQuery<TResult>` – read-only operations
  - `ICommandHandler<TCommand>`, `ICommandHandler<TCommand,TResult>` – command handlers
  - `IQueryHandler<TQuery,TResult>` – query handlers
- Sample implementations: `ShowPanelCommand`, `HidePanelCommand`, `HandleActuatorSwitchCommand`, `GetActivePanelQuery`, `GetAllPanelNamesQuery`, `GetConfigurationValueQuery`

#### Issue #205: Repository Pattern for Data Access ✅
- `IRepository<TEntity,TKey>` interface in `Libraries/ACATCore/DataAccess/`
- `RepositoryBase<TEntity,TKey>` abstract base with lazy-load caching
- `ConfigurationRepository` – key/value configuration data with `GetValue`/`SetValue` helpers
- `PreferencesRepository` – wraps XML-persisted `PreferencesBase` objects

### Tests ✅
- `ACATCore.Tests.Architecture` project with 30 MSTest unit tests covering:
  - Event bus publish/subscribe, unsubscribe, disposal, null-guards
  - CQRS command/query objects and handler contracts
  - Repository CRUD operations, null-guards, and default behavior
- Added to CI workflow (`test.yml`)

### Key Deliverables

| Artifact | Location |
|----------|----------|
| Interface extraction guide | `docs/INTERFACE_EXTRACTION_GUIDE.md` |
| Event bus interfaces | `Libraries/ACATCore/EventManagement/IEvent.cs`, `IEventBus.cs` |
| Event bus implementation | `Libraries/ACATCore/EventManagement/EventBus.cs` |
| CQRS interfaces | `Libraries/ACATCore/Patterns/CQRS/` |
| Repository interfaces | `Libraries/ACATCore/DataAccess/IRepository.cs` |
| Repository base & implementations | `Libraries/ACATCore/DataAccess/RepositoryBase.cs`, `ConfigurationRepository.cs`, `PreferencesRepository.cs` |
| Architecture unit tests | `Libraries/ACATCore.Tests.Architecture/` |

---

## Phase 2: DI Infrastructure 🔄 IN PROGRESS

**Duration**: 3 weeks  
**Timeline**: February 2026  
**Status**: 🔄 **In Progress**  
**Issues**: #209 (Setup Service Container), #210 (Refactor Context Class), #212–#216

### DI Infrastructure Sub-tasks

#### Issue #209 / #212: Setup Service Container ✅
- `Microsoft.Extensions.DependencyInjection` integrated into all core projects
- `ServiceCollectionExtensions.cs` created with per-module registration methods (`AddActuatorManagement()`, `AddAgentManagement()`, etc.)
- `ServiceConfiguration.cs` provides convenience methods: `AddACATServices()`, `AddACATInfrastructure()`, `CreateServiceProvider()`
- All five application entry points (`ACATApp`, `ACATTalk`, `ACATConfig`, `ACATConfigNext`, `ACATWatch`) updated to call `InitializeDependencyInjection()`

#### Issue #210 / #215: Refactor Context Class ✅
- All `AppXxx` static manager properties now resolve from the DI container when `Context.ServiceProvider` is set, with automatic fallback to singleton instances
- `Context.GetManager<TInterface>()` public method for interface-based resolution
- Thread-safe `ResolveManager<T>()` private helper captures provider reference once to prevent TOCTOU races
- Gradual migration adapter: **no changes required in the 383+ existing `Context.App*` call sites** – DI takes effect automatically
- `ServiceProvider` is effectively Context's scoped lifetime bridge to the DI container
- Tests: `ContextDependencyInjectionTests.cs` (6 tests), `ContextThreadSafetyTests.cs` (6 tests)

#### Issue #213: Extract Core Interfaces ✅
All manager interfaces created and registered in DI:
- `IActuatorManager`, `IAgentManager`, `ITTSManager`, `IPanelManager`
- `IThemeManager`, `IWordPredictionManager`, `ISpellCheckManager`
- `IAbbreviationsManager`, `ICommandManager`, `IAutomationEventManager`

#### Issues #214, #216: Factory Patterns & Extension Loading 🔄
- Factory interfaces and implementations completed for all managers
- Extension loading with DI: planned

### Tests for DI Infrastructure
- `ACATCore.Tests.Configuration` project with tests covering:
  - `ServiceConfigurationTests.cs` – service registration and lifetime
  - `ManagerFactoryTests.cs` – factory pattern resolution
  - `ContextDependencyInjectionTests.cs` – Context DI integration
  - `ContextThreadSafetyTests.cs` – concurrent access safety
  - `FactoryRegistrationTests.cs` – factory singleton behavior
  - `ServiceLifetimeTests.cs` – singleton lifetime verification

### Key Deliverables

| Artifact | Location |
|----------|----------|
| Service container setup | `Libraries/ACATCore/DependencyInjection/ServiceCollectionExtensions.cs` |
| Service configuration | `Libraries/ACATCore/Utility/ServiceConfiguration.cs` |
| Context DI bridge | `Libraries/ACATCore/PanelManagement/Context.cs` |
| Manager interfaces | Each manager's `Interfaces/` subdirectory |
| DI guide | `DEPENDENCY_INJECTION_GUIDE.md` |
| DI tests | `Libraries/ACATCore.Tests.Configuration/` |

### Success Criteria

| Criterion | Target | Status |
|-----------|--------|--------|
| Service container configured | Yes | ✅ Complete |
| All managers registered in DI | Yes | ✅ Complete |
| Context properties use DI | Yes | ✅ Complete |
| Manager interfaces defined | All major managers | ✅ Complete |
| Factory pattern implemented | All managers | ✅ Complete |
| Context.GetManager<T>() | Yes | ✅ Complete |
| Backward compatibility | Yes | ✅ Complete |
| Tests for DI | Yes | ✅ Complete |
| No breaking changes | Yes | ✅ Complete |
| Documentation complete | Yes | ✅ Complete |

---

## Phase 3: Async/Await Patterns 📋 FUTURE

**Duration**: 4-6 weeks (estimated)  
**Timeline**: TBD (after Phase 2)  
**Status**: 📋 **Future planning**

### Preliminary Objectives

1. **Async Infrastructure**
   - Identify blocking I/O operations
   - Convert to async/await patterns
   - Update service interfaces to support async

2. **Performance Optimization**
   - Implement async file I/O
   - Async network operations
   - Parallel processing where appropriate
   - Performance benchmarking

3. **Threading Model**
   - Review UI thread safety
   - Implement proper synchronization
   - Use `ConfigureAwait` appropriately

### Dependencies
- ✅ Phase 1 complete
- ⏸️ Phase 2 complete (DI infrastructure needed)

---

## Phase 4: UI Modernization 📋 FUTURE

**Duration**: 8-12 weeks (estimated)  
**Timeline**: TBD (after Phase 3)  
**Status**: 📋 **Future planning**

### Preliminary Objectives

1. **WinUI 3 Migration**
   - Migrate from Windows Forms to WinUI 3
   - Modern XAML-based UI
   - Better accessibility support
   - Improved theming

2. **MVVM Architecture**
   - Implement MVVM pattern
   - Data binding for UI updates
   - Command pattern for user actions

3. **Modern Controls**
   - Use modern UI controls
   - Implement responsive layouts
   - Enhance user experience

### Dependencies
- ✅ Phase 1 complete
- ⏸️ Phase 2 complete
- ⏸️ Phase 3 complete
- 📋 Windows App SDK / WinUI 3 readiness

---

## Overall Project Timeline

```
Phase 1: Foundation
├─ Week 1: Logging Infrastructure (Issues #1-3)
├─ Week 2: Logging Testing (Issues #4-5)
├─ Week 3: Configuration Analysis (Issues #6-8)
└─ Week 4: Configuration Implementation (Issues #9-12)
Status: ✅ Complete (February 11, 2026)

Phase 2: Dependency Injection ⏸️
├─ Week 1-2: Analysis & Infrastructure (Issues #13-14)
├─ Week 3-4: Core Services (Issues #15-17)
├─ Week 5-6: Service Implementation (Issues #18-20)
└─ Week 7-8: Testing & Documentation (Issues #21-24)
Status: ⏸️ Planned (Start TBD)

Phase 3: Async/Await Patterns 📋
└─ TBD (4-6 weeks after Phase 2)

Phase 4: UI Modernization 📋
└─ TBD (8-12 weeks after Phase 3)
```

---

## Key Decisions & Architecture

### Technology Choices

#### Logging (Phase 1) ✅
- **Framework**: Microsoft.Extensions.Logging
- **File logging**: Serilog with rolling file sink
- **Rationale**: Industry standard, flexible, performant

#### Configuration (Phase 1) ✅
- **Format**: JSON with JSON Schema
- **Validation**: FluentValidation
- **Backward compatibility**: Maintained XML support
- **Rationale**: Modern, validated, great tooling support

#### Dependency Injection (Phase 2) ⏸️
- **Framework**: Microsoft.Extensions.DependencyInjection
- **Pattern**: Constructor injection preferred
- **Legacy support**: Service locator for existing code
- **Rationale**: Standard .NET DI container, well-tested

### Architectural Principles

1. **Backward Compatibility**
   - No breaking changes in public APIs
   - Maintain support for existing configurations
   - Gradual transition paths

2. **Test-Driven Development**
   - Write tests before or alongside implementation
   - Maintain high test coverage (>80%)
   - Integration tests for end-to-end scenarios

3. **Incremental Delivery**
   - Small, frequent changes
   - Continuous integration
   - Regular validation

4. **Documentation-First**
   - Document as you build
   - User guides and developer docs
   - Architecture decision records (ADRs)

5. **Performance-Conscious**
   - Performance benchmarks for all changes
   - Target <5% overhead
   - Validate with real-world scenarios

---

## Project Governance

### Stakeholders
- **Technical Lead**: Architecture and technical decisions
- **Project Manager**: Timeline and resource management
- **QA Lead**: Testing strategy and validation
- **Product Owner**: Requirements and priorities

### Decision-Making Process
1. Proposal: Technical team proposes approach
2. Review: Architecture team reviews proposal
3. Approval: Stakeholders approve direction
4. Implementation: Team executes with oversight
5. Validation: QA validates completion

### Communication Cadence
- **Daily standups**: Progress and blockers
- **Weekly updates**: Status to stakeholders
- **Bi-weekly demos**: Show completed work
- **Phase retrospectives**: Lessons learned

---

## Risk Management

### Phase 1 Risks (Resolved) ✅
1. ~~Scope estimation~~ - Resolved with analysis tool
2. ~~Multiple entry points~~ - Resolved with helper classes
3. ~~Windows-only builds~~ - Resolved with Windows CI
4. ~~File locking~~ - Resolved with retry logic

### Phase 2 Risks (Active) ⚠️
1. **Larger scope than Phase 1**
   - Impact: Timeline may extend
   - Mitigation: Break into sub-phases
   - Owner: Project Manager

2. **API changes required**
   - Impact: Potential breaking changes
   - Mitigation: Adapter pattern for compatibility
   - Owner: Architecture team

3. **Team bandwidth**
   - Impact: Resource constraints
   - Mitigation: Secure dedicated resources
   - Owner: Project Manager

### Overall Project Risks
1. **Technology changes**
   - Microsoft may update frameworks
   - Keep up with .NET releases
   - Plan for framework migrations

2. **Team turnover**
   - Knowledge loss if team members leave
   - Maintain comprehensive documentation
   - Cross-training and pair programming

3. **Scope creep**
   - Additional requirements emerge
   - Strict change control process
   - Prioritize based on value

---

## Success Metrics

### Phase 1 Metrics ✅
- ✅ All logging uses modern ILogger (100%)
- ✅ JSON configuration system working (100%)
- ✅ Migration tool functional (100%)
- ✅ Tests passing (100%)
- ✅ Performance targets met (exceeded by 30-50%)
- ✅ Documentation complete (100%)

### Overall Project Metrics (Cumulative)
- **Code modernization**: 30% complete (Phase 1 done)
- **Test coverage**: ~85% for modernized code
- **Performance**: <2% overhead (target: <5%)
- **Documentation**: Comprehensive and up-to-date
- **Developer satisfaction**: High (based on retrospective)

### Target by End of Phase 2
- Code modernization: 65% complete
- Test coverage: >80%
- Performance: <3% overhead
- All major services using DI

---

## Next Steps

### Immediate (Next 2 Weeks)
1. ✅ Complete Phase 1 documentation
2. ✅ Conduct Phase 1 retrospective
3. ⏸️ Present Phase 1 results to stakeholders
4. ⏸️ Get approval for Phase 2

### Short Term (Next Month)
1. ⏸️ Schedule Phase 2 kickoff meeting
2. ⏸️ Begin Phase 2 analysis (Issue #13)
3. ⏸️ Assign Phase 2 resources
4. ⏸️ Set up Phase 2 project board

### Medium Term (Next Quarter)
1. ⏸️ Complete Phase 2 implementation
2. ⏸️ Begin Phase 3 planning
3. ⏸️ Evaluate technology landscape
4. ⏸️ Update roadmap based on progress

---

## Resources & References

### Documentation
- [Documentation Index](INDEX.md)
- [Logging Migration Guide](LOGGING_MIGRATION_GUIDE.md)
- [JSON Configuration Guide](docs/JSON_CONFIGURATION_GUIDE.md)
- [Developer Guide](docs/JSON_CONFIGURATION_DEVELOPER_GUIDE.md)

### External Resources
- [Microsoft.Extensions.Logging Docs](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging)
- [Microsoft.Extensions.DependencyInjection Docs](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [FluentValidation Documentation](https://fluentvalidation.net/)
- [JSON Schema Specification](https://json-schema.org/)

### Tools
- `log_migration_tool.py` - Analysis tool for logging migration
- `ConfigMigrationTool` - XML to JSON migration tool
- Visual Studio 2022 - Primary IDE
- .NET 4.8.1 - Target framework

---

## Appendix A: Ticket Dependencies

### Phase 1 Dependencies (Complete)
```
#1 (Logging Setup)
  ├─→ #2 (Log Migration)
  │    └─→ #3 (DI Entry Points)
  │         └─→ #4 (Tests)
  │              └─→ #5 (Cleanup)
  │
#6 (XML Analysis)
  └─→ #7 (JSON Schemas)
       └─→ #8 (Migration Tool)
            └─→ #9 (JSON Loading)
                 └─→ #10 (Documentation)
                      └─→ #11 (Integration Tests)
                           └─→ #12 (Handoff)
```

### Phase 2 Dependencies (Planned)
```
#13 (DI Analysis)
  └─→ #14 (DI Infrastructure)
       ├─→ #15 (Configuration Service)
       ├─→ #16 (Actuator Services)
       ├─→ #17 (Theme Services)
       ├─→ #18 (Scanner Services)
       └─→ #19 (Window Services)
            └─→ #20 (Additional Services)
                 └─→ #21 (DI Tests)
                      └─→ #22 (Integration Tests)
                           └─→ #23 (Documentation)
                                └─→ #24 (Handoff)
```

---

## Appendix B: Change Log

### Version 2.0 (2026-02-11)
- Updated with Phase 1 completion status
- Added Phase 1 metrics and lessons learned
- Detailed Phase 2 planning
- Added preliminary Phase 3 and Phase 4 outlines

### Version 1.0 (2026-01-15)
- Initial modernization plan created
- Phase 1 scope defined
- High-level roadmap established

---

**Document Owner**: ACAT Modernization Team  
**Last Review**: February 11, 2026  
**Next Review**: Phase 2 Kickoff (TBD)  
**Status**: Living Document - Updated per phase
