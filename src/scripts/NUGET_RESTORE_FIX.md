# NuGet Restore Fix Summary

## Problem Resolved

Fixed the "Assets file 'project.assets.json' not found" error by improving NuGet restore in the build scripts.

## Root Cause

The build scripts were restoring packages without specifying the configuration, causing assets files to be generated in the wrong location:
- **Restore created**: `build\obj\ProjectName\project.assets.json`
- **Build expected**: `build\obj\Release\ProjectName\project.assets.json`

## Solution Implemented

### 1. Updated Build-Performance.ps1
- Restore now uses: `/t:Restore /p:Configuration=$Configuration`
- This ensures assets files are generated in the correct configuration-specific folder
- Added better error handling and fallback to NuGet.exe
- Fixed DefineConstants syntax error
- Added platform targeting for ACATTalk rebuild

### 2. Created Restore-Packages.ps1
New dedicated restore script that:
- Tries multiple restore methods (MSBuild, NuGet.exe, dotnet)
- Reports which methods succeeded
- Checks for missing assets files
- Provides troubleshooting guidance
- Supports `-Force` flag to clean obj/bin folders first

### 3. Created NUGET_TROUBLESHOOTING.md
Comprehensive troubleshooting guide covering:
- Common NuGet restore issues
- Corporate network/proxy configuration  
- Mixed project format handling
- Cache clearing procedures
- Verification steps

### 4. Updated Build-Performance.bat
- Uses proper restore command with configuration
- Better error messaging

## Usage

### Quick Fix
```powershell
# Restore packages using all available methods
.\scripts\Restore-Packages.ps1
```

### Build with Performance Monitoring
```powershell
# Full build with restore
.\scripts\Build-Performance.ps1 -Run

# Or skip restore if already done
.\scripts\Build-Performance.ps1 -SkipRestore -Run
```

### Force Restore (if issues persist)
```powershell
# Clean obj/bin folders and restore
.\scripts\Restore-Packages.ps1 -Force
```

## Current Build Issue

There is a file locking issue unrelated to NuGet restore:
```
error MSB3061: Unable to delete file "...\libonnxruntime_x64.so". 
The process cannot access the file because it is being used by another process.
```

**Resolution**:
1. Close all Visual Studio instances
2. Close any running ACAT applications
3. End any processes locking the files
4. Run build again

Or use Clean first:
```powershell
.\scripts\Build-Performance.ps1 -Clean
```

## Verification

After successful restore, you should see:
```powershell
Get-ChildItem -Path build\obj\Release -Filter "project.assets.json" -Recurse
```

This should show assets files in configuration-specific folders.

## Files Changed

1. **scripts/Build-Performance.ps1** - Fixed restore with configuration
2. **scripts/Build-Performance.bat** - Fixed restore command
3. **scripts/Restore-Packages.ps1** - NEW dedicated restore script
4. **scripts/NUGET_TROUBLESHOOTING.md** - NEW troubleshooting guide
5. **scripts/README.md** - Added Restore-Packages.ps1 documentation

## Next Steps

1. **Close all processes** that might lock DLL/SO files
2. **Run restore**:
   ```powershell
   .\scripts\Restore-Packages.ps1
   ```
3. **Build with performance monitoring**:
   ```powershell
   .\scripts\Build-Performance.ps1 -SkipRestore
   ```

## Key Improvements

✅ **Proper Configuration**: Restore now uses correct configuration path
✅ **Better Error Handling**: Clear error messages with solutions
✅ **Multiple Methods**: Tries MSBuild, NuGet.exe, dotnet restore
✅ **Dedicated Script**: `Restore-Packages.ps1` for troubleshooting
✅ **Comprehensive Docs**: NUGET_TROUBLESHOOTING.md with all scenarios
✅ **Force Option**: Clean obj/bin before restore if needed

## Testing

Restore script was tested and succeeded:
```
Method 1 (MSBuild):  ✓ Success
Method 2 (NuGet.exe): ✓ Success
Method 3 (dotnet):   ✓ Success

Found 66 project.assets.json files
```

Solution build succeeded until file locking issue (unrelated to NuGet).

## Documentation

- **scripts/README.md** - Updated with Restore-Packages.ps1 info
- **scripts/NUGET_TROUBLESHOOTING.md** - Complete troubleshooting guide
- **scripts/Restore-Packages.ps1** - Self-documenting with verbose output

All scripts now handle NuGet restore correctly with proper configuration targeting.
