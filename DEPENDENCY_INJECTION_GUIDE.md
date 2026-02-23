# ACAT Dependency Injection Infrastructure

## Overview

This document describes the comprehensive dependency injection (DI) infrastructure implemented in ACAT Phase 2. The implementation uses Microsoft.Extensions.DependencyInjection throughout ACAT core and extensions to improve testability, reduce coupling, and provide better lifetime management.

## Key Components

### 1. ServiceConfiguration Class

**Location:** `src/Libraries/ACATCore/Utility/ServiceConfiguration.cs`

The `ServiceConfiguration` class is the central configuration point for dependency injection in ACAT. It follows the same pattern as `LoggingConfiguration` and provides extension methods for `IServiceCollection`.

#### Key Methods:

- **`AddACATServices()`** - Registers all ACAT manager services with their interfaces and factories
- **`AddACATInfrastructure()`** - Registers both logging and core services (convenience method)
- **`CreateServiceProvider()`** - Creates a fully configured service provider
- **`CreateServiceProvider(ILoggerFactory)`** - Creates service provider with custom logger factory

#### Usage Example:

```csharp
// Simple setup with all infrastructure
var services = new ServiceCollection();
services.AddACATInfrastructure();
var serviceProvider = services.BuildServiceProvider();
Context.ServiceProvider = serviceProvider;

// Or use the convenience method
var serviceProvider = ServiceConfiguration.CreateServiceProvider();
Context.ServiceProvider = serviceProvider;
```

### 2. Manager Interfaces

All core managers now have corresponding interfaces for improved testability:

| Manager | Interface | Location |
|---------|-----------|----------|
| ActuatorManager | IActuatorManager | ActuatorManagement/ |
| AgentManager | IAgentManager | AgentManagement/ |
| TTSManager | ITTSManager | TTSManagement/ |
| PanelManager | IPanelManager | PanelManagement/ |
| ThemeManager | IThemeManager | ThemeManagement/ |
| WordPredictionManager | IWordPredictionManager | WordPredictorManagement/ |
| SpellCheckManager | ISpellCheckManager | SpellCheckManagement/ |
| AbbreviationsManager | IAbbreviationsManager | AbbreviationsManagement/ |
| CommandManager | ICommandManager | CommandManagement/ |
| AutomationEventManager | IAutomationEventManager | Utility/ |

#### Interface Design:

Each interface extracts all public methods, properties, and events from its corresponding manager (excluding the static `Instance` property). All interfaces that implement `IDisposable` in their concrete classes also include it in the interface.

### 3. Factory Pattern

Each manager has a corresponding factory interface and implementation for advanced scenarios:

| Factory Interface | Factory Implementation |
|-------------------|------------------------|
| IActuatorManagerFactory | ActuatorManagerFactory |
| IAgentManagerFactory | AgentManagerFactory |
| ITTSManagerFactory | TTSManagerFactory |
| IPanelManagerFactory | PanelManagerFactory |
| IThemeManagerFactory | ThemeManagerFactory |
| IWordPredictionManagerFactory | WordPredictionManagerFactory |
| ISpellCheckManagerFactory | SpellCheckManagerFactory |
| IAbbreviationsManagerFactory | AbbreviationsManagerFactory |
| ICommandManagerFactory | CommandManagerFactory |
| IAutomationEventManagerFactory | AutomationEventManagerFactory |

#### Factory Usage:

```csharp
// Resolve factory from DI
var factory = serviceProvider.GetService<IActuatorManagerFactory>();

// Create manager instance
var manager = factory.Create();
```

### 4. Configuration Services Registration

**Location:** `src/Libraries/ACATCore/DependencyInjection/ServiceCollectionExtensions.cs`

The `AddACATConfiguration()` extension method registers all configuration infrastructure services with the DI container:

| Service | Lifetime | Purpose |
|---------|----------|---------|
| `JsonSchemaValidator` | Singleton | Validates JSON files against JSON Schema definitions before deserialization |
| `ConfigurationReloadService` | Singleton | Monitors configuration files and fires reload events on changes |
| `EnvironmentConfiguration` | Singleton | Resolves environment-specific configuration files (Development/Production/etc.) |

#### Usage Example:

```csharp
// Register configuration services explicitly
services.AddACATConfiguration();

// Or use the all-in-one method (includes configuration services)
services.AddACATServices();
```

#### Using JsonSchemaValidator via DI:

```csharp
// Constructor injection
public class MyConfigConsumer
{
    private readonly JsonSchemaValidator _schemaValidator;

    public MyConfigConsumer(JsonSchemaValidator schemaValidator)
    {
        _schemaValidator = schemaValidator;
        _schemaValidator.LoadSchema("my-config", "schemas/json/my-config.schema.json");
    }

    public MyConfig LoadConfig(string filePath)
    {
        var loader = new JsonConfigurationLoader<MyConfig>(
            schemaValidator: _schemaValidator,
            schemaName: "my-config",
            strictMode: true  // Fail on schema violations
        );
        return loader.Load(filePath);
    }
}

// Or resolve from service provider
var schemaValidator = serviceProvider.GetRequiredService<JsonSchemaValidator>();
```

#### JsonSchemaValidator Integration with JsonConfigurationLoader (Task #211):

`JsonSchemaValidator` was integrated into `JsonConfigurationLoader<T>` as an optional pre-deserialization validation step. This means schema validation runs *before* JSON deserialization, providing early failure for structurally invalid files.

```csharp
// Create loader with schema validation
var loader = new JsonConfigurationLoader<ActuatorConfig>(
    validator: new ActuatorSettingsValidator(),  // FluentValidation (post-deserialization)
    schemaValidator: schemaValidator,            // JSON Schema (pre-deserialization)
    schemaName: "actuator-settings",
    strictMode: false  // Warn on schema failure; true = fail on schema failure
);
var config = loader.Load("ActuatorSettings.json");
```

The two-layer validation approach:
1. **JSON Schema validation** (pre-deserialization) – structural checks via `JsonSchemaValidator`
2. **FluentValidation** (post-deserialization) – semantic/business rule checks via `IValidator<T>`

### 5. Context Class Enhancements

**Location:** `src/Libraries/ACATCore/PanelManagement/Context.cs`

The `Context` class has been redesigned to use DI while maintaining full backward compatibility. All static manager properties now resolve through the DI container when `ServiceProvider` is configured, and fall back to the static singleton instances when it is not.

#### Static Manager Properties (DI-Aware)

Every `AppXxx` property uses `ResolveManager<T>()` internally:

```csharp
// All properties transparently resolve from DI when ServiceProvider is set:
Context.AppActuatorManager      // resolves ActuatorManager from DI or singleton
Context.AppAgentMgr             // resolves AgentManager from DI or singleton
Context.AppPanelManager         // resolves PanelManager from DI or singleton
Context.AppTTSManager           // resolves TTSManager from DI or singleton
Context.AppWordPredictionManager // resolves WordPredictionManager from DI or singleton
// ...and all other manager properties
```

#### Interface-Based Resolution (Preferred for New Code)

```csharp
// Resolve by interface type (throws if ServiceProvider is null)
var actuatorManager = Context.GetManager<IActuatorManager>();
```

#### Internal Adapter (Backward Compatibility)

```csharp
// Private helper – resolves from DI first, falls back to singleton
private static T ResolveManager<T>(Func<T> fallback) where T : class
```

This adapter pattern means **existing code that uses `Context.AppXxx` properties does not need to be changed** – once `Context.ServiceProvider` is configured, DI instances are automatically used.

#### Existing Functionality Preserved:

- Static `ServiceProvider` property remains for setting up DI
- All existing static manager properties (`AppActuatorManager`, etc.) continue to work
- Three-phase initialization (`PreInit()`, `Init()`, `PostInit()`) unchanged

### 5. Extension Loading with DI

**Location:** `src/Libraries/ACATCore/Utility/ExtensionInstantiator.cs`

Extension loading already supports DI (implemented in Phase 1). The `ExtensionInstantiator` class provides:

```csharp
// Batch extension creation
IEnumerable<IExtension> CreateExtensionInstances(
    IServiceProvider serviceProvider,
    IEnumerable<Type> extensionTypes,
    ILogger logger = null)

// Single extension creation
object CreateExtensionInstance(
    IServiceProvider serviceProvider,
    Type extensionType,
    ILogger logger = null)
```

## Service Lifetimes

All managers are registered as **Singletons** to match existing ACAT behavior:

```csharp
// Concrete type registration
services.AddSingleton<ActuatorManager>(provider => ActuatorManager.Instance);

// Interface registration (resolves to same singleton)
services.AddSingleton<IActuatorManager>(provider => 
    provider.GetRequiredService<ActuatorManager>());

// Factory registration
services.AddSingleton<IActuatorManagerFactory, ActuatorManagerFactory>();
```

This ensures:
- Only one instance of each manager exists per application
- Interface and concrete type resolve to the same instance
- Factory pattern available for testing scenarios

## Application Integration

### Entry Points Updated

The following applications have been updated to use the new DI infrastructure:

1. **ACATApp** - Main dashboard application
2. **ACATTalk** - Speech output application
3. **ACATConfigNext** - Modern configuration UI
4. **ACATWatch** - Monitoring application
5. **ACATConfig** - Classic configuration UI

### Integration Pattern:

```csharp
// Standard pattern used across all entry points
private static void InitializeDependencyInjection()
{
    // Option 1: Use existing logger factory
    var services = new ServiceCollection();
    services.AddSingleton<ILoggerFactory>(existingLoggerFactory);
    services.AddLogging();
    services.AddACATServices();
    var serviceProvider = services.BuildServiceProvider();
    Context.ServiceProvider = serviceProvider;

    // Option 2: Use complete infrastructure
    var services = new ServiceCollection();
    services.AddACATInfrastructure();
    var serviceProvider = services.BuildServiceProvider();
    Context.ServiceProvider = serviceProvider;
}
```

## Testing

### Test Projects

Three test projects validate the DI infrastructure:

1. **ACATCore.Tests.Configuration** - Core DI tests
   - `ServiceConfigurationTests.cs` - Service registration and lifetime tests
   - `ManagerFactoryTests.cs` - Factory pattern tests
   - `ContextDependencyInjectionTests.cs` - Context DI resolution tests
   - `ExtensionLoadingIntegrationTests.cs` - Extension loading with DI tests

2. **ACATCore.Tests.Logging** - Logging infrastructure tests (Phase 1)

3. **ACATCore.Tests.Integration** - Integration tests (Phase 1)

### Test Coverage

The test suite validates:
- ✅ All managers can be resolved by concrete type
- ✅ All managers can be resolved by interface type
- ✅ Interface and concrete types resolve to same singleton instance
- ✅ All factories can be resolved and create valid instances
- ✅ Factory-created instances match directly-resolved instances
- ✅ Context.GetManager<T>() correctly resolves from DI
- ✅ Extension instantiation works with DI
- ✅ Null/error cases handled appropriately
- ✅ Configuration services (JsonSchemaValidator, ConfigurationReloadService, EnvironmentConfiguration) resolve from DI

## Migration Guide

### For New Code

Use dependency injection throughout:

```csharp
// Constructor injection (preferred)
public class MyComponent
{
    private readonly IActuatorManager _actuatorManager;
    private readonly ILogger<MyComponent> _logger;

    public MyComponent(
        IActuatorManager actuatorManager,
        ILogger<MyComponent> logger)
    {
        _actuatorManager = actuatorManager;
        _logger = logger;
    }
}

// Or resolve from Context
public void SomeMethod()
{
    var actuatorManager = Context.GetManager<IActuatorManager>();
    // Use manager...
}
```

### For Existing Code

Existing code continues to work without changes:

```csharp
// Static access still works (backward compatible)
var manager = Context.AppActuatorManager;
```

However, gradual migration to DI is recommended:

```csharp
// Before (legacy)
var manager = Context.AppActuatorManager;

// After (DI-aware)
var manager = Context.GetManager<IActuatorManager>();
```

## Benefits

### 1. Improved Testability

- Mock interfaces in unit tests instead of concrete classes
- Test components in isolation
- Use factory pattern for complex test scenarios

### 2. Reduced Coupling

- Components depend on interfaces, not concrete implementations
- Easier to swap implementations
- Better separation of concerns

### 3. Better Lifetime Management

- Explicit control over object lifetimes
- Clear ownership and disposal
- Prevention of memory leaks

### 4. Clearer Dependencies

- Constructor injection makes dependencies explicit
- Easier to understand component requirements
- Improved code documentation through type signatures

## Future Enhancements

### Potential Improvements:

1. **Constructor Injection in Managers** - Refactor managers to use constructor injection instead of singleton pattern
2. **Scoped Services** - Add support for scoped lifetime where appropriate
3. **Configuration Options Pattern** - Use IOptions<T> for manager configuration
4. **Health Checks** - Implement health check interfaces for managers
5. **Hosted Services** - Convert background services to IHostedService pattern

### Breaking Changes Required:

None of these improvements require breaking changes. The current implementation maintains full backward compatibility while enabling gradual migration.

## Troubleshooting

### Common Issues:

**Issue:** `InvalidOperationException: ServiceProvider is not configured`

**Solution:** Ensure `Context.ServiceProvider` is set during application initialization:
```csharp
Context.ServiceProvider = serviceProvider;
```

**Issue:** Manager returns null when resolved

**Solution:** Ensure `AddACATServices()` is called during DI setup:
```csharp
services.AddACATServices();
```

**Issue:** Different instances returned

**Solution:** Verify that both interface and concrete type resolve from the same service provider:
```csharp
// Both should return same instance
var manager1 = serviceProvider.GetService<ActuatorManager>();
var manager2 = serviceProvider.GetService<IActuatorManager>();
Assert.AreSame(manager1, manager2);
```

## Summary

The ACAT dependency injection infrastructure provides a modern, testable foundation while maintaining complete backward compatibility. All new code should use DI patterns, while existing code continues to function without modifications.

**Key Deliverables Completed:**
- ✅ Service container configuration (ServiceConfiguration class)
- ✅ Configuration services registration (JsonSchemaValidator, ConfigurationReloadService, EnvironmentConfiguration)
- ✅ Lifetime management (Singleton pattern via DI)
- ✅ Factory patterns for all managers
- ✅ Interface extraction for testability (10 interfaces)
- ✅ Context class enhancements with DI support
- ✅ Extension loading integration verified
- ✅ Entry point updates (5 applications)
- ✅ Comprehensive test coverage (4 test classes, 30+ tests)
- ✅ JsonSchemaValidator integrated into JsonConfigurationLoader (Task #211)

**Phase 2 Tasks:** #212, #213, #214, #215, #216 - **COMPLETE**
