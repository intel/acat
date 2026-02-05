# Microsoft.Extensions.Logging Infrastructure Setup

This document describes the Microsoft.Extensions.Logging infrastructure that has been added to the ACAT project.

## Overview

ACAT now uses Microsoft.Extensions.Logging as its logging framework, replacing the previous custom logging implementation. This provides:

- **Structured logging** with modern logging patterns
- **Multiple log sinks** (Console, File)
- **Configurable log levels** (Debug, Info, Warning, Error)
- **Log file rotation** with size limits and retention policies
- **Integration with dependency injection** for testability

## Packages Added

The following NuGet packages have been added to all ACAT projects:

- **Microsoft.Extensions.Logging** (8.0.0) - Core logging abstractions
- **Microsoft.Extensions.Logging.Console** (8.0.0) - Console logging provider
- **Serilog.Extensions.Logging.File** (3.0.0) - File logging provider with rotation
- **Microsoft.Extensions.DependencyInjection** (8.0.0) - Dependency injection support

## LoggingConfiguration Class

A new `LoggingConfiguration` utility class has been added to `ACAT.Core.Utility` namespace:

```csharp
using ACAT.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Method 1: Use with dependency injection
var services = new ServiceCollection();
services.AddACATLogging();
var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<MyClass>>();

// Method 2: Create a standalone logger factory
var loggerFactory = LoggingConfiguration.CreateLoggerFactory();
var logger = loggerFactory.CreateLogger<MyClass>();

// Method 3: Create a logger directly
var logger = LoggingConfiguration.CreateLogger<MyClass>();
```

## Configuration Details

### Log File Location

Log files are created in the ACAT logs directory (typically `%APPDATA%\ACAT\Logs`):
- **File pattern**: `acat-{Date}.txt`
- **Example**: `acat-20260205.txt`

### Log File Settings

- **File size limit**: 10 MB per file
- **Retained files**: 7 days of log files
- **Automatic rotation**: Files are rotated when size limit is reached

### Log Levels

Log levels are set based on build configuration:

- **Debug builds** (`#if DEBUG`): Minimum level = `Debug`
  - Logs: Debug, Information, Warning, Error, Critical
  
- **Release builds**: Minimum level = `Information`
  - Logs: Information, Warning, Error, Critical

## Usage Examples

### Basic Logging

```csharp
using Microsoft.Extensions.Logging;

public class MyService
{
    private readonly ILogger<MyService> _logger;
    
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public void DoWork()
    {
        _logger.LogInformation("Starting work");
        _logger.LogDebug("Debug details: {Details}", someDetails);
        
        try
        {
            // Do work
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while doing work");
        }
    }
}
```

### Structured Logging

```csharp
// Use named parameters for structured logging
_logger.LogInformation("User {UserId} logged in at {LoginTime}", userId, DateTime.Now);

// Use @ to capture complex objects
_logger.LogInformation("Processing {@Configuration}", configObject);
```

### Console Output

In Debug mode, log messages are also written to the console for real-time monitoring during development.

## Migration from Legacy Log Class

The existing `Log.cs` class remains in place for backward compatibility. Future tickets will migrate existing logging calls to use the new Microsoft.Extensions.Logging infrastructure.

### Mapping Legacy to New Logging Levels

| Legacy Log Method | New Logging Method | Log Level |
|-------------------|-------------------|-----------|
| `Log.Verbose()` | `_logger.LogTrace()` or `_logger.LogDebug()` | Trace/Debug |
| `Log.Debug()` | `_logger.LogDebug()` or `_logger.LogInformation()` | Debug/Information |
| `Log.Info()` | `_logger.LogInformation()` | Information |
| `Log.Warn()` | `_logger.LogWarning()` | Warning |
| `Log.Error()` | `_logger.LogError()` | Error |
| `Log.Exception()` | `_logger.LogError(exception, ...)` | Error |

## Testing Logging Configuration

To test that logging is working correctly:

1. **Build the project**:
   ```bash
   dotnet build src/ACAT.sln
   ```

2. **Run an ACAT application**:
   ```bash
   dotnet run --project src/Applications/ACATApp/ACATApp.csproj
   ```

3. **Check for log files**:
   - Navigate to `%APPDATA%\ACAT\Logs`
   - Verify that `acat-{Date}.txt` files are created
   - Open the file to verify log entries are being written

4. **Verify console output** (Debug builds):
   - Run the application from command line
   - Verify log messages appear in the console

## Troubleshooting

### No log files created

- Check that the logs directory exists and is writable
- Verify that `LoggingConfiguration.AddACATLogging()` is called during application startup
- Check that the logger is being used (not null)

### Missing log messages

- Verify the log level configuration matches your needs
- In Release builds, Debug-level messages are not logged
- Check that exceptions aren't being swallowed

### Large log files

- The default file size limit is 10 MB
- Files are automatically rotated when the limit is reached
- Old log files are retained for 7 days

## Related Documentation

- [Microsoft.Extensions.Logging Documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)
- [Serilog File Provider](https://github.com/serilog/serilog-extensions-logging-file)
- [Structured Logging Best Practices](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-guidance)

## Future Enhancements

The following enhancements are planned for future releases:

1. **Log Level Configuration**: Add runtime configuration for log levels via config files
2. **Additional Sinks**: Support for Windows Event Log, database logging, etc.
3. **Log Filtering**: Category-based filtering for fine-grained control
4. **Performance Monitoring**: Integration with application metrics and telemetry
5. **Migration Tool**: Automated migration of legacy Log calls to new API

---

**Last Updated**: February 2026  
**Version**: 1.0.0
