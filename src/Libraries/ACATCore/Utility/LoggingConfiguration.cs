////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using System;
using System.IO;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Configures Microsoft.Extensions.Logging infrastructure for ACAT
    /// Provides structured logging with Console, Debug, and File sinks
    /// Uses async file logging for optimal performance
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Maximum size of a log file before rotation (10 MB)
        /// </summary>
        private const long MaxFileSizeBytes = 10_000_000;

        /// <summary>
        /// Number of days of log files to retain
        /// </summary>
        private const int RetainedFileCountLimit = 7;

        /// <summary>
        /// Buffer size for async logging (number of messages)
        /// Larger buffer = better performance but higher memory usage
        /// </summary>
        private const int AsyncBufferSize = 10000;

        /// <summary>
        /// Whether to block when buffer is full (false = drop messages, true = block caller)
        /// For UI responsiveness, we set to false
        /// </summary>
        private const bool BlockWhenFull = false;

        /// <summary>
        /// Default Seq server URL for structured logging
        /// Can be overridden via ACAT_SEQ_URL environment variable
        /// </summary>
        private const string DefaultSeqServerUrl = "http://localhost:5341";
        
        /// <summary>
        /// Adds ACAT logging configuration to the service collection
        /// </summary>
        /// <param name="services">The service collection to configure</param>
        /// <returns>The configured service collection for chaining</returns>
        public static IServiceCollection AddACATLogging(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                // PERFORMANCE: Console and Debug providers write synchronously and are expensive
                // Only enable these when actively debugging specific issues
                // Uncomment the lines below if you need console/debug output:
                // builder.AddConsole();
                // builder.AddDebug();

                // Set minimum log level - Information provides good balance of visibility and performance
                // Note: File logging is configured separately in ConfigureFileLogging()
#if DEBUG
                builder.SetMinimumLevel(LogLevel.Trace);
#else
                builder.SetMinimumLevel(LogLevel.Information);
#endif
            });

            return services;
        }
        
        /// <summary>
        /// Configures async file logging on the logger factory using Serilog
        /// This uses a background thread with buffering for optimal performance
        /// </summary>
        /// <param name="loggerFactory">The logger factory to configure</param>
        /// <returns>The configured logger factory for chaining</returns>
        public static ILoggerFactory ConfigureFileLogging(this ILoggerFactory loggerFactory)
        {
            // Get log directory and file path
            string logDirectory = GetLogDirectory();
            // Add timestamp to filename for unique log files per session
            string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string logFilePath = Path.Combine(logDirectory, $"acat-{timestamp}-.txt");

            // Configure Serilog with async file sink
            // Set minimum level to Debug in DEBUG builds, Information in RELEASE builds
            var loggerConfig = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .Enrich.FromLogContext()
                .WriteTo.Async(a => a.File(
                    logFilePath,
                    rollingInterval: Serilog.RollingInterval.Day,
                    retainedFileCountLimit: RetainedFileCountLimit,
                    fileSizeLimitBytes: MaxFileSizeBytes,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
                ), bufferSize: AsyncBufferSize, blockWhenFull: BlockWhenFull);

#if DEBUG
            // Add Seq sink for structured logging in DEBUG builds
            // Can be disabled by setting ACAT_SEQ_ENABLED=false environment variable
            string seqEnabled = Environment.GetEnvironmentVariable("ACAT_SEQ_ENABLED") ?? "true";
            if (seqEnabled.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                string seqUrl = Environment.GetEnvironmentVariable("ACAT_SEQ_URL") ?? DefaultSeqServerUrl;
                try
                {
                    loggerConfig.WriteTo.Seq(
                        seqUrl,
                        apiKey: Environment.GetEnvironmentVariable("ACAT_SEQ_APIKEY"), // Optional API key
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug);

                    // Log to console that Seq is enabled (will only show if console logging is enabled)
                    System.Diagnostics.Debug.WriteLine($"Seq logging enabled to {seqUrl}");
                }
                catch (Exception ex)
                {
                    // Don't fail startup if Seq is unavailable
                    System.Diagnostics.Debug.WriteLine($"Failed to configure Seq logging: {ex.Message}");
                }
            }
#endif

            Logger serilogLogger = loggerConfig.CreateLogger();

            // Add the Serilog logger to the factory
            loggerFactory.AddSerilog(serilogLogger, dispose: true);

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
        /// Lazy singleton instance of the logger factory to ensure single file sink
        /// </summary>
        private static readonly Lazy<ILoggerFactory> _sharedLoggerFactory = new Lazy<ILoggerFactory>(() => CreateLoggerFactory());

        /// <summary>
        /// Gets the shared singleton logger factory instance
        /// This ensures all loggers in the application write to the same log file
        /// </summary>
        /// <returns>The shared ILoggerFactory instance</returns>
        public static ILoggerFactory GetSharedLoggerFactory()
        {
            return _sharedLoggerFactory.Value;
        }

        /// <summary>
        /// Creates a standalone logger factory for scenarios where dependency injection is not available
        /// Uses async file logging for optimal performance
        /// WARNING: Only call this ONCE at application startup. Each call creates a new file sink.
        /// After creating the factory, use it to create all loggers rather than calling this repeatedly.
        /// </summary>
        /// <returns>Configured ILoggerFactory instance</returns>
        public static ILoggerFactory CreateLoggerFactory()
        {
            var services = new ServiceCollection();
            services.AddACATLogging();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // Configure async file logging on the logger factory
            loggerFactory.ConfigureFileLogging();

            return loggerFactory;
        }

        /// <summary>
        /// Creates a logger of the specified type using a shared singleton logger factory
        /// This ensures all loggers write to the same log file
        /// </summary>
        /// <typeparam name="T">The type for which to create the logger</typeparam>
        /// <returns>Configured ILogger instance</returns>
        public static ILogger<T> CreateLogger<T>()
        {
            return _sharedLoggerFactory.Value.CreateLogger<T>();
        }

        /// <summary>
        /// Creates a logger with the specified category name using a shared singleton logger factory
        /// This ensures all loggers write to the same log file
        /// </summary>
        /// <param name="categoryName">The category name for the logger</param>
        /// <returns>Configured ILogger instance</returns>
        public static Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return _sharedLoggerFactory.Value.CreateLogger(categoryName);
        }
    }
}
