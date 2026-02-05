# ACAT Logging Tests

Comprehensive unit test suite for the ACAT logging infrastructure.

## Overview

This test project validates the logging functionality across three main components:
- **Log** class (legacy logging with TraceSwitch)
- **LoggingConfiguration** class (Microsoft.Extensions.Logging integration)
- **CachedLog** class (in-memory accumulation and batch persistence)

## Test Results

**Total Tests:** 32  
**Passed:** 32  
**Failed:** 0  
**Execution Time:** ~1.5 seconds ✓

## Test Coverage

### 1. Legacy Log Level Tests (9 tests)
Tests for the traditional `Log` class with TraceSwitch functionality:
- Verbose, Info, Warning, Error, and Off level configuration
- Debug method string parameter acceptance
- Exception logging (string and Exception object)
- IsNull utility method validation

### 2. CachedLog Behavior Tests (7 tests)
Tests for the in-memory log accumulation system:
- Instance creation
- Entry addition with type and data
- Multiple entries before save
- Save operation success
- Empty log saving
- Special character handling (commas, quotes, newlines)
- Sequential save operations with file append

### 3. Modern Logging Configuration Tests (11 tests)
Tests for Microsoft.Extensions.Logging integration:
- Generic logger creation (`CreateLogger<T>()`)
- Category-based logger creation
- Logger factory instantiation
- Debug, Information, Warning, and Error level logging
- Structured logging with parameters
- Exception logging with context messages
- Multiple logger instance coexistence
- Factory-based logger generation

### 4. Performance Tests (5 tests)
Tests for performance and concurrency:
- High volume logging (1000 messages < 2 seconds)
- Concurrent multi-threaded logging (10 threads × 50 messages)
- Rapid sequential calls (400 messages < 1 second)
- Single log call response time (< 100ms)
- Parallel exception logging

## Building and Running

### Prerequisites
- .NET Framework 4.8.1
- Mono (for running on Linux)
- MSTest framework 3.7.0

### Build
```bash
cd src/Libraries/ACATCore.Tests.Logging
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Run with Verbosity
```bash
dotnet test --verbosity normal
```

## Project Structure

```
ACATCore.Tests.Logging/
├── ACATCore.Tests.Logging.csproj    # Project configuration
├── TestWorkspace.cs                  # Test helper utilities
├── LegacyLogLevelTests.cs           # Log class tests
├── CachedLogBehaviorTests.cs        # CachedLog tests
├── ModernLoggingConfigTests.cs      # LoggingConfiguration tests
├── LoggingPerformanceTests.cs       # Performance and concurrency tests
└── README.md                         # This file
```

## Test Utilities

### TestWorkspace Class
Provides shared functionality for test resource management:
- `CreateIsolatedFolder()` - Creates unique temporary directories
- `CleanupAll()` - Removes all test artifacts
- `ReadFileWithRetry()` - Reads files with retry logic for locks
- `WaitForFile()` - Waits for file availability with timeout

## CI/CD Integration

### Adding to Build Pipeline
Add to your workflow:
```yaml
- name: Run Logging Tests
  run: dotnet test src/Libraries/ACATCore.Tests.Logging/ACATCore.Tests.Logging.csproj --logger "trx"
```

## Notes

- Tests use isolated temporary directories to avoid conflicts
- Cleanup happens automatically in `[TestCleanup]` methods
- Performance thresholds are deliberately generous for CI environments
- Tests are designed to work on both Windows and Linux (with Mono)

## Maintenance

When adding new logging features:
1. Add corresponding test methods to appropriate test class
2. Follow existing naming patterns: `[When/Verify]<Condition>_<ExpectedBehavior>`
3. Keep tests focused and independent
4. Clean up resources in `[TestCleanup]`
5. Aim for < 5 second total test execution time

## License

Copyright 2013-2019; 2023 Intel Corporation  
SPDX-License-Identifier: Apache-2.0
