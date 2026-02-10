# ACAT Configuration Migration Tool

A command-line tool for migrating ACAT XML configuration files to JSON format with validation and backup capabilities.

## Features

- **Automated Migration**: Convert XML configuration files to JSON format
- **Schema Detection**: Automatically identifies configuration types (ActuatorSettings, Theme, PanelConfig)
- **Validation**: Validates converted JSON against FluentValidation rules
- **Dry Run Mode**: Preview changes without modifying files
- **Backup & Rollback**: Create backups and restore if needed
- **Progress Reporting**: Visual progress bars and detailed migration reports
- **Batch Processing**: Process entire directories with subdirectories

## Supported Configuration Types

1. **ActuatorSettings** - Input device configurations (keyboard, camera, BCI, external switches)
2. **Theme** - Color schemes and styling for UI elements
3. **PanelConfig** - UI panel layouts for scanners, keyboards, menus, and dialogs

## Installation

### Prerequisites

- .NET 8.0 or later

### Build from Source

```bash
cd src/Applications/ConfigMigrationTool/ACAT.ConfigMigrationTool
dotnet build -c Release
```

The compiled executable will be in `src/build/bin/ACAT.ConfigMigrationTool.exe` (or `.dll` on non-Windows)

## Usage

### Basic Commands

#### 1. Migrate XML to JSON

Convert XML configuration files to JSON format:

```bash
# Basic migration
ConfigMigrationTool migrate --input "C:\ACAT\Config" --output "C:\ACAT\ConfigJson"

# With backup (recommended)
ConfigMigrationTool migrate --input "C:\ACAT\Config" --output "C:\ACAT\ConfigJson" --backup

# Dry run (preview only, no changes)
ConfigMigrationTool migrate --input "C:\ACAT\Config" --output "C:\ACAT\ConfigJson" --dry-run
```

**Options:**
- `--input, -i` (required): Input directory containing XML files
- `--output, -o` (required): Output directory for JSON files
- `--dry-run, -d`: Preview changes without actually converting files
- `--backup, -b`: Create backup copies of original XML files (.backup extension)

#### 2. Validate JSON Files

Validate JSON configuration files against schemas:

```bash
ConfigMigrationTool validate --input "C:\ACAT\ConfigJson"
```

**Options:**
- `--input, -i` (required): Directory containing JSON files to validate

#### 3. Rollback Migration

Restore original XML files from backups:

```bash
ConfigMigrationTool rollback --backup "C:\ACAT\Config"
```

**Options:**
- `--backup, -b` (required): Directory containing .backup files to restore

### Examples

#### Example 1: Safe Migration with Preview

```bash
# Step 1: Preview what will be converted
ConfigMigrationTool migrate -i "./config-xml" -o "./config-json" --dry-run

# Step 2: Run actual migration with backup
ConfigMigrationTool migrate -i "./config-xml" -o "./config-json" --backup

# Step 3: Validate the converted files
ConfigMigrationTool validate -i "./config-json"
```

#### Example 2: Migrate Default User Configuration

```bash
ConfigMigrationTool migrate \
  --input "C:\ACAT\Install\Users\DefaultUser" \
  --output "C:\ACAT\Install\Users\DefaultUser-JSON" \
  --backup
```

#### Example 3: Rollback if Issues Found

```bash
# If migration had issues, rollback to original files
ConfigMigrationTool rollback --backup "C:\ACAT\Config"
```

## Migration Report

After each operation, the tool generates a detailed report:

```
========================================
  MIGRATION REPORT
========================================

Duration: 1.23 seconds
Total Files: 15
Successful: 13
Failed: 0
Skipped: 2
Backed Up: 13

Successfully Converted:
  ✓ ActuatorSettings.json
  ✓ Theme.json
  ✓ MainMenu.json
  ...

Warnings:
  ⚠ ActuatorSettings.xml: Enabled switches that actuate must have a command
  ⚠ SomePanel.xml: Animations should have at least one step

========================================
```

## Understanding the Output

### Success Indicators

- **✓ (Green)**: File successfully converted
- **Total Files**: Number of XML files found
- **Successful**: Files converted without errors
- **Failed**: Files that could not be converted (check Errors section)
- **Skipped**: Files with unknown schema types

### Warning Messages

Warnings indicate validation issues in the source XML that were preserved in the JSON:
- Configuration is still converted
- JSON is valid syntactically
- May need manual review for business logic issues

Common warnings:
- "Enabled switches that actuate must have a command"
- "Animations should have at least one step"
- "Color scheme names should be unique"

### Error Messages

Errors prevent file conversion:
- Invalid XML structure
- Missing required elements
- Malformed data

## File Structure

The tool preserves directory structure:

```
Input:
  config-xml/
    ├── ActuatorSettings.xml
    ├── themes/
    │   └── Default/
    │       └── Theme.xml
    └── panels/
        └── MainMenu.xml

Output:
  config-json/
    ├── ActuatorSettings.json
    ├── themes/
    │   └── Default/
    │       └── Theme.json
    └── panels/
        └── MainMenu.json
```

## Backup Files

When using `--backup`, original files are copied with a `.backup` extension:

```
config-xml/
  ├── ActuatorSettings.xml
  ├── ActuatorSettings.xml.backup  <-- Backup
  ├── Theme.xml
  └── Theme.xml.backup             <-- Backup
```

## Troubleshooting

### Issue: "Directory not found"

**Solution**: Ensure the input directory path is correct and accessible

### Issue: "Validation failed" during migration

**Cause**: Source XML has business logic issues (e.g., missing required commands)

**Solution**: 
1. Review warnings in the migration report
2. Files are still converted (warnings don't block migration)
3. Manually fix validation issues in the generated JSON if needed

### Issue: "Unknown schema type, skipped"

**Cause**: XML file doesn't match ActuatorSettings, Theme, or PanelConfig format

**Solution**: 
1. Verify the XML file is a supported configuration type
2. Check for XML structure errors
3. These files may be different configuration types not yet supported

### Issue: Permission errors

**Solution**: 
- Run with appropriate permissions
- Ensure output directory is writable
- Check input files are not locked by another process

## Technical Details

### Dependencies

- **System.CommandLine**: CLI argument parsing
- **System.Text.Json**: JSON serialization
- **FluentValidation**: Configuration validation
- **Spectre.Console**: Progress bars and formatted output
- **NJsonSchema**: JSON schema support

### Validation Rules

#### ActuatorSettings
- At least one actuator required
- At least one actuator must be enabled
- Enabled actuators need at least one enabled switch
- Switch names must be unique within actuator
- Enabled switches that actuate must have a command

#### Theme
- At least one color scheme required
- Color scheme names must be unique
- Essential schemes recommended (Scanner, ScannerButton, etc.)

#### PanelConfig
- Widget attribute names must be unique
- Animation names must be unique
- Animations should have steps
- Container widgets should have children

## JSON Output Format

The tool uses camelCase for JSON properties and preserves all data from XML:

```json
{
  "description": "The default theme with high contrast",
  "colorSchemes": [
    {
      "name": "Scanner",
      "background": "#232433",
      "foreground": "White",
      "highlightBackground": "#ffaa00",
      "highlightForeground": "#232433"
    }
  ]
}
```

## Exit Codes

- `0`: Success (all operations completed without errors)
- `1`: Failure (one or more files failed to convert or validate)

## Support

For issues or questions:
1. Check the migration report for detailed error messages
2. Review validation warnings
3. Consult the ACAT schemas documentation in `schemas/README.md`

## License

Copyright 2013-2019; 2023 Intel Corporation  
SPDX-License-Identifier: Apache-2.0
