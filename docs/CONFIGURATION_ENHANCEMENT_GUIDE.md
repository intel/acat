# Configuration System Enhancement Guide

## Overview

This document describes the enhanced configuration system features introduced in Phase 2 of the ACAT modernization project. The enhancements include:

1. **JSON Schema Validation** - Validate configurations against JSON schemas
2. **Configuration Hot-Reload** - Automatically reload configurations when files change
3. **Environment-Specific Configuration** - Support for different environments (Development, Testing, Staging, Production)
4. **Configuration Versioning** - Track and migrate configuration versions

## JSON Schema Validation

### Overview

The `JsonSchemaValidator` class provides basic JSON schema validation to ensure configuration files conform to expected structures.

### Usage

```csharp
using ACAT.Core.Configuration;
using Microsoft.Extensions.Logging;

// Create validator
var validator = new JsonSchemaValidator(logger);

// Load schema
string schemaPath = "schemas/json/actuator-settings.schema.json";
validator.LoadSchema("actuator-settings", schemaPath);

// Validate configuration
string configPath = "config/ActuatorSettings.json";
bool isValid = validator.Validate("actuator-settings", configPath, out List<string> errors);

if (!isValid)
{
    foreach (var error in errors)
    {
        Console.WriteLine($"Validation error: {error}");
    }
}
```

### Features

- Load and cache JSON schemas
- Validate JSON files against schemas
- Validate JSON content strings
- Detailed error reporting
- Type checking (string, number, boolean, array, object)
- Required property validation

## Configuration Hot-Reload

### Overview

The `ConfigurationReloadService` monitors configuration files for changes and notifies listeners when files are modified.

### Usage

```csharp
using ACAT.Core.Configuration;
using Microsoft.Extensions.Logging;

// Create reload service
var reloadService = new ConfigurationReloadService(logger);

// Subscribe to reload events
reloadService.ConfigurationReloaded += (sender, e) =>
{
    logger.LogInformation("Configuration reloaded: {FilePath}", e.FilePath);
    // Reload your configuration here
    ReloadMyConfiguration(e.FilePath);
};

reloadService.ConfigurationReloadFailed += (sender, e) =>
{
    logger.LogError("Configuration reload failed: {FilePath}. Error: {Error}", 
        e.FilePath, e.ErrorMessage);
};

// Start monitoring
string configPath = "config/settings.json";
reloadService.StartMonitoring(configPath);

// Stop monitoring when done
reloadService.StopMonitoring(configPath);

// Or stop all monitoring
reloadService.StopAll();

// Clean up
reloadService.Dispose();
```

### Features

- File system monitoring with FileSystemWatcher
- Debouncing to prevent multiple reloads (500ms delay)
- Event-based notifications
- Support for multiple files
- Automatic cleanup on dispose

### Integration with JsonConfigurationLoader

```csharp
using ACAT.Core.Utility;

// Create loader with hot-reload enabled
var loader = new JsonConfigurationLoader<MyConfig>(
    validator: myValidator,
    logger: logger,
    enableHotReload: true  // Enable hot-reload
);

// Subscribe to reload events
loader.ConfigurationReloaded += (sender, e) =>
{
    // Reload configuration
    var newConfig = loader.Load(e.FilePath);
    ApplyNewConfiguration(newConfig);
};

// Load and enable monitoring
var config = loader.Load("config/settings.json");
loader.EnableHotReload("config/settings.json");

// Disable when done
loader.DisableHotReload("config/settings.json");
loader.Dispose();
```

## Environment-Specific Configuration

### Overview

The `EnvironmentConfiguration` class supports loading different configuration files based on the environment (Development, Testing, Staging, Production).

### Supported Environments

```csharp
public enum ConfigurationEnvironment
{
    Development,
    Testing,
    Staging,
    Production
}
```

### Environment Detection

The system detects the environment from environment variables in this order:

1. `ACAT_ENVIRONMENT`
2. `DOTNET_ENVIRONMENT`
3. `ASPNETCORE_ENVIRONMENT`
4. Defaults to `Production` if none are set

### Setting Environment

**Option 1: Environment Variable**

```bash
# Windows
set ACAT_ENVIRONMENT=Development

# Linux/Mac
export ACAT_ENVIRONMENT=Development
```

**Option 2: Programmatic**

```csharp
var envConfig = new EnvironmentConfiguration(logger);
envConfig.SetEnvironment(ConfigurationEnvironment.Development);
```

### File Naming Convention

Environment-specific files use the naming pattern: `filename.{Environment}.extension`

Examples:
- Base: `config.json`
- Development: `config.Development.json`
- Testing: `config.Testing.json`
- Staging: `config.Staging.json`
- Production: `config.Production.json`

### Usage

```csharp
using ACAT.Core.Configuration;

// Create environment configuration
var envConfig = new EnvironmentConfiguration(logger);

// Get environment-specific file path
string basePath = "config/settings.json";
string envPath = envConfig.GetEnvironmentFilePath(basePath);
// Returns: "config/settings.Development.json" if in Development and file exists
//          "config/settings.json" otherwise

// Load configuration with environment overrides
var config = envConfig.LoadWithEnvironmentOverrides<MyConfig>(basePath);
```

### Environment Variable Overrides

You can override individual configuration properties using environment variables:

```bash
# Override property "Port" in configuration
set ACAT_PORT=9090

# Override property "Enabled"
set ACAT_ENABLED=true
```

The system will automatically apply these overrides when loading configuration.

### Integration with JsonConfigurationLoader

```csharp
// Create loader with environment support
var loader = new JsonConfigurationLoader<MyConfig>(
    validator: myValidator,
    logger: logger,
    enableHotReload: false,
    useEnvironmentConfig: true  // Enable environment-specific config
);

// Load with environment support
var config = loader.LoadWithEnvironment("config/settings.json");
// Automatically loads "settings.Development.json" in Development environment
// And applies environment variable overrides

// Check current environment
var env = loader.GetCurrentEnvironment();
logger.LogInformation("Current environment: {Environment}", env);
```

## Configuration Versioning

### Overview

The `ConfigurationVersionManager` class tracks configuration file versions and supports migrations between versions.

### Version Format

Configuration versions use semantic versioning: `Major.Minor.Patch`

Example: `1.2.3`
- Major: Incompatible changes
- Minor: Backward-compatible features
- Patch: Backward-compatible bug fixes

### Adding Version to Configuration

```json
{
  "version": "1.0.0",
  "name": "My Configuration",
  "settings": {
    ...
  }
}
```

Alternative property name:
```json
{
  "configVersion": "1.0.0",
  ...
}
```

### Usage

```csharp
using ACAT.Core.Configuration;

// Create version manager
var versionManager = new ConfigurationVersionManager(logger);

// Set current version for configuration type
versionManager.SetCurrentVersion("actuator-settings", 
    new ConfigurationVersion(2, 0, 0));

// Check if file needs migration
string configPath = "config/ActuatorSettings.json";
if (versionManager.NeedsMigration("actuator-settings", configPath))
{
    logger.LogInformation("Configuration needs migration");
    
    // Migrate (creates backup automatically)
    bool success = versionManager.MigrateConfiguration(
        "actuator-settings", 
        configPath, 
        createBackup: true
    );
}

// Get configuration version
var version = versionManager.GetConfigurationVersion(configPath);
logger.LogInformation("Configuration version: {Version}", version);
```

### Creating Migration Handlers

Implement `IConfigurationMigration` interface:

```csharp
public class ActuatorSettings_1_0_to_2_0 : IConfigurationMigration
{
    public ConfigurationVersion FromVersion => new ConfigurationVersion(1, 0, 0);
    public ConfigurationVersion ToVersion => new ConfigurationVersion(2, 0, 0);

    public bool Migrate(JsonElement source, out JsonElement result, out string error)
    {
        try
        {
            // Perform migration logic
            // Example: Rename property, add new fields, etc.
            
            var dict = new Dictionary<string, object>();
            
            // Copy existing properties
            foreach (var prop in source.EnumerateObject())
            {
                if (prop.Name == "oldPropertyName")
                {
                    // Rename property
                    dict["newPropertyName"] = prop.Value.ToString();
                }
                else
                {
                    dict[prop.Name] = prop.Value.Clone();
                }
            }
            
            // Add new required properties
            dict["newField"] = "defaultValue";
            dict["version"] = "2.0.0";
            
            // Serialize back to JsonElement
            string json = JsonSerializer.Serialize(dict);
            result = JsonDocument.Parse(json).RootElement;
            error = null;
            
            return true;
        }
        catch (Exception ex)
        {
            result = default;
            error = ex.Message;
            return false;
        }
    }
}

// Register migration
var migration = new ActuatorSettings_1_0_to_2_0();
versionManager.RegisterMigration("actuator-settings", migration);
```

## Complete Example

### Comprehensive Configuration Loading

```csharp
using ACAT.Core.Configuration;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using Microsoft.Extensions.Logging;

public class ConfigurationManager
{
    private readonly ILogger _logger;
    private readonly JsonConfigurationLoader<MyConfig> _loader;
    private readonly JsonSchemaValidator _schemaValidator;
    private readonly ConfigurationVersionManager _versionManager;
    private MyConfig _currentConfig;

    public ConfigurationManager(ILogger logger)
    {
        _logger = logger;
        
        // Initialize schema validator
        _schemaValidator = new JsonSchemaValidator(_logger);
        _schemaValidator.LoadSchema("my-config", "schemas/json/my-config.schema.json");
        
        // Initialize version manager
        _versionManager = new ConfigurationVersionManager(_logger);
        _versionManager.SetCurrentVersion("my-config", new ConfigurationVersion(2, 0, 0));
        
        // Initialize loader with all features
        _loader = new JsonConfigurationLoader<MyConfig>(
            validator: new MyConfigValidator(),
            logger: _logger,
            enableHotReload: true,
            useEnvironmentConfig: true
        );
        
        // Subscribe to reload events
        _loader.ConfigurationReloaded += OnConfigurationReloaded;
    }

    public MyConfig LoadConfiguration(string basePath)
    {
        try
        {
            // Get environment-specific path
            string configPath = GetConfigPath(basePath);
            
            // Check if migration needed
            if (_versionManager.NeedsMigration("my-config", configPath))
            {
                _logger.LogInformation("Migrating configuration...");
                _versionManager.MigrateConfiguration("my-config", configPath, createBackup: true);
            }
            
            // Validate schema
            if (!_schemaValidator.Validate("my-config", configPath, out List<string> errors))
            {
                _logger.LogError("Schema validation failed:");
                foreach (var error in errors)
                {
                    _logger.LogError("  - {Error}", error);
                }
                throw new Exception("Configuration schema validation failed");
            }
            
            // Load with environment overrides
            _currentConfig = _loader.LoadWithEnvironment(basePath);
            
            // Enable hot-reload
            _loader.EnableHotReload(configPath);
            
            _logger.LogInformation("Configuration loaded successfully from: {Path}", configPath);
            return _currentConfig;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configuration");
            throw;
        }
    }

    private void OnConfigurationReloaded(object sender, ConfigurationReloadEventArgs e)
    {
        _logger.LogInformation("Configuration file changed, reloading: {FilePath}", e.FilePath);
        
        if (e.Success)
        {
            // Reload configuration
            _currentConfig = _loader.Load(e.FilePath);
            
            // Notify application
            NotifyConfigurationChanged();
        }
    }

    private string GetConfigPath(string basePath)
    {
        var envConfig = new EnvironmentConfiguration(_logger);
        return envConfig.GetEnvironmentFilePath(basePath);
    }

    private void NotifyConfigurationChanged()
    {
        // Notify your application that configuration has changed
        ConfigurationChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler ConfigurationChanged;
}
```

## Best Practices

### 1. Schema Validation

- Always validate configurations against schemas before use
- Keep schemas up-to-date with code changes
- Include schemas in version control
- Provide clear error messages for validation failures

### 2. Hot-Reload

- Use hot-reload only when appropriate (not all configs need it)
- Test reload behavior thoroughly
- Handle reload failures gracefully
- Consider thread-safety when reloading

### 3. Environment Configuration

- Use Production as the default environment
- Never include sensitive data in Development/Testing configs
- Document environment-specific differences
- Test all environments before deployment

### 4. Versioning

- Always include version in configuration files
- Increment version appropriately (major/minor/patch)
- Create migration handlers for breaking changes
- Always create backups before migration
- Test migrations thoroughly

### 5. Error Handling

- Always handle configuration load failures
- Provide meaningful error messages
- Log all configuration operations
- Have fallback/default configurations

### 6. Security

- Never log sensitive configuration values
- Secure environment variable access
- Validate all loaded configurations
- Use appropriate file permissions

## Troubleshooting

### Hot-Reload Not Working

**Problem:** Configuration changes not detected

**Solutions:**
- Ensure `enableHotReload=true` when creating loader
- Call `EnableHotReload(filePath)` explicitly
- Check file permissions
- Verify FileSystemWatcher limitations on your OS
- Check debounce delay (500ms default)

### Environment Config Not Loading

**Problem:** Environment-specific file not loaded

**Solutions:**
- Verify environment variable is set correctly
- Check file naming convention (e.g., `config.Development.json`)
- Ensure environment-specific file exists
- Set `useEnvironmentConfig=true` when creating loader

### Schema Validation Errors

**Problem:** Valid configuration fails schema validation

**Solutions:**
- Check schema is loaded correctly
- Verify JSON syntax
- Check required properties
- Validate property types
- Review schema definition

### Migration Failures

**Problem:** Configuration migration fails

**Solutions:**
- Check migration handler logic
- Verify version numbers
- Review error messages
- Restore from backup if needed
- Test migration with copy first

## API Reference

See inline XML documentation in source files:
- `JsonSchemaValidator.cs`
- `ConfigurationReloadService.cs`
- `EnvironmentConfiguration.cs`
- `ConfigurationVersioning.cs`
- `JsonConfigurationLoader.cs`

## Related Documentation

- [JSON Configuration Developer Guide](JSON_CONFIGURATION_DEVELOPER_GUIDE.md)
- [Configuration Migration Guide](LOGGING_MIGRATION_GUIDE.md)
- [Testing Guide](src/Libraries/ACATCore.Tests.Configuration/TESTING_GUIDE.md)
