# Quick Start: Integration Tests

**For**: Developers running Phase 1 integration tests  
**Time**: 5 minutes to get started

---

## Running the Tests

### Option 1: Command Line (Recommended)
```bash
# Navigate to test directory
cd src/Libraries/ACATCore.Tests.Integration

# Run all integration tests
dotnet test

# Run specific scenario
dotnet test --filter FullyQualifiedName~FreshInstall
dotnet test --filter FullyQualifiedName~XmlMigration
dotnet test --filter FullyQualifiedName~LoggingProduction
dotnet test --filter FullyQualifiedName~ConfigurationValidation
```

### Option 2: Visual Studio
1. Open `src/ACAT.sln`
2. Build solution (Ctrl+Shift+B)
3. Open Test Explorer (Test → Test Explorer)
4. Click "Run All" or select specific tests

---

## What Gets Tested

### 🆕 Fresh Install (6 tests)
Validates first-run experience with default configs and logs

### 🔄 XML Migration (8 tests)
Validates XML-to-JSON migration preserves all settings

### ⚡ Logging Performance (8 tests)
Validates production logging meets performance requirements
- **Key test**: 10,000 logs in < 100ms

### ✅ Config Validation (9 tests)
Validates error handling and fallback to defaults

---

## Test Results

Expected output:
```
Test run for ACATCore.Tests.Integration.dll (.NET Framework 4.8.1)
Total tests: 31
     Passed: 31
     Failed: 0
  Total time: ~5-10 seconds
```

---

## Troubleshooting

### Build Fails on Linux
**Issue**: PowerShell dependency in ACATResources  
**Solution**: Run tests on Windows environment

### File Lock Errors
**Issue**: Test cleanup fails due to file locks  
**Solution**: Wait a few seconds and run again (retry logic included)

### Test Times Out
**Issue**: Performance test exceeds time limit  
**Solution**: Check system load, close resource-heavy apps

---

## More Information

- Full documentation: `src/Libraries/ACATCore.Tests.Integration/README.md`
- Testing infrastructure guide: `TESTING_INFRASTRUCTURE.md`
- Consolidated docs index: `INDEX.md`

---

**Questions?** See comprehensive README in test directory
