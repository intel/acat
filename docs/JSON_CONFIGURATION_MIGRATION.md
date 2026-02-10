# JSON Configuration Migration Guide

## Overview
ACAT has migrated from XML to JSON for configuration files. This guide explains the changes and how to migrate existing configurations.

## Migrated Configuration Types

### ✅ Abbreviations Configuration
- **Old Format**: `Abbreviations.xml`
- **New Format**: `Abbreviations.json`
- **POCO**: `AbbreviationsJson` (namespace: `ACAT.Core.Configuration`)
- **Validator**: `AbbreviationsValidator` (namespace: `ACAT.Core.Validation`)

#### JSON Structure
```json
{
  "abbreviations": [
    {
      "word": "btw",
      "replaceWith": "by the way",
      "mode": "Write"
    }
  ]
}
```

#### Valid Mode Values
- `Write` - Expand abbreviation as text
- `Speak` - Speak the expansion
- `None` - No expansion

#### Example
See `/schemas/examples/abbreviations.example.json` for a complete example.

### ✅ Pronunciations Configuration
- **Old Format**: `Pronunciations.xml`
- **New Format**: `Pronunciations.json`
- **POCO**: `PronunciationsJson` (namespace: `ACAT.Core.Configuration`)
- **Validator**: `PronunciationsValidator` (namespace: `ACAT.Core.Validation`)

#### JSON Structure
```json
{
  "pronunciations": [
    {
      "word": "github",
      "pronunciation": "git hub"
    }
  ]
}
```

#### Example
See `/schemas/examples/pronunciations.example.json` for a complete example.

### ✅ Previously Migrated (from Ticket #7)
- **Actuator Settings**: `ActuatorSettings.json`
- **Panel Configuration**: `PanelConfig.json`
- **Theme Configuration**: `Theme.json`

## Backward Compatibility

All migrated configuration classes maintain backward compatibility with XML files:

1. **File Detection**: The system checks for JSON files first, then falls back to XML if JSON is not found
2. **Automatic Format Detection**: Based on file extension (`.json` or `.xml`)
3. **Transparent Loading**: No code changes required in consuming code

### Example File Lookup Order
For `Abbreviations`:
1. Looks for `Abbreviations.json` in resources directory
2. Looks for `Abbreviations.json` in user directory
3. Falls back to `Abbreviations.xml` in resources directory
4. Falls back to `Abbreviations.xml` in user directory
5. Creates default `Abbreviations.json` if none found

## Migration Steps

### Manual Migration
1. Locate your existing XML configuration file
2. Convert to JSON format using the examples as a template
3. Save with `.json` extension
4. The system will automatically use the JSON file

### Using ConfigMigrationTool
The `ConfigMigrationTool` application can convert XML configurations to JSON automatically.

## Features

### Validation
All JSON configurations are validated using FluentValidation:
- Required fields are checked
- Data types are validated
- Enum values are verified
- Custom business rules are applied

### Error Handling
- **Invalid JSON**: Falls back to defaults with error message
- **Missing File**: Creates default configuration
- **Validation Failure**: Falls back to defaults with detailed error messages

### JSON Format Features
- **Comments**: Supports `//` and `/* */` style comments
- **Trailing Commas**: Allows trailing commas for easier editing
- **Case-Insensitive**: Property names are case-insensitive during deserialization
- **Indented Output**: Saved files are pretty-printed for readability

## Code Examples

### Loading Abbreviations
```csharp
var abbreviations = new Abbreviations();
abbreviations.Load(); // Automatically tries JSON first, then XML

// Or specify a file
abbreviations.Load("/path/to/Abbreviations.json");
```

### Saving Abbreviations
```csharp
var abbreviations = new Abbreviations();
abbreviations.Add(new Abbreviation("btw", "by the way", AbbreviationMode.Write));
abbreviations.Save(); // Saves as JSON
```

### Using JsonConfigurationLoader Directly
```csharp
var loader = new JsonConfigurationLoader<AbbreviationsJson>(
    new AbbreviationsValidator(),
    logger
);

var config = loader.Load("/path/to/Abbreviations.json");
```

## Testing

Comprehensive test coverage is provided in:
- `ACATCore.Tests.Configuration/AbbreviationsTests.cs`
- `ACATCore.Tests.Configuration/PronunciationsTests.cs`

Run tests with:
```bash
cd src/Libraries/ACATCore.Tests.Configuration
dotnet test
```

## TTS Engine Settings

TTS engine pronunciation file references have been updated:
- **SAPI Engine**: `SAPIPronunciations.json` (was `.xml`)
- **TTS Client**: `TTSPronunciations.json` (was `.xml`)

Legacy XML files will still work due to backward compatibility.

## Troubleshooting

### "Configuration validation failed"
Check the application logs for specific validation errors. Common issues:
- Empty required fields
- Invalid enum values (e.g., mode must be "Write", "Speak", or "None")
- Malformed JSON syntax

### "Configuration file not found"
The system will create a default configuration. Check:
1. File path is correct
2. File has read permissions
3. File extension is `.json` or `.xml`

### "JSON parsing error"
Check JSON syntax:
- All brackets and braces are matched
- Strings are properly quoted
- No trailing commas at the end of arrays/objects (unless using comment-tolerant parser)

## Future Migrations

Other configuration types may be migrated to JSON in future releases. The pattern established here will be followed:
1. Create JSON POCO
2. Create FluentValidation validator
3. Update loader to support both JSON and XML
4. Add comprehensive tests
5. Update documentation

## Related Files

- POCOs: `/src/Libraries/ACATCore/Configuration/`
- Validators: `/src/Libraries/ACATCore/Validation/`
- Tests: `/src/Libraries/ACATCore.Tests.Configuration/`
- Examples: `/schemas/examples/`
- JsonConfigurationLoader: `/src/Libraries/ACATCore/Utility/JsonConfigurationLoader.cs`
