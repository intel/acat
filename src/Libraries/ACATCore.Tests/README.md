# ACATCore.Tests — Test Data Framework

This project provides the shared test fixture infrastructure and test data builders for all ACAT unit and integration tests.

---

## Structure

```
ACATCore.Tests/
├── Fixtures/
│   └── TestFixtureBase.cs          # Base class for MSTest fixtures
├── Builders/
│   ├── ConfigurationBuilder.cs     # Fluent builders for JSON configuration POCOs
│   ├── PreferencesBuilder.cs       # Builder for XML preferences data
│   ├── ScannerBuilder.cs           # Builder for scanner descriptor test data
│   └── AgentBuilder.cs             # Builder for agent descriptor test data
└── TestData/
    ├── TestDataManager.cs          # State/directory management for tests
    ├── sample-abbreviations.json   # Sample abbreviations configuration
    ├── sample-actuator-settings.json # Sample actuator settings configuration
    ├── sample-panel-config.json    # Sample panel/scanner configuration
    └── sample-preferences.xml      # Sample preferences XML file
```

---

## Fixtures

### `TestFixtureBase`

All MSTest test classes that need isolated temp directory management should inherit from `TestFixtureBase`.

```csharp
[TestClass]
public class MyTests : TestFixtureBase
{
    protected override void OnSetUp()
    {
        // TestDirectory is already created and available here
    }

    [TestMethod]
    public void MyTest()
    {
        string file = WriteTestFile("config.json", "{}");
        // file is inside TestDirectory and cleaned up automatically
    }
}
```

**Features:**
- `TestDirectory` — a unique temp directory per test, deleted on teardown.
- `CreateTempDirectory()` — create additional isolated directories.
- `WriteTestFile(name, content)` — write a file inside `TestDirectory`.
- `RegisterDisposable<T>(resource)` — auto-dispose resources at teardown.
- `OnSetUp()` / `OnTearDown()` — override hooks for test-specific logic.

---

## Builders

All builders follow the **fluent builder** pattern: call `WithXxx()` methods to configure the object, then call `Build()` to obtain the constructed instance.

### `AbbreviationsConfigurationBuilder`

Builds `AbbreviationsJson` instances for configuration tests.

```csharp
var config = new AbbreviationsConfigurationBuilder()
    .WithAbbreviation("btw", "by the way")
    .WithAbbreviation("omg", "oh my goodness", "Speak")
    .Build();

// Or use pre-populated defaults:
var config = AbbreviationsConfigurationBuilder.WithDefaults().Build();
```

### `ActuatorSettingsConfigurationBuilder`

Builds `ActuatorSettingsJson` instances.

```csharp
var settings = new ActuatorSettingsConfigurationBuilder()
    .WithKeyboardActuator()
    .Build();

var settings = ActuatorSettingsConfigurationBuilder.WithDefaults().Build();
```

### `PanelConfigurationBuilder`

Builds `PanelConfigJson` instances.

```csharp
var panel = new PanelConfigurationBuilder()
    .WithColorScheme("Dialog")
    .WithWidgetAttribute("Title", "My Panel")
    .Build();

var menu = PanelConfigurationBuilder.AsSimpleMenu().Build();
```

### `PreferencesBuilder`

Produces XML strings for loading through `PreferencesBase.Load<T>`.

```csharp
string xml = new PreferencesBuilder()
    .WithProperty("AutoSwitchScannerEnable", true)
    .WithProperty("ScanTime", 1000)
    .BuildXml();

File.WriteAllText(prefsFile, xml);
var prefs = PreferencesBase.Load<MyPreferences>(prefsFile);
```

### `ScannerBuilder`

Builds `ScannerDescriptor` objects describing a scanner configuration.

```csharp
var scanner = ScannerBuilder.AsAlphabetScanner()
    .WithScanTime(500)
    .Build();

var cursor = ScannerBuilder.AsCursorScanner().Build();
```

### `AgentBuilder`

Builds `AgentDescriptor` objects describing an agent configuration.

```csharp
var agent = AgentBuilder.AsNotepadAgent()
    .WithEnabled(true)
    .Build();

var generic = new AgentBuilder()
    .WithName("MyAgent")
    .WithSupportedProcess("myapp")
    .Build();
```

---

## Test Data Manager

`TestDataManager` provides:

- **Temp directory lifecycle** — call `CreateTempDirectory()` and all directories are deleted when `Dispose()` is called.
- **Sample file access** — `GetSampleFilePath(fileName)`, `ReadSampleFile(fileName)`, and `CopySampleFileTo(fileName, dir)` resolve paths to the bundled `TestData/` sample files.
- **In-memory state store** — `SetState` / `GetState` / `ClearState` for sharing state within a single test run.

```csharp
using (var manager = new TestDataManager())
{
    string dir = manager.CreateTempDirectory();
    string dest = TestDataManager.CopySampleFileTo("sample-abbreviations.json", dir);
    // use dest...
} // dir is deleted automatically
```

---

## Sample Configuration Files

| File | Description |
|---|---|
| `sample-abbreviations.json` | Three abbreviation entries (Write and Speak modes) |
| `sample-actuator-settings.json` | Single keyboard actuator with two switch bindings |
| `sample-panel-config.json` | Simple two-row panel with a title and one button |
| `sample-preferences.xml` | Minimal XML preferences with three properties |

These files are copied to the test output directory at build time (`PreserveNewest`).
