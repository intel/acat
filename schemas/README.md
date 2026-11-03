# ACAT JSON Configuration Schemas

This directory contains JSON schemas, POCO classes, validators, and examples for ACAT's configuration types.

## Overview

As part of the XML-to-JSON migration effort (Issues #6, #7, and #9), we have created:
- **JSON Schemas** - For VS Code IntelliSense and validation
- **C# POCO Classes** - For strongly-typed configuration with System.Text.Json
- **FluentValidation Validators** - For business rule validation
- **Example JSON Files** - Showing proper usage

## Directory Structure

```
schemas/
├── json/                                   # JSON Schema files
│   ├── actuator-settings.schema.json      # Input device configuration
│   ├── theme.schema.json                  # Color schemes and styling
│   ├── panel-config.schema.json           # UI panel layouts
│   ├── abbreviations.schema.json          # Text abbreviation expansions
│   └── pronunciations.schema.json         # Custom word pronunciations
├── examples/                               # Example JSON files
│   ├── actuator-settings.example.json
│   ├── theme.example.json
│   ├── main-menu.example.json
│   ├── talk-application-scanner.example.json
│   ├── keyboard-qwerty.example.json
│   ├── abbreviations.example.json
│   └── pronunciations.example.json
└── README.md                               # This file

src/Libraries/ACATCore/
├── Configuration/                          # C# POCO classes
│   ├── ActuatorSettingsJson.cs
│   ├── ThemeJson.cs
│   ├── PanelConfigJson.cs
│   ├── AbbreviationsJson.cs
│   └── PronunciationsJson.cs
└── Validation/                             # FluentValidation validators
    ├── ActuatorSettingsValidator.cs
    ├── ThemeValidator.cs
    ├── PanelConfigValidator.cs
    ├── AbbreviationsValidator.cs
    └── PronunciationsValidator.cs
```

## Schemas

### 1. ActuatorSettings (Priority P1)

**Purpose:** Configure input devices (switches) - keyboard, camera, BCI, external switches

**Files:**
- Schema: `schemas/json/actuator-settings.schema.json`
- POCO: `src/Libraries/ACATCore/Configuration/ActuatorSettingsJson.cs`
- Validator: `src/Libraries/ACATCore/Validation/ActuatorSettingsValidator.cs`
- Example: `schemas/examples/actuator-settings.example.json`

**Key Features:**
- Supports multiple actuator types (Keyboard, Camera, BCI, External)
- Each actuator has multiple switch configurations
- Preserves GUIDs from XML migration
- Supports variable references (e.g., `@MinActuationHoldTime`)
- Factory methods for common scenarios

**Usage:**
```csharp
using ACAT.Core.Configuration;
using ACAT.Core.Validation;
using System.Text.Json;

// Deserialize from JSON
var json = File.ReadAllText("actuator-settings.json");
var settings = JsonSerializer.Deserialize<ActuatorSettingsJson>(json);

// Validate
var validator = new ActuatorSettingsValidator();
var result = validator.Validate(settings);
if (!result.IsValid)
{
    foreach (var error in result.Errors)
        Console.WriteLine(error.ErrorMessage);
}

// Create default settings
var defaultSettings = ActuatorSettingsJson.CreateDefault();
```

### 2. Theme

**Purpose:** Color schemes and styling for ACAT user interface

**Files:**
- Schema: `schemas/json/theme.schema.json`
- POCO: `src/Libraries/ACATCore/Configuration/ThemeJson.cs`
- Validator: `src/Libraries/ACATCore/Validation/ThemeValidator.cs`
- Example: `schemas/examples/theme.example.json`

**Key Features:**
- Multiple color schemes for different UI elements
- Supports color names (e.g., "White") and hex codes (e.g., "#232433")
- Optional background images
- Separate colors for normal, highlight, and selected states
- Factory methods for common themes

**Usage:**
```csharp
using ACAT.Core.Configuration;
using System.Text.Json;

// Create default high contrast theme
var theme = ThemeJson.CreateDefaultHighContrast();

// Serialize to JSON
var json = JsonSerializer.Serialize(theme, new JsonSerializerOptions 
{ 
    WriteIndented = true 
});
File.WriteAllText("theme.json", json);
```

### 3. PanelConfig (MainMenu, TalkApplicationScanner, KeyboardQwertyUserControl)

**Purpose:** UI layout configuration for scanners, keyboards, menus, and dialogs

**Files:**
- Schema: `schemas/json/panel-config.schema.json`
- POCO: `src/Libraries/ACATCore/Configuration/PanelConfigJson.cs`
- Validator: `src/Libraries/ACATCore/Validation/PanelConfigValidator.cs`
- Examples:
  - `schemas/examples/main-menu.example.json`
  - `schemas/examples/talk-application-scanner.example.json`
  - `schemas/examples/keyboard-qwerty.example.json`

**Key Features:**
- Widget attributes with font, label, and command settings
- Hierarchical layout with nested widgets
- Animation sequences for scanning behavior
- Support for DTD entity references (preserved from XML)
- Contextual widget enabling

**Usage:**
```csharp
using ACAT.Core.Configuration;
using ACAT.Core.Validation;
using System.Text.Json;

// Load and validate panel configuration
var json = File.ReadAllText("main-menu.json");
var panel = JsonSerializer.Deserialize<PanelConfigJson>(json);

var validator = new PanelConfigValidator();
var result = validator.Validate(panel);
```

### 4. Abbreviations

**Purpose:** Text abbreviation expansions for faster typing

**Files:**
- Schema: `schemas/json/abbreviations.schema.json`
- POCO: `src/Libraries/ACATCore/Configuration/AbbreviationsJson.cs`
- Validator: `src/Libraries/ACATCore/Validation/AbbreviationsValidator.cs`
- Example: `schemas/examples/abbreviations.example.json`

**Key Features:**
- Expand abbreviations to full text (e.g., "btw" → "by the way")
- Multiple expansion modes (Write, Speak, None)
- Simple word and replacement pairs
- Factory methods for common abbreviations

**Usage:**
```csharp
using ACAT.Core.Configuration;
using System.Text.Json;

// Load abbreviations
var json = File.ReadAllText("abbreviations.json");
var abbreviations = JsonSerializer.Deserialize<AbbreviationsJson>(json);

// Create default abbreviations
var defaultAbbreviations = AbbreviationsJson.CreateDefault();
```

### 5. Pronunciations

**Purpose:** Custom word pronunciations for text-to-speech engines

**Files:**
- Schema: `schemas/json/pronunciations.schema.json`
- POCO: `src/Libraries/ACATCore/Configuration/PronunciationsJson.cs`
- Validator: `src/Libraries/ACATCore/Validation/PronunciationsValidator.cs`
- Example: `schemas/examples/pronunciations.example.json`

**Key Features:**
- Customize how words are pronounced by TTS
- Simple word and pronunciation pairs
- Supports phonetic spelling
- Factory methods for common technical terms

**Usage:**
```csharp
using ACAT.Core.Configuration;
using System.Text.Json;

// Load pronunciations
var json = File.ReadAllText("pronunciations.json");
var pronunciations = JsonSerializer.Deserialize<PronunciationsJson>(json);

// Create default pronunciations
var defaultPronunciations = PronunciationsJson.CreateDefault();
```

## VS Code IntelliSense

To enable IntelliSense in VS Code:

1. Open your JSON file
2. Add the `$schema` property at the top:
   ```json
   {
     "$schema": "../json/actuator-settings.schema.json",
     ...
   }
   ```
3. VS Code will now provide autocomplete and validation

## Migration Notes

### From XML to JSON

When migrating XML configurations to JSON:

1. **Preserve GUIDs** - Actuator and panel IDs must remain the same
2. **Variable References** - Keep references like `@MinActuationHoldTime`, `@CmdEnterKey`
3. **DTD Entities** - Convert to variables or inline values
4. **Nested Structures** - XML hierarchy maps to JSON objects/arrays
5. **Validation** - Use FluentValidation validators to ensure correctness

### Special Handling

- **Variable References:** Strings starting with `@` are variable references
- **File Paths:** Relative paths for images, audio files
- **HTML Markup:** Descriptions may contain HTML for help links
- **Unicode:** Preserved in JSON (no special encoding needed)

## Validation Rules

### ActuatorSettings

- ✅ At least one actuator required
- ✅ At least one actuator must be enabled
- ✅ Actuator IDs must be unique and valid GUIDs
- ✅ Enabled actuators need at least one enabled switch
- ✅ Switch names must be unique within an actuator
- ✅ Enabled switches that actuate must have a command

### Theme

- ✅ At least one color scheme required
- ✅ Color scheme names must be unique
- ✅ Colors must be valid (hex codes or color names)
- ✅ Essential schemes like "Scanner" should be present
- ✅ Background color or image required for each scheme
- ✅ Foreground recommended when background is specified

### PanelConfig

- ✅ Widget attribute names must be unique
- ✅ Animation names must be unique
- ✅ Container widgets should have children
- ✅ Animations should have at least one step
- ✅ All widget references in animations must exist

### Abbreviations

- ✅ Word field is required and cannot be empty
- ✅ ReplaceWith field is required and cannot be empty
- ✅ Mode must be one of: "Write", "Speak", or "None"
- ✅ Word must be at least 1 character long
- ✅ ReplaceWith must be at least 1 character long

### Pronunciations

- ✅ Word field is required and cannot be empty
- ✅ Pronunciation field is required and cannot be empty
- ✅ Word must be at least 1 character long
- ✅ Pronunciation must be at least 1 character long

## Testing

### Unit Tests

To test deserialization and validation:

```csharp
using System.Text.Json;
using FluentValidation.TestHelper;
using Xunit;

public class ActuatorSettingsTests
{
    [Fact]
    public void CanDeserializeActuatorSettings()
    {
        var json = File.ReadAllText("actuator-settings.example.json");
        var settings = JsonSerializer.Deserialize<ActuatorSettingsJson>(json);
        Assert.NotNull(settings);
        Assert.NotEmpty(settings.ActuatorSettings);
    }

    [Fact]
    public void ValidatorDetectsMissingName()
    {
        var validator = new ActuatorSettingValidator();
        var actuator = new ActuatorSettingJson { Name = "" };
        var result = validator.TestValidate(actuator);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
```

### Manual Testing

1. **JSON Schema Validation:**
   - Open example files in VS Code
   - Check for red squiggles (validation errors)
   - Test autocomplete (Ctrl+Space)

2. **Deserialization:**
   - Load JSON files using System.Text.Json
   - Verify all properties are correctly mapped

3. **Validation:**
   - Run FluentValidation validators
   - Test both valid and invalid configurations
   - Verify error messages are clear

## Dependencies

- **System.Text.Json** (9.0.7) - JSON serialization
- **FluentValidation** (11.9.0) - Validation rules
- **System.ComponentModel.DataAnnotations** - Basic attributes

## Future Work

1. **Additional Schemas** - Create schemas for remaining configuration types
2. **Migration Tool** - Automated XML-to-JSON converter
3. **Integration Tests** - End-to-end testing with real ACAT components
4. **Documentation** - Expand with more examples and tutorials
5. **Schema Versioning** - Support for backward compatibility

## References

- Issue #6: XML Configuration Analysis
- Issue #7: JSON Schema Generation (this issue)
- [JSON Schema Specification](http://json-schema.org/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [System.Text.Json Documentation](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview)

## License

Copyright 2013-2019; 2023 Intel Corporation  
SPDX-License-Identifier: Apache-2.0
