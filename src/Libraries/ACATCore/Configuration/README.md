# Configuration System Enhancements

This directory contains the enhanced configuration system components for ACAT Phase 2.

## Components

### Core Classes

#### JsonSchemaValidator
- **Purpose**: Validate JSON configuration files against JSON schemas
- **File**: `JsonSchemaValidator.cs`
- **Key Features**:
  - Load and cache JSON schemas
  - Validate files or content strings
  - Type checking and required property validation
  - Detailed error reporting

#### ConfigurationReloadService
- **Purpose**: Monitor configuration files and automatically reload on changes
- **File**: `ConfigurationReloadService.cs`
- **Key Features**:
  - FileSystemWatcher integration
  - Debouncing (500ms) to prevent multiple reloads
  - Event-based notifications
  - Support for multiple files

#### EnvironmentConfiguration
- **Purpose**: Support environment-specific configuration (Development, Testing, Staging, Production)
- **File**: `EnvironmentConfiguration.cs`
- **Key Features**:
  - Automatic environment detection from environment variables
  - Environment-specific file path resolution
  - Local override file support (`config.local.json`) — gitignored for developer-specific settings
  - Full configuration hierarchy: base → environment-specific → local override → env vars
  - Environment variable overrides (ACAT_*)
  - Configuration override management

#### ConfigurationVersioning
- **Purpose**: Track and migrate configuration versions
- **File**: `ConfigurationVersioning.cs`
- **Key Features**:
  - Semantic versioning (Major.Minor.Patch)
  - Migration framework with IConfigurationMigration interface
  - Automatic backup before migration
  - Migration path resolution

### JSON Configuration Models

- `ActuatorSettingsJson.cs` - Actuator configuration model
- `ThemeJson.cs` - Theme configuration model
- `PanelConfigJson.cs` - Panel configuration model
- `AbbreviationsJson.cs` - Abbreviations model
- `PronunciationsJson.cs` - Pronunciations model
- `PreferredWordPredictorsJson.cs` - Word predictor preferences model

### Examples

`ConfigurationExamples.cs` contains complete working examples:

1. **Example1_SchemaValidation** - Basic schema validation
2. **Example2_HotReload** - Configuration hot-reload
3. **Example3_EnvironmentConfig** - Environment-specific configuration
4. **Example4_Versioning** - Configuration versioning and migration
5. **Example5_CompleteConfigurationManager** - All features combined

## Quick Start

### 1. Basic Configuration Loading with Schema Validation

```csharp
var schemaValidator = new JsonSchemaValidator(logger);
schemaValidator.LoadSchema("my-config", "schemas/json/my-config.schema.json");

// Pass schemaValidator to loader for automatic pre-deserialization validation
var loader = new JsonConfigurationLoader<MyConfig>(
    logger: logger,
    schemaValidator: schemaValidator,
    schemaName: "my-config"
);
var config = loader.Load("config/settings.json");
```

Use **strict mode** to treat schema validation failures as errors (returns default/null instead of deserializing):

```csharp
var loader = new JsonConfigurationLoader<MyConfig>(
    logger: logger,
    schemaValidator: schemaValidator,
    schemaName: "my-config",
    strictMode: true  // Fail on schema violations
);
var config = loader.Load("config/settings.json");
```

### 2. Configuration with Hot-Reload

```csharp
var loader = new JsonConfigurationLoader<MyConfig>(
    validator: null,
    logger: logger,
    enableHotReload: true
);

loader.ConfigurationReloaded += (sender, e) =>
{
    // Reload and apply new configuration
    var newConfig = loader.Load(e.FilePath);
    ApplyConfiguration(newConfig);
};

var config = loader.Load("config/settings.json");
loader.EnableHotReload("config/settings.json");
```

### 3. Environment-Specific Configuration

```csharp
// Set environment variable
Environment.SetEnvironmentVariable("ACAT_ENVIRONMENT", "Development");

var loader = new JsonConfigurationLoader<MyConfig>(
    validator: null,
    logger: logger,
    enableHotReload: false,
    useEnvironmentConfig: true
);

// Automatically loads "settings.Development.json" in Development environment
var config = loader.LoadWithEnvironment("config/settings.json");
```

### 4. Configuration Versioning

```csharp
var versionManager = new ConfigurationVersionManager(logger);
versionManager.SetCurrentVersion("my-config", new ConfigurationVersion(2, 0, 0));

// Register migration handlers
// versionManager.RegisterMigration("my-config", new MyMigration_1_to_2());

if (versionManager.NeedsMigration("my-config", configPath))
{
    versionManager.MigrateConfiguration("my-config", configPath, createBackup: true);
}
```

## Environment Variables

### Environment Selection
- `ACAT_ENVIRONMENT` - Primary (recommended)
- `DOTNET_ENVIRONMENT` - Secondary
- `ASPNETCORE_ENVIRONMENT` - Tertiary
- Default: `Production`

### Configuration Overrides
Use `ACAT_<PropertyName>` format to override specific configuration properties:

```bash
# Windows
set ACAT_PORT=9090
set ACAT_ENABLED=true

# Linux/Mac
export ACAT_PORT=9090
export ACAT_ENABLED=true
```

## File Naming Conventions

### Configuration Hierarchy (lowest to highest priority)

Files are loaded and merged in the following order:
1. **Base**: `config.json` — shared defaults
2. **Environment-specific**: `config.{Environment}.json` — environment overrides
3. **Local override**: `config.local.json` — developer-specific overrides (**gitignored**)
4. **Environment variables**: `ACAT_*` — runtime overrides

### Environment-Specific Files
- Base: `config.json`
- Development: `config.Development.json`
- Testing: `config.Testing.json`
- Staging: `config.Staging.json`
- Production: `config.Production.json`

### Local Override Files (gitignored)
- `config.local.json`

Local override files allow individual developers to maintain machine-specific settings
without polluting the repository.  They are automatically excluded from source control
via `.gitignore` (`*.local.json`).

### Backup Files
Migration creates automatic backups: `config.json.backup.20260218123456`

## JSON Schema Files

Schemas are located in `/schemas/json/`:
- `actuator-settings.schema.json`
- `theme.schema.json`
- `panel-config.schema.json`
- `abbreviations.schema.json`
- `pronunciations.schema.json`

## Tests

Comprehensive test suite in `ACATCore.Tests.Configuration/ConfigurationEnhancementsTests.cs`:
- 20+ unit tests
- Schema validation tests
- Hot-reload tests
- Environment configuration tests
- Versioning tests
- Integration tests

Run tests:
```bash
dotnet test src/Libraries/ACATCore.Tests.Configuration/ACATCore.Tests.Configuration.csproj
```

## Documentation

Complete documentation available in:
- `/docs/CONFIGURATION_ENHANCEMENT_GUIDE.md` - Complete user guide with examples
- Inline XML documentation in all source files

## Best Practices

1. **Always validate** - Use schema validation before loading
2. **Version your configs** - Include version field in all configuration files
3. **Test migrations** - Test on copies before migrating production configs
4. **Use environment configs** - Separate Development/Production settings
5. **Handle reload failures** - Always have fallback configuration
6. **Secure sensitive data** - Never log or expose sensitive configuration values

## Integration with Existing Code

The enhanced JsonConfigurationLoader maintains backward compatibility:

```csharp
// Old usage still works
var loader = new JsonConfigurationLoader<MyConfig>(validator, logger);
var config = loader.Load(configPath);

// New features are opt-in
var loaderWithFeatures = new JsonConfigurationLoader<MyConfig>(
    validator: validator,
    logger: logger,
    enableHotReload: true,          // Opt-in
    useEnvironmentConfig: true,     // Opt-in
    schemaValidator: schemaValidator, // Opt-in: pre-deserialization JSON schema validation
    schemaName: "my-config",        // Required when schemaValidator is provided
    strictMode: true                // Opt-in: fail on schema violations (default: warn)
);
```

## Migration from Legacy Code

1. Existing code using `JsonConfigurationLoader` continues to work
2. New features are opt-in via constructor parameters
3. No breaking changes to existing APIs
4. New methods are additive only

## Support

For issues or questions:
1. Check documentation: `/docs/CONFIGURATION_ENHANCEMENT_GUIDE.md`
2. Review examples: `ConfigurationExamples.cs`
3. Run tests to verify installation
4. Check inline XML documentation

## License

Copyright 2013-2019; 2023 Intel Corporation
SPDX-License-Identifier: Apache-2.0
