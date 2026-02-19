# ACAT Testing Quick Reference

**Version**: 1.0 | **Target**: Developers | **Updated**: February 2026

---

## 🎯 Test Types

| Type | Speed | Dependencies | Use For |
|------|-------|-------------|---------|
| **Unit** | Fast (<100ms) | Mocked | Single components |
| **Integration** | Medium (<5s) | Real/Mixed | Component interaction |
| **Performance** | Varies | Real | Performance validation |

---

## 📦 Setup

### 1. Add Reference
```xml
<ProjectReference Include="..\ACATCore.Tests.Shared\ACATCore.Tests.Shared.csproj" />
<PackageReference Include="Moq" Version="4.20.72" />
```

### 2. Inherit BaseTest
```csharp
[TestClass]
public class MyTests : BaseTest { }
```

---

## 🔨 Common Patterns

### Unit Test (AAA Pattern)
```csharp
[TestMethod]
[TestCategory("Unit")]
public void Method_Scenario_ExpectedBehavior()
{
    // Arrange
    var mock = MockHelper.CreateMockLogger<MyClass>();
    var sut = new MyClass(mock.Object);
    
    // Act
    var result = sut.DoWork();
    
    // Assert
    Assert.IsNotNull(result);
    MockHelper.VerifyLoggerCalled(mock, LogLevel.Information);
}
```

### Integration Test
```csharp
[TestMethod]
[TestCategory("Integration")]
public void Integration_Scenario_ExpectedBehavior()
{
    using (var workspace = new TestWorkspace("MyTest"))
    {
        // Arrange
        workspace.CreateFile("config.json", "{}");
        var service = new MyService(workspace.WorkspaceRoot);
        
        // Act
        var result = service.Process();
        
        // Assert
        Assert.IsTrue(workspace.FileExists("output.txt"));
    }
}
```

### Performance Test
```csharp
[TestMethod]
[TestCategory("Performance")]
public void Performance_LargeDataset_CompletesQuickly()
{
    var data = TestDataGenerator.RandomList(() => new Item(), 10000);
    
    AssertCompletesWithin(() =>
    {
        processor.ProcessAll(data);
    }, TimeSpan.FromSeconds(1), "10K items in <1s");
}
```

---

## 🎭 Mocking

### Create Mocks
```csharp
// Simple mock
var mock = MockHelper.CreateMockLogger<MyClass>();

// Capturing mock
var messages = new List<string>();
var mock = MockHelper.CreateCapturingLogger<MyClass>(messages);

// Strict mock (throws on unexpected calls)
var mock = MockHelper.CreateStrictMock<IMyInterface>();
```

### Setup & Verify
```csharp
// Setup
mock.Setup(x => x.Method(It.IsAny<string>())).Returns("result");

// Verify
MockHelper.VerifyLoggerCalled(mock, LogLevel.Information);
MockHelper.VerifyLoggerCalledWithMessage(mock, "expected");
mock.Verify(x => x.Method("value"), Times.Once);
```

---

## 📊 Test Data

### Random Data
```csharp
string name = TestDataGenerator.RandomString(10);
int value = TestDataGenerator.RandomInt(1, 100);
bool flag = TestDataGenerator.RandomBool();
string guid = TestDataGenerator.RandomGuid();
string item = TestDataGenerator.RandomItem("A", "B", "C");
var list = TestDataGenerator.RandomList(() => new Item(), 10);
```

### Builder Pattern
```csharp
public class MyDataBuilder : TestDataBuilder<MyData>
{
    private string _name = "Default";
    
    public MyDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public override MyData Build()
    {
        return new MyData { Name = _name };
    }
    
    public override void Reset()
    {
        _name = "Default";
    }
}

// Usage
var data = new MyDataBuilder().WithName("Test").Build();
```

---

## ✅ Assertions

### Collections
```csharp
AssertHelper.CollectionContainsExactly(items, "A", "B", "C");
AssertHelper.CollectionContainsAll(items, "A", "B");
AssertHelper.CollectionDoesNotContain(items, "X", "Y");
```

### Strings
```csharp
AssertHelper.StringContains(text, "substring");
AssertHelper.StringDoesNotContain(text, "bad");
AssertHelper.StringStartsWith(text, "prefix");
AssertHelper.StringEndsWith(text, "suffix");
```

### Ranges & Numbers
```csharp
AssertHelper.InRange(value, 1, 100);
AssertHelper.DoubleClose(actual, expected, 0.001);
AssertHelper.DateTimeClose(actual, expected, TimeSpan.FromSeconds(1));
```

### Predicates
```csharp
AssertHelper.All(items, x => x.IsValid);
AssertHelper.Any(items, x => x.IsSpecial);
AssertHelper.None(items, x => x.IsDeleted);
```

### Exceptions & Timing
```csharp
AssertThrows<ArgumentNullException>(() => Method(null));
AssertNoThrow(() => Method(validArg));
AssertCompletesWithin(() => LongOperation(), TimeSpan.FromSeconds(5));
```

---

## 📂 File Management

### BaseTest - Temporary Files
```csharp
// Automatic TestDirectory created/cleaned up
string file = CreateTempFile("test.txt", "content");
string dir = CreateTempDirectory("subdir");
```

### TestWorkspace - Integration Tests
```csharp
using (var workspace = new TestWorkspace("TestName"))
{
    // Create
    workspace.CreateDirectory("configs");
    workspace.CreateFile("configs/settings.json", "{}");
    
    // Check
    bool exists = workspace.FileExists("configs/settings.json");
    
    // Read
    string content = workspace.ReadFile("configs/settings.json");
    
    // Get path
    string fullPath = workspace.GetPath("configs");
    
    // List files
    var files = workspace.GetFiles("*.json");
    
    // Auto cleanup on dispose
}
```

---

## 🔍 Test Context

### Logging
```csharp
WriteTestInfo("Important information");
WriteTestDebug("Debug details");
WriteTestWarning("Warning message");
```

### Performance
```csharp
// Stopwatch automatically started
var elapsed = Stopwatch.ElapsedMilliseconds;

// Measure specific operation
var duration = MeasureTime(() => Operation());
```

---

## 🏃 Running Tests

### Command Line
```bash
# All tests
dotnet test

# Specific project
dotnet test Libraries/ACATCore.Tests.Logging/

# By category
dotnet test --filter "TestCategory=Unit"
dotnet test --filter "TestCategory=Integration"

# By name
dotnet test --filter "FullyQualifiedName~ActuatorSettings"

# Detailed output
dotnet test --logger "console;verbosity=detailed"

# Specific configuration
dotnet test --configuration Release
```

### Visual Studio
1. Open Test Explorer: `Test → Test Explorer` (Ctrl+E, T)
2. Build solution: `Ctrl+Shift+B`
3. Run All: Click "Run All" button
4. Run Selected: Right-click → Run Selected Tests
5. Debug: Right-click → Debug Selected Tests

---

## 📝 Naming Conventions

### Test Projects
Pattern: `[Component].Tests.[Category]`
- `ACATCore.Tests.Logging`
- `ACATCore.Tests.Configuration`
- `ACATCore.Tests.Integration`

### Test Classes
Pattern: `[ComponentName]Tests`
- `ActuatorSettingsTests`
- `LoggingInfrastructureTests`
- `ConfigurationServiceTests`

### Test Methods
Pattern: `[Method]_[Scenario]_[ExpectedBehavior]`
- `LoadConfiguration_ValidFile_ReturnsConfiguration`
- `ValidateSettings_EmptyList_ThrowsException`
- `SerializeJson_DefaultSettings_ProducesValidJson`

---

## 🏷️ Test Categories

```csharp
[TestCategory("Unit")]        // Fast, isolated unit tests
[TestCategory("Integration")] // Component interaction tests
[TestCategory("Performance")] // Performance validation tests
[TestCategory("Smoke")]       // Quick smoke tests
```

Filter by category:
```bash
dotnet test --filter "TestCategory=Unit"
dotnet test --filter "TestCategory=Integration"
```

---

## 🚫 Common Mistakes

### ❌ Don't
```csharp
// Shared state between tests
private static Data _data;

// Tests depending on order
[TestMethod] public void Test1() { /* modifies state */ }
[TestMethod] public void Test2() { /* depends on Test1 */ }

// No assertions
[TestMethod] public void Test() { Method(); } // No assert!

// Testing multiple things
[TestMethod] public void TestEverything() { /* 20 assertions */ }
```

### ✅ Do
```csharp
// Each test creates its own data
[TestMethod] public void Test1()
{
    var data = CreateTestData();
    // test code
}

// Independent tests
[TestMethod] public void Method_Scenario1_Behavior1() { }
[TestMethod] public void Method_Scenario2_Behavior2() { }

// Clear assertions
[TestMethod]
public void Method_Scenario_ExpectedBehavior()
{
    var result = Method();
    Assert.AreEqual(expected, result, "Clear message");
}

// Test one thing
[TestMethod]
public void LoadConfig_ValidFile_ReturnsConfiguration()
{
    var config = loader.Load("valid.json");
    Assert.IsNotNull(config);
}
```

---

## 📚 Documentation

**Comprehensive Guides**:
- [TESTING_INFRASTRUCTURE.md](TESTING_INFRASTRUCTURE.md) - Full infrastructure guide
- [TESTING_MIGRATION_GUIDE.md](TESTING_MIGRATION_GUIDE.md) - Migration guide
- [ACATCore.Tests.Shared README](src/Libraries/ACATCore.Tests.Shared/README.md) - Library docs

**Examples**:
- [ExampleTests.cs](src/Libraries/ACATCore.Tests.Shared/ExampleTests.cs) - Usage examples
- [TESTING_GUIDE.md](src/Libraries/ACATCore.Tests.Configuration/TESTING_GUIDE.md) - Configuration tests

**Project Docs**:
- [QUICK_START_INTEGRATION_TESTS.md](QUICK_START_INTEGRATION_TESTS.md) - Quick start
- [Phase 1 Integration Testing](PHASE_1_INTEGRATION_TESTING_SUMMARY.md) - Phase 1 report

---

## 💡 Tips

1. **Inherit BaseTest** - Automatic setup/cleanup
2. **Use TestWorkspace** - Isolated file operations
3. **Mock dependencies** - Fast, reliable tests
4. **Test one thing** - Single responsibility
5. **Clear names** - Self-documenting tests
6. **Add categories** - Enable filtering
7. **Write tests first** - TDD approach
8. **Keep tests fast** - Quick feedback loop

---

## 🆘 Help

**Questions?** See comprehensive docs above or contact ACAT dev team.

**Issues?** Check [Troubleshooting](TESTING_INFRASTRUCTURE.md#troubleshooting) section.

---

**Last Updated**: February 2026 | **Version**: 1.0
