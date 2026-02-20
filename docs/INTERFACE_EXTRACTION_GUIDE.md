# Interface Extraction Guide

**Version**: 1.0  
**Status**: Active  
**Last Updated**: February 2026

---

## Overview

This guide establishes the standards and workflow for extracting interfaces from
concrete classes in the ACAT codebase.  Interface extraction is the foundation
for dependency injection, unit-testing with mocks, and the event-driven and
CQRS patterns introduced in Phase 2.

---

## When to Extract an Interface

Extract an interface when **any** of the following apply:

| Trigger | Rationale |
|---------|-----------|
| The class is injected as a dependency into another class | Enables swappable implementations and mock objects |
| The class manages a shared resource (manager, service, store) | Prevents tight coupling to a single implementation |
| The class will have more than one concrete implementation | Enforces the Open/Closed Principle |
| The class needs to be testable in isolation | Unit tests should depend on interfaces, not concretes |
| The class raises domain events consumed by unrelated subsystems | Decouples publishers from consumers |

Do **not** extract an interface for:

- Simple data objects / DTOs
- Private or `internal` utility helpers
- Sealed leaf classes with a single, clearly-terminal implementation

---

## Naming Conventions

| Rule | Example |
|------|---------|
| Prefix with `I` | `IPanelManager`, `IActuatorManager` |
| Use the same root noun as the class | `PanelManager` → `IPanelManager` |
| Avoid redundant words like `IInterface` or `IAbstract` | ❌ `IPanelManagerInterface` |
| Event args classes do **not** get an `I` prefix | `PanelShownEventArgs` |

---

## Directory and Namespace Layout

```
Libraries/ACATCore/
  PanelManagement/
    Interfaces/          ← interfaces that belong to PanelManagement
      IPanelManager.cs
      IScannerPanel.cs
  ActuatorManagement/
    Interfaces/          ← interfaces that belong to ActuatorManagement
      IActuatorManager.cs
  EventManagement/       ← cross-cutting event bus interfaces
    IEventBus.cs
    IEvent.cs
  Patterns/
    CQRS/                ← CQRS marker interfaces
      ICommand.cs
      IQuery.cs
  DataAccess/            ← repository interfaces
    IRepository.cs
```

Namespace pattern: `ACAT.Core.<Subsystem>[.Interfaces]`

---

## Extraction Checklist

Use this checklist every time you extract an interface:

- [ ] Identify all **public methods and properties** used by external callers
- [ ] Write the interface in the appropriate `Interfaces/` subfolder
- [ ] Keep only the **minimum required surface** in the interface
- [ ] Update the concrete class to implement the new interface
- [ ] Register the interface→implementation binding in the DI container
- [ ] Replace `new ConcreteClass()` call sites with constructor injection
- [ ] Add a `Mock<IMyInterface>` in `Mocks/ManagerMocks.cs` (test project)
- [ ] Write at least one unit test that depends only on the interface

---

## Component Priority List

The following components were identified as the highest-priority targets for
interface extraction in Phase 2.

### Tier 1 – Already Extracted ✅

| Interface | Concrete Class | Location |
|-----------|---------------|----------|
| `IPanelManager` | `PanelManager` | `PanelManagement/` |
| `IAgentManager` | `AgentManager` | `AgentManagement/` |
| `IActuatorManager` | `ActuatorManager` | `ActuatorManagement/` |
| `IWordPredictionManager` | `WordPredictionManager` | `WordPredictorManagement/` |
| `ITTSManager` | `TTSManager` | `TTSManagement/` |
| `IEventBus` | `EventBus` | `EventManagement/` |
| `IRepository<T,K>` | `RepositoryBase<T,K>` | `DataAccess/` |

### Tier 2 – Planned

| Candidate Class | Proposed Interface | Rationale |
|-----------------|--------------------|-----------|
| `ThemeManager` | `IThemeManager` | Theme switching; testable in isolation |
| `ConfigurationReloadService` | `IConfigurationReloadService` | Swap for test doubles |
| `ScannerCommon` | `IScannerCommon` | Reduce coupling in panel management |

---

## Migration Strategy

1. **Green-field extraction** – For new code, always write the interface first
   (interface-first design).
2. **Refactor in place** – For existing classes:
   a. Create the interface in the `Interfaces/` subfolder.
   b. Add `: IMyInterface` to the concrete class declaration.
   c. The compiler will flag any missing members – implement them.
   d. Update callers to accept `IMyInterface` instead of the concrete type.
   e. Register in DI (see `DEPENDENCY_INJECTION_GUIDE.md`).
3. **Adapter pattern** – When an existing public API must not change, create an
   adapter that wraps the legacy concrete and implements the new interface.

---

## Backward Compatibility

- Extracting an interface is a **non-breaking change** at the binary level as long
  as the concrete class still exists.
- Changing a constructor parameter from `ConcreteClass` to `IMyInterface` is a
  **source-level breaking change** for direct callers.  Use the adapter pattern or
  provide a `[Obsolete]` overload as a migration bridge.

---

## Testing Requirements

Every extracted interface **must** have a corresponding mock in the test project:

```csharp
// Mocks/ManagerMocks.cs
public static Mock<IMyManager> CreateMyManager()
{
    var mock = new Mock<IMyManager>();
    mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(true);
    return mock;
}
```

And at least one test that substitutes the mock via constructor injection:

```csharp
[Fact]
public void MyConsumer_UsesInterface_CalledOnce()
{
    var mock = MockFactory.CreateMyManager();
    var sut = new MyConsumer(mock.Object);
    sut.DoWork();
    mock.Verify(m => m.PerformAction(), Times.Once);
}
```

---

## Related Documents

- [Dependency Injection Guide](../DEPENDENCY_INJECTION_GUIDE.md)
- [Architecture Modernization Plan](../ACAT_MODERNIZATION_PLAN.md)
- [Event System](../src/Libraries/ACATCore/EventManagement/IEventBus.cs)
- [CQRS Patterns](../src/Libraries/ACATCore/Patterns/CQRS/)
- [Repository Pattern](../src/Libraries/ACATCore/DataAccess/)
