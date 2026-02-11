# ACAT JSON Configuration - Quick Reference

**Version:** 1.0  
**Last Updated:** February 2026

This quick reference provides essential information for working with ACAT's JSON configuration system.

---

## Configuration File Locations

| Configuration Type | Default Location |
|--------------------|------------------|
| ActuatorSettings | `%APPDATA%\ACAT\Config\ActuatorSettings.json` |
| Theme | `%APPDATA%\ACAT\Config\Theme.json` |
| Abbreviations | `%APPDATA%\ACAT\Config\Abbreviations.json` |
| Pronunciations | `%APPDATA%\ACAT\Config\Pronunciations.json` |
| PanelConfig | `%APPDATA%\ACAT\Config\Panels\[PanelName].json` |

**Note:** `%APPDATA%` typically resolves to `C:\Users\[YourName]\AppData\Roaming\`

---

## Migration Tool Commands

### Convert XML to JSON
```bash
ConfigMigrationTool migrate --input "C:\ACAT\Config" --output "C:\ACAT\ConfigJSON" --backup
```

### Preview Migration (Dry Run)
```bash
ConfigMigrationTool migrate --input "C:\ACAT\Config" --output "C:\ACAT\ConfigJSON" --dry-run
```

### Validate JSON Files
```bash
ConfigMigrationTool validate --input "C:\ACAT\ConfigJSON"
```

### Rollback Migration
```bash
ConfigMigrationTool rollback --backup "C:\ACAT\Config"
```

---

## VS Code IntelliSense Setup

Add to the top of your JSON file:
```json
{
  "$schema": "path/to/schema.json",
  ...
}
```

### Schema Paths

| Configuration Type | Schema Path |
|--------------------|-------------|
| ActuatorSettings | `../json/actuator-settings.schema.json` |
| Theme | `../json/theme.schema.json` |
| PanelConfig | `../json/panel-config.schema.json` |
| Abbreviations | `../json/abbreviations.schema.json` |
| Pronunciations | `../json/pronunciations.schema.json` |

---

## Common Configuration Patterns

### ActuatorSettings - Add Keyboard Switch

```json
{
  "name": "Keyboard",
  "enabled": true,
  "switchSettings": [
    {
      "name": "Trigger",
      "source": "F12",
      "enabled": true,
      "actuate": true,
      "command": "@Trigger",
      "minHoldTime": "@MinActuationHoldTime"
    }
  ]
}
```

### Theme - Define Color Scheme

```json
{
  "name": "Scanner",
  "background": "#232433",
  "foreground": "White",
  "highlightBackground": "#ffaa00",
  "highlightForeground": "#232433"
}
```

### Abbreviations - Add Text Expansion

```json
{
  "word": "btw",
  "replaceWith": "by the way",
  "mode": "Write"
}
```

### Pronunciations - Custom TTS

```json
{
  "word": "github",
  "pronunciation": "git hub"
}
```

---

## Validation Rules

### ActuatorSettings
- ✅ At least one actuator required
- ✅ At least one actuator must be enabled
- ✅ Actuator IDs must be unique GUIDs
- ✅ Enabled switches that actuate must have a command

### Theme
- ✅ At least one color scheme required
- ✅ Color scheme names must be unique
- ✅ Colors must be valid hex codes or color names

### Abbreviations
- ✅ Word and replaceWith cannot be empty
- ✅ Mode must be "Write", "Speak", or "None"

### Pronunciations
- ✅ Word and pronunciation cannot be empty

---

## Common Commands

| Command | Description |
|---------|-------------|
| `@Trigger` | Primary selection/activation |
| `@CmdEnterKey` | Simulate Enter key |
| `@CmdTabKey` | Simulate Tab key |
| `@CmdUndo` | Undo last action |
| `@CmdSettings` | Open settings |
| `@CmdExit` | Exit application |

---

## Color Formats

### Hex Codes
```json
"background": "#232433"       // RGB
"background": "#FF232433"     // ARGB (with alpha)
```

### Color Names
```json
"background": "White"
"background": "Black"
"background": "Red"
"background": "Blue"
```

---

## Abbreviation Modes

| Mode | Behavior |
|------|----------|
| `Write` | Replace abbreviation with full text in document |
| `Speak` | Speak full text via TTS without writing |
| `None` | Disable expansion for this abbreviation |

---

## Troubleshooting Quick Fixes

### Error: Configuration validation failed
**Fix:** Check logs for specific errors. Common issues:
- Missing required fields
- Invalid enum values
- Malformed JSON syntax

### Error: JSON parsing error
**Fix:** Validate JSON syntax:
- Check for matching brackets `{}` and `[]`
- Ensure all strings have double quotes `"`
- Remove trailing commas at end of arrays/objects

### IntelliSense not working
**Fix:** 
1. Add `$schema` property to JSON file
2. Ensure file extension is `.json`
3. Reload VS Code window (`Ctrl+Shift+P` → "Developer: Reload Window")

### Changes not taking effect
**Fix:**
1. Restart ACAT to reload configuration
2. Verify correct file location
3. Check logs for loading errors

---

## JSON Syntax Examples

### Valid JSON
```json
{
  "name": "Example",
  "enabled": true,
  "items": [
    { "id": 1, "value": "first" },
    { "id": 2, "value": "second" }
  ]
}
```

### Common Mistakes
```json
// ❌ Bad: Trailing comma
{
  "name": "Example",
  "enabled": true,
}

// ❌ Bad: Single quotes
{ 'name': 'Example' }

// ❌ Bad: Unquoted property names
{ name: "Example" }

// ❌ Bad: Comments (standard JSON)
{
  // This comment is not valid in standard JSON
  "name": "Example"
}
// Note: VS Code JSON parser DOES support comments
```

---

## Example Files

Copy from `schemas/examples/` to get started:

```bash
# Windows
copy schemas\examples\abbreviations.example.json %APPDATA%\ACAT\Config\Abbreviations.json

# PowerShell
Copy-Item schemas\examples\abbreviations.example.json $env:APPDATA\ACAT\Config\Abbreviations.json
```

---

## Schema Files Location

All schemas are in: `schemas/json/`

- `actuator-settings.schema.json`
- `theme.schema.json`
- `panel-config.schema.json`
- `abbreviations.schema.json`
- `pronunciations.schema.json`

---

## Testing Configuration

### Validate with ConfigMigrationTool
```bash
ConfigMigrationTool validate --input "C:\ACAT\Config"
```

### Test in VS Code
1. Open JSON file
2. Look for red squiggles (validation errors)
3. Press `Ctrl+Space` for autocomplete
4. Hover over properties for documentation

### Test in ACAT
1. Place configuration file in correct location
2. Restart ACAT
3. Check logs: `%APPDATA%\ACAT\Logs\`
4. Verify behavior in application

---

## Keyboard Shortcuts (VS Code)

| Shortcut | Action |
|----------|--------|
| `Ctrl+Space` | Show autocomplete suggestions |
| `Shift+Alt+F` | Format JSON document |
| `Ctrl+K Ctrl+I` | Show hover information |
| `F2` | Rename symbol |
| `Ctrl+Shift+P` | Open command palette |

---

## Documentation Links

- **User Guide:** [docs/JSON_CONFIGURATION_GUIDE.md](JSON_CONFIGURATION_GUIDE.md)
- **Developer Guide:** [docs/JSON_CONFIGURATION_DEVELOPER_GUIDE.md](JSON_CONFIGURATION_DEVELOPER_GUIDE.md)
- **Migration Guide:** [docs/JSON_CONFIGURATION_MIGRATION.md](JSON_CONFIGURATION_MIGRATION.md)
- **Schema README:** [schemas/README.md](../schemas/README.md)
- **Migration Tool:** [ConfigMigrationTool README](../src/Applications/ConfigMigrationTool/ACAT.ConfigMigrationTool/README.md)

---

## Support

**Check Logs:** `%APPDATA%\ACAT\Logs\`  
**Example Files:** `schemas/examples/`  
**Contact:** <ACAT@intel.com>

---

## License

Copyright 2013-2019; 2023 Intel Corporation  
SPDX-License-Identifier: Apache-2.0
