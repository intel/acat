# ACAT Configuration JSON Schemas

This directory contains JSON Schema files for all ACAT configuration types. These schemas enable
editor IntelliSense, document required fields and data types, and serve as the authoritative
specification for each configuration format.

## Schema Files

| File | Description |
|------|-------------|
| [`AppPreferences.schema.json`](./AppPreferences.schema.json) | System-wide application preferences (scan timing, display, logging, etc.) |
| [`ActuatorSettings.schema.json`](./ActuatorSettings.schema.json) | Input device configuration (keyboard, camera, BCI, external switches) |
| [`AgentConfigurations.schema.json`](./AgentConfigurations.schema.json) | Application and functional agent configurations |
| [`WordPredictorSettings.schema.json`](./WordPredictorSettings.schema.json) | Word prediction engine settings per language |
| [`TTSEngineSettings.schema.json`](./TTSEngineSettings.schema.json) | Text-to-speech engine settings per language |
| [`PanelConfig.schema.json`](./PanelConfig.schema.json) | UI panel layouts (scanners, keyboards, menus, dialogs) |
| [`ThemeSettings.schema.json`](./ThemeSettings.schema.json) | UI color schemes and styling |

## Required Fields Summary

| Schema | Required Fields |
|--------|----------------|
| AppPreferences | `currentUser`, `currentProfile` |
| ActuatorSettings | `actuatorSettings` (array); each entry: `name`, `id`, `enabled` |
| AgentConfigurations | `agents` (array); each entry: `name`, `id` |
| WordPredictorSettings | `wordPredictors` (array); each entry: `language`, `id` |
| TTSEngineSettings | `ttsEngines` (array); each entry: `language`, `id` |
| PanelConfig | `widgetAttributes`, `layout` (with `colorScheme`) |
| ThemeSettings | `description`, `colorSchemes` (array); each entry: `name` |

## Schema Versioning Strategy

### Version Format

Schemas use **semantic versioning** (`MAJOR.MINOR.PATCH`) declared via the top-level `"version"` field:

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "version": "1.0.0",
  ...
}
```

All schemas in this directory are currently at version **1.0.0**.

### Version Change Guidelines

| Change Type | Version Bump | Example |
|-------------|-------------|---------|
| Adding a new **optional** property | `MINOR` (1.0.0 → 1.1.0) | Adding `"enableSounds"` to AppPreferences |
| Adding a new **required** property | `MAJOR` (1.0.0 → 2.0.0) | Making a previously optional field required |
| Removing or renaming a property | `MAJOR` (1.0.0 → 2.0.0) | Renaming `"scanTime"` to `"scanTimeMs"` |
| Changing a property's type | `MAJOR` (1.0.0 → 2.0.0) | Changing `"fontSize"` from `string` to `number` |
| Tightening a constraint | `MAJOR` (1.0.0 → 2.0.0) | Reducing `maximum` value of a range |
| Relaxing a constraint | `MINOR` (1.0.0 → 1.1.0) | Increasing `maximum` value of a range |
| Fixing a typo in a description | `PATCH` (1.0.0 → 1.0.1) | Correcting documentation only |

### Backward Compatibility

- **PATCH** and **MINOR** changes are backward-compatible: existing valid config files remain valid.
- **MAJOR** changes are breaking: config files written against the previous major version may
  fail validation and may require migration.

### Migration

When a **MAJOR** version change is introduced:

1. The old schema version is retained in `Config/Schemas/archive/vX/` for reference.
2. A migration note is added to this README describing the breaking changes.
3. Any required migration tooling is placed in `src/Tools/ConfigMigration/`.

## JSON Schema Draft

All schemas use **JSON Schema Draft 7** (`http://json-schema.org/draft-07/schema#`), which is
widely supported by editors and validators.

## VS Code IntelliSense

To enable schema validation and autocomplete in VS Code, add a `$schema` reference to your config file:

```json
{
  "$schema": "../../Config/Schemas/AppPreferences.schema.json",
  "currentUser": "DefaultUser",
  "currentProfile": "Default"
}
```

Alternatively, configure workspace-wide schema associations in `.vscode/settings.json`:

```json
{
  "json.schemas": [
    {
      "fileMatch": ["**/ActuatorSettings.json"],
      "url": "./Config/Schemas/ActuatorSettings.schema.json"
    },
    {
      "fileMatch": ["**/Theme.json"],
      "url": "./Config/Schemas/ThemeSettings.schema.json"
    },
    {
      "fileMatch": ["**/AppPreferences.json"],
      "url": "./Config/Schemas/AppPreferences.schema.json"
    }
  ]
}
```

## Relationship to `schemas/json/`

The `schemas/json/` directory at the repository root contains schemas developed as part of the
XML-to-JSON migration effort (see issue #7). The schemas in this `Config/Schemas/` directory are
the canonical location for configuration schemas going forward, organized by configuration type
with the versioning strategy described above.

| `Config/Schemas/` | `schemas/json/` (legacy reference) |
|-------------------|-------------------------------------|
| `ActuatorSettings.schema.json` | `actuator-settings.schema.json` |
| `ThemeSettings.schema.json` | `theme.schema.json` |
| `PanelConfig.schema.json` | `panel-config.schema.json` |
| *(app preferences)* | *(not present)* |
| *(agent configurations)* | *(not present)* |
| *(word predictor settings)* | *(not present)* |
| *(TTS engine settings)* | *(not present)* |

## License

Copyright 2013-2019; 2023 Intel Corporation  
SPDX-License-Identifier: Apache-2.0
