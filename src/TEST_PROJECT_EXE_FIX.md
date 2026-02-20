# Test Project Executable Output Fix

## Problem

Test projects `ACATCore.Tests` and `ACATExtension.Tests` were outputting executables to the shared `build\bin\Debug` directory, potentially causing test discovery to scan application executables.

## Root Cause

Both test projects had:
- `<OutputType>Exe</OutputType>` (required by xUnit v3)
- Output to centralized `build\bin\Debug` directory (from Directory.Build.props)

xUnit v3 **requires** test projects to be executables - cannot be changed to libraries.

## Solution

Added local output path overrides to both test projects:

```xml
<!-- Override Directory.Build.props centralized output -->
<OutputPath>bin\$(Configuration)\</OutputPath>
<BaseIntermediateOutputPath>obj\</BaseIntermediateOutputPath>
<IntermediateOutputPath>obj\$(Configuration)\</IntermediateOutputPath>
```

Also removed conflicting MSTest packages from ACATExtension.Tests (it should only use xUnit).

## Files Modified

1. **Libraries\ACATCore.Tests\ACATCore.Tests.csproj**
   - Kept `<OutputType>Exe</OutputType>` (required by xUnit)
   - Added local output path overrides

2. **Libraries\ACATExtension.Tests\ACATExtension.Tests.csproj**
   - Kept `<OutputType>Exe</OutputType>` (required by xUnit)
   - Added local output path overrides
   - Removed conflicting MSTest packages

## Verification

### Before Fix:
```
build\bin\Debug\
├── ACATCore.Tests.exe          ❌ Test exe in shared directory
├── ACATExtension.Tests.exe     ❌ Test exe in shared directory
├── ACATApp.exe                 ✅ Application exe
└── ACATConfig.exe              ✅ Application exe
```

### After Fix:
```
build\bin\Debug\
├── ACATApp.exe                 ✅ Application exe only
└── ACATConfig.exe              ✅ Application exe only

Libraries\ACATCore.Tests\bin\Debug\
└── ACATCore.Tests.exe          ✅ Test exe in local directory

Libraries\ACATExtension.Tests\bin\Debug\
└── ACATExtension.Tests.exe     ✅ Test exe in local directory
```

## Test Projects Output Summary

| Project | Framework | OutputType | Output Location | Reason |
|---------|-----------|------------|-----------------|--------|
| ACATCore.Tests.Configuration | MSTest | Library | Local bin\ | MSTest allows libraries |
| ACATCore.Tests.Integration | MSTest | Library | Local bin\ | MSTest allows libraries |
| ACATCore.Tests.Logging | MSTest | Library | Local bin\ | MSTest allows libraries |
| **ACATCore.Tests** | **xUnit v3** | **Exe** | **Local bin\** | **xUnit v3 requires Exe** |
| **ACATExtension.Tests** | **xUnit v3** | **Exe** | **Local bin\** | **xUnit v3 requires Exe** |

## Benefits

✅ **Test discovery no longer scans shared build directory**  
✅ **Application executables isolated from test executables**  
✅ **Faster test discovery** (fewer directories to scan)  
✅ **Cleaner build output**  
✅ **No ACATApp/ACATConfig launching during test discovery**

## xUnit v3 Requirement

xUnit v3 enforces test projects must be executables:

```xml
<!-- From xunit.v3.core.mtp-v1.targets -->
<Error Text="xUnit.net v3 test projects must be executable (set project property '&lt;OutputType&gt;Exe&lt;/OutputType&gt;'). 
       If this is not a test project, reference xunit.v3.extensibility.core instead." 
       Condition=" '$(OutputType)' != 'Exe' " />
```

**Cannot change to library** - must keep as Exe and use local output paths.

## Next Steps

If you want to convert to libraries in the future:
1. Migrate from xUnit v3 to MSTest (like other test projects)
2. Then change `<OutputType>` to Library
3. Remove the xUnit v3 packages

But for now, local output paths solve the problem while keeping xUnit.

## Testing

```powershell
# Verify test executables are not in shared directory
Get-ChildItem -Path "build\bin\Debug" -Filter "*Test*.exe"
# Should return: none

# Verify application executables are still there
Get-ChildItem -Path "build\bin\Debug" -Filter "ACAT*.exe"
# Should return: ACATApp.exe, ACATConfig.exe, ACATTalk.exe

# Verify test executables are in local directories
Get-ChildItem -Path "Libraries\ACATCore.Tests\bin\Debug" -Filter "*.exe"
# Should return: ACATCore.Tests.exe

Get-ChildItem -Path "Libraries\ACATExtension.Tests\bin\Debug" -Filter "*.exe"
# Should return: ACATExtension.Tests.exe
```

## Status

✅ **Fixed and verified**

Test executables now output to local directories, keeping them separate from application executables in the shared build directory.
