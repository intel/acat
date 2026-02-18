# Performance Monitoring Scripts - Migration Summary

## What Changed

The performance monitoring scripts have been moved from `Applications\ACATTalk\` to the solution-level `scripts\` folder and enhanced with NuGet restore support.

## New Location

**Before:**
```
Applications\ACATTalk\
├── Build-Performance.ps1
├── Build-Performance.bat
├── Analyze-Performance.ps1
└── BUILD_SCRIPT_UPDATES.md
```

**After:**
```
scripts/                           # ← New solution-level location
├── Build-Performance.ps1          # Enhanced with NuGet restore
├── Build-Performance.bat          # Enhanced with NuGet restore
├── Analyze-Performance.ps1        # Updated paths
└── README.md                      # Comprehensive documentation

Applications/ACATTalk/             # Documentation stays with project
├── PerformanceMonitor.cs
├── Program.cs (instrumented)
├── PERFORMANCE_MONITORING.md
└── QUICK_START.md
```

## Why This Change?

1. **Better Organization** - Build scripts affect the entire solution, not just ACATTalk
2. **Clearer Responsibility** - Project folder contains code and docs, scripts folder contains build automation
3. **Easier to Find** - Developers expect build scripts at solution root level
4. **NuGet Restore** - Scripts now properly restore packages before building

## What's New

### 1. NuGet Package Restore
Scripts now automatically restore NuGet packages:
```powershell
msbuild ACAT.sln /t:Restore
```

### 2. Improved Error Handling
- Better fallback logic when NuGet.exe not in PATH
- Uses MSBuild restore as fallback
- Continue on restore warnings

### 3. Skip Restore Option
For faster builds when packages are already restored:
```powershell
.\scripts\Build-Performance.ps1 -SkipRestore
```

### 4. Solution-Level Documentation
New `scripts\README.md` provides:
- Complete script reference
- Usage examples
- Troubleshooting guide
- CI/CD integration examples

## Updated Commands

### Building

**Old (from Applications\ACATTalk):**
```powershell
cd Applications\ACATTalk
.\Build-Performance.ps1 -Run
```

**New (from solution root):**
```powershell
.\scripts\Build-Performance.ps1 -Run
```

### Analyzing

**Old:**
```powershell
cd Applications\ACATTalk
.\Analyze-Performance.ps1
```

**New:**
```powershell
.\scripts\Analyze-Performance.ps1
```

### Batch File

**Old:**
```cmd
cd Applications\ACATTalk
Build-Performance.bat
```

**New:**
```cmd
scripts\Build-Performance.bat
```

## Features Added

### NuGet Restore
```powershell
# Automatically restores packages
.\scripts\Build-Performance.ps1

# Skip restore for faster builds
.\scripts\Build-Performance.ps1 -SkipRestore
```

### Improved Build Process
1. **Restore** - NuGet packages restored first
2. **Build** - Entire solution built with dependencies
3. **Rebuild** - ACATTalk rebuilt with PERFORMANCE symbol
4. **Run** - Optionally launch application

### Better Path Resolution
Scripts now work from any location and properly resolve:
- Solution path
- Project path
- Output path

## Migration Guide

If you had the old scripts:

1. **Delete old scripts** (already done):
   - `Applications\ACATTalk\Build-Performance.ps1`
   - `Applications\ACATTalk\Build-Performance.bat`
   - `Applications\ACATTalk\Analyze-Performance.ps1`

2. **Use new scripts** from solution root:
   ```powershell
   .\scripts\Build-Performance.ps1 -Run
   .\scripts\Analyze-Performance.ps1
   ```

3. **Update any bookmarks/shortcuts** to point to `scripts\` folder

4. **Documentation updated**:
   - `Applications\ACATTalk\QUICK_START.md`
   - `Applications\ACATTalk\PERFORMANCE_MONITORING.md`

## Compatibility

✅ **Scripts work from solution root** - Primary usage
✅ **Scripts work from scripts folder** - Direct execution
✅ **NuGet restore automatic** - No manual restore needed
✅ **Fallback to MSBuild restore** - Works without NuGet.exe
✅ **Same report location** - `%USERPROFILE%\ACATTalk_PerformanceReports\`

## Quick Reference

```powershell
# From solution root (recommended)
.\scripts\Build-Performance.ps1 -Run              # Build and run
.\scripts\Build-Performance.ps1 -Clean -Run       # Clean, build, and run
.\scripts\Build-Performance.ps1 -SkipRestore      # Fast build (skip restore)
.\scripts\Analyze-Performance.ps1                 # Analyze latest report
.\scripts\Analyze-Performance.ps1 -Compare        # Compare with previous

# Or use batch file
scripts\Build-Performance.bat
```

## Documentation

- **`scripts\README.md`** - Complete script documentation
- **`Applications\ACATTalk\QUICK_START.md`** - Quick start guide
- **`Applications\ACATTalk\PERFORMANCE_MONITORING.md`** - Detailed guide

## Benefits

✅ **Proper Organization** - Scripts at solution level where they belong
✅ **NuGet Restore** - Automatic package restoration
✅ **Better Paths** - Works from solution root
✅ **Enhanced Docs** - scripts\README.md with full reference
✅ **CI/CD Ready** - Easy integration with build pipelines
✅ **Faster Builds** - Optional skip restore for repeated builds

## Next Steps

1. **Run from new location**:
   ```powershell
   .\scripts\Build-Performance.ps1 -Run
   ```

2. **Read the documentation**:
   - `scripts\README.md` - Script reference
   - `Applications\ACATTalk\QUICK_START.md` - Getting started

3. **Try the analysis tool**:
   ```powershell
   .\scripts\Analyze-Performance.ps1 -Compare
   ```

## Support

For questions:
- See `scripts\README.md` for script usage
- See `Applications\ACATTalk\PERFORMANCE_MONITORING.md` for performance monitoring details
- Check troubleshooting sections in documentation
