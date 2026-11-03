# JSON Configuration Testing Guide

## Overview

This document describes how to test the JSON schemas, POCO classes, and validators created for ACAT's top 5 configurations.

## Test Prerequisites

1. Visual Studio Code with JSON extension
2. .NET Framework 4.8.1 or later
3. FluentValidation 11.9.0

## 1. JSON Schema Validation Tests

### VS Code IntelliSense Test

1. Open VS Code
2. Open `/schemas/examples/actuator-settings.example.json`
3. Verify the file has `"$schema": "../json/actuator-settings.schema.json"` at the top
4. Test autocomplete:
   - Type `"` and press Ctrl+Space - should show property suggestions
   - Start typing `"actuator` - should autocomplete to `"actuatorSettings"`
5. Test validation:
   - Remove a required field (e.g., `"name"`) - should show red squiggle
   - Add an invalid GUID - should show validation error
   - Use invalid color in theme - should show error

**Expected Results:**
- ✅ IntelliSense shows property suggestions
- ✅ Required fields are validated
- ✅ Data types are validated
- ✅ Patterns (GUID, color) are validated

### Command-Line Schema Validation

```bash
# Install ajv-cli (JSON schema validator)
npm install -g ajv-cli

# Validate actuator settings
ajv validate -s schemas/json/actuator-settings.schema.json \
    -d schemas/examples/actuator-settings.example.json

# Validate theme
ajv validate -s schemas/json/theme.schema.json \
    -d schemas/examples/theme.example.json

# Validate panel configs
ajv validate -s schemas/json/panel-config.schema.json \
    -d schemas/examples/main-menu.example.json
```

**Expected Results:**
- ✅ All example files pass validation
- ✅ Invalid files produce clear error messages

## 2. C# Deserialization Tests

### Manual Testing

Create a test console application:

```csharp
using ACAT.Core.Configuration;
using System;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main()
    {
        // Test ActuatorSettings
        var actuatorJson = File.ReadAllText("schemas/examples/actuator-settings.example.json");
        var actuatorSettings = JsonSerializer.Deserialize<ActuatorSettingsJson>(actuatorJson);
        
        Console.WriteLine($"Loaded {actuatorSettings.ActuatorSettings.Count} actuators");
        Console.WriteLine($"First actuator: {actuatorSettings.ActuatorSettings[0].Name}");
        
        // Test Theme
        var themeJson = File.ReadAllText("schemas/examples/theme.example.json");
        var theme = JsonSerializer.Deserialize<ThemeJson>(themeJson);
        
        Console.WriteLine($"Theme: {theme.Description}");
        Console.WriteLine($"Color schemes: {theme.ColorSchemes.Count}");
        
        // Test Panel Config
        var panelJson = File.ReadAllText("schemas/examples/main-menu.example.json");
        var panel = JsonSerializer.Deserialize<PanelConfigJson>(panelJson);
        
        Console.WriteLine($"Layout color scheme: {panel.Layout.ColorScheme}");
        Console.WriteLine($"Widget attributes: {panel.WidgetAttributes.Count}");
    }
}
```

**Expected Results:**
- ✅ All example files deserialize without errors
- ✅ Properties are correctly mapped
- ✅ Nested objects are properly created
- ✅ Arrays and lists are populated

### Round-Trip Serialization Test

```csharp
// Create settings programmatically
var settings = ActuatorSettingsJson.CreateDefault();

// Serialize to JSON
var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions 
{ 
    WriteIndented = true 
});

// Deserialize back
var deserialized = JsonSerializer.Deserialize<ActuatorSettingsJson>(json);

// Verify equality
Assert.AreEqual(settings.ActuatorSettings.Count, deserialized.ActuatorSettings.Count);
Assert.AreEqual(settings.ActuatorSettings[0].Name, deserialized.ActuatorSettings[0].Name);
```

**Expected Results:**
- ✅ Serialization produces valid JSON
- ✅ Deserialization recreates equivalent object
- ✅ No data loss in round-trip

## 3. FluentValidation Tests

### Valid Configuration Tests

```csharp
using ACAT.Core.Configuration;
using ACAT.Core.Validation;

// Test valid ActuatorSettings
var validator = new ActuatorSettingsValidator();
var settings = ActuatorSettingsJson.CreateDefault();
var result = validator.Validate(settings);

Console.WriteLine($"Is valid: {result.IsValid}");
// Expected: true
```

**Test Cases:**
- ✅ Default settings are valid
- ✅ Factory-created settings are valid
- ✅ Example JSON files are valid

### Invalid Configuration Tests

```csharp
// Test 1: No actuators
var emptySettings = new ActuatorSettingsJson();
var result1 = validator.Validate(emptySettings);
// Expected: false, error "At least one actuator must be configured"

// Test 2: No enabled actuators
var disabledSettings = new ActuatorSettingsJson
{
    ActuatorSettings = new List<ActuatorSettingJson>
    {
        new ActuatorSettingJson 
        { 
            Name = "Test", 
            Id = Guid.NewGuid().ToString(), 
            Enabled = false 
        }
    }
};
var result2 = validator.Validate(disabledSettings);
// Expected: false, error "At least one actuator must be enabled"

// Test 3: Invalid GUID
var invalidGuid = new ActuatorSettingsJson
{
    ActuatorSettings = new List<ActuatorSettingJson>
    {
        new ActuatorSettingJson 
        { 
            Name = "Test", 
            Id = "not-a-guid", 
            Enabled = true 
        }
    }
};
var result3 = validator.Validate(invalidGuid);
// Expected: false, error "must be a valid GUID"

// Test 4: Duplicate IDs
var duplicateIds = new ActuatorSettingsJson
{
    ActuatorSettings = new List<ActuatorSettingJson>
    {
        new ActuatorSettingJson { Name = "Test1", Id = "d91a1877-c92b-4d7e-9ab6-f01f30b12df9", Enabled = true },
        new ActuatorSettingJson { Name = "Test2", Id = "d91a1877-c92b-4d7e-9ab6-f01f30b12df9", Enabled = false }
    }
};
var result4 = validator.Validate(duplicateIds);
// Expected: false, error "Actuator IDs must be unique"
```

**Test Cases:**
- ✅ Detects missing required fields
- ✅ Detects invalid GUIDs
- ✅ Detects duplicate IDs
- ✅ Detects business rule violations

### Theme Validation Tests

```csharp
var themeValidator = new ThemeValidator();

// Test 1: Invalid color format
var invalidColor = new ThemeJson
{
    Description = "Test",
    ColorSchemes = new List<ColorSchemeJson>
    {
        new ColorSchemeJson 
        { 
            Name = "Scanner", 
            Background = "not-a-color", 
            Foreground = "White" 
        }
    }
};
var result = themeValidator.Validate(invalidColor);
// Expected: false, error "must be a valid color"

// Test 2: Duplicate scheme names
var duplicateSchemes = new ThemeJson
{
    Description = "Test",
    ColorSchemes = new List<ColorSchemeJson>
    {
        new ColorSchemeJson { Name = "Scanner", Background = "#232433", Foreground = "White" },
        new ColorSchemeJson { Name = "Scanner", Background = "#111111", Foreground = "Black" }
    }
};
var result2 = themeValidator.Validate(duplicateSchemes);
// Expected: false, error "Color scheme names must be unique"
```

## 4. Unit Test Execution

### Run All Configuration Tests

```bash
cd src/Libraries/ACATCore.Tests.Configuration
dotnet test
```

**Expected Output:**
```
Test run for ACATCore.Tests.Configuration.dll (.NETFramework,Version=v4.8.1)
Total tests: 44
     Passed: 44
 Total time: 2.5 Seconds
```

### Run Specific Test Class

```bash
dotnet test --filter FullyQualifiedName~ActuatorSettingsJsonTests
dotnet test --filter FullyQualifiedName~ActuatorSettingsValidatorTests
dotnet test --filter FullyQualifiedName~ThemeJsonTests
dotnet test --filter FullyQualifiedName~ThemeValidatorTests
dotnet test --filter FullyQualifiedName~PanelConfigJsonTests
dotnet test --filter FullyQualifiedName~PanelConfigValidatorTests
```

## 5. Integration Tests

### Test 1: Load XML and Compare with JSON

```csharp
// Load existing XML configuration
var xmlPath = "src/Applications/Install/Users/DefaultUser/ActuatorSettings.xml";
var xmlConfig = ActuatorConfig.Load(xmlPath);

// Load equivalent JSON configuration
var jsonPath = "schemas/examples/actuator-settings.example.json";
var jsonConfig = JsonSerializer.Deserialize<ActuatorSettingsJson>(
    File.ReadAllText(jsonPath));

// Verify equivalence
Assert.AreEqual(xmlConfig.ActuatorSettings.Count, 
    jsonConfig.ActuatorSettings.Count);

for (int i = 0; i < xmlConfig.ActuatorSettings.Count; i++)
{
    Assert.AreEqual(xmlConfig.ActuatorSettings[i].Name, 
        jsonConfig.ActuatorSettings[i].Name);
    Assert.AreEqual(xmlConfig.ActuatorSettings[i].Id.ToString(), 
        jsonConfig.ActuatorSettings[i].Id);
    Assert.AreEqual(xmlConfig.ActuatorSettings[i].Enabled, 
        jsonConfig.ActuatorSettings[i].Enabled);
}
```

### Test 2: Validate All Example Files

```csharp
var exampleFiles = new[]
{
    "schemas/examples/actuator-settings.example.json",
    "schemas/examples/theme.example.json",
    "schemas/examples/main-menu.example.json",
    "schemas/examples/talk-application-scanner.example.json",
    "schemas/examples/keyboard-qwerty.example.json"
};

foreach (var file in exampleFiles)
{
    Console.WriteLine($"Testing {Path.GetFileName(file)}...");
    
    // Deserialize
    var json = File.ReadAllText(file);
    object config = null;
    
    if (file.Contains("actuator"))
        config = JsonSerializer.Deserialize<ActuatorSettingsJson>(json);
    else if (file.Contains("theme"))
        config = JsonSerializer.Deserialize<ThemeJson>(json);
    else
        config = JsonSerializer.Deserialize<PanelConfigJson>(json);
    
    Assert.IsNotNull(config);
    Console.WriteLine($"  ✓ Deserialized successfully");
    
    // Validate
    // (validation code here)
    
    Console.WriteLine($"  ✓ Validation passed");
}
```

## 6. Performance Tests

### Deserialization Performance

```csharp
var json = File.ReadAllText("schemas/examples/actuator-settings.example.json");
var iterations = 1000;

var sw = Stopwatch.StartNew();
for (int i = 0; i < iterations; i++)
{
    var settings = JsonSerializer.Deserialize<ActuatorSettingsJson>(json);
}
sw.Stop();

Console.WriteLine($"Avg deserialization time: {sw.ElapsedMilliseconds / iterations}ms");
// Expected: < 5ms per deserialization
```

### Validation Performance

```csharp
var settings = ActuatorSettingsJson.CreateDefault();
var validator = new ActuatorSettingsValidator();
var iterations = 1000;

var sw = Stopwatch.StartNew();
for (int i = 0; i < iterations; i++)
{
    var result = validator.Validate(settings);
}
sw.Stop();

Console.WriteLine($"Avg validation time: {sw.ElapsedMilliseconds / iterations}ms");
// Expected: < 2ms per validation
```

## Test Results Summary

### Expected Test Coverage

| Component | Tests | Description |
|-----------|-------|-------------|
| ActuatorSettingsJson | 8 | Serialization, deserialization, factory methods |
| ActuatorSettingsValidator | 12 | All validation rules |
| ThemeJson | 5 | Serialization, deserialization, factory methods |
| ThemeValidator | 8 | All validation rules |
| PanelConfigJson | 3 | Serialization, deserialization |
| PanelConfigValidator | 8 | All validation rules |
| **Total** | **44** | |

### Success Criteria

- ✅ All unit tests pass
- ✅ All example JSON files are valid
- ✅ VS Code IntelliSense works correctly
- ✅ Deserialization works for all configurations
- ✅ All validators catch invalid configurations
- ✅ Round-trip serialization preserves data
- ✅ Performance meets requirements

## Known Issues

1. **Build on Linux**: The ACATResources project requires PowerShell for dependency extraction, which fails on Linux. Use Windows or manually extract dependencies.

2. **Test Execution**: Currently tests require building on Windows due to dependency issues. Tests are validated to compile but not yet executed in CI.

## Next Steps

1. Execute tests on Windows environment
2. Add tests to CI/CD pipeline
3. Create XML-to-JSON migration utility
4. Add integration tests with real ACAT components
5. Performance benchmarking
6. Add tests for remaining configuration types
