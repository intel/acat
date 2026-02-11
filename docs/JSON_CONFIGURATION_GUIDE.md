# ACAT JSON Configuration Guide

**Version:** 1.0  
**Last Updated:** February 2026

## Table of Contents

1. [Overview](#overview)
2. [Migration from XML](#migration-from-xml)
3. [Configuration Files Reference](#configuration-files-reference)
4. [VS Code IntelliSense Setup](#vs-code-intellisense-setup)
5. [Troubleshooting](#troubleshooting)
6. [Best Practices](#best-practices)

---

## Overview

ACAT has modernized its configuration system by migrating from XML to JSON format. This change provides several benefits:

### Benefits of JSON Configuration

- **🚀 Better Tooling:** IntelliSense support in VS Code and other editors
- **✅ Validation:** Real-time schema validation while editing
- **📝 Simpler Syntax:** More readable and easier to edit than XML
- **🔧 Modern Standard:** JSON is the industry standard for configuration
- **🛡️ Type Safety:** Built-in validation with FluentValidation
- **📖 Self-Documenting:** Schema provides inline documentation

### Supported Configuration Types

ACAT supports five main JSON configuration types:

1. **ActuatorSettings** - Input device configurations (keyboard, camera, BCI, switches)
2. **Theme** - Color schemes and UI styling
3. **PanelConfig** - UI panel layouts (scanners, keyboards, menus)
4. **Abbreviations** - Text abbreviation expansions
5. **Pronunciations** - Custom word pronunciations for text-to-speech

---

## Migration from XML

### Using the ConfigMigrationTool

The `ConfigMigrationTool` is a command-line utility that automates the migration of XML configurations to JSON format.

#### Step 1: Locate Your Configuration Files

Your ACAT configuration files are typically located in:
- **User Configurations:** `%APPDATA%\ACAT\Config\`
- **Default Configurations:** `C:\Program Files\ACAT\Install\Users\DefaultUser\`

#### Step 2: Run the Migration Tool

**Option A: Safe Migration (Recommended)**

```bash
# Preview changes first
ConfigMigrationTool migrate --input "C:\Users\YourName\AppData\Roaming\ACAT\Config" --output "C:\Users\YourName\AppData\Roaming\ACAT\ConfigJSON" --dry-run

# Run actual migration with backup
ConfigMigrationTool migrate --input "C:\Users\YourName\AppData\Roaming\ACAT\Config" --output "C:\Users\YourName\AppData\Roaming\ACAT\ConfigJSON" --backup

# Validate the results
ConfigMigrationTool validate --input "C:\Users\YourName\AppData\Roaming\ACAT\ConfigJSON"
```

**Option B: Quick Migration**

```bash
ConfigMigrationTool migrate -i "C:\ACAT\Config" -o "C:\ACAT\ConfigJSON" --backup
```

#### Step 3: Review Migration Report

After migration, review the report for:
- **✓ Successfully Converted:** Files that migrated without issues
- **⚠ Warnings:** Validation issues (files still work but may need attention)
- **✗ Errors:** Files that failed to convert

#### Step 4: Update ACAT to Use JSON

Replace your old XML configuration directory with the new JSON directory, or configure ACAT to use the new location.

### Manual Migration

If you prefer to migrate manually:

1. Copy the appropriate example file from `schemas/examples/`
2. Open your old XML configuration
3. Transfer the settings to the JSON format
4. Save with `.json` extension
5. Validate using VS Code with IntelliSense

---

## Configuration Files Reference

### 1. ActuatorSettings.json

**Purpose:** Configure input devices (switches) for ACAT control

**Location:** `%APPDATA%\ACAT\Config\ActuatorSettings.json`

**Schema:** `schemas/json/actuator-settings.schema.json`

#### Schema Reference

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `actuatorSettings` | array | Yes | List of input device configurations |
| `actuatorSettings[].name` | string | Yes | Device name (e.g., "Keyboard", "Camera") |
| `actuatorSettings[].id` | string (GUID) | Yes | Unique identifier for the actuator |
| `actuatorSettings[].description` | string | No | User-friendly description with optional HTML |
| `actuatorSettings[].enabled` | boolean | Yes | Enable/disable this device |
| `actuatorSettings[].switchSettings` | array | Yes | Button/key mappings for this device |
| `switchSettings[].name` | string | Yes | Switch name (e.g., "Trigger") |
| `switchSettings[].source` | string | Yes | Input source (e.g., "F12", "LeftClick") |
| `switchSettings[].enabled` | boolean | Yes | Enable/disable this switch |
| `switchSettings[].actuate` | boolean | Yes | Whether this switch triggers actions |
| `switchSettings[].command` | string | Conditional | Command to execute (required if actuate=true) |
| `switchSettings[].minHoldTime` | string/number | No | Minimum hold time in milliseconds |
| `switchSettings[].beepFile` | string | No | Audio feedback file path |

#### Example

```json
{
  "$schema": "../json/actuator-settings.schema.json",
  "actuatorSettings": [
    {
      "name": "Keyboard",
      "id": "d91a1877-c92b-4d7e-9ab6-f01f30b12df9",
      "description": "Use the computer keyboard as a switch to control ACAT.",
      "enabled": true,
      "imageFileName": "KeyboardSwitch.jpg",
      "switchSettings": [
        {
          "name": "Trigger",
          "source": "F12",
          "description": "Main trigger key",
          "enabled": true,
          "actuate": true,
          "command": "@Trigger",
          "minHoldTime": "@MinActuationHoldTime",
          "beepFile": "beep.wav"
        }
      ]
    }
  ]
}
```

#### Variable References

ActuatorSettings supports variable references prefixed with `@`:
- `@Trigger` - Default trigger command
- `@MinActuationHoldTime` - System-defined minimum hold time
- `@CmdEnterKey` - Enter key command

#### Common Commands

| Command | Description |
|---------|-------------|
| `@Trigger` | Primary selection/activation |
| `@CmdEnterKey` | Simulate Enter key |
| `@CmdTabKey` | Simulate Tab key |
| `@CmdUndo` | Undo last action |

---

### 2. Theme.json

**Purpose:** Define color schemes and styling for ACAT UI

**Location:** `%APPDATA%\ACAT\Config\Theme.json`

**Schema:** `schemas/json/theme.schema.json`

#### Schema Reference

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `description` | string | No | Theme description |
| `colorSchemes` | array | Yes | List of color schemes for different UI elements |
| `colorSchemes[].name` | string | Yes | Scheme name (e.g., "Scanner", "ScannerButton") |
| `colorSchemes[].background` | string | Yes | Background color (hex or color name) |
| `colorSchemes[].foreground` | string | No | Foreground/text color |
| `colorSchemes[].highlightBackground` | string | No | Highlighted state background |
| `colorSchemes[].highlightForeground` | string | No | Highlighted state foreground |
| `colorSchemes[].selectedBackground` | string | No | Selected state background |
| `colorSchemes[].selectedForeground` | string | No | Selected state foreground |
| `colorSchemes[].backgroundImage` | string | No | Path to background image file |

#### Example

```json
{
  "$schema": "../json/theme.schema.json",
  "description": "Default high contrast theme for ACAT",
  "colorSchemes": [
    {
      "name": "Scanner",
      "background": "#232433",
      "foreground": "White",
      "highlightBackground": "#ffaa00",
      "highlightForeground": "#232433"
    },
    {
      "name": "ScannerButton",
      "background": "#5865F2",
      "foreground": "White",
      "highlightBackground": "#ffaa00",
      "highlightForeground": "Black"
    }
  ]
}
```

#### Color Format

Colors can be specified in two formats:
- **Hex Codes:** `#RRGGBB` or `#AARRGGBB` (e.g., `#232433`, `#FF5865F2`)
- **Color Names:** Standard color names (e.g., `White`, `Black`, `Red`, `Blue`)

#### Essential Color Schemes

For proper ACAT functionality, include these schemes:
- `Scanner` - Main scanner window
- `ScannerButton` - Scanner buttons
- `ContextMenu` - Context menus
- `DialogPanel` - Dialog boxes
- `TalkWindow` - Talk window interface

---

### 3. PanelConfig.json

**Purpose:** Define UI panel layouts for scanners, keyboards, menus

**Location:** `%APPDATA%\ACAT\Config\Panels\[PanelName].json`

**Schema:** `schemas/json/panel-config.schema.json`

#### Schema Reference

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `widgetAttributes` | array | Yes | Widget definitions with properties |
| `widgetAttributes[].name` | string | Yes | Widget unique identifier |
| `widgetAttributes[].label` | string | No | Display text for button/label |
| `widgetAttributes[].command` | string | No | Command to execute on activation |
| `widgetAttributes[].font` | object | No | Font settings (family, size, bold, italic) |
| `animationSequences` | array | No | Scanning animation definitions |
| `layout` | object | Yes | Hierarchical widget layout |

#### Example - Main Menu

```json
{
  "$schema": "../json/panel-config.schema.json",
  "widgetAttributes": [
    {
      "name": "BtnSettings",
      "label": "Settings",
      "command": "@CmdSettings",
      "font": { "family": "Arial", "size": 16, "bold": true }
    },
    {
      "name": "BtnExit",
      "label": "Exit",
      "command": "@CmdExit",
      "font": { "family": "Arial", "size": 16, "bold": true }
    }
  ],
  "animationSequences": [
    {
      "name": "MainMenuScan",
      "steps": [
        { "widget": "BtnSettings", "highlightDuration": 1500 },
        { "widget": "BtnExit", "highlightDuration": 1500 }
      ]
    }
  ]
}
```

#### Widget Types

Common widget types in ACAT:
- `Button` - Clickable button
- `Label` - Static text display
- `Container` - Layout container for other widgets
- `TextBox` - Text input field

---

### 4. Abbreviations.json

**Purpose:** Define text abbreviation expansions

**Location:** `%APPDATA%\ACAT\Config\Abbreviations.json`

**Schema:** `schemas/json/abbreviations.schema.json`

#### Schema Reference

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `abbreviations` | array | Yes | List of abbreviation definitions |
| `abbreviations[].word` | string | Yes | The abbreviation to replace (e.g., "btw") |
| `abbreviations[].replaceWith` | string | Yes | Full text expansion (e.g., "by the way") |
| `abbreviations[].mode` | string | Yes | Expansion mode: "Write", "Speak", or "None" |

#### Example

```json
{
  "$schema": "../json/abbreviations.schema.json",
  "abbreviations": [
    {
      "word": "btw",
      "replaceWith": "by the way",
      "mode": "Write"
    },
    {
      "word": "omg",
      "replaceWith": "oh my goodness",
      "mode": "Speak"
    }
  ]
}
```

#### Expansion Modes

- **Write:** Replace abbreviation with full text in the document
- **Speak:** Speak the full text via text-to-speech without writing
- **None:** Disable expansion for this abbreviation

---

### 5. Pronunciations.json

**Purpose:** Define custom pronunciations for text-to-speech

**Location:** `%APPDATA%\ACAT\Config\Pronunciations.json`

**Schema:** `schemas/json/pronunciations.schema.json`

#### Schema Reference

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `pronunciations` | array | Yes | List of pronunciation definitions |
| `pronunciations[].word` | string | Yes | Word to customize (e.g., "github") |
| `pronunciations[].pronunciation` | string | Yes | How to pronounce it (e.g., "git hub") |

#### Example

```json
{
  "$schema": "../json/pronunciations.schema.json",
  "pronunciations": [
    {
      "word": "github",
      "pronunciation": "git hub"
    },
    {
      "word": "ACAT",
      "pronunciation": "A cat"
    },
    {
      "word": "sql",
      "pronunciation": "sequel"
    }
  ]
}
```

#### Tips for Pronunciations

- Use phonetic spelling to guide pronunciation
- Break compound words with spaces (e.g., "git hub" instead of "github")
- Use capital letters for emphasis (e.g., "A P I" for letter-by-letter)
- Test with your TTS engine to verify

---

## VS Code IntelliSense Setup

Visual Studio Code provides excellent support for JSON schemas, offering autocomplete and real-time validation.

### Method 1: Using $schema Property (Recommended)

Add the `$schema` property at the top of your JSON file:

```json
{
  "$schema": "../json/actuator-settings.schema.json",
  "actuatorSettings": [
    ...
  ]
}
```

**Relative paths from typical locations:**
- User configs: `../../../path/to/acat/schemas/json/actuator-settings.schema.json`
- Or use the example files as templates (they already have the correct paths)

### Method 2: Workspace Settings

Create `.vscode/settings.json` in your ACAT directory:

```json
{
  "json.schemas": [
    {
      "fileMatch": ["**/ActuatorSettings.json"],
      "url": "./schemas/json/actuator-settings.schema.json"
    },
    {
      "fileMatch": ["**/Theme.json"],
      "url": "./schemas/json/theme.schema.json"
    },
    {
      "fileMatch": ["**/Abbreviations.json"],
      "url": "./schemas/json/abbreviations.schema.json"
    },
    {
      "fileMatch": ["**/Pronunciations.json"],
      "url": "./schemas/json/pronunciations.schema.json"
    },
    {
      "fileMatch": ["**/Panels/**/*.json"],
      "url": "./schemas/json/panel-config.schema.json"
    }
  ]
}
```

### Method 3: User Settings (Global)

For system-wide IntelliSense, add to VS Code user settings:

1. Press `Ctrl+Shift+P` (Windows/Linux) or `Cmd+Shift+P` (Mac)
2. Type "Preferences: Open User Settings (JSON)"
3. Add the schema associations:

```json
{
  "json.schemas": [
    {
      "fileMatch": ["**/ACAT/**/ActuatorSettings.json"],
      "url": "C:/path/to/acat/schemas/json/actuator-settings.schema.json"
    }
  ]
}
```

### IntelliSense Features

Once configured, you'll get:

- **📝 Autocomplete:** Press `Ctrl+Space` to see available properties
- **✅ Validation:** Red squiggles indicate errors
- **📖 Hover Documentation:** Hover over properties to see descriptions
- **🔍 Quick Info:** See allowed values for enums
- **⚡ Snippets:** Quickly insert common patterns

---

## Troubleshooting

### Configuration Validation Failed

**Symptom:** ACAT shows "Configuration validation failed" error

**Common Causes:**
1. **Missing Required Fields**
   ```json
   // ❌ Bad: Missing 'mode'
   { "word": "btw", "replaceWith": "by the way" }
   
   // ✅ Good: All required fields present
   { "word": "btw", "replaceWith": "by the way", "mode": "Write" }
   ```

2. **Invalid Enum Values**
   ```json
   // ❌ Bad: Invalid mode
   { "mode": "WriteBoth" }
   
   // ✅ Good: Valid mode
   { "mode": "Write" }
   ```

3. **Malformed JSON**
   ```json
   // ❌ Bad: Trailing comma
   {
     "word": "btw",
     "replaceWith": "by the way",
     "mode": "Write",  // ← Remove this comma
   }
   ```

**Solution:** Check the application logs for specific validation errors and fix the indicated issues.

### Configuration File Not Found

**Symptom:** ACAT creates default configurations on startup

**Causes:**
- Configuration file path is incorrect
- File has wrong extension (should be `.json`)
- File has no read permissions

**Solution:**
1. Verify file path: `%APPDATA%\ACAT\Config\`
2. Check file extension is `.json` not `.json.txt`
3. Ensure file is readable (right-click → Properties → Security)

### JSON Parsing Error

**Symptom:** "JSON parsing error" or "Unexpected token"

**Common Syntax Errors:**
```json
// ❌ Bad: Missing quotes around property names (not JSON)
{ word: "btw", replaceWith: "by the way" }

// ✅ Good: Property names in quotes
{ "word": "btw", "replaceWith": "by the way" }

// ❌ Bad: Single quotes (not valid JSON)
{ 'word': 'btw' }

// ✅ Good: Double quotes
{ "word": "btw" }

// ❌ Bad: Trailing comma in array
["item1", "item2",]

// ✅ Good: No trailing comma
["item1", "item2"]
```

**Solution:** Use a JSON validator or VS Code with IntelliSense to catch syntax errors.

### IntelliSense Not Working

**Symptom:** No autocomplete or validation in VS Code

**Solutions:**
1. **Check schema path:** Ensure `$schema` property points to correct file
2. **Install JSON Language Features:** Already included in VS Code
3. **Reload window:** Press `Ctrl+Shift+P` → "Developer: Reload Window"
4. **Check file extension:** Must be `.json` not `.txt`

### Migration Tool Warnings

**Symptom:** ConfigMigrationTool shows warnings after migration

**Common Warnings:**
- "Enabled switches that actuate must have a command"
  - **Fix:** Add `"command": "@Trigger"` to actuating switches
  
- "Animations should have at least one step"
  - **Fix:** Add animation steps or remove empty animation
  
- "Color scheme names should be unique"
  - **Fix:** Rename duplicate color scheme names

**Note:** Warnings don't prevent migration. Files are still converted and usable.

### Configuration Changes Not Taking Effect

**Symptom:** Modified configuration doesn't change ACAT behavior

**Solutions:**
1. **Restart ACAT:** Configuration is loaded at startup
2. **Check file location:** ACAT may be loading from a different location
3. **Verify JSON syntax:** Syntax errors may cause fallback to defaults
4. **Check logs:** Application logs show which config file was loaded

### Colors Not Displaying Correctly

**Symptom:** Theme colors appear wrong or default

**Solutions:**
1. **Check color format:**
   ```json
   // ✅ Valid formats
   "background": "#232433"
   "background": "White"
   "background": "#FF232433"  // With alpha channel
   
   // ❌ Invalid formats
   "background": "232433"      // Missing #
   "background": "#GGG"        // Invalid hex
   ```

2. **Include both background and foreground:**
   ```json
   {
     "name": "Scanner",
     "background": "#232433",
     "foreground": "White"  // Recommended
   }
   ```

3. **Verify scheme name matches usage:**
   - Check that scheme names like "Scanner", "ScannerButton" match the names referenced in code

---

## Best Practices

### 1. Always Use Schema References

Include `$schema` property for IntelliSense support:
```json
{
  "$schema": "../json/abbreviations.schema.json",
  ...
}
```

### 2. Use Comments for Documentation

JSON doesn't support comments, but VS Code's JSON parser does:
```json
{
  // This is a configuration comment
  "abbreviations": [
    {
      "word": "btw",  // Common internet abbreviation
      "replaceWith": "by the way",
      "mode": "Write"
    }
  ]
}
```

### 3. Format for Readability

Use consistent indentation (2 or 4 spaces):
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

VS Code can auto-format: `Shift+Alt+F` (Windows/Linux) or `Shift+Option+F` (Mac)

### 4. Backup Before Migration

Always create backups before migrating:
```bash
ConfigMigrationTool migrate -i "C:\ACAT\Config" -o "C:\ACAT\ConfigJSON" --backup
```

### 5. Validate After Changes

After making manual changes, validate:
```bash
ConfigMigrationTool validate -i "C:\ACAT\Config"
```

### 6. Use Example Files as Templates

Copy from `schemas/examples/` instead of creating from scratch:
```bash
copy schemas\examples\abbreviations.example.json %APPDATA%\ACAT\Config\Abbreviations.json
```

### 7. Keep Variable References

Don't replace variable references with hard-coded values:
```json
// ✅ Good: Keep variable reference
{
  "command": "@Trigger",
  "minHoldTime": "@MinActuationHoldTime"
}

// ❌ Bad: Hard-coded values
{
  "command": "Trigger",
  "minHoldTime": 100
}
```

### 8. Test in Stages

When making changes:
1. Make one change at a time
2. Restart ACAT to test
3. If it works, make the next change
4. If it fails, revert and investigate

---

## Additional Resources

### Schema Files
- Location: `schemas/json/`
- All schemas: `actuator-settings.schema.json`, `theme.schema.json`, `panel-config.schema.json`, `abbreviations.schema.json`, `pronunciations.schema.json`

### Example Files
- Location: `schemas/examples/`
- Use as templates for creating new configurations

### Migration Tool
- Location: `src/build/bin/ConfigMigrationTool.exe`
- Documentation: `src/Applications/ConfigMigrationTool/ACAT.ConfigMigrationTool/README.md`

### Developer Documentation
- See `docs/JSON_CONFIGURATION_DEVELOPER_GUIDE.md` for extending the system
- See `docs/JSON_CONFIGURATION_MIGRATION.md` for technical migration details

### Support
For issues or questions:
- Check application logs in `%APPDATA%\ACAT\Logs\`
- Review validation errors in ConfigMigrationTool output
- Contact: <ACAT@intel.com>

---

## License

Copyright 2013-2019; 2023 Intel Corporation  
SPDX-License-Identifier: Apache-2.0
