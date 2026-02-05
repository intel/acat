////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Configures Microsoft.Extensions.Logging infrastructure for ACAT
    /// Provides structured logging with Console and File sinks
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Adds ACAT logging configuration to the service collection
        /// </summary>
        /// <param name="services">The service collection to configure</param>
        /// <returns>The configured service collection for chaining</returns>
        public static IServiceCollection AddACATLogging(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                // Add console logging
                builder.AddConsole();

                // Set minimum log level based on build configuration
#if DEBUG
                builder.SetMinimumLevel(LogLevel.Debug);
#else
                builder.SetMinimumLevel(LogLevel.Information);
#endif
            });

            return services;
        }
        
        /// <summary>
        /// Configures file logging on the logger factory
        /// This must be called after the service provider is built
        /// </summary>
        /// <param name="loggerFactory">The logger factory to configure</param>
        /// <returns>The configured logger factory for chaining</returns>
        public static ILoggerFactory ConfigureFileLogging(this ILoggerFactory loggerFactory)
        {
            // Configure log file path: logs/acat-{Date}.txt
            string logDirectory = GetLogDirectory();
            string logFilePath = Path.Combine(logDirectory, "acat-.txt");

            // Add file logging with Serilog
            loggerFactory.AddFile(logFilePath, LogLevel.Information, 
                fileSizeLimitBytes: 10_000_000, // 10MB
                retainedFileCountLimit: 7); // Keep 7 days of logs

            return loggerFactory;
        }

        /// <summary>
        /// Gets the directory where log files should be stored
        /// Creates the directory if it doesn't exist
        /// </summary>
        /// <returns>Full path to the logs directory</returns>
        private static string GetLogDirectory()
        {
            string logDirectory;

            try
            {
                // Try to use the existing ACAT logs directory
                logDirectory = FileUtils.GetLogsDir();
                
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch
            {
                // Fallback to application path
                logDirectory = SmartPath.ApplicationPath;
                
                // Try to create a logs subdirectory
                try
                {
                    string logsSubDir = Path.Combine(logDirectory, "logs");
                    if (!Directory.Exists(logsSubDir))
                    {
                        Directory.CreateDirectory(logsSubDir);
                    }
                    logDirectory = logsSubDir;
                }
                catch
                {
                    // If that fails, just use the application path
                }
            }

            return logDirectory;
        }

        /// <summary>
        /// Creates a standalone logger factory for scenarios where dependency injection is not available
        /// </summary>
        /// <returns>Configured ILoggerFactory instance</returns>
        public static ILoggerFactory CreateLoggerFactory()
        {
            var services = new ServiceCollection();
            services.AddACATLogging();
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            
            // Configure file logging on the logger factory
            loggerFactory.ConfigureFileLogging();
            
            return loggerFactory;
        }

        /// <summary>
        /// Creates a logger of the specified type
        /// </summary>
        /// <typeparam name="T">The type for which to create the logger</typeparam>
        /// <returns>Configured ILogger instance</returns>
        public static ILogger<T> CreateLogger<T>()
        {
            var loggerFactory = CreateLoggerFactory();
            return loggerFactory.CreateLogger<T>();
        }

        /// <summary>
        /// Creates a logger with the specified category name
        /// </summary>
        /// <param name="categoryName">The category name for the logger</param>
        /// <returns>Configured ILogger instance</returns>
        public static ILogger CreateLogger(string categoryName)
        {
            var loggerFactory = CreateLoggerFactory();
            return loggerFactory.CreateLogger(categoryName);
        }
    }
}
