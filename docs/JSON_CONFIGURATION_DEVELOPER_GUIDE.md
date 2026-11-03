# ACAT JSON Configuration - Developer Guide

**Version:** 1.0  
**Last Updated:** February 2026  
**Audience:** Developers extending ACAT's configuration system

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Adding New Configuration Types](#adding-new-configuration-types)
3. [Creating POCO Classes](#creating-poco-classes)
4. [Creating Validators](#creating-validators)
5. [Creating JSON Schemas](#creating-json-schemas)
6. [Loading and Saving Configurations](#loading-and-saving-configurations)
7. [Testing](#testing)
8. [Best Practices](#best-practices)

---

## Architecture Overview

ACAT's JSON configuration system follows a layered architecture:

```
┌─────────────────────────────────────────┐
│     Application Code (Consumers)        │
│  - ActuatorManager, ThemeManager, etc.  │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│    Configuration Classes (POCOs)        │
│  - ActuatorSettingsJson                 │
│  - ThemeJson, AbbreviationsJson, etc.   │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│  JsonConfigurationLoader<T>             │
│  - Deserialization                      │
│  - Validation                           │
│  - Error Handling                       │
└──────────────┬──────────────────────────┘
               │
┌──────────────┴──────────────────────────┐
│                                         │
▼                                         ▼
┌──────────────────┐       ┌─────────────────────┐
│ System.Text.Json │       │ FluentValidation    │
│ (Serialization)  │       │ (Business Rules)    │
└──────────────────┘       └─────────────────────┘
```

### Key Components

1. **POCO Classes** (`Configuration/` directory)
   - Strongly-typed C# classes
   - Map directly to JSON structure
   - Located in: `src/Libraries/ACATCore/Configuration/`

2. **Validators** (`Validation/` directory)
   - FluentValidation rules
   - Business logic validation
   - Located in: `src/Libraries/ACATCore/Validation/`

3. **JSON Schemas** (`schemas/json/` directory)
   - JSON Schema Draft-07 format
   - Powers VS Code IntelliSense
   - Located in: `schemas/json/`

4. **JsonConfigurationLoader** (`Utility/` directory)
   - Generic loader for all configuration types
   - Handles deserialization and validation
   - Located in: `src/Libraries/ACATCore/Utility/JsonConfigurationLoader.cs`

5. **Example Files** (`schemas/examples/` directory)
   - Serve as templates and documentation
   - Located in: `schemas/examples/`

---

## Adding New Configuration Types

Follow these steps to add a new configuration type to ACAT:

### Step 1: Analyze Existing Configuration

Before creating a new configuration type, analyze the existing format (XML or otherwise):

1. Locate the existing configuration file
2. Document the structure and all properties
3. Identify required vs. optional fields
4. Note any special validation rules
5. Find where the configuration is currently loaded in code

**Example Analysis Template:**
```markdown
## PluginSettings Analysis

**Current Format:** XML
**Location:** %APPDATA%\ACAT\Config\PluginSettings.xml

**Structure:**
- Root: <PluginSettings>
- Children: <Plugin> elements
  - Attributes: name, enabled, version
  - Children: <Setting> elements with key/value pairs

**Usage:** 
- Loaded by: PluginManager.cs
- Accessed by: Multiple plugin classes

**Validation Rules:**
- Plugin name must be unique
- At least one plugin must be enabled
- Version must be in format x.y.z
```

### Step 2: Create POCO Class

Create a new file in `src/Libraries/ACATCore/Configuration/`:

**File:** `PluginSettingsJson.cs`

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// JSON configuration for ACAT plugins
    /// </summary>
    public class PluginSettingsJson
    {
        /// <summary>
        /// List of plugin configurations
        /// </summary>
        [JsonPropertyName("plugins")]
        public List<PluginJson> Plugins { get; set; } = new();

        /// <summary>
        /// Factory method to create default settings
        /// </summary>
        public static PluginSettingsJson CreateDefault()
        {
            return new PluginSettingsJson
            {
                Plugins = new List<PluginJson>
                {
                    new PluginJson
                    {
                        Name = "CorePlugin",
                        Enabled = true,
                        Version = "1.0.0"
                    }
                }
            };
        }
    }

    /// <summary>
    /// Individual plugin configuration
    /// </summary>
    public class PluginJson
    {
        /// <summary>
        /// Plugin name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Whether the plugin is enabled
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Plugin version (x.y.z format)
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Plugin-specific settings
        /// </summary>
        [JsonPropertyName("settings")]
        public Dictionary<string, string> Settings { get; set; } = new();
    }
}
```

### Step 3: Create Validator

Create a new file in `src/Libraries/ACATCore/Validation/`:

**File:** `PluginSettingsValidator.cs`

```csharp
using ACAT.Core.Configuration;
using FluentValidation;
using System;

namespace ACAT.Core.Validation
{
    /// <summary>
    /// Validator for plugin settings configuration
    /// </summary>
    public class PluginSettingsValidator : AbstractValidator<PluginSettingsJson>
    {
        public PluginSettingsValidator()
        {
            // At least one plugin required
            RuleFor(x => x.Plugins)
                .NotEmpty()
                .WithMessage("At least one plugin must be configured");

            // Validate each plugin
            RuleForEach(x => x.Plugins)
                .SetValidator(new PluginValidator());

            // At least one enabled plugin
            RuleFor(x => x.Plugins)
                .Must(plugins => plugins.Any(p => p.Enabled))
                .WithMessage("At least one plugin must be enabled");

            // Plugin names must be unique
            RuleFor(x => x.Plugins)
                .Must(plugins => plugins.Select(p => p.Name).Distinct().Count() == plugins.Count)
                .WithMessage("Plugin names must be unique");
        }
    }

    /// <summary>
    /// Validator for individual plugin configuration
    /// </summary>
    public class PluginValidator : AbstractValidator<PluginJson>
    {
        public PluginValidator()
        {
            // Name is required
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Plugin name is required");

            // Version must be in x.y.z format
            RuleFor(x => x.Version)
                .NotEmpty()
                .WithMessage("Plugin version is required")
                .Matches(@"^\d+\.\d+\.\d+$")
                .WithMessage("Version must be in format x.y.z (e.g., 1.0.0)");
        }
    }
}
```

### Step 4: Create JSON Schema

Create a new file in `schemas/json/`:

**File:** `plugin-settings.schema.json`

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "$id": "https://github.com/intel/acat/schemas/plugin-settings.schema.json",
  "title": "ACAT Plugin Settings",
  "description": "Configuration for ACAT plugins",
  "type": "object",
  "properties": {
    "plugins": {
      "type": "array",
      "description": "List of plugin configurations",
      "items": {
        "$ref": "#/definitions/plugin"
      },
      "minItems": 1
    }
  },
  "required": ["plugins"],
  "definitions": {
    "plugin": {
      "type": "object",
      "description": "Individual plugin configuration",
      "properties": {
        "name": {
          "type": "string",
          "description": "Plugin name",
          "minLength": 1
        },
        "enabled": {
          "type": "boolean",
          "description": "Whether the plugin is enabled",
          "default": true
        },
        "version": {
          "type": "string",
          "description": "Plugin version in x.y.z format",
          "pattern": "^\\d+\\.\\d+\\.\\d+$"
        },
        "settings": {
          "type": "object",
          "description": "Plugin-specific settings as key-value pairs",
          "additionalProperties": {
            "type": "string"
          }
        }
      },
      "required": ["name", "enabled", "version"],
      "additionalProperties": false
    }
  },
  "additionalProperties": false
}
```

### Step 5: Create Example File

Create a new file in `schemas/examples/`:

**File:** `plugin-settings.example.json`

```json
{
  "$schema": "../json/plugin-settings.schema.json",
  "plugins": [
    {
      "name": "CorePlugin",
      "enabled": true,
      "version": "1.0.0",
      "settings": {
        "maxRetries": "3",
        "timeout": "5000"
      }
    },
    {
      "name": "TextToSpeechPlugin",
      "enabled": true,
      "version": "2.1.0",
      "settings": {
        "voice": "Microsoft Zira",
        "rate": "0"
      }
    }
  ]
}
```

### Step 6: Integrate with Application

Update the consuming code to use the new configuration:

```csharp
using ACAT.Core.Configuration;
using ACAT.Core.Validation;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;

public class PluginManager
{
    private readonly ILogger<PluginManager> _logger;
    private PluginSettingsJson _settings;

    public PluginManager(ILogger<PluginManager> logger)
    {
        _logger = logger;
        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        var loader = new JsonConfigurationLoader<PluginSettingsJson>(
            new PluginSettingsValidator(),
            _logger
        );

        var configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ACAT", "Config", "PluginSettings.json"
        );

        _settings = loader.Load(configPath);

        if (_settings == null)
        {
            _logger.LogWarning("Failed to load plugin settings, using defaults");
            _settings = PluginSettingsJson.CreateDefault();
        }
    }

    public void SaveConfiguration()
    {
        var loader = new JsonConfigurationLoader<PluginSettingsJson>(
            new PluginSettingsValidator(),
            _logger
        );

        var configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ACAT", "Config", "PluginSettings.json"
        );

        loader.Save(_settings, configPath);
    }
}
```

### Step 7: Add Migration Support

If migrating from XML, add support to ConfigMigrationTool:

1. Create XML deserializer in `ConfigMigrationTool/XmlDeserializers.cs`
2. Add detection logic in `ConfigMigrationTool/ConfigurationMigrator.cs`
3. Add tests in `ConfigMigrationTool.Tests/`

### Step 8: Add Tests

Create comprehensive tests in `ACATCore.Tests.Configuration/`:

**File:** `PluginSettingsTests.cs`

```csharp
using ACAT.Core.Configuration;
using ACAT.Core.Validation;
using FluentValidation.TestHelper;
using System.Text.Json;
using Xunit;

namespace ACATCore.Tests.Configuration
{
    public class PluginSettingsTests
    {
        [Fact]
        public void CanDeserializePluginSettings()
        {
            var json = File.ReadAllText("plugin-settings.example.json");
            var settings = JsonSerializer.Deserialize<PluginSettingsJson>(json);
            
            Assert.NotNull(settings);
            Assert.NotEmpty(settings.Plugins);
        }

        [Fact]
        public void ValidatorAcceptsValidConfiguration()
        {
            var validator = new PluginSettingsValidator();
            var settings = PluginSettingsJson.CreateDefault();
            
            var result = validator.Validate(settings);
            
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidatorRejectsEmptyPlugins()
        {
            var validator = new PluginSettingsValidator();
            var settings = new PluginSettingsJson { Plugins = new() };
            
            var result = validator.TestValidate(settings);
            
            result.ShouldHaveValidationErrorFor(x => x.Plugins);
        }

        [Fact]
        public void ValidatorRejectsInvalidVersion()
        {
            var validator = new PluginValidator();
            var plugin = new PluginJson 
            { 
                Name = "Test",
                Enabled = true,
                Version = "invalid" 
            };
            
            var result = validator.TestValidate(plugin);
            
            result.ShouldHaveValidationErrorFor(x => x.Version);
        }
    }
}
```

---

## Creating POCO Classes

### Naming Conventions

- **Class Name:** `[ConfigurationType]Json` (e.g., `PluginSettingsJson`)
- **Namespace:** `ACAT.Core.Configuration`
- **File Name:** Same as class name

### Property Guidelines

```csharp
public class ExampleJson
{
    /// <summary>
    /// Always include XML documentation
    /// </summary>
    [JsonPropertyName("propertyName")]  // Use camelCase for JSON
    public string PropertyName { get; set; } = string.Empty;  // Provide defaults

    /// <summary>
    /// Use nullable for optional fields
    /// </summary>
    [JsonPropertyName("optionalField")]
    public string? OptionalField { get; set; }

    /// <summary>
    /// Initialize collections to avoid null reference exceptions
    /// </summary>
    [JsonPropertyName("items")]
    public List<ItemJson> Items { get; set; } = new();

    /// <summary>
    /// Use enums for restricted values
    /// </summary>
    [JsonPropertyName("mode")]
    [JsonConverter(typeof(JsonStringEnumConverter))]  // Serialize as string
    public ModeEnum Mode { get; set; }
}
```

### Factory Methods

Always include a `CreateDefault()` factory method:

```csharp
/// <summary>
/// Factory method to create default settings
/// </summary>
public static PluginSettingsJson CreateDefault()
{
    return new PluginSettingsJson
    {
        Plugins = new List<PluginJson>
        {
            new PluginJson
            {
                Name = "CorePlugin",
                Enabled = true,
                Version = "1.0.0"
            }
        }
    };
}
```

### Serialization Options

Use System.Text.Json with these options:

```csharp
private static readonly JsonSerializerOptions _options = new()
{
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};
```

---

## Creating Validators

### FluentValidation Patterns

```csharp
public class ExampleValidator : AbstractValidator<ExampleJson>
{
    public ExampleValidator()
    {
        // Required fields
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        // String length
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must be 500 characters or less");

        // Pattern matching
        RuleFor(x => x.Version)
            .Matches(@"^\d+\.\d+\.\d+$")
            .WithMessage("Version must be in format x.y.z");

        // Range validation
        RuleFor(x => x.Timeout)
            .GreaterThan(0)
            .LessThanOrEqualTo(60000)
            .WithMessage("Timeout must be between 1 and 60000 milliseconds");

        // Collection validation
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        // Nested validation
        RuleForEach(x => x.Items)
            .SetValidator(new ItemValidator());

        // Custom validation
        RuleFor(x => x.Items)
            .Must(HaveUniqueNames)
            .WithMessage("Item names must be unique");

        // Conditional validation
        When(x => x.Enabled, () =>
        {
            RuleFor(x => x.Command)
                .NotEmpty()
                .WithMessage("Command is required when enabled");
        });
    }

    private bool HaveUniqueNames(List<ItemJson> items)
    {
        return items.Select(i => i.Name).Distinct().Count() == items.Count;
    }
}
```

### Validation Severity

Use different severity levels for different types of issues:

```csharp
// Error: Critical issue, prevents usage
RuleFor(x => x.Name)
    .NotEmpty()
    .WithMessage("Name is required")
    .WithSeverity(Severity.Error);

// Warning: Not ideal but doesn't prevent usage
RuleFor(x => x.Description)
    .NotEmpty()
    .WithMessage("Description is recommended")
    .WithSeverity(Severity.Warning);

// Info: Helpful suggestion
RuleFor(x => x.Tags)
    .NotEmpty()
    .WithMessage("Adding tags helps with organization")
    .WithSeverity(Severity.Info);
```

---

## Creating JSON Schemas

### Schema Structure

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "$id": "https://github.com/intel/acat/schemas/your-config.schema.json",
  "title": "Human Readable Title",
  "description": "Detailed description of the configuration",
  "type": "object",
  "properties": {
    "propertyName": {
      "type": "string",
      "description": "Property description (appears in IntelliSense)"
    }
  },
  "required": ["propertyName"],
  "additionalProperties": false
}
```

### Common Patterns

**String with Pattern:**
```json
{
  "version": {
    "type": "string",
    "description": "Version number",
    "pattern": "^\\d+\\.\\d+\\.\\d+$",
    "examples": ["1.0.0", "2.1.3"]
  }
}
```

**Enum:**
```json
{
  "mode": {
    "type": "string",
    "description": "Operation mode",
    "enum": ["Write", "Speak", "None"],
    "default": "Write"
  }
}
```

**Array:**
```json
{
  "items": {
    "type": "array",
    "description": "List of items",
    "items": {
      "$ref": "#/definitions/item"
    },
    "minItems": 1,
    "maxItems": 100
  }
}
```

**Number with Range:**
```json
{
  "timeout": {
    "type": "number",
    "description": "Timeout in milliseconds",
    "minimum": 0,
    "maximum": 60000,
    "default": 5000
  }
}
```

**Object with Properties:**
```json
{
  "font": {
    "type": "object",
    "description": "Font settings",
    "properties": {
      "family": {
        "type": "string",
        "default": "Arial"
      },
      "size": {
        "type": "number",
        "minimum": 8,
        "maximum": 72,
        "default": 12
      }
    },
    "required": ["family", "size"]
  }
}
```

**Using Definitions:**
```json
{
  "definitions": {
    "plugin": {
      "type": "object",
      "properties": {
        "name": { "type": "string" },
        "enabled": { "type": "boolean" }
      },
      "required": ["name"]
    }
  },
  "properties": {
    "plugins": {
      "type": "array",
      "items": {
        "$ref": "#/definitions/plugin"
      }
    }
  }
}
```

---

## Loading and Saving Configurations

### Using JsonConfigurationLoader

The `JsonConfigurationLoader<T>` class provides a consistent way to load and save configurations:

```csharp
using ACAT.Core.Configuration;
using ACAT.Core.Validation;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;

public class ConfigurationExample
{
    private readonly ILogger<ConfigurationExample> _logger;

    public ConfigurationExample(ILogger<ConfigurationExample> logger)
    {
        _logger = logger;
    }

    public PluginSettingsJson LoadConfiguration(string filePath)
    {
        // Create loader with validator
        var loader = new JsonConfigurationLoader<PluginSettingsJson>(
            new PluginSettingsValidator(),
            _logger
        );

        // Load configuration
        var config = loader.Load(filePath);

        // Handle load failure
        if (config == null)
        {
            _logger.LogWarning("Failed to load configuration, using defaults");
            config = PluginSettingsJson.CreateDefault();
        }

        return config;
    }

    public void SaveConfiguration(PluginSettingsJson config, string filePath)
    {
        var loader = new JsonConfigurationLoader<PluginSettingsJson>(
            new PluginSettingsValidator(),
            _logger
        );

        // Save configuration (validates before saving)
        if (!loader.Save(config, filePath))
        {
            _logger.LogError("Failed to save configuration");
        }
    }
}
```

### Direct Serialization

For cases where you don't need validation:

```csharp
using System.Text.Json;

public class DirectSerialization
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true
    };

    public T LoadJson<T>(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json, _options);
    }

    public void SaveJson<T>(T config, string filePath)
    {
        var json = JsonSerializer.Serialize(config, _options);
        File.WriteAllText(filePath, json);
    }
}
```

### Backward Compatibility with XML

To maintain backward compatibility:

```csharp
public PluginSettingsJson LoadWithFallback(string basePath, string fileName)
{
    // Try JSON first
    var jsonPath = Path.Combine(basePath, $"{fileName}.json");
    if (File.Exists(jsonPath))
    {
        return LoadJsonConfiguration(jsonPath);
    }

    // Fall back to XML
    var xmlPath = Path.Combine(basePath, $"{fileName}.xml");
    if (File.Exists(xmlPath))
    {
        return LoadXmlConfiguration(xmlPath);
    }

    // Use defaults
    _logger.LogInformation("No configuration found, using defaults");
    return PluginSettingsJson.CreateDefault();
}
```

---

## Testing

### Unit Test Structure

```csharp
using Xunit;
using FluentValidation.TestHelper;
using System.Text.Json;

namespace ACATCore.Tests.Configuration
{
    public class PluginSettingsTests
    {
        // Test deserialization
        [Fact]
        public void CanDeserialize_ValidJson()
        {
            // Arrange
            var json = @"{
                ""plugins"": [
                    {
                        ""name"": ""Test"",
                        ""enabled"": true,
                        ""version"": ""1.0.0""
                    }
                ]
            }";

            // Act
            var settings = JsonSerializer.Deserialize<PluginSettingsJson>(json);

            // Assert
            Assert.NotNull(settings);
            Assert.Single(settings.Plugins);
            Assert.Equal("Test", settings.Plugins[0].Name);
        }

        // Test validation success
        [Fact]
        public void Validator_AcceptsValidConfiguration()
        {
            // Arrange
            var validator = new PluginSettingsValidator();
            var settings = PluginSettingsJson.CreateDefault();

            // Act
            var result = validator.Validate(settings);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        // Test validation failure
        [Fact]
        public void Validator_RejectsEmptyPluginList()
        {
            // Arrange
            var validator = new PluginSettingsValidator();
            var settings = new PluginSettingsJson { Plugins = new() };

            // Act
            var result = validator.TestValidate(settings);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Plugins);
        }

        // Test specific property validation
        [Theory]
        [InlineData("1.0.0", true)]
        [InlineData("2.1.3", true)]
        [InlineData("invalid", false)]
        [InlineData("1.0", false)]
        public void Validator_ChecksVersionFormat(string version, bool shouldBeValid)
        {
            // Arrange
            var validator = new PluginValidator();
            var plugin = new PluginJson
            {
                Name = "Test",
                Enabled = true,
                Version = version
            };

            // Act
            var result = validator.Validate(plugin);

            // Assert
            Assert.Equal(shouldBeValid, result.IsValid);
        }

        // Test factory method
        [Fact]
        public void CreateDefault_ReturnsValidConfiguration()
        {
            // Act
            var settings = PluginSettingsJson.CreateDefault();

            // Assert
            Assert.NotNull(settings);
            Assert.NotEmpty(settings.Plugins);

            var validator = new PluginSettingsValidator();
            var result = validator.Validate(settings);
            Assert.True(result.IsValid);
        }

        // Test round-trip serialization
        [Fact]
        public void RoundTrip_PreservesData()
        {
            // Arrange
            var original = PluginSettingsJson.CreateDefault();
            var options = new JsonSerializerOptions { WriteIndented = true };

            // Act
            var json = JsonSerializer.Serialize(original, options);
            var deserialized = JsonSerializer.Deserialize<PluginSettingsJson>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.Plugins.Count, deserialized.Plugins.Count);
            Assert.Equal(original.Plugins[0].Name, deserialized.Plugins[0].Name);
        }
    }
}
```

### Integration Tests

```csharp
[Fact]
public void Integration_LoadSaveAndValidate()
{
    // Arrange
    var tempFile = Path.GetTempFileName();
    var logger = new NullLogger<JsonConfigurationLoader<PluginSettingsJson>>();
    var loader = new JsonConfigurationLoader<PluginSettingsJson>(
        new PluginSettingsValidator(),
        logger
    );

    try
    {
        // Create and save
        var original = PluginSettingsJson.CreateDefault();
        Assert.True(loader.Save(original, tempFile));

        // Load and verify
        var loaded = loader.Load(tempFile);
        Assert.NotNull(loaded);
        Assert.Equal(original.Plugins.Count, loaded.Plugins.Count);
    }
    finally
    {
        File.Delete(tempFile);
    }
}
```

---

## Best Practices

### 1. Follow Naming Conventions

- **POCO Classes:** `ConfigTypeJson` (e.g., `PluginSettingsJson`)
- **Validators:** `ConfigTypeValidator` (e.g., `PluginSettingsValidator`)
- **JSON Properties:** camelCase (e.g., `pluginName`)
- **C# Properties:** PascalCase (e.g., `PluginName`)
- **Schema Files:** kebab-case (e.g., `plugin-settings.schema.json`)

### 2. Always Provide Defaults

```csharp
public class PluginJson
{
    public string Name { get; set; } = string.Empty;  // ✅ Good
    public List<ItemJson> Items { get; set; } = new();  // ✅ Good
    
    // ❌ Bad - can cause NullReferenceException
    public List<ItemJson> Items { get; set; }
}
```

### 3. Use Enums for Fixed Values

```csharp
public enum ModeEnum
{
    Write,
    Speak,
    None
}

public class ConfigJson
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ModeEnum Mode { get; set; }
}
```

### 4. Document Everything

```csharp
/// <summary>
/// Configuration for ACAT plugins
/// </summary>
public class PluginSettingsJson
{
    /// <summary>
    /// List of plugin configurations
    /// </summary>
    [JsonPropertyName("plugins")]
    public List<PluginJson> Plugins { get; set; } = new();
}
```

### 5. Validate Before Use

```csharp
var validator = new PluginSettingsValidator();
var result = validator.Validate(settings);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        _logger.LogError($"Validation error: {error.ErrorMessage}");
    }
    return PluginSettingsJson.CreateDefault();
}
```

### 6. Handle Errors Gracefully

```csharp
try
{
    var config = loader.Load(filePath);
    return config ?? PluginSettingsJson.CreateDefault();
}
catch (JsonException ex)
{
    _logger.LogError(ex, "Failed to parse JSON configuration");
    return PluginSettingsJson.CreateDefault();
}
catch (IOException ex)
{
    _logger.LogError(ex, "Failed to read configuration file");
    return PluginSettingsJson.CreateDefault();
}
```

### 7. Test Thoroughly

- Unit tests for POCO deserialization
- Unit tests for all validation rules
- Integration tests for load/save cycle
- Test with example files
- Test error conditions

### 8. Keep Schemas in Sync

When updating POCO classes, also update:
- JSON Schema file
- FluentValidation validator
- Example files
- Documentation

### 9. Use Factory Methods

```csharp
// ✅ Good - factory method
var settings = PluginSettingsJson.CreateDefault();

// ❌ Bad - manual construction
var settings = new PluginSettingsJson 
{ 
    Plugins = new List<PluginJson> 
    { 
        new PluginJson { /* ... */ }
    }
};
```

### 10. Consider Backward Compatibility

When adding new properties:
```csharp
// ✅ Good - optional with default
public string NewFeature { get; set; } = "default";

// ❌ Bad - breaks existing configs
public string NewFeature { get; set; }  // Required but not in old files
```

---

## Common Pitfalls

### 1. Null Reference Exceptions

```csharp
// ❌ Bad
public List<ItemJson> Items { get; set; }

// Later...
settings.Items.Add(item);  // NullReferenceException if Items is null

// ✅ Good
public List<ItemJson> Items { get; set; } = new();
```

### 2. Case-Sensitive Deserialization

```csharp
// ❌ Bad - case-sensitive by default
var options = new JsonSerializerOptions();

// ✅ Good - case-insensitive
var options = new JsonSerializerOptions 
{ 
    PropertyNameCaseInsensitive = true 
};
```

### 3. Forgetting Enum Converter

```csharp
// ❌ Bad - serializes as number
public ModeEnum Mode { get; set; }

// ✅ Good - serializes as string
[JsonConverter(typeof(JsonStringEnumConverter))]
public ModeEnum Mode { get; set; }
```

### 4. Not Handling Validation Errors

```csharp
// ❌ Bad - silently fails
var result = validator.Validate(settings);

// ✅ Good - logs and handles errors
var result = validator.Validate(settings);
if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        _logger.LogError(error.ErrorMessage);
    }
    return CreateDefault();
}
```

---

## Additional Resources

### Code Examples
- **POCO Classes:** `src/Libraries/ACATCore/Configuration/`
- **Validators:** `src/Libraries/ACATCore/Validation/`
- **Tests:** `src/Libraries/ACATCore.Tests.Configuration/`

### Documentation
- **User Guide:** `docs/JSON_CONFIGURATION_GUIDE.md`
- **Schema README:** `schemas/README.md`
- **Migration Guide:** `docs/JSON_CONFIGURATION_MIGRATION.md`

### External References
- [JSON Schema Specification](http://json-schema.org/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [System.Text.Json Documentation](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview)

---

## License

Copyright 2013-2019; 2023 Intel Corporation  
SPDX-License-Identifier: Apache-2.0
