# ACAT Interface Extraction Guide

**Version:** 1.0  
**Last Updated:** February 2026  
**Audience:** Developers contributing to the ACAT Architecture Modernization effort

---

## Table of Contents

1. [Overview](#overview)
2. [When to Extract an Interface](#when-to-extract-an-interface)
3. [Naming Conventions](#naming-conventions)
4. [Interface Location Standards](#interface-location-standards)
5. [Interface Design Guidelines](#interface-design-guidelines)
6. [Backward Compatibility Approach](#backward-compatibility-approach)
7. [Testing Requirements](#testing-requirements)
8. [Component Extraction Priority List](#component-extraction-priority-list)
9. [Interface Extraction Checklist](#interface-extraction-checklist)
10. [Examples](#examples)

---

## Overview

Interface extraction is a key step in the ACAT Architecture Modernization effort (Phase 2: Dependency Injection). Extracting interfaces from concrete classes enables:

- **Testability**: Swap real implementations with mocks in unit tests.
- **Decoupling**: Consumers depend on abstractions, not concrete types.
- **Extensibility**: Alternative implementations can be provided without changing call sites.
- **Dependency Injection**: Interfaces are the unit of registration in the DI container.

The patterns in this guide are consistent with the interfaces already extracted for the 10 core manager classes (see [DEPENDENCY_INJECTION_GUIDE.md](../DEPENDENCY_INJECTION_GUIDE.md)).

---

## When to Extract an Interface

Extract an interface when **all** of the following are true:

| Condition | Rationale |
|-----------|-----------|
| The class has **public methods or properties** consumed by other classes | Enables decoupling of consumers |
| The class is a **singleton manager or service** | Managers are the primary injection points in ACAT |
| The class will be **registered in the DI container** | Interfaces are required for DI registration |
| Unit tests for **consumers** of the class would benefit from mocking | Measurably improves test isolation |

**Do not** extract an interface when:

- The class is a pure data container (POCO/DTO) with no behavior.
- The class is `sealed` and has no plausible alternative implementations.
- The class is used only within the same assembly and is never injected.
- A suitable framework interface (e.g., `IDisposable`, `IEnumerable<T>`) already covers the contract.

---

## Naming Conventions

### Interface Name

Use the **`I` prefix** followed by the concrete class name:

```
ConcreteClassName  →  IConcreteClassName
```

Examples:

| Concrete Class | Interface |
|----------------|-----------|
| `ActuatorManager` | `IActuatorManager` |
| `ThemeManager` | `IThemeManager` |
| `SpellCheckManager` | `ISpellCheckManager` |
| `WordPredictionManager` | `IWordPredictionManager` |

### Factory Interface Name

Factories follow the same pattern with `Factory` appended:

```
IConcreteClassNameFactory
```

Examples: `IActuatorManagerFactory`, `IThemeManagerFactory`

### File Name

The interface file name must match the interface name exactly:

```
IActuatorManager.cs
IThemeManager.cs
IActuatorManagerFactory.cs
```

---

## Interface Location Standards

### Manager-Level Interfaces

Place the interface file **in the same directory** as the concrete manager class:

```
src/Libraries/ACATCore/
└── ActuatorManagement/
    ├── ActuatorManager.cs        ← concrete class
    ├── IActuatorManager.cs       ← manager interface (same folder)
    └── IActuatorManagerFactory.cs ← factory interface (same folder)
```

This is the established pattern for the 10 core DI-ready managers and must be followed for all new manager interfaces.

### Domain / Component Interfaces

For interfaces that define a capability contract within a subsystem (e.g., `IActuator`, `IActuatorSwitch`), place them in an **`Interfaces/` subdirectory** of the subsystem:

```
src/Libraries/ACATCore/
└── ActuatorManagement/
    ├── Interfaces/
    │   ├── IActuator.cs          ← domain interface
    │   └── IActuatorSwitch.cs    ← domain interface
    └── ...
```

### Summary Table

| Interface Type | Location |
|----------------|----------|
| Manager interface (`IXxxManager`) | Same directory as the concrete manager |
| Factory interface (`IXxxManagerFactory`) | Same directory as the concrete manager |
| Domain / component interface | `Interfaces/` subdirectory of the owning module |

---

## Interface Design Guidelines

### Include in the Interface

- All **public instance methods** that consumers call.
- All **public instance properties** that consumers read or write.
- All **public events** that consumers subscribe to.
- `IDisposable` if the concrete class implements it.

### Exclude from the Interface

- The static `Instance` property (singleton accessor). Consumers that need the singleton should call `Context.GetManager<IXxxManager>()` instead.
- Static helper or factory methods (extract to a separate factory interface if needed).
- Internal implementation details not relevant to consumers.

### XML Documentation

Carry the XML `<summary>` comment from the concrete member to the interface member so that IntelliSense works at the interface level:

```csharp
/// <summary>
/// Initializes and starts the actuator manager.
/// </summary>
bool Init();
```

---

## Backward Compatibility Approach

Extracting an interface must never require existing call sites to change. The following techniques maintain backward compatibility:

### 1. Concrete Class Implements the Interface

The concrete manager implements the new interface:

```csharp
// Before
public class ActuatorManager { ... }

// After
public class ActuatorManager : IActuatorManager { ... }
```

Existing code that references `ActuatorManager` directly continues to compile and work.

### 2. Static Properties Remain Unchanged

Context class static properties (`Context.AppActuatorManager`, etc.) are **not removed**. They continue to return the singleton concrete instance.

### 3. New DI-Aware Access Path

New code uses the interface via the DI container:

```csharp
// Preferred for new code
var manager = Context.GetManager<IActuatorManager>();

// Legacy code continues to work unchanged
var manager = Context.AppActuatorManager;
```

### 4. Service Registration Maps Both Types

In `ServiceConfiguration.AddACATServices()`, both the concrete type and the interface are registered so either can be resolved:

```csharp
services.AddSingleton<ActuatorManager>(provider => ActuatorManager.Instance);
services.AddSingleton<IActuatorManager>(provider =>
    provider.GetRequiredService<ActuatorManager>());
```

---

## Testing Requirements

Every extracted interface must be validated by tests in the appropriate test project under `src/Libraries/ACATCore.Tests.Configuration/`.

### Minimum Test Coverage

| Test | What to Verify |
|------|----------------|
| **Registration test** | `serviceProvider.GetService<IXxxManager>()` returns a non-null instance. |
| **Singleton test** | Resolving by concrete type and by interface type returns the **same** object reference. |
| **Factory test** | `IXxxManagerFactory.Create()` returns a valid instance. |
| **Mock substitution test** | A mock of `IXxxManager` can be injected and its methods called without any real implementation. |

### Example Test Pattern

```csharp
[TestClass]
public class MyManagerDiTests
{
    private IServiceProvider _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddACATServices();
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public void MyManager_CanBeResolvedByInterface()
    {
        var manager = _serviceProvider.GetService<IMyManager>();
        Assert.IsNotNull(manager);
    }

    [TestMethod]
    public void MyManager_InterfaceAndConcreteAreTheSameInstance()
    {
        var byInterface = _serviceProvider.GetService<IMyManager>();
        var byConcrete  = _serviceProvider.GetService<MyManager>();
        Assert.AreSame(byInterface, byConcrete);
    }
}
```

---

## Component Extraction Priority List

The table below ranks ACAT subsystems by their extraction priority. Priority is determined by:

- **DI readiness** – whether the manager is already registered in `ServiceConfiguration`.
- **Test impact** – how much unit-test coverage is blocked by the absence of an interface.
- **Consumer count** – how many other classes depend on this component.
- **Complexity** – lower complexity = lower migration risk.

### Tier 1 – Complete ✅ (Core Managers, Phase 2)

These interfaces were extracted as part of the Phase 2 DI infrastructure work and are fully registered in the DI container.

| Component | Interface | Factory | Status |
|-----------|-----------|---------|--------|
| ActuatorManager | `IActuatorManager` | `IActuatorManagerFactory` | ✅ Done |
| AgentManager | `IAgentManager` | `IAgentManagerFactory` | ✅ Done |
| TTSManager | `ITTSManager` | `ITTSManagerFactory` | ✅ Done |
| PanelManager | `IPanelManager` | `IPanelManagerFactory` | ✅ Done |
| ThemeManager | `IThemeManager` | `IThemeManagerFactory` | ✅ Done |
| WordPredictionManager | `IWordPredictionManager` | `IWordPredictionManagerFactory` | ✅ Done |
| SpellCheckManager | `ISpellCheckManager` | `ISpellCheckManagerFactory` | ✅ Done |
| AbbreviationsManager | `IAbbreviationsManager` | `IAbbreviationsManagerFactory` | ✅ Done |
| CommandManager | `ICommandManager` | `ICommandManagerFactory` | ✅ Done |
| AutomationEventManager | `IAutomationEventManager` | `IAutomationEventManagerFactory` | ✅ Done |

### Tier 2 – High Priority (Next extraction targets)

These components have a high consumer count and will unlock the most unit-test improvements.

| Component | Suggested Interface | Priority Rationale |
|-----------|---------------------|--------------------|
| AnimationManager | `IAnimationManager` | Already has domain interface; needs manager-level DI interface |
| UserControlManager | `IUserControlManager` | High UI-layer coupling; blocks panel unit tests |
| PreferencesManager | `IPreferencesManager` | Used widely; enables offline config tests |
| ExtensionInstantiator | `IExtensionInstantiator` | Already partially DI-aware; complete the contract |
| ConfigurationManager | `IConfigurationManager` | Central to JSON config system; improves config-layer tests |

### Tier 3 – Medium Priority

| Component | Suggested Interface | Priority Rationale |
|-----------|---------------------|--------------------|
| AuditLogger | `IAuditLogger` | Needed for audit-trail test isolation |
| Interpreter | `IInterpreter` | Enables scripting engine tests |
| UserManager | `IUserManager` | Multi-user profile testing |
| WidgetManager | `IWidgetManager` | UI widget isolation tests |

### Tier 4 – Low Priority / Future

| Component | Notes |
|-----------|-------|
| Individual Widget classes | Extract after `IWidgetManager` |
| Setup/Install utilities | Low test-coverage value |
| Migration tools | Standalone apps; DI not critical |

---

## Interface Extraction Checklist

Use this checklist for each component you extract. Check off each item before marking the extraction complete.

### Analysis

- [ ] Identified the concrete class to extract from.
- [ ] Verified the class is a manager or service (not a POCO).
- [ ] Listed all public instance members (methods, properties, events) that consumers use.
- [ ] Confirmed the class will be registered in the DI container.
- [ ] Checked that no existing interface already covers this contract.

### Design

- [ ] Interface name follows `IConcreteClassName` convention.
- [ ] Factory interface name follows `IConcreteClassNameFactory` convention.
- [ ] Interface file placed in the correct location (same folder as manager, or `Interfaces/` for domain types).
- [ ] Static `Instance` property excluded from the interface.
- [ ] `IDisposable` included if the concrete class implements it.
- [ ] XML `<summary>` documentation added to each interface member.

### Implementation

- [ ] Concrete class declaration updated to implement the new interface.
- [ ] Factory class created (`XxxManagerFactory.cs`) implementing `IXxxManagerFactory`.
- [ ] `ServiceConfiguration.AddACATServices()` updated to register both the interface and factory.
- [ ] `Context.ServiceProvider` resolves the interface without error.
- [ ] Existing static accessor (`Context.AppXxxManager`) still compiles and returns the correct instance.

### Testing

- [ ] Test class created in `ACATCore.Tests.Configuration`.
- [ ] Registration test: interface resolves to a non-null instance.
- [ ] Singleton test: interface and concrete type resolve to the same instance.
- [ ] Factory test: factory creates a valid instance.
- [ ] Mock substitution test: a mock can be injected in place of the real implementation.
- [ ] All new tests pass (`dotnet test`).

### Documentation

- [ ] Interface and its members have XML doc comments.
- [ ] `DEPENDENCY_INJECTION_GUIDE.md` manager table updated with the new entry.
- [ ] This guide's **Component Extraction Priority List** updated (move component to Tier 1).
- [ ] PR description references this checklist and the relevant issue.

### Review

- [ ] Code reviewed by at least one other team member.
- [ ] Architecture review sign-off obtained (for Tier 2+ components).
- [ ] No unintended breaking changes in public API surface.

---

## Examples

### Example 1: Extracting `IAnimationManager`

**Step 1 – Create the interface file**

`src/Libraries/ACATCore/AnimationManagement/IAnimationManager.cs`

```csharp
////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.AnimationManagement
{
    /// <summary>
    /// Interface for AnimationManager to support dependency injection.
    /// Manages animation sequences for scanner panels.
    /// </summary>
    public interface IAnimationManager : IDisposable
    {
        /// <summary>
        /// Starts the animation engine.
        /// </summary>
        bool Start();

        /// <summary>
        /// Stops the animation engine.
        /// </summary>
        void Stop();

        // ... remaining public members
    }
}
```

**Step 2 – Update concrete class**

```csharp
public class AnimationManager : IAnimationManager
{
    // existing implementation unchanged
}
```

**Step 3 – Register in ServiceConfiguration**

```csharp
services.AddSingleton<AnimationManager>(provider => AnimationManager.Instance);
services.AddSingleton<IAnimationManager>(provider =>
    provider.GetRequiredService<AnimationManager>());
services.AddSingleton<IAnimationManagerFactory, AnimationManagerFactory>();
```

---

### Example 2: Using the interface in a consumer

**Before (tightly coupled)**

```csharp
public class MyPanel
{
    private void StartAnimation()
    {
        AnimationManager.Instance.Start();
    }
}
```

**After (DI-aware, using interface)**

```csharp
public class MyPanel
{
    private readonly IAnimationManager _animationManager;

    public MyPanel(IAnimationManager animationManager)
    {
        _animationManager = animationManager;
    }

    private void StartAnimation()
    {
        _animationManager.Start();
    }
}
```

**Backward-compatible alternative (no constructor change required)**

```csharp
private void StartAnimation()
{
    var animationManager = Context.GetManager<IAnimationManager>();
    animationManager?.Start();
}
```

---

## Related Documents

- [DEPENDENCY_INJECTION_GUIDE.md](../DEPENDENCY_INJECTION_GUIDE.md) – DI infrastructure overview and registration patterns.
- [ACAT_MODERNIZATION_PLAN.md](../ACAT_MODERNIZATION_PLAN.md) – Overall architecture modernization roadmap.
- [TESTING_INFRASTRUCTURE.md](../TESTING_INFRASTRUCTURE.md) – Test project structure and conventions.
- [LOGGING_MIGRATION_GUIDE.md](../LOGGING_MIGRATION_GUIDE.md) – Parallel guide for the logging modernization effort.

---

**Document Owner:** ACAT Architecture Team  
**Last Review:** February 2026  
**Next Review:** After Tier 2 extraction is complete
