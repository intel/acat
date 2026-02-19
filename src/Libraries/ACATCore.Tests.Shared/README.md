# ACATCore.Tests.Shared

Shared testing utilities and base classes for all ACAT test projects.

## Overview

This library provides common testing infrastructure including:
- Base test classes with setup/teardown
- Test data builders and generators
- Mock helpers using Moq framework
- Test fixture management
- Enhanced assertion utilities

## Components

### BaseTest

Base class for all unit tests providing:
- Automatic test directory creation/cleanup
- Performance measurement with Stopwatch
- Test logging utilities
- Common assertions and helpers

**Usage:**
```csharp
[TestClass]
public class MyTests : BaseTest
{
    [TestMethod]
    public void TestSomething()
    {
        // TestDirectory is automatically created
        string testFile = CreateTempFile("test.json", "{}");
        
        // Test code here
        WriteTestInfo("Test is running");
        
        // Automatic cleanup happens in base class
    }
}
```

### TestDataBuilder

Builder pattern for creating test data:

```csharp
public class MyDataBuilder : TestDataBuilder<MyData>
{
    private string _name = "Default";
    private int _value = 0;
    
    public MyDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public MyDataBuilder WithValue(int value)
    {
        _value = value;
        return this;
    }
    
    public override MyData Build()
    {
        return new MyData { Name = _name, Value = _value };
    }
    
    public override void Reset()
    {
        _name = "Default";
        _value = 0;
    }
}

// Usage
var data = new MyDataBuilder()
    .WithName("Test")
    .WithValue(42)
    .Build();
```

### TestDataGenerator

Utilities for generating random test data:

```csharp
// Generate random strings
string name = TestDataGenerator.RandomString(10);

// Generate random values
int value = TestDataGenerator.RandomInt(1, 100);
bool flag = TestDataGenerator.RandomBool();
string guid = TestDataGenerator.RandomGuid();

// Generate random dates
DateTime date = TestDataGenerator.RandomDate();

// Select random item
string item = TestDataGenerator.RandomItem("A", "B", "C");

// Generate lists
var items = TestDataGenerator.RandomList(() => new MyData(), 10);
```

### MockHelper

Utilities for creating mocks with Moq:

```csharp
// Create mock logger
var mockLogger = MockHelper.CreateMockLogger<MyClass>();

// Create capturing logger
var capturedMessages = new List<string>();
var mockLogger = MockHelper.CreateCapturingLogger<MyClass>(capturedMessages);

// Create mock logger factory
var mockFactory = MockHelper.CreateMockLoggerFactory();

// Verify logger was called
MockHelper.VerifyLoggerCalled(mockLogger, LogLevel.Information);
MockHelper.VerifyLoggerCalledWithMessage(mockLogger, "Expected message");

// Create strict/loose mocks
var strictMock = MockHelper.CreateStrictMock<IMyInterface>();
var looseMock = MockHelper.CreateLooseMock<IMyInterface>();
```

### TestWorkspace

Isolated workspace for integration tests:

```csharp
using (var workspace = new TestWorkspace("MyTest"))
{
    // Create directories and files
    workspace.CreateDirectory("configs");
    workspace.CreateFile("configs/settings.json", "{}");
    
    // Check existence
    bool exists = workspace.FileExists("configs/settings.json");
    
    // Read files
    string content = workspace.ReadFile("configs/settings.json");
    
    // Get paths
    string fullPath = workspace.GetPath("configs");
    
    // Automatic cleanup on dispose
}
```

### AssertHelper

Enhanced assertions:

```csharp
// Collection assertions
AssertHelper.CollectionContainsExactly(actual, "A", "B", "C");
AssertHelper.CollectionContainsAll(actual, "A", "B");
AssertHelper.CollectionDoesNotContain(actual, "X", "Y");

// String assertions
AssertHelper.StringContains(actual, "substring");
AssertHelper.StringDoesNotContain(actual, "bad");
AssertHelper.StringStartsWith(actual, "prefix");
AssertHelper.StringEndsWith(actual, "suffix");

// Range assertions
AssertHelper.InRange(value, 1, 100);
AssertHelper.DateTimeClose(actual, expected, TimeSpan.FromSeconds(1));
AssertHelper.DoubleClose(actual, expected, 0.001);

// Predicate assertions
AssertHelper.All(collection, x => x.IsValid);
AssertHelper.Any(collection, x => x.IsSpecial);
AssertHelper.None(collection, x => x.IsDeleted);
```

## Usage in Test Projects

Add reference to this library in your test project:

```xml
<ItemGroup>
  <ProjectReference Include="..\ACATCore.Tests.Shared\ACATCore.Tests.Shared.csproj" />
</ItemGroup>
```

Then use the utilities:

```csharp
using ACATCore.Tests.Shared;

[TestClass]
public class MyTests : BaseTest
{
    [TestMethod]
    public void TestWithMocks()
    {
        // Create mocks
        var mockLogger = MockHelper.CreateMockLogger<MyClass>();
        var testData = TestDataGenerator.RandomString();
        
        // Create system under test
        var sut = new MyClass(mockLogger.Object);
        
        // Act
        sut.DoSomething(testData);
        
        // Assert
        MockHelper.VerifyLoggerCalled(mockLogger, LogLevel.Information);
        AssertHelper.StringContains(sut.Result, testData);
    }
}
```

## Best Practices

1. **Inherit from BaseTest**: Use BaseTest for automatic setup/teardown
2. **Use TestWorkspace**: For tests that need file system isolation
3. **Mock external dependencies**: Use MockHelper for creating mocks
4. **Generate test data**: Use TestDataGenerator for random but valid data
5. **Use descriptive assertions**: AssertHelper provides better error messages
6. **Clean up resources**: BaseTest and TestWorkspace handle cleanup automatically

## Testing Patterns

### Unit Test Pattern
```csharp
[TestClass]
public class ServiceTests : BaseTest
{
    [TestMethod]
    public void Method_Scenario_ExpectedBehavior()
    {
        // Arrange
        var mockDependency = MockHelper.CreateMockLogger<Service>();
        var sut = new Service(mockDependency.Object);
        
        // Act
        var result = sut.DoWork();
        
        // Assert
        Assert.IsNotNull(result);
        MockHelper.VerifyLoggerCalled(mockDependency, LogLevel.Information);
    }
}
```

### Integration Test Pattern
```csharp
[TestClass]
public class IntegrationTests : BaseTest
{
    [TestMethod]
    public void Integration_Scenario_ExpectedBehavior()
    {
        using (var workspace = new TestWorkspace("IntegrationTest"))
        {
            // Arrange
            workspace.CreateFile("config.json", "{}");
            var service = new Service(workspace.WorkspaceRoot);
            
            // Act
            var result = service.Process();
            
            // Assert
            Assert.IsTrue(workspace.FileExists("output.txt"));
        }
    }
}
```

## Dependencies

- MSTest 3.7.0
- Moq 4.20.72
- Microsoft.Extensions.DependencyInjection 8.0.0
- Microsoft.Extensions.Logging 8.0.0
- FluentValidation 11.9.0
- System.Text.Json 9.0.7
