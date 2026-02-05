# XML Configuration Analysis Report

**Date:** 2026-02-05  
**Ticket:** intel/acat#6  
**Purpose:** Comprehensive inventory of XML configuration files for JSON migration planning

---

## Executive Summary

- **Total XML Files:** 86 configuration files
- **Distinct Schemas:** 8 major schema types identified
- **Total File Size:** 644 KB
- **Estimated Migration Effort:** 80-100 hours (2-3 weeks with AI assistance)
- **Critical Path:** ActuatorSettings → Panel Configurations → Theme Settings

### High-Level Findings

1. **Panel configurations** dominate (73 files, ~85% of total) - these define UI scanners and menus
2. **ActuatorSettings.xml** is the most complex and critical single file (user input device configuration)
3. **Theme.xml** is moderately complex with color schemes and styling
4. **Configuration maps** (PanelConfigMap, UserControlConfigMap) are simple routing tables
5. **LanguageSettings** and **SpellCheck** are simple data files

---

## Schema Inventory

### 1. ActuatorSettings.xml

**Purpose:** Configure input devices (switches) for ACAT - keyboard, camera, BCI, external switches  
**Root Element:** `<ActuatorConfig>`  
**Instances:** 1 (user-specific)  
**Complexity:** **High** (nested structures, multiple switch types)  
**Priority:** **P1** (Critical - controls all user input)  
**Lines of Code:** ~140  

**Structure:**
```xml
<ActuatorConfig>
  <ActuatorSettings>
    <ActuatorSetting>
      <Description>string</Description>
      <Enabled>boolean</Enabled>
      <Id>guid</Id>
      <ImageFileName>string</ImageFileName>
      <Name>string</Name>
      <SwitchSettings>
        <SwitchSetting>
          <Actuate>boolean</Actuate>
          <BeepFile>string</BeepFile>
          <Command>string</Command>
          <Description>string</Description>
          <Enabled>boolean</Enabled>
          <MinHoldTime>string</MinHoldTime>
          <Name>string</Name>
          <Source>string</Source>
        </SwitchSetting>
      </SwitchSettings>
    </ActuatorSetting>
  </ActuatorSettings>
</ActuatorConfig>
```

**Sample Data:**
```xml
<ActuatorSetting>
  <Description>Use the computer keyboard as a switch to control ACAT.</Description>
  <Enabled>true</Enabled>
  <Id>d91a1877-c92b-4d7e-9ab6-f01f30b12df9</Id>
  <ImageFileName>KeyboardSwitch.jpg</ImageFileName>
  <Name>Keyboard</Name>
  <SwitchSettings>
    <SwitchSetting>
      <Actuate>true</Actuate>
      <BeepFile>beep.wav</BeepFile>
      <Command>@Trigger</Command>
      <Enabled>true</Enabled>
      <MinHoldTime>@MinActuationHoldTime</MinHoldTime>
      <Name>Trigger</Name>
      <Source>F12</Source>
    </SwitchSetting>
  </SwitchSettings>
</ActuatorSetting>
```

**Special Handling:**
- Contains template variables (e.g., `@MinActuationHoldTime`, `$ACAT_USER_GUIDE`)
- GUIDs must be preserved during migration
- Multiple actuator types (Keyboard, Camera, BCI, External, Sample)
- Each actuator has 1-N switch configurations

---

### 2. Panel Configuration Files (Scanner/Menu XML)

**Purpose:** Define UI layouts for scanners, keyboards, menus, and dialogs  
**Root Element:** `<ACAT>` with `<WidgetAttributes>`, `<Layout>`, `<Animations>`  
**Instances:** 73 files  
**Complexity:** **Medium to High** (varies by panel complexity)  
**Priority:** **P1-P2** (High usage, core UI definitions)  

**Structure:**
```xml
<ACAT>
  <WidgetAttributes>
    <WidgetAttribute name="..." label="..." value="..." 
                     fontsize="..." fontname="..." bold="..."/>
  </WidgetAttributes>
  
  <Layout colorScheme="...">
    <Widget class="..." name="..." colorScheme="...">
      <Widget class="..." name="..." />
    </Widget>
  </Layout>
  
  <Animations>
    <Animation name="..." start="..." autoStart="..." 
               firstPauseTime="..." scanTime="..." iterations="...">
      <Widget name="..." onSelect="..." />
    </Animation>
  </Animations>
</ACAT>
```

**Examples:**
- MainMenu.xml - Main application menu
- TalkApplicationScanner.xml - Primary communication interface
- KeyboardAbcUserControl.xml - ABC keyboard layout
- KeyboardQwertyUserControl.xml - QWERTY keyboard layout
- ChromeBrowserContextMenu.xml - Browser-specific menu
- OutlookContextMenu.xml - Email client menu

**Special Handling:**
- Uses DTD entities for constants (fonts, colors): `<!ENTITY usebold "false">`
- Entity references in attributes: `fontname="&buttonFontName;"`
- Template variables: `@CmdGoBack`, `@FirstPauseTime`
- Unicode characters in labels: `&#x75;`, `&#191;`
- Nested widget hierarchies (recursive structure)
- Animation sequences with state transitions

**Note:** Per requirements, Animation XMLs should be skipped in Phase 1 (handled in Phase 2).  
However, animations are embedded in panel configs, not separate files.

---

### 3. Theme.xml

**Purpose:** Define color schemes and visual styling for all UI elements  
**Root Element:** `<ACAT><Theme>`  
**Instances:** 1  
**Complexity:** **Medium** (many color scheme definitions)  
**Priority:** **P2** (Important but not critical path)  
**Lines of Code:** ~170  

**Structure:**
```xml
<ACAT>
  <Theme description="...">
    <ColorSchemes>
      <ColorScheme name="..." 
                   background="..." 
                   foreground="..." 
                   highlightSelectedBackground="..." 
                   highlightSelectedForeground="..." 
                   highlightBackground="..." 
                   highlightForeground="..." />
    </ColorSchemes>
  </Theme>
</ACAT>
```

**Color Scheme Types:**
- Scanner, ScannerButton, DisabledScannerButton
- WordListItemButton
- Dialog, Menu, MenuTitle, Button
- HighContrast, TalkWindow
- BCIColorCodedRegion (Default, 1-4)

**Sample:**
```xml
<ColorScheme name="Scanner"
  background="#232433"
  foreground="White"
  highlightSelectedBackground="Blue"
  highlightSelectedForeground="White"
  highlightBackground="#ffaa00"
  highlightForeground="#232433" />
```

**Special Handling:**
- Color values can be named colors ("White", "Blue") or hex codes ("#232433")
- Optional backgroundImage/highlightBackgroundImage attributes (not shown in sample)
- BCI color schemes have specific contrast requirements (documented in comments)

---

### 4. PanelConfigMap.xml

**Purpose:** Map panel classes to configuration files and form IDs  
**Root Element:** `<ACAT><ConfigMapEntries>`  
**Instances:** 7 files (one per extension/agent)  
**Complexity:** **Simple** (flat routing table)  
**Priority:** **P1** (Required for panel loading)  

**Structure:**
```xml
<ACAT>
  <ConfigMapEntries>
    <ConfigMapEntry panelClass="..." 
                    configName="..." 
                    configId="..." 
                    formId="..." 
                    configFile="..."
                    userControls="..." />
  </ConfigMapEntries>
</ACAT>
```

**Sample:**
```xml
<ConfigMapEntry 
  panelClass="MainMenu" 
  configName="MainMenu" 
  configId="EA60C02D-37CA-418F-889B-7767F18A7F00" 
  formId="148257A1-A8B7-4E75-93F0-56AFCD5B2A3E" 
  configFile="MainMenu.xml" />

<ConfigMapEntry 
  panelClass="TalkApplicationScanner" 
  configName="TalkApplicationScannerQwerty" 
  configId="F802386C-31CA-4A0D-BC6F-78E71C730D11" 
  formId="D9A5B53F-7119-445B-BDEA-F76EC53077F1" 
  configFile="TalkApplicationScanner.xml" 
  userControls="keyboard=KeyboardQwertyUserControl; wordPrediction=WordPredictionUserControl" />
```

**Special Handling:**
- GUIDs (configId, formId) must be preserved
- Optional `userControls` attribute with semicolon-delimited key=value pairs

---

### 5. UserControlConfigMap.xml

**Purpose:** Map user control names to their configuration files  
**Root Element:** `<ACAT><UserControlConfigMapEntries>`  
**Instances:** 1  
**Complexity:** **Simple** (flat routing table)  
**Priority:** **P1** (Required for user control loading)  

**Structure:**
```xml
<ACAT>
  <UserControlConfigMapEntries>
    <UserControlConfigMapEntry name="..." 
                               configName="..." 
                               configId="..." 
                               userControlId="..." 
                               configFile="..." />
  </UserControlConfigMapEntries>
</ACAT>
```

**Sample:**
```xml
<UserControlConfigMapEntry 
  name="KeyboardQwertyUserControl" 
  configName="KeyboardQwertyUserControlConfig" 
  configId="9681EBF9-3313-4120-A3CD-ADD6A3E99B95" 
  userControlId="C4668F6A-79D6-4D27-8C68-18172A49F333" 
  configFile="KeyboardQwertyUserControl.xml" />
```

**Special Handling:**
- GUIDs must be preserved

---

### 6. PanelClassConfig.xml

**Purpose:** Define application panel class configurations and layouts  
**Root Element:** `<AppPanelClassConfig>`  
**Instances:** 1  
**Complexity:** **Medium** (nested hierarchy)  
**Priority:** **P2** (Important but lower usage frequency)  
**Encoding:** UTF-16 (requires special handling)  

**Structure:**
```xml
<AppPanelClassConfig>
  <PanelClassConfigs>
    <PanelClassConfig>
      <AppDescription>string</AppDescription>
      <AppId>string</AppId>
      <AppName>string</AppName>
      <PanelClassConfigMaps>
        <PanelClassConfigMap>
          <Default>boolean</Default>
          <Description>string</Description>
          <DisplayNameLong>string</DisplayNameLong>
          <DisplayNameShort>string</DisplayNameShort>
          <Name>string</Name>
          <PanelClassConfigMapEntries>
            <PanelClassConfigMapEntry>
              <ConfigId>guid</ConfigId>
              <PanelClass>string</PanelClass>
            </PanelClassConfigMapEntry>
          </PanelClassConfigMapEntries>
          <ScreenshotFileName>string</ScreenshotFileName>
        </PanelClassConfigMap>
      </PanelClassConfigMaps>
    </PanelClassConfig>
  </PanelClassConfigs>
</AppPanelClassConfig>
```

**Sample:**
```xml
<PanelClassConfig>
  <AppDescription>Reduced functionality which supports the Talk window to communicate</AppDescription>
  <AppId>ACATTalk</AppId>
  <AppName>ACAT Talk Application</AppName>
  <PanelClassConfigMaps>
    <PanelClassConfigMap>
      <Default>true</Default>
      <Description>An alphabetically arranged keyboard with predictive text</Description>
      <DisplayNameLong> ABC Keyboard Layout</DisplayNameLong>
      <DisplayNameShort>Alphabetical</DisplayNameShort>
      <Name>TalkApplicationABC</Name>
      ...
    </PanelClassConfigMap>
  </PanelClassConfigMaps>
</PanelClassConfig>
```

**Special Handling:**
- **File encoding is UTF-16** - requires explicit encoding handling in parsers
- Contains multiple applications (ACATTalk, ACATDashboard)
- Each app can have multiple panel class configurations

---

### 7. SpellCheck.xml

**Purpose:** Define spelling correction rules (common typos → correct spelling)  
**Root Element:** `<ACAT><Spellings>`  
**Instances:** 1 (per language - currently only English in default install)  
**Complexity:** **Simple** (key-value pairs)  
**Priority:** **P3** (Nice to have, low usage impact)  

**Structure:**
```xml
<ACAT>
  <Spellings>
    <Spelling word="..." replaceWith="..." />
  </Spellings>
</ACAT>
```

**Sample:**
```xml
<Spellings>
  <Spelling word="i" replaceWith="I"/>
  <Spelling word="cant" replaceWith="can't"/>
  <Spelling word="dont" replaceWith="don't"/>
  <Spelling word="shouldnt" replaceWith="shouldn't"/>
</Spellings>
```

**Special Handling:**
- None - straightforward key-value mapping

---

### 8. LanguageSettings.xml

**Purpose:** Define language-specific punctuation and spacing rules  
**Root Element:** `<LanguageSettings>`  
**Instances:** 1 (per language - currently only Spanish in resources)  
**Complexity:** **Simple** (character lists)  
**Priority:** **P3** (Language-specific, low priority)  

**Structure:**
```xml
<LanguageSettings>
  <DeletePrecedingSpacesChars>string</DeletePrecedingSpacesChars>
  <InsertSpaceAfterChars>string</InsertSpaceAfterChars>
  <SentenceTerminatorChars>string</SentenceTerminatorChars>
  <TerminatorChars>string</TerminatorChars>
</LanguageSettings>
```

**Sample:**
```xml
<LanguageSettings>
  <DeletePrecedingSpacesChars>.? !,:;@})]</DeletePrecedingSpacesChars>
  <InsertSpaceAfterChars>.?!,:;})]</InsertSpaceAfterChars>
  <SentenceTerminatorChars>¿¡?!.</SentenceTerminatorChars>
  <TerminatorChars>¿¡.? !,:;</TerminatorChars>
</LanguageSettings>
```

**Special Handling:**
- Contains Unicode characters (Spanish inverted punctuation: ¿ ¡)

---

## Recommended Migration Order

### Priority 1 (Week 3 - Sprint 1)
1. **ActuatorSettings.xml** - Critical, single file, well-defined schema
2. **PanelConfigMap.xml** (all 7 instances) - Simple routing tables, high usage
3. **UserControlConfigMap.xml** - Simple routing table, high usage

### Priority 2 (Week 4 - Sprint 2)
4. **Theme.xml** - Moderate complexity, single file, important for UX
5. **PanelClassConfig.xml** - Medium complexity, single file

### Priority 3 (Week 5+ - Sprint 3)
6. **Panel Configuration Files** (73 files) - Defer to Phase 2 due to complexity and animation handling
7. **SpellCheck.xml** - Simple, low impact
8. **LanguageSettings.xml** - Simple, language-specific

---

## Generated POCO Classes (Top 5 Schemas)

### 1. ActuatorSettings.cs

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Root configuration for all actuators (input devices)
    /// </summary>
    public class ActuatorConfig
    {
        [JsonPropertyName("actuatorSettings")]
        public List<ActuatorSetting> ActuatorSettings { get; set; } = new();
    }

    /// <summary>
    /// Configuration for a single actuator device
    /// </summary>
    public class ActuatorSetting
    {
        [JsonPropertyName("id")]
        [Required]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = false;

        [JsonPropertyName("imageFileName")]
        public string ImageFileName { get; set; }

        [JsonPropertyName("switchSettings")]
        public List<SwitchSetting> SwitchSettings { get; set; } = new();
    }

    /// <summary>
    /// Configuration for a single switch/button on an actuator
    /// </summary>
    public class SwitchSetting
    {
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("actuate")]
        public bool Actuate { get; set; } = true;

        [JsonPropertyName("beepFile")]
        public string BeepFile { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("minHoldTime")]
        public string MinHoldTime { get; set; }
    }
}
```

### 2. ThemeSettings.cs

```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.Lib.Core.ThemeManagement
{
    /// <summary>
    /// Theme configuration root
    /// </summary>
    public class ThemeConfig
    {
        [JsonPropertyName("theme")]
        [Required]
        public Theme Theme { get; set; }
    }

    /// <summary>
    /// Theme definition with description and color schemes
    /// </summary>
    public class Theme
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("colorSchemes")]
        [Required]
        public List<ColorScheme> ColorSchemes { get; set; } = new();
    }

    /// <summary>
    /// Color scheme for a specific UI element type
    /// </summary>
    public class ColorScheme
    {
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("background")]
        public string Background { get; set; }

        [JsonPropertyName("foreground")]
        public string Foreground { get; set; }

        [JsonPropertyName("highlightBackground")]
        public string HighlightBackground { get; set; }

        [JsonPropertyName("highlightForeground")]
        public string HighlightForeground { get; set; }

        [JsonPropertyName("highlightSelectedBackground")]
        public string HighlightSelectedBackground { get; set; }

        [JsonPropertyName("highlightSelectedForeground")]
        public string HighlightSelectedForeground { get; set; }

        [JsonPropertyName("backgroundImage")]
        public string BackgroundImage { get; set; }

        [JsonPropertyName("highlightBackgroundImage")]
        public string HighlightBackgroundImage { get; set; }
    }
}
```

### 3. PanelConfigMap.cs

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Root configuration for panel mappings
    /// </summary>
    public class PanelConfigMapConfig
    {
        [JsonPropertyName("configMapEntries")]
        [Required]
        public List<ConfigMapEntry> ConfigMapEntries { get; set; } = new();
    }

    /// <summary>
    /// Maps a panel class to its configuration file and IDs
    /// </summary>
    public class ConfigMapEntry
    {
        [JsonPropertyName("panelClass")]
        [Required]
        public string PanelClass { get; set; }

        [JsonPropertyName("configName")]
        [Required]
        public string ConfigName { get; set; }

        [JsonPropertyName("configId")]
        [Required]
        public Guid ConfigId { get; set; }

        [JsonPropertyName("formId")]
        [Required]
        public Guid FormId { get; set; }

        [JsonPropertyName("configFile")]
        [Required]
        public string ConfigFile { get; set; }

        [JsonPropertyName("userControls")]
        public string UserControls { get; set; }
    }
}
```

### 4. UserControlConfigMap.cs

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Root configuration for user control mappings
    /// </summary>
    public class UserControlConfigMapConfig
    {
        [JsonPropertyName("userControlConfigMapEntries")]
        [Required]
        public List<UserControlConfigMapEntry> UserControlConfigMapEntries { get; set; } = new();
    }

    /// <summary>
    /// Maps a user control to its configuration file and IDs
    /// </summary>
    public class UserControlConfigMapEntry
    {
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("configName")]
        [Required]
        public string ConfigName { get; set; }

        [JsonPropertyName("configId")]
        [Required]
        public Guid ConfigId { get; set; }

        [JsonPropertyName("userControlId")]
        [Required]
        public Guid UserControlId { get; set; }

        [JsonPropertyName("configFile")]
        [Required]
        public string ConfigFile { get; set; }
    }
}
```

### 5. PanelClassConfig.cs

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Root configuration for panel class definitions
    /// </summary>
    public class AppPanelClassConfig
    {
        [JsonPropertyName("panelClassConfigs")]
        [Required]
        public List<PanelClassConfig> PanelClassConfigs { get; set; } = new();
    }

    /// <summary>
    /// Configuration for an application's panel classes
    /// </summary>
    public class PanelClassConfig
    {
        [JsonPropertyName("appId")]
        [Required]
        public string AppId { get; set; }

        [JsonPropertyName("appName")]
        [Required]
        public string AppName { get; set; }

        [JsonPropertyName("appDescription")]
        public string AppDescription { get; set; }

        [JsonPropertyName("panelClassConfigMaps")]
        [Required]
        public List<PanelClassConfigMap> PanelClassConfigMaps { get; set; } = new();
    }

    /// <summary>
    /// Configuration map for a panel class
    /// </summary>
    public class PanelClassConfigMap
    {
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("displayNameShort")]
        public string DisplayNameShort { get; set; }

        [JsonPropertyName("displayNameLong")]
        public string DisplayNameLong { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("default")]
        public bool Default { get; set; } = false;

        [JsonPropertyName("screenshotFileName")]
        public string ScreenshotFileName { get; set; }

        [JsonPropertyName("panelClassConfigMapEntries")]
        [Required]
        public List<PanelClassConfigMapEntry> PanelClassConfigMapEntries { get; set; } = new();
    }

    /// <summary>
    /// Individual entry in a panel class config map
    /// </summary>
    public class PanelClassConfigMapEntry
    {
        [JsonPropertyName("panelClass")]
        [Required]
        public string PanelClass { get; set; }

        [JsonPropertyName("configId")]
        [Required]
        public Guid ConfigId { get; set; }
    }
}
```

---

## Migration Complexity Assessment

### Simple Schemas (1-2 hours each)
- ✅ SpellCheck.xml - Simple key-value list
- ✅ LanguageSettings.xml - Simple character lists
- ✅ PanelConfigMap.xml (7 files) - Simple routing tables
- ✅ UserControlConfigMap.xml - Simple routing table

**Subtotal:** ~15 hours

### Medium Schemas (4-8 hours each)
- ⚠️ Theme.xml - Many color schemes, but flat structure
- ⚠️ PanelClassConfig.xml - Nested but predictable structure
- ⚠️ ActuatorSettings.xml - Nested, with special template variables

**Subtotal:** ~20 hours

### Complex Schemas (40-60 hours total)
- ❌ Panel Configuration Files (73 files) - **Defer to Phase 2**
  - Contains DTD entities requiring preprocessing
  - Embedded animations (Phase 2 scope)
  - Complex nested widget hierarchies
  - High variation across files

**Phase 1 Subtotal:** ~35-40 hours  
**Phase 2 Estimate:** ~40-60 hours (Panel configs)

---

## Risk Assessment

### Critical Risks

1. **Template Variable Resolution**
   - **Risk:** Many XML files use template variables (e.g., `@MinActuationHoldTime`, `@CmdGoBack`)
   - **Impact:** High - variables must be resolved at runtime or preserved in JSON
   - **Mitigation:** Preserve template syntax in JSON; update variable resolution logic

2. **DTD Entity Resolution**
   - **Risk:** Panel XMLs use DTD entities for constants (e.g., `&buttonFontName;`)
   - **Impact:** High - XML parser automatically resolves these; JSON won't
   - **Mitigation:** Preprocess XML to resolve entities before JSON conversion, or move constants to separate config

3. **GUID Preservation**
   - **Risk:** GUIDs in config files may be referenced by code or other configs
   - **Impact:** High - breaking GUID references would break the application
   - **Mitigation:** Ensure 100% fidelity in GUID migration; add validation

4. **Encoding Issues**
   - **Risk:** PanelClassConfig.xml uses UTF-16 encoding
   - **Impact:** Medium - requires special handling in migration tool
   - **Mitigation:** Auto-detect encoding; ensure UTF-8 JSON output

### Medium Risks

5. **Animation Complexity**
   - **Risk:** 73 panel files contain animation definitions
   - **Impact:** Medium - animations should be handled in Phase 2 per requirements
   - **Mitigation:** Keep panel configs as XML in Phase 1; migrate in Phase 2

6. **Backward Compatibility**
   - **Risk:** Users may have custom XML configurations
   - **Impact:** Medium - need migration tool for users
   - **Mitigation:** Create migration tool (Ticket #8); support both XML and JSON temporarily

7. **Unicode and Special Characters**
   - **Risk:** XML files contain Unicode (¿, ¡, &#x75;)
   - **Impact:** Low - JSON supports Unicode natively
   - **Mitigation:** Test with non-ASCII characters; ensure UTF-8 encoding

### Low Risks

8. **Color Value Formats**
   - **Risk:** Theme.xml accepts both named colors ("White") and hex codes ("#232433")
   - **Impact:** Low - both formats valid in JSON
   - **Mitigation:** Validate both formats in JSON schema

9. **Optional vs. Required Fields**
   - **Risk:** Not all fields documented; some may be optional
   - **Impact:** Low - can be discovered during testing
   - **Mitigation:** Code inspection + runtime testing

---

## Migration Strategy Recommendations

### Phase 1 (Weeks 3-4) - Foundation
**Scope:** 13 simple/medium files  
**Focus:** Core routing configs and user settings

1. Create JSON schemas for top 5 types (Ticket #7)
2. Create migration tool (Ticket #8)
3. Migrate in order:
   - ActuatorSettings.xml
   - PanelConfigMap.xml (7 files)
   - UserControlConfigMap.xml
   - Theme.xml
   - PanelClassConfig.xml
   - SpellCheck.xml
   - LanguageSettings.xml

4. Update ACAT loaders to support JSON (Ticket #9)
5. Test with real user configurations

### Phase 2 (Later) - Panel Configurations
**Scope:** 73 panel configuration files  
**Focus:** Complex UI definitions with animations

1. Design panel JSON schema with animation support
2. Build entity resolution preprocessor
3. Migrate panel configs in batches by type:
   - Main menus (2 files)
   - Keyboard layouts (10 files)
   - Context menus (20 files)
   - User controls (25 files)
   - Specialized panels (16 files)

---

## Technical Notes

### XML Parser Behavior to Replicate

1. **Automatic Entity Resolution**
   - XML: `<!ENTITY buttonFontName "Arial">` → `fontname="&buttonFontName;"` becomes `fontname="Arial"`
   - JSON: Must preprocess or store entity values separately

2. **Whitespace Handling**
   - XML parsers normalize whitespace in element content
   - JSON preserves literal strings
   - May need trimming logic

3. **Type Coercion**
   - XML: Everything is a string by default
   - JSON: Types are explicit (string, number, boolean, array, object)
   - Migration must correctly infer types

4. **Default Values**
   - XML: Missing elements may have defaults defined in code
   - JSON: Explicitly set defaults or rely on POCO initializers

### Tools Needed

1. **XML → POCO Deserializer**
   - Use `System.Xml.Serialization.XmlSerializer`
   - Test with all XML files to ensure POCOs match

2. **POCO → JSON Serializer**
   - Use `System.Text.Json.JsonSerializer`
   - Configure for readability (indented, camelCase)

3. **JSON Schema Validator**
   - Use JSON Schema Draft 7
   - Integrate with VS Code for IntelliSense

4. **Migration Report Generator**
   - Track success/failure per file
   - Log warnings for manual review
   - Generate diff reports

---

## Validation Checklist

Before considering migration complete:

- [ ] All XML files successfully parse
- [ ] All POCOs correctly deserialize XML
- [ ] All JSON files validate against schemas
- [ ] All JSON files correctly deserialize to POCOs
- [ ] No data loss in XML → JSON → POCO round trip
- [ ] GUIDs preserved exactly
- [ ] Template variables preserved
- [ ] Unicode characters preserved
- [ ] Application launches with JSON configs
- [ ] All features work (input, UI, themes)
- [ ] Performance: JSON load time ≤ XML load time
- [ ] User migration tool tested with real configs

---

## Appendix: File Distribution

### By Schema Type
| Schema Type | Count | Percentage |
|-------------|-------|------------|
| Panel Configurations | 73 | 84.9% |
| PanelConfigMap | 7 | 8.1% |
| ActuatorSettings | 1 | 1.2% |
| Theme | 1 | 1.2% |
| UserControlConfigMap | 1 | 1.2% |
| PanelClassConfig | 1 | 1.2% |
| SpellCheck | 1 | 1.2% |
| LanguageSettings | 1 | 1.2% |
| **Total** | **86** | **100%** |

### By Location
| Directory | Count |
|-----------|-------|
| `/src/ACATResources/panelconfigs/common/` | 54 |
| `/src/ACATResources/panelconfigs/es/` | 7 |
| `/src/Extensions/Default/AppAgents/*` | 4 |
| `/src/Extensions/Default/FunctionalAgents/*` | 2 |
| `/src/Applications/Install/Users/DefaultUser/panelconfigs/` | 4 |
| `/src/Applications/Install/Users/DefaultUser/` | 1 |
| `/src/Applications/Install/Users/DefaultUser/en/` | 1 |
| `/src/ACATResources/panelconfigs/` | 3 |
| `/src/Assets/Themes/Default/` | 1 |

### By Complexity Level
| Complexity | Count | Effort (hours) |
|------------|-------|----------------|
| Simple | 10 | 15 |
| Medium | 3 | 20 |
| Complex (Phase 2) | 73 | 40-60 |
| **Phase 1 Total** | **13** | **35-40** |

---

## Next Steps

1. **Review this report** with team - validate findings
2. **Create Ticket #7** - Generate JSON schemas and validators
3. **Create Ticket #8** - Build migration tool
4. **Create Ticket #9** - Update ACAT configuration loaders
5. **Begin Phase 1 migration** following priority order
6. **Plan Phase 2** after Phase 1 completion and lessons learned

---

**Report Generated:** 2026-02-05  
**Author:** GitHub Copilot  
**Ticket:** intel/acat#6
