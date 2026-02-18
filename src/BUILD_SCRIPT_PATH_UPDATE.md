# Build Script Output Path Update

## Summary
Updated the performance build scripts to use the correct centralized build output directory defined in `Directory.Build.props`.

## Changes Made

### 1. Build-Performance.ps1
**File:** `scripts/Build-Performance.ps1`  
**Line 31:** Updated output path

```powershell
# Before
$outputPath = Join-Path $solutionDir "Applications\ACATTalk\build\bin\$Configuration\ACATTalk.exe"

# After
$outputPath = Join-Path $solutionDir "build\bin\$Configuration\ACATTalk.exe"
```

### 2. Build-Performance.bat
**File:** `scripts/Build-Performance.bat`  
**Line 79:** Updated start command

```batch
REM Before
start "" "Applications\ACATTalk\build\bin\Release\ACATTalk.exe"

REM After
start "" "build\bin\Release\ACATTalk.exe"
```

## Reason for Change

The ACAT solution uses `Directory.Build.props` to centralize build output:

```xml
<PropertyGroup>
  <BaseOutputPath>$(SolutionRoot)build\</BaseOutputPath>
  <BuildPath>$(BaseOutputPath)bin\$(Configuration)\</BuildPath>
  <OutputPath>$(BuildPath)</OutputPath>
</PropertyGroup>
```

This means all projects output to:
- **Actual location:** `src\build\bin\{Configuration}\`
- **Old (incorrect) path:** `src\Applications\ACATTalk\build\bin\{Configuration}\`

The scripts were pointing to a non-existent location.

## Verification

✅ **Debug Build Output:**
```
C:\Users\mbeale\source\acat_clean\src\build\bin\Debug\ACATTalk.exe
```

✅ **Release Build Output:**
```
C:\Users\mbeale\source\acat_clean\src\build\bin\Release\ACATTalk.exe
```

Both files exist at the correct location after the build.

## Impact

### What Changed
- Build scripts now correctly locate the ACATTalk.exe after build
- Scripts can successfully launch the application with `-Run` flag
- Output paths displayed to users are now accurate

### What Didn't Change
- Build process itself (still works the same way)
- Performance monitoring functionality (unchanged)
- Report generation location (still `%USERPROFILE%\ACATTalk_PerformanceReports\`)
- Documentation files (already correct, didn't reference old path)

## Testing

```powershell
# Test the PowerShell script
.\scripts\Build-Performance.ps1 -Configuration Release

# Verify output path
Test-Path "C:\Users\mbeale\source\acat_clean\src\build\bin\Release\ACATTalk.exe"
# Returns: True ✅

# Test with -Run flag
.\scripts\Build-Performance.ps1 -Configuration Release -Run
# Should launch ACATTalk successfully ✅
```

## Related Files

### Modified
- ✅ `scripts/Build-Performance.ps1` - Line 31
- ✅ `scripts/Build-Performance.bat` - Line 79

### Verified (No Changes Needed)
- ✅ `scripts/README.md` - No path references
- ✅ `Applications/ACATTalk/QUICK_START.md` - No specific path references
- ✅ `Applications/ACATTalk/PERFORMANCE_MONITORING.md` - No specific path references

## Build Configuration Reference

For reference, the centralized build configuration in `Directory.Build.props`:

| Property | Value |
|----------|-------|
| `BaseOutputPath` | `$(SolutionRoot)build\` |
| `BuildPath` | `$(BaseOutputPath)bin\$(Configuration)\` |
| `OutputPath` | `$(BuildPath)` |
| `IntermediatePath` | `$(BaseOutputPath)obj\$(Configuration)\` |

**Result:** All projects build to `src\build\bin\{Configuration}\` regardless of their location in the solution.

## Notes

- This is a **path correction only** - no functional changes to performance monitoring
- The centralized build directory makes it easier to manage outputs from multiple projects
- All ACAT projects follow this convention (ACATApp, ACATTalk, ACATConfig, etc.)
- This aligns with the existing `Directory.Build.props` configuration that was already in place
