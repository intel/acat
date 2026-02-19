# ACAT Testing Infrastructure

**Version**: 1.0  
**Last Updated**: February 2026  
**Status**: Active

---

## Overview

This document describes the comprehensive testing infrastructure for the ACAT project, including unit testing, integration testing, mocking, test data management, and CI/CD automation.

## Table of Contents

1. [Testing Philosophy](#testing-philosophy)
2. [Test Organization](#test-organization)
3. [Test Types](#test-types)
4. [Testing Framework](#testing-framework)
5. [Shared Test Utilities](#shared-test-utilities)
6. [Writing Tests](#writing-tests)
7. [Running Tests](#running-tests)
8. [CI/CD Integration](#cicd-integration)
9. [Best Practices](#best-practices)
10. [Troubleshooting](#troubleshooting)

---

## Testing Philosophy

### Core Principles

1. **Testability First**: Design code with testing in mind
2. **Fast Feedback**: Tests should run quickly and provide immediate feedback
3. **Isolation**: Tests should be independent and not affect each other
4. **Repeatability**: Tests should produce the same results every time
5. **Clarity**: Tests should be easy to read and understand
6. **Comprehensive Coverage**: Test happy paths, edge cases, and error conditions

### Testing Pyramid

```
     /\
    /  \    End-to-End Tests (Few)
   /----\
  /      \  Integration Tests (Some)
 /--------\
/__________\ Unit Tests (Many)
```

- **Unit Tests (70%)**: Fast, isolated tests of individual components
- **Integration Tests (20%)**: Tests of component interactions
- **End-to-End Tests (10%)**: Full system tests

---

## Test Organization

### Directory Structure

```
src/
├── Libraries/
│   ├── ACATCore.Tests.Shared/          # Shared test utilities
│   │   ├── BaseTest.cs                 # Base test class
│   │   ├── MockHelper.cs               # Mocking utilities
│   │   ├── TestDataBuilder.cs          # Test data builders
│   │   ├── TestFixture.cs              # Fixture management
│   │   ├── AssertHelper.cs             # Enhanced assertions
│   │   ├── ExampleTests.cs             # Usage examples
│   │   └── README.md                   # Documentation
│   │
│   ├── ACATCore.Tests.Logging/         # Logging unit tests
│   ├── ACATCore.Tests.Configuration/   # Configuration unit tests
│   └── ACATCore.Tests.Integration/     # Integration tests
│
└── Applications/
    └── ConfigMigrationTool/
        └── ACAT.ConfigMigrationTool.Tests/  # Tool-specific tests
```

### Naming Conventions

**Test Projects**: `[Component].Tests.[Category]`
- Examples: `ACATCore.Tests.Logging`, `ACATCore.Tests.Integration`

**Test Classes**: `[ComponentName]Tests`
- Examples: `ActuatorSettingsTests`, `LoggingInfrastructureTests`

**Test Methods**: `[Method]_[Scenario]_[ExpectedBehavior]`
- Examples:
  - `LoadConfiguration_ValidFile_ReturnsConfiguration`
  - `ValidateSettings_NoActuators_ThrowsException`
  - `SerializeToJson_DefaultSettings_ProducesValidJson`

---

## Test Types

### 1. Unit Tests

**Purpose**: Test individual components in isolation

**Characteristics**:
- Fast execution (< 100ms per test)
- No external dependencies
- Use mocks for dependencies
- Test single responsibility

**Example**:
```csharp
[TestClass]
public class ActuatorSettingsTests : BaseTest
{
    [TestMethod]
    public void CreateDefault_NoParameters_ReturnsValidSettings()
    {
        // Arrange & Act
        var settings = ActuatorSettingsJson.CreateDefault();
        
        // Assert
        Assert.IsNotNull(settings);
        Assert.IsNotNull(settings.ActuatorSettings);
        Assert.AreEqual(1, settings.ActuatorSettings.Count);
    }
}
```

### 2. Integration Tests

**Purpose**: Test component interactions and system integration

**Characteristics**:
- Slower execution (< 5s per test)
- May use real dependencies
- Test multiple components together
- Isolated test environment

**Example**:
```csharp
[TestClass]
public class ConfigurationIntegrationTests : BaseTest
{
    [TestMethod]
    public void LoadAndValidate_ValidConfiguration_Succeeds()
    {
        using (var workspace = new TestWorkspace("ConfigTest"))
        {
            // Arrange
            workspace.CreateFile("config.json", validJsonContent);
            var loader = new JsonConfigurationLoader<Settings>();
            
            // Act
            var settings = loader.Load(workspace.GetPath("config.json"));
            
            // Assert
            Assert.IsNotNull(settings);
            Assert.IsTrue(settings.IsValid);
        }
    }
}
```

### 3. Performance Tests

**Purpose**: Validate performance requirements

**Characteristics**:
- Measure execution time
- Test resource usage
- Validate against benchmarks
- May run multiple iterations

**Example**:
```csharp
[TestMethod]
public void LoggingPerformance_10000Messages_CompletesUnder100ms()
{
    // Arrange
    var logger = CreateLogger();
    
    // Act
    AssertCompletesWithin(() =>
    {
        for (int i = 0; i < 10000; i++)
        {
            logger.LogInformation($"Message {i}");
        }
    }, TimeSpan.FromMilliseconds(100), "10K messages should log in < 100ms");
}
```

---

## Testing Framework

### MSTest Framework

ACAT uses **MSTest 3.7.0** as the primary testing framework.

**Key Attributes**:
- `[TestClass]`: Marks a class as containing tests
- `[TestMethod]`: Marks a method as a test
- `[TestInitialize]`: Runs before each test
- `[TestCleanup]`: Runs after each test
- `[ClassInitialize]`: Runs once before any test in the class
- `[ClassCleanup]`: Runs once after all tests in the class
- `[TestCategory("Category")]`: Categorizes tests for filtering

**Test Categories**:
- `[TestCategory("Unit")]`: Unit tests
- `[TestCategory("Integration")]`: Integration tests
- `[TestCategory("Performance")]`: Performance tests
- `[TestCategory("Smoke")]`: Quick smoke tests

### Moq Framework

**Moq 4.20.72** is used for creating test doubles (mocks, stubs, spies).

**Common Patterns**:

```csharp
// Create a mock
var mockLogger = new Mock<ILogger<MyClass>>();

// Setup behavior
mockLogger.Setup(x => x.LogInformation(It.IsAny<string>()))
    .Callback<string>(msg => Console.WriteLine(msg));

// Use the mock
var sut = new MyClass(mockLogger.Object);
sut.DoWork();

// Verify calls
mockLogger.Verify(x => x.LogInformation("Expected message"), Times.Once);
```

### FluentValidation

**FluentValidation 11.9.0** is used for configuration validation testing.

```csharp
[TestMethod]
public void Validate_InvalidSettings_ReturnsErrors()
{
    // Arrange
    var validator = new ActuatorSettingsValidator();
    var settings = new ActuatorSettingsJson(); // Empty
    
    // Act
    var result = validator.Validate(settings);
    
    // Assert
    Assert.IsFalse(result.IsValid);
    Assert.IsTrue(result.Errors.Count > 0);
}
```

---

## Shared Test Utilities

The `ACATCore.Tests.Shared` library provides common testing infrastructure.

### BaseTest Class

Inherit from `BaseTest` for automatic setup/teardown:

```csharp
[TestClass]
public class MyTests : BaseTest
{
    [TestMethod]
    public void MyTest()
    {
        // TestDirectory is automatically created
        var file = CreateTempFile("test.txt", "content");
        
        // Stopwatch is running
        WriteTestInfo($"Elapsed: {Stopwatch.ElapsedMilliseconds}ms");
        
        // Automatic cleanup happens
    }
}
```

**Features**:
- Automatic test directory creation/cleanup
- Performance measurement with Stopwatch
- Test logging (WriteTestInfo, WriteTestDebug, WriteTestWarning)
- Helper methods (CreateTempFile, CreateTempDirectory)
- Common assertions (AssertThrows, AssertNoThrow, AssertCompletesWithin)

### MockHelper

Create and configure mocks easily:

```csharp
// Create mock logger
var mockLogger = MockHelper.CreateMockLogger<MyClass>();

// Create capturing logger
var messages = new List<string>();
var capturingLogger = MockHelper.CreateCapturingLogger<MyClass>(messages);

// Verify calls
MockHelper.VerifyLoggerCalled(mockLogger, LogLevel.Information);
MockHelper.VerifyLoggerCalledWithMessage(mockLogger, "expected");
```

### TestDataGenerator

Generate random test data:

```csharp
// Random values
string name = TestDataGenerator.RandomString(10);
int value = TestDataGenerator.RandomInt(1, 100);
bool flag = TestDataGenerator.RandomBool();
string guid = TestDataGenerator.RandomGuid();
DateTime date = TestDataGenerator.RandomDate();

// Random selections
string item = TestDataGenerator.RandomItem("A", "B", "C");

// Random lists
var items = TestDataGenerator.RandomList(() => new MyData(), 10);
```

### TestWorkspace

Isolated workspace for integration tests:

```csharp
using (var workspace = new TestWorkspace("MyTest"))
{
    // Create structure
    workspace.CreateDirectory("configs");
    workspace.CreateFile("configs/settings.json", "{}");
    
    // Verify
    Assert.IsTrue(workspace.FileExists("configs/settings.json"));
    
    // Read
    string content = workspace.ReadFile("configs/settings.json");
    
    // Get paths
    string fullPath = workspace.GetPath("configs");
    
    // Automatic cleanup
}
```

### AssertHelper

Enhanced assertions:

```csharp
// Collections
AssertHelper.CollectionContainsExactly(actual, "A", "B", "C");
AssertHelper.CollectionContainsAll(actual, "A", "B");

// Strings
AssertHelper.StringContains(actual, "substring");
AssertHelper.StringStartsWith(actual, "prefix");

// Ranges
AssertHelper.InRange(value, 1, 100);
AssertHelper.DoubleClose(actual, expected, 0.001);

// Predicates
AssertHelper.All(items, x => x.IsValid);
AssertHelper.Any(items, x => x.IsSpecial);
```

---

## Writing Tests

### Test Structure (AAA Pattern)

**Arrange-Act-Assert**:

```csharp
[TestMethod]
public void Method_Scenario_ExpectedBehavior()
{
    // Arrange: Set up test data and dependencies
    var mockLogger = MockHelper.CreateMockLogger<MyClass>();
    var sut = new MyClass(mockLogger.Object);
    var input = "test data";
    
    // Act: Execute the code under test
    var result = sut.ProcessInput(input);
    
    // Assert: Verify the expected outcome
    Assert.IsNotNull(result);
    Assert.AreEqual("expected", result.Value);
    MockHelper.VerifyLoggerCalled(mockLogger, LogLevel.Information);
}
```

### Test Data Builders

Create reusable test data builders:

```csharp
public class ActuatorSettingsBuilder : TestDataBuilder<ActuatorSettingsJson>
{
    private List<ActuatorSettingJson> _actuators = new List<ActuatorSettingJson>();
    
    public ActuatorSettingsBuilder WithActuator(string name, bool enabled = true)
    {
        _actuators.Add(new ActuatorSettingJson
        {
            Name = name,
            Id = TestDataGenerator.RandomGuid(),
            Enabled = enabled
        });
        return this;
    }
    
    public override ActuatorSettingsJson Build()
    {
        return new ActuatorSettingsJson { ActuatorSettings = _actuators };
    }
    
    public override void Reset()
    {
        _actuators.Clear();
    }
}

// Usage
var settings = new ActuatorSettingsBuilder()
    .WithActuator("Keyboard", true)
    .WithActuator("Camera", false)
    .Build();
```

### Mocking Dependencies

```csharp
[TestMethod]
public void ProcessData_ValidInput_LogsInformation()
{
    // Arrange
    var mockLogger = MockHelper.CreateMockLogger<DataProcessor>();
    var mockConfig = new Mock<IConfiguration>();
    mockConfig.Setup(x => x.GetValue("Setting")).Returns("value");
    
    var processor = new DataProcessor(mockLogger.Object, mockConfig.Object);
    
    // Act
    processor.ProcessData("input");
    
    // Assert
    MockHelper.VerifyLoggerCalled(mockLogger, LogLevel.Information);
    mockConfig.Verify(x => x.GetValue("Setting"), Times.Once);
}
```

---

## Running Tests

### From Command Line

```bash
# Run all tests
cd src
dotnet test

# Run specific test project
dotnet test Libraries/ACATCore.Tests.Logging/ACATCore.Tests.Logging.csproj

# Run tests by category
dotnet test --filter "TestCategory=Unit"
dotnet test --filter "TestCategory=Integration"

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests in specific configuration
dotnet test --configuration Release

# Run tests with coverage (requires coverage tool)
dotnet test --collect:"XPlat Code Coverage"
```

### From Visual Studio

1. Open Test Explorer: `Test → Test Explorer` (Ctrl+E, T)
2. Build solution: `Build → Build Solution` (Ctrl+Shift+B)
3. Run tests:
   - Run All: Click "Run All" button
   - Run Selected: Right-click test → Run Selected Tests
   - Debug: Right-click test → Debug Selected Tests

### Test Filtering

```bash
# By test name
dotnet test --filter "FullyQualifiedName~ActuatorSettings"

# By category
dotnet test --filter "TestCategory=Unit"

# By multiple criteria
dotnet test --filter "TestCategory=Unit&FullyQualifiedName~Configuration"
```

---

## CI/CD Integration

### GitHub Actions Workflow

The `build.yml` workflow runs tests automatically:

```yaml
- name: Run Unit Tests - Logging
  run: |
    dotnet test Libraries/ACATCore.Tests.Logging/ACATCore.Tests.Logging.csproj 
      --configuration ${{ matrix.configuration }} 
      --logger "trx;LogFileName=logging-tests.trx" 
      --no-build 
      --results-directory TestResults

- name: Publish Test Results
  uses: dorny/test-reporter@v1
  if: always()
  with:
    name: Test Results (${{ matrix.configuration }})
    path: 'src/TestResults/*.trx'
    reporter: dotnet-trx
```

### Test Execution

Tests run automatically:
- **On Push**: To `master` branch
- **On PR**: Before merge
- **Manual**: Via workflow_dispatch

### Test Reports

- Test results uploaded as artifacts
- Test reports published with `test-reporter` action
- Failures prevent merge

---

## Best Practices

### 1. Test Naming

✅ **Good**:
```csharp
LoadConfiguration_FileNotFound_ThrowsFileNotFoundException()
ValidateSettings_EmptyActuatorList_ReturnsFalse()
SerializeJson_DefaultSettings_ReturnsValidJson()
```

❌ **Bad**:
```csharp
Test1()
TestLoadConfig()
ConfigurationTest()
```

### 2. Test Independence

✅ **Good**: Each test creates its own data
```csharp
[TestMethod]
public void Test1()
{
    var data = CreateTestData();
    // test code
}

[TestMethod]
public void Test2()
{
    var data = CreateTestData();
    // test code
}
```

❌ **Bad**: Tests share state
```csharp
private static MyData _sharedData;

[TestMethod]
public void Test1()
{
    _sharedData.Modify();
}

[TestMethod]
public void Test2()
{
    // Depends on Test1 running first
}
```

### 3. Clear Assertions

✅ **Good**: Specific assertions with messages
```csharp
Assert.AreEqual(expected, actual, "Configuration should load default settings");
AssertHelper.StringContains(result, "success", "Response should contain success message");
```

❌ **Bad**: Generic assertions
```csharp
Assert.IsTrue(result.Contains("something"));
```

### 4. Test One Thing

✅ **Good**: Single responsibility
```csharp
[TestMethod]
public void LoadConfig_ValidFile_ReturnsConfiguration()
{
    var config = loader.Load("valid.json");
    Assert.IsNotNull(config);
}

[TestMethod]
public void LoadConfig_InvalidFile_ThrowsException()
{
    AssertThrows<Exception>(() => loader.Load("invalid.json"));
}
```

❌ **Bad**: Multiple responsibilities
```csharp
[TestMethod]
public void TestEverything()
{
    // Tests loading, validation, saving, and error handling
}
```

### 5. Use Test Doubles Appropriately

**Mock**: For verification of behavior
```csharp
var mockLogger = new Mock<ILogger>();
// ... use logger ...
mockLogger.Verify(x => x.Log(...), Times.Once);
```

**Stub**: For providing data
```csharp
var stubConfig = new Mock<IConfig>();
stubConfig.Setup(x => x.GetValue()).Returns("value");
```

**Fake**: Simplified working implementation
```csharp
public class FakeRepository : IRepository
{
    private List<Item> _items = new List<Item>();
    public void Add(Item item) => _items.Add(item);
    public Item Get(int id) => _items.FirstOrDefault(x => x.Id == id);
}
```

### 6. Test Edge Cases

Always test:
- Empty collections/strings
- Null values
- Boundary values (min, max)
- Invalid input
- Exception conditions

### 7. Performance Tests

```csharp
[TestMethod]
[TestCategory("Performance")]
public void Operation_LargeDataset_CompletesQuickly()
{
    var data = GenerateLargeDataset(10000);
    
    AssertCompletesWithin(() =>
    {
        processor.Process(data);
    }, TimeSpan.FromSeconds(1), "Should process 10K items in < 1s");
}
```

---

## Troubleshooting

### Common Issues

#### 1. Tests Fail on Build Server but Pass Locally

**Possible Causes**:
- File path differences (Windows vs Linux)
- Time zone differences
- Environment variables
- Dependency on local resources

**Solution**:
- Use `Path.Combine` for paths
- Use UTC times or relative times
- Mock external dependencies
- Use TestWorkspace for file isolation

#### 2. Intermittent Test Failures

**Possible Causes**:
- Race conditions
- Shared state between tests
- Timing dependencies
- External resource availability

**Solution**:
- Make tests independent
- Use locks for concurrency
- Add retry logic where appropriate
- Mock external resources

#### 3. Slow Tests

**Possible Causes**:
- Too many integration tests
- Not using mocks
- Large data generation
- File I/O operations

**Solution**:
- Convert to unit tests where possible
- Use mocks for dependencies
- Generate minimal test data
- Use in-memory alternatives

#### 4. Test Cleanup Failures

**Possible Causes**:
- File locks (Windows)
- Permission issues
- Resources not disposed

**Solution**:
- Use `using` statements
- Add retry logic in cleanup
- Ensure proper disposal
- Use TestWorkspace which handles cleanup

### Debug Tips

1. **Run single test**: Isolate the failing test
2. **Add WriteTestInfo**: Log intermediate values
3. **Use debugger**: Set breakpoints in test code
4. **Check test output**: Review console output
5. **Verify assumptions**: Test your test data

---

## Summary

The ACAT testing infrastructure provides:

✅ **Comprehensive Framework**: MSTest + Moq + FluentValidation  
✅ **Shared Utilities**: BaseTest, MockHelper, TestDataBuilder, AssertHelper  
✅ **Test Organization**: Clear structure and naming conventions  
✅ **CI/CD Integration**: Automated testing in GitHub Actions  
✅ **Best Practices**: Documented patterns and examples  

**For Questions or Issues**: See project documentation or contact the ACAT development team.

---

**Related Documentation**:
- [ACATCore.Tests.Shared README](../src/Libraries/ACATCore.Tests.Shared/README.md)
- [Testing Guide](../src/Libraries/ACATCore.Tests.Configuration/TESTING_GUIDE.md)
- [Quick Start: Integration Tests](QUICK_START_INTEGRATION_TESTS.md)
- [Phase 1 Integration Testing Summary](PHASE_1_INTEGRATION_TESTING_SUMMARY.md)
