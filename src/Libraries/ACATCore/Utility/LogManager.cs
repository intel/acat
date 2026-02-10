////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Central logging manager that provides loggers from any context.
    /// Can be used before or after DI container initialization.
    /// Thread-safe and singleton-based.
    /// </summary>
    public static class LogManager
    {
        private static readonly object _lock = new object();
        private static ILoggerFactory _loggerFactory;
        private static bool _isInitialized = false;

        /// <summary>
        /// Initialize the logging system with a specific logger factory.
        /// This should be called early in application startup.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to use</param>
        public static void Initialize(ILoggerFactory loggerFactory)
        {
            lock (_lock)
            {
                _loggerFactory = loggerFactory;
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Gets a logger for the specified type.
        /// Creates a default logger factory if not yet initialized.
        /// </summary>
        /// <typeparam name="T">The type to create a logger for</typeparam>
        /// <returns>Logger instance (never null)</returns>
        public static ILogger<T> GetLogger<T>()
        {
            EnsureInitialized();
            return _loggerFactory.CreateLogger<T>();
        }

        /// <summary>
        /// Gets a logger with the specified category name.
        /// Creates a default logger factory if not yet initialized.
        /// </summary>
        /// <param name="categoryName">The category name for the logger</param>
        /// <returns>Logger instance (never null)</returns>
        public static ILogger GetLogger(string categoryName)
        {
            EnsureInitialized();
            return _loggerFactory.CreateLogger(categoryName);
        }

        /// <summary>
        /// Gets a logger for the specified type.
        /// Creates a default logger factory if not yet initialized.
        /// </summary>
        /// <param name="type">The type to create a logger for</param>
        /// <returns>Logger instance (never null)</returns>
        public static ILogger GetLogger(Type type)
        {
            EnsureInitialized();
            return _loggerFactory.CreateLogger(type);
        }

        /// <summary>
        /// Ensures the logger factory is initialized.
        /// If not explicitly initialized, creates a default factory.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (_lock)
                {
                    if (!_isInitialized)
                    {
                        // Create default factory if not explicitly initialized
                        _loggerFactory = LoggingConfiguration.CreateLoggerFactory();
                        _isInitialized = true;
                    }
                }
            }
        }
    }
}
