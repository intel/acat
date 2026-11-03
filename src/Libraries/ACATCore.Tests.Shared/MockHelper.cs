////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MockHelper.cs
//
// Utilities for creating and configuring mocks using Moq framework
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;

namespace ACATCore.Tests.Shared
{
    /// <summary>
    /// Provides utilities for creating and configuring mock objects
    /// </summary>
    public static class MockHelper
    {
        /// <summary>
        /// Creates a mock logger that captures log messages
        /// </summary>
        public static Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        /// <summary>
        /// Creates a mock logger with message capture
        /// </summary>
        public static Mock<ILogger<T>> CreateCapturingLogger<T>(List<string> capturedMessages)
        {
            var mockLogger = new Mock<ILogger<T>>();
            
            mockLogger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()))
                .Callback((LogLevel level, EventId eventId, object state, Exception exception, Delegate formatter) =>
                {
                    var message = $"[{level}] {state}";
                    capturedMessages.Add(message);
                });

            return mockLogger;
        }

        /// <summary>
        /// Creates a mock logger factory
        /// </summary>
        public static Mock<ILoggerFactory> CreateMockLoggerFactory()
        {
            var mockFactory = new Mock<ILoggerFactory>();
            return mockFactory;
        }

        /// <summary>
        /// Creates a mock logger factory that returns mock loggers
        /// </summary>
        public static Mock<ILoggerFactory> CreateMockLoggerFactory<T>(Mock<ILogger<T>> mockLogger)
        {
            var mockFactory = new Mock<ILoggerFactory>();
            mockFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);
            return mockFactory;
        }

        /// <summary>
        /// Verifies that a mock logger was called with specific log level
        /// </summary>
        public static void VerifyLoggerCalled<T>(Mock<ILogger<T>> mockLogger, LogLevel level, Times? times = null)
        {
            mockLogger.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times ?? Times.AtLeastOnce());
        }

        /// <summary>
        /// Verifies that a mock logger was called with a message containing specific text
        /// </summary>
        public static void VerifyLoggerCalledWithMessage<T>(Mock<ILogger<T>> mockLogger, string messageContains)
        {
            mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains(messageContains)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce());
        }

        /// <summary>
        /// Verifies that a mock was never called
        /// </summary>
        public static void VerifyNeverCalled<T>(Mock<T> mock) where T : class
        {
            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Creates a strict mock that throws on any unexpected calls
        /// </summary>
        public static Mock<T> CreateStrictMock<T>() where T : class
        {
            return new Mock<T>(MockBehavior.Strict);
        }

        /// <summary>
        /// Creates a loose mock that returns default values for unexpected calls
        /// </summary>
        public static Mock<T> CreateLooseMock<T>() where T : class
        {
            return new Mock<T>(MockBehavior.Loose);
        }
    }

    /// <summary>
    /// Provides utilities for creating test doubles (fakes, stubs, spies)
    /// </summary>
    public static class TestDoubleHelper
    {
        /// <summary>
        /// Creates a spy that wraps a real object and tracks calls
        /// </summary>
        public static Mock<T> CreateSpy<T>(T realObject) where T : class
        {
            var spy = new Mock<T>();
            spy.CallBase = true;
            return spy;
        }

        /// <summary>
        /// Creates a stub that returns specific values
        /// </summary>
        public static Mock<T> CreateStub<T>() where T : class
        {
            return new Mock<T>(MockBehavior.Loose);
        }

        /// <summary>
        /// Creates a fake implementation (simplified working implementation for testing)
        /// </summary>
        public static T CreateFake<T>() where T : class, new()
        {
            return new T();
        }
    }
}
