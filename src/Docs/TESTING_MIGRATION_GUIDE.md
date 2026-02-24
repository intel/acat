# Testing Infrastructure Migration Guide

**Purpose**: Guide for migrating existing tests to use the new shared testing infrastructure  
**Target Audience**: Developers maintaining ACAT test projects  
**Version**: 1.0

---

## Overview

This guide helps you migrate existing test projects to use the new `ACATCore.Tests.Shared` library, which provides:
- Base test classes with automatic setup/teardown
- Mocking utilities using Moq
- Test data builders and generators
- Enhanced assertions
- Test workspace management

## Migration Checklist

- [ ] Add reference to ACATCore.Tests.Shared
- [ ] Update test classes to inherit from BaseTest
- [ ] Replace manual setup/teardown with BaseTest features
- [ ] Replace test directory management with TestWorkspace
- [ ] Add Moq package and use MockHelper
- [ ] Use TestDataGenerator for random data
- [ ] Replace basic assertions with AssertHelper
- [ ] Update CI/CD configuration if needed

---

## Step 1: Add Project Reference

### Update .csproj File

Add the shared library reference:

```xml
<ItemGroup>
  <ProjectReference Include="..\ACATCore.Tests.Shared\ACATCore.Tests.Shared.csproj" />
</ItemGroup>
```

### Add Moq Package (if not already present)

```xml
<ItemGroup>
  <PackageReference Include="Moq" Version="4.20.72" />
</ItemGroup>
```

---

## Step 2: Migrate Test Classes

### Before: Basic Test Class

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ACATCore.Tests.MyFeature
{
    [TestClass]
    public class MyFeatureTests
    {
        private string _testDirectory;

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "MyTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch
                {
                    // Ignore
                }
            }
        }

        [TestMethod]
        public void TestMethod()
        {
            string filePath = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(filePath, "content");
            
            Assert.IsTrue(File.Exists(filePath));
        }
    }
}
```

### After: Using BaseTest

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACATCore.Tests.Shared;

namespace ACATCore.Tests.MyFeature
{
    [TestClass]
    public class MyFeatureTests : BaseTest
    {
        // No need for TestInitialize/TestCleanup - BaseTest handles it!

        [TestMethod]
        public void TestMethod()
        {
            // TestDirectory is automatically created by BaseTest
            string filePath = CreateTempFile("test.txt", "content");
            
            Assert.IsTrue(System.IO.File.Exists(filePath));
            
            // Automatic cleanup happens in BaseTest.TestCleanup
        }
    }
}
```

**Benefits**:
- ✅ No manual directory creation/cleanup
- ✅ Automatic performance tracking with Stopwatch
- ✅ Built-in test logging (WriteTestInfo, WriteTestDebug)
- ✅ Helper methods (CreateTempFile, CreateTempDirectory)

---

## Step 3: Migrate Test Data Creation

### Before: Manual Test Data

```csharp
[TestMethod]
public void TestWithData()
{
    var settings = new ActuatorSettingsJson
    {
        ActuatorSettings = new List<ActuatorSettingJson>
        {
            new ActuatorSettingJson
            {
                Name = "TestActuator",
                Id = "d91a1877-c92b-4d7e-9ab6-f01f30b12df9",
                Enabled = true
            }
        }
    };
    
    // Test code
}
```

### After: Using TestDataBuilder

```csharp
// Create a builder class (reusable across tests)
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

// Use in tests
[TestMethod]
public void TestWithData()
{
    var settings = new ActuatorSettingsBuilder()
        .WithActuator("Keyboard", true)
        .WithActuator("Camera", false)
        .Build();
    
    // Test code - cleaner and more readable!
}
```

**Benefits**:
- ✅ Reusable across multiple tests
- ✅ Fluent, readable API
- ✅ Random data generation built-in
- ✅ Easy to create variations

---

## Step 4: Add Mocking

### Before: Direct Dependencies

```csharp
[TestMethod]
public void TestLogging()
{
    // Can't easily test logging without real logger
    var service = new MyService();
    service.DoWork();
    // No way to verify logging happened
}
```

### After: Using MockHelper

```csharp
[TestMethod]
public void TestLogging()
{
    // Create mock logger
    var mockLogger = MockHelper.CreateMockLogger<MyService>();
    
    // Create service with mock
    var service = new MyService(mockLogger.Object);
    
    // Execute
    service.DoWork();
    
    // Verify logging
    MockHelper.VerifyLoggerCalled(mockLogger, LogLevel.Information);
    MockHelper.VerifyLoggerCalledWithMessage(mockLogger, "Expected message");
}
```

**Or capture messages**:

```csharp
[TestMethod]
public void TestLogging_CaptureMessages()
{
    var capturedMessages = new List<string>();
    var mockLogger = MockHelper.CreateCapturingLogger<MyService>(capturedMessages);
    
    var service = new MyService(mockLogger.Object);
    service.DoWork();
    
    Assert.AreEqual(2, capturedMessages.Count);
    AssertHelper.StringContains(capturedMessages[0], "Started");
    AssertHelper.StringContains(capturedMessages[1], "Completed");
}
```

**Benefits**:
- ✅ Test logging without real infrastructure
- ✅ Verify logging behavior
- ✅ Capture and inspect log messages
- ✅ No side effects from logging

---

## Step 5: Enhance Assertions

### Before: Basic Assertions

```csharp
[TestMethod]
public void TestCollections()
{
    var items = GetItems();
    
    Assert.IsTrue(items.Contains("A"));
    Assert.IsTrue(items.Contains("B"));
    Assert.IsFalse(items.Contains("X"));
    
    Assert.AreEqual(3, items.Count);
}

[TestMethod]
public void TestStrings()
{
    var result = GetResult();
    
    Assert.IsTrue(result.Contains("success"));
    Assert.IsTrue(result.StartsWith("Result:"));
}
```

### After: Using AssertHelper

```csharp
[TestMethod]
public void TestCollections()
{
    var items = GetItems();
    
    AssertHelper.CollectionContainsAll(items, "A", "B");
    AssertHelper.CollectionDoesNotContain(items, "X");
    AssertHelper.CollectionContainsExactly(items, "A", "B", "C");
}

[TestMethod]
public void TestStrings()
{
    var result = GetResult();
    
    AssertHelper.StringContains(result, "success");
    AssertHelper.StringStartsWith(result, "Result:");
}
```

**Benefits**:
- ✅ More readable assertions
- ✅ Better error messages
- ✅ Less boilerplate code
- ✅ Consistent assertion style

---

## Step 6: Migrate Integration Tests

### Before: Manual Workspace Management

```csharp
[TestClass]
public class IntegrationTests
{
    private string _testDir;

    [TestInitialize]
    public void Setup()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "IntegrationTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
        Directory.CreateDirectory(Path.Combine(_testDir, "configs"));
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testDir))
        {
            try
            {
                Directory.Delete(_testDir, true);
            }
            catch
            {
                System.Threading.Thread.Sleep(100);
                try { Directory.Delete(_testDir, true); } catch { }
            }
        }
    }

    [TestMethod]
    public void TestIntegration()
    {
        string configPath = Path.Combine(_testDir, "configs", "settings.json");
        File.WriteAllText(configPath, "{}");
        
        var service = new ConfigService(_testDir);
        var result = service.LoadConfig("configs/settings.json");
        
        Assert.IsNotNull(result);
    }
}
```

### After: Using TestWorkspace

```csharp
[TestClass]
public class IntegrationTests : BaseTest
{
    [TestMethod]
    public void TestIntegration()
    {
        using (var workspace = new TestWorkspace("IntegrationTest"))
        {
            // Create directory structure
            workspace.CreateDirectory("configs");
            workspace.CreateFile("configs/settings.json", "{}");
            
            // Use workspace
            var service = new ConfigService(workspace.WorkspaceRoot);
            var result = service.LoadConfig("configs/settings.json");
            
            // Verify
            Assert.IsNotNull(result);
            Assert.IsTrue(workspace.FileExists("configs/settings.json"));
            
            // Automatic cleanup on dispose
        }
    }
}
```

**Benefits**:
- ✅ Automatic cleanup with `using` statement
- ✅ Built-in retry logic for file locks
- ✅ Helper methods for common operations
- ✅ Path management utilities

---

## Step 7: Add Test Categories

### Add Categories for Filtering

```csharp
[TestClass]
public class MyTests : BaseTest
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Fast")]
    public void FastUnitTest()
    {
        // Quick unit test
    }
    
    [TestMethod]
    [TestCategory("Integration")]
    public void SlowIntegrationTest()
    {
        // Slower integration test
    }
    
    [TestMethod]
    [TestCategory("Performance")]
    public void PerformanceTest()
    {
        // Performance test
    }
}
```

**Run by category**:
```bash
dotnet test --filter "TestCategory=Unit"
dotnet test --filter "TestCategory=Integration"
dotnet test --filter "TestCategory=Performance"
```

---

## Step 8: Update Test Naming

### Follow Consistent Naming Pattern

**Pattern**: `[Method]_[Scenario]_[ExpectedBehavior]`

**Before**:
```csharp
[TestMethod]
public void Test1() { }

[TestMethod]
public void TestLoadConfig() { }

[TestMethod]
public void ConfigurationTest() { }
```

**After**:
```csharp
[TestMethod]
public void LoadConfiguration_ValidFile_ReturnsConfiguration() { }

[TestMethod]
public void LoadConfiguration_FileNotFound_ThrowsException() { }

[TestMethod]
public void ValidateSettings_EmptyList_ReturnsFalse() { }
```

---

## Migration Examples

### Example 1: Simple Unit Test

**Before**:
```csharp
[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void TestAdd()
    {
        var calc = new Calculator();
        int result = calc.Add(2, 3);
        Assert.AreEqual(5, result);
    }
}
```

**After**:
```csharp
[TestClass]
public class CalculatorTests : BaseTest
{
    [TestMethod]
    [TestCategory("Unit")]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        var calc = new Calculator();
        
        // Act
        int result = calc.Add(2, 3);
        
        // Assert
        Assert.AreEqual(5, result);
        WriteTestInfo($"2 + 3 = {result}");
    }
}
```

### Example 2: Test with Mocking

**Before**:
```csharp
[TestClass]
public class ServiceTests
{
    [TestMethod]
    public void TestService()
    {
        var service = new MyService();
        service.Process("data");
        // Can't verify internal behavior
    }
}
```

**After**:
```csharp
[TestClass]
public class ServiceTests : BaseTest
{
    [TestMethod]
    [TestCategory("Unit")]
    public void Process_ValidData_LogsSuccess()
    {
        // Arrange
        var mockLogger = MockHelper.CreateMockLogger<MyService>();
        var service = new MyService(mockLogger.Object);
        
        // Act
        service.Process("data");
        
        // Assert
        MockHelper.VerifyLoggerCalled(mockLogger, LogLevel.Information);
        MockHelper.VerifyLoggerCalledWithMessage(mockLogger, "Success");
    }
}
```

### Example 3: Integration Test

**Before**:
```csharp
[TestClass]
public class FileServiceTests
{
    private string _tempDir;

    [TestInitialize]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [TestMethod]
    public void TestReadWrite()
    {
        var service = new FileService(_tempDir);
        service.Write("test.txt", "content");
        string result = service.Read("test.txt");
        Assert.AreEqual("content", result);
    }
}
```

**After**:
```csharp
[TestClass]
public class FileServiceTests : BaseTest
{
    [TestMethod]
    [TestCategory("Integration")]
    public void ReadWrite_ValidFile_PreservesContent()
    {
        using (var workspace = new TestWorkspace("FileServiceTest"))
        {
            // Arrange
            var service = new FileService(workspace.WorkspaceRoot);
            
            // Act
            service.Write("test.txt", "content");
            string result = service.Read("test.txt");
            
            // Assert
            Assert.AreEqual("content", result);
            Assert.IsTrue(workspace.FileExists("test.txt"));
        }
    }
}
```

---

## Common Patterns

### Pattern 1: Test Data Setup

```csharp
// Create reusable test data builder
public class TestDataBuilder : TestDataBuilder<MyData>
{
    private string _name = "Default";
    
    public TestDataBuilder WithName(string name)
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

// Use in test class
[TestClass]
public class MyTests : BaseTest
{
    private TestDataBuilder _dataBuilder;
    
    [TestInitialize]
    public void Setup()
    {
        base.TestInitialize(); // Call base if you override
        _dataBuilder = new TestDataBuilder();
    }
    
    [TestMethod]
    public void Test1()
    {
        var data = _dataBuilder.WithName("Test1").Build();
        // test code
    }
}
```

### Pattern 2: Mock Setup

```csharp
[TestClass]
public class ServiceTests : BaseTest
{
    private Mock<ILogger<MyService>> _mockLogger;
    private Mock<IConfiguration> _mockConfig;
    
    [TestInitialize]
    public void Setup()
    {
        base.TestInitialize();
        _mockLogger = MockHelper.CreateMockLogger<MyService>();
        _mockConfig = new Mock<IConfiguration>();
    }
    
    [TestMethod]
    public void TestMethod()
    {
        // Arrange
        _mockConfig.Setup(x => x.GetValue("key")).Returns("value");
        var service = new MyService(_mockLogger.Object, _mockConfig.Object);
        
        // Act
        service.DoWork();
        
        // Assert
        MockHelper.VerifyLoggerCalled(_mockLogger, LogLevel.Information);
        _mockConfig.Verify(x => x.GetValue("key"), Times.Once);
    }
}
```

### Pattern 3: Performance Testing

```csharp
[TestClass]
public class PerformanceTests : BaseTest
{
    [TestMethod]
    [TestCategory("Performance")]
    public void Operation_LargeDataset_CompletesQuickly()
    {
        // Arrange
        var data = TestDataGenerator.RandomList(
            () => new MyData(), 
            10000);
        var processor = new DataProcessor();
        
        // Act & Assert
        AssertCompletesWithin(() =>
        {
            processor.ProcessAll(data);
        }, TimeSpan.FromSeconds(1), "Should process 10K items in < 1s");
        
        WriteTestInfo($"Processed in {Stopwatch.ElapsedMilliseconds}ms");
    }
}
```

---

## Troubleshooting

### Issue: Tests still use old patterns

**Solution**: Refactor incrementally. Start with new tests, then gradually update existing tests.

### Issue: Moq conflicts with existing mocking

**Solution**: Remove old mocking library, update all mocks to use Moq with MockHelper.

### Issue: BaseTest conflicts with existing setup

**Solution**: Can override TestInitialize/TestCleanup but call base methods:
```csharp
[TestInitialize]
public override void TestInitialize()
{
    base.TestInitialize();
    // Your custom setup
}
```

### Issue: Build errors after adding reference

**Solution**: Clean and rebuild:
```bash
dotnet clean
dotnet build
```

---

## Checklist for Complete Migration

For each test project:

- [ ] Add reference to ACATCore.Tests.Shared
- [ ] Add Moq package if needed
- [ ] Update all test classes to inherit from BaseTest
- [ ] Replace manual directory management with TestDirectory/TestWorkspace
- [ ] Add MockHelper for all mocking scenarios
- [ ] Create TestDataBuilder classes for complex test data
- [ ] Replace basic assertions with AssertHelper
- [ ] Add test categories (Unit, Integration, Performance)
- [ ] Update test method names to follow convention
- [ ] Add XML documentation to test classes
- [ ] Update README if project has one
- [ ] Run all tests to verify migration
- [ ] Update CI/CD if needed

---

## Next Steps

1. **Prioritize**: Start with most actively maintained test projects
2. **Incremental**: Migrate one project at a time
3. **Test**: Verify all tests still pass after migration
4. **Document**: Update project README with new patterns
5. **Share**: Help other developers with migration

---

## Benefits Summary

After migration, you'll have:

✅ **Less Boilerplate**: BaseTest handles setup/teardown  
✅ **Better Mocking**: MockHelper simplifies mock creation  
✅ **Cleaner Tests**: TestDataBuilder and generators  
✅ **Enhanced Assertions**: AssertHelper with better messages  
✅ **Easier Maintenance**: Shared utilities across all test projects  
✅ **Consistent Patterns**: Standard approach across codebase  

---

## Questions or Issues?

See:
- [TESTING_INFRASTRUCTURE.md](TESTING_INFRASTRUCTURE.md) - Comprehensive guide
- [ACATCore.Tests.Shared README](src/Libraries/ACATCore.Tests.Shared/README.md) - Library documentation
- [Example Tests](src/Libraries/ACATCore.Tests.Shared/ExampleTests.cs) - Usage examples

Or contact the ACAT development team.
