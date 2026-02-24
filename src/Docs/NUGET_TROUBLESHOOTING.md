# NuGet Package Restore Troubleshooting

## Problem: Assets file 'project.assets.json' not found

### Symptoms
```
error NETSDK1004: Assets file 'C:\...\build\obj\Release\ProjectName\project.assets.json' not found. 
Run a NuGet package restore to generate this file.
```

### Solution

#### Quick Fix (Try First)
```powershell
# Run the dedicated restore script
.\scripts\Restore-Packages.ps1
```

This script will:
- ✅ Try MSBuild restore
- ✅ Try NuGet.exe restore
- ✅ Try dotnet restore
- ✅ Show which methods succeeded
- ✅ Check for missing assets files

#### Manual Fix

**Option 1: MSBuild Restore (Recommended)**
```powershell
msbuild ACAT.sln /t:Restore /p:RestorePackagesConfig=true
```

**Option 2: NuGet.exe Restore**
```powershell
# Download nuget.exe if needed
# From: https://www.nuget.org/downloads

nuget restore ACAT.sln
```

**Option 3: dotnet restore**
```powershell
dotnet restore ACAT.sln
```

#### Force Restore (If Quick Fix Fails)
```powershell
# Clean everything and restore
.\scripts\Restore-Packages.ps1 -Force
```

This will:
1. Delete all `obj` and `bin` folders
2. Run all restore methods
3. Verify assets files are created

### Common Causes

#### 1. **First Build**
- NuGet packages not yet restored
- **Solution**: Run `.\scripts\Restore-Packages.ps1`

#### 2. **Corporate Network/Proxy**
- Proxy blocking NuGet.org
- Corporate package source required

**Solution**: Configure NuGet.config
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="http_proxy" value="http://proxy:port" />
  </config>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="Corporate" value="https://your-feed.com/nuget" />
  </packageSources>
</configuration>
```

#### 3. **Mixed Project Formats**
- Some projects use `packages.config`
- Some use `PackageReference`

**Solution**: Use MSBuild restore with both options
```powershell
msbuild ACAT.sln /t:Restore /p:RestorePackagesConfig=true
```

#### 4. **Corrupted Package Cache**
- Cached packages corrupted
- **Solution**: Clear cache
```powershell
# Clear all NuGet caches
nuget locals all -clear

# Or specific caches
nuget locals global-packages -clear
nuget locals http-cache -clear
nuget locals temp -clear

# Then restore
.\scripts\Restore-Packages.ps1
```

#### 5. **Build Output Path Changed**
- Custom build props changed output paths
- **Solution**: Clean and restore
```powershell
msbuild ACAT.sln /t:Clean
.\scripts\Restore-Packages.ps1 -Force
```

### Verification

After restore, verify assets files exist:
```powershell
# Check for project.assets.json files
Get-ChildItem -Path build\obj -Filter "project.assets.json" -Recurse | Select-Object FullName
```

You should see multiple `project.assets.json` files, one per project.

### Build After Restore

Once restore succeeds:

**Using Build Script:**
```powershell
# Skip restore since we just did it
.\scripts\Build-Performance.ps1 -SkipRestore
```

**Using MSBuild:**
```powershell
msbuild ACAT.sln /p:Configuration=Release
```

### Still Having Issues?

#### Check NuGet Configuration
```powershell
# View all NuGet settings
nuget config -Show

# Check package sources
nuget sources list
```

#### Check Visual Studio NuGet Settings
1. Tools → Options → NuGet Package Manager
2. Check "Package Sources"
3. Ensure nuget.org is enabled
4. Check "Clear All NuGet Cache(s)"

#### Check Project File
Look for `<RestorePackages>` in .csproj files:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RestorePackages>true</RestorePackages>
  </PropertyGroup>
</Project>
```

#### Enable Diagnostic Logging
```powershell
msbuild ACAT.sln /t:Restore /v:diag > restore.log 2>&1
```

Review `restore.log` for detailed error messages.

### Integration with Build-Performance.ps1

The `Build-Performance.ps1` script automatically runs restore:

```powershell
# With restore (default)
.\scripts\Build-Performance.ps1

# Skip restore if already done
.\scripts\Build-Performance.ps1 -SkipRestore
```

If Build-Performance.ps1 restore fails, run the dedicated restore script:
```powershell
.\scripts\Restore-Packages.ps1
.\scripts\Build-Performance.ps1 -SkipRestore
```

### Environment Variables

Some helpful environment variables:

```powershell
# Set NuGet config location
$env:NUGET_PACKAGES = "C:\path\to\packages"

# HTTP proxy
$env:HTTP_PROXY = "http://proxy:port"
$env:HTTPS_PROXY = "http://proxy:port"
```

### Quick Reference

| Problem | Solution |
|---------|----------|
| First build | `.\scripts\Restore-Packages.ps1` |
| Restore failed | `.\scripts\Restore-Packages.ps1 -Force` |
| Corporate network | Configure NuGet.config with proxy |
| Corrupted cache | `nuget locals all -clear` then restore |
| Build after restore | `.\scripts\Build-Performance.ps1 -SkipRestore` |

### Support

If issues persist:
1. Check `restore.log` for diagnostics
2. Verify internet/network connectivity
3. Check firewall/antivirus settings
4. Ensure Visual Studio is up to date
5. Try restore from Visual Studio UI (right-click solution → Restore NuGet Packages)

### Additional Resources

- [NuGet Package Restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore)
- [MSBuild /t:Restore](https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets#restore-target)
- [Troubleshooting Package Restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore-troubleshooting)
