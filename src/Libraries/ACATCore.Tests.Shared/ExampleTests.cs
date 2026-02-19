////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ExampleTests.cs
//
// Example tests demonstrating the testing infrastructure
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ACATCore.Tests.Shared.Examples
{
    /// <summary>
    /// Example tests demonstrating BaseTest usage
    /// </summary>
    [TestClass]
    public class BaseTestExamples : BaseTest
    {
        [TestMethod]
        public void ExampleTest_CreatingTempFile()
        {
            // BaseTest automatically creates TestDirectory for us
            var filePath = CreateTempFile("example.txt", "Hello World");
            
            // Test that file was created
            Assert.IsTrue(System.IO.File.Exists(filePath));
            
            // Read and verify content
            var content = System.IO.File.ReadAllText(filePath);
            Assert.AreEqual("Hello World", content);
            
            // Cleanup happens automatically in TestCleanup
        }

        [TestMethod]
        public void ExampleTest_PerformanceMeasurement()
        {
            // Stopwatch is automatically started in TestInitialize
            System.Threading.Thread.Sleep(50);
            
            // Verify elapsed time
            Assert.IsTrue(Stopwatch.ElapsedMilliseconds >= 50);
            
            // Test context shows elapsed time in TestCleanup
        }

        [TestMethod]
        public void ExampleTest_CustomAssertions()
        {
            // Test throwing exceptions
            AssertThrows<System.ArgumentNullException>(() => 
            {
                throw new System.ArgumentNullException("test");
            });
            
            // Test no exception thrown
            AssertNoThrow(() => 
            {
                int x = 1 + 1;
            });
            
            // Test completion time
            AssertCompletesWithin(() => 
            {
                System.Threading.Thread.Sleep(10);
            }, System.TimeSpan.FromMilliseconds(50), "Should complete quickly");
        }
    }

    /// <summary>
    /// Example tests demonstrating MockHelper usage
    /// </summary>
    [TestClass]
    public class MockHelperExamples : BaseTest
    {
        [TestMethod]
        public void ExampleTest_MockLogger()
        {
            // Create a mock logger
            var mockLogger = MockHelper.CreateMockLogger<MockHelperExamples>();
            
            // Use the logger
            mockLogger.Object.LogInformation("Test message");
            
            // Verify it was called
            MockHelper.VerifyLoggerCalled(mockLogger, LogLevel.Information);
        }

        [TestMethod]
        public void ExampleTest_CapturingLogger()
        {
            // Create a logger that captures messages
            var capturedMessages = new List<string>();
            var mockLogger = MockHelper.CreateCapturingLogger<MockHelperExamples>(capturedMessages);
            
            // Log some messages
            mockLogger.Object.LogInformation("Message 1");
            mockLogger.Object.LogWarning("Message 2");
            
            // Verify messages were captured
            Assert.AreEqual(2, capturedMessages.Count);
            AssertHelper.StringContains(capturedMessages[0], "Message 1");
            AssertHelper.StringContains(capturedMessages[1], "Message 2");
        }
    }

    /// <summary>
    /// Example tests demonstrating TestDataGenerator usage
    /// </summary>
    [TestClass]
    public class TestDataGeneratorExamples : BaseTest
    {
        [TestMethod]
        public void ExampleTest_RandomData()
        {
            // Generate random strings
            string name = TestDataGenerator.RandomString(10);
            Assert.AreEqual(10, name.Length);
            
            // Generate random numbers
            int value = TestDataGenerator.RandomInt(1, 100);
            AssertHelper.InRange(value, 1, 100);
            
            // Generate random GUIDs
            string guid = TestDataGenerator.RandomGuid();
            Assert.IsFalse(string.IsNullOrEmpty(guid));
            
            // Select random items
            string item = TestDataGenerator.RandomItem("A", "B", "C");
            Assert.IsTrue(item == "A" || item == "B" || item == "C");
        }

        [TestMethod]
        public void ExampleTest_RandomList()
        {
            // Generate a list of random data
            var items = TestDataGenerator.RandomList(() => 
                TestDataGenerator.RandomString(5), 10);
            
            Assert.AreEqual(10, items.Count);
            AssertHelper.All(items, x => x.Length == 5);
        }
    }

    /// <summary>
    /// Example tests demonstrating AssertHelper usage
    /// </summary>
    [TestClass]
    public class AssertHelperExamples : BaseTest
    {
        [TestMethod]
        public void ExampleTest_CollectionAssertions()
        {
            var items = new List<string> { "A", "B", "C" };
            
            // Test collection contains all items
            AssertHelper.CollectionContainsAll(items, "A", "B");
            
            // Test collection contains exactly these items
            AssertHelper.CollectionContainsExactly(items, "A", "B", "C");
            
            // Test collection does not contain items
            AssertHelper.CollectionDoesNotContain(items, "X", "Y");
        }

        [TestMethod]
        public void ExampleTest_StringAssertions()
        {
            string text = "Hello World";
            
            AssertHelper.StringContains(text, "World");
            AssertHelper.StringDoesNotContain(text, "Goodbye");
            AssertHelper.StringStartsWith(text, "Hello");
            AssertHelper.StringEndsWith(text, "World");
        }

        [TestMethod]
        public void ExampleTest_PredicateAssertions()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5 };
            
            // Test all items satisfy predicate
            AssertHelper.All(numbers, x => x > 0);
            
            // Test at least one item satisfies predicate
            AssertHelper.Any(numbers, x => x == 3);
            
            // Test no items satisfy predicate
            AssertHelper.None(numbers, x => x > 10);
        }
    }

    /// <summary>
    /// Example tests demonstrating TestWorkspace usage
    /// </summary>
    [TestClass]
    public class TestWorkspaceExamples : BaseTest
    {
        [TestMethod]
        public void ExampleTest_IsolatedWorkspace()
        {
            using (var workspace = new TestWorkspace("MyTest"))
            {
                // Create directory structure
                workspace.CreateDirectory("configs");
                workspace.CreateDirectory("logs");
                
                // Create files
                workspace.CreateFile("configs/settings.json", "{}");
                workspace.CreateFile("logs/app.log", "Log entry");
                
                // Verify structure
                Assert.IsTrue(workspace.DirectoryExists("configs"));
                Assert.IsTrue(workspace.FileExists("configs/settings.json"));
                
                // Read file
                string content = workspace.ReadFile("configs/settings.json");
                Assert.AreEqual("{}", content);
                
                // Get full path
                string fullPath = workspace.GetPath("configs/settings.json");
                Assert.IsTrue(System.IO.File.Exists(fullPath));
                
                // List files
                var files = workspace.GetFiles("*.json");
                Assert.AreEqual(1, files.Length);
                
                // Automatic cleanup on dispose
            }
        }
    }
}
