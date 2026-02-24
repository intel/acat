# Quick Reference: Fast Test Builds

## TL;DR

**For unit tests (fast, no PowerShell):**
```powershell
dotnet test /p:Configuration=TestOnly
```

**For running applications (includes all resources):**
```powershell
dotnet build /p:Configuration=Debug
```

---

## Build Configurations

| Configuration | Use Case | Speed | Resources |
|--------------|----------|-------|-----------|
| `TestOnly` | Unit tests | ⚡ Fast | ❌ No |
| `Debug` | Development | 🐢 Normal | ✅ Yes |
| `Release` | Production | 🐢 Normal | ✅ Yes |

---

## Common Commands

### Build Test Projects Only
```powershell
# Single project
dotnet build Libraries/ACATCore.Tests.Configuration/ACATCore.Tests.Configuration.csproj /p:Configuration=TestOnly

# All test projects
dotnet build --filter "*.Tests" /p:Configuration=TestOnly
```

### Run Tests
```powershell
# Fast build + test
dotnet test /p:Configuration=TestOnly

# Test specific project
dotnet test Libraries/ACATCore.Tests.Configuration/ACATCore.Tests.Configuration.csproj /p:Configuration=TestOnly
```

### Clean Test Builds
```powershell
# Clean TestOnly outputs
dotnet clean /p:Configuration=TestOnly

# Clean all configurations
dotnet clean
```

---

## What Gets Skipped in TestOnly?

### ❌ Skipped (saves time):
- ConvAssist.zip extraction (~2-3 seconds)
- Asset file copying (~1-2 seconds)
- Install files copying (~1 second)
- Panel config copying (~0.5 seconds)

### ✅ Still Included:
- Project compilation
- Test assemblies
- Dependencies
- Debug symbols

**Result:** ~50% faster test builds! 🚀

---

## Troubleshooting

### Tests fail with missing resources?
You're trying to run application code in TestOnly mode.

**Fix:** Use Debug configuration:
```powershell
dotnet build /p:Configuration=Debug
```

### PowerShell errors in CI/CD?
MSBuild's native Unzip should handle it. If not:

**Fix:** Ensure .NET SDK 2.1.400+ or Visual Studio 2017 15.8+

---

## CI/CD Examples

### GitHub Actions:
```yaml
- run: dotnet test /p:Configuration=TestOnly
```

### Azure DevOps:
```yaml
- script: dotnet test /p:Configuration=TestOnly
```

### Generic:
```bash
dotnet test /p:Configuration=TestOnly /p:ContinuousIntegrationBuild=true
```

---

## Performance

| Build Type | Time | Output Size |
|-----------|------|-------------|
| Debug | ~12-15s | ~500 MB |
| **TestOnly** | **~6-8s** | **~50 MB** |
| Release | ~15-20s | ~450 MB |

---

## Files Changed

- ✅ `ACATResources\ACATResources.csproj` - Uses native Unzip, conditional targets
- ✅ `Directory.Build.props` - Added TestOnly configuration

---

## When to Use Each

### Use `TestOnly` for:
- ✅ Running unit tests
- ✅ CI/CD test pipelines
- ✅ Quick validation builds
- ✅ Copilot agent builds

### Use `Debug` for:
- ✅ Running ACATTalk/ACATApp
- ✅ Integration testing
- ✅ Debugging applications
- ✅ Full feature testing

### Use `Release` for:
- ✅ Production deployments
- ✅ Performance testing
- ✅ Distribution packages
