# Restore NuGet packages for ACAT solution
# This script attempts to restore NuGet packages using multiple methods

param(
    [Parameter(Mandatory=$false)]
    [switch]$Force
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ACAT NuGet Package Restore" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Resolve paths
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionDir = Split-Path -Parent $scriptPath
$solutionPath = Join-Path $solutionDir "ACAT.sln"

if (-not (Test-Path $solutionPath)) {
    Write-Error "ACAT.sln not found at: $solutionPath"
    exit 1
}

Write-Host "Solution: $solutionPath" -ForegroundColor Gray
Write-Host ""

# Find MSBuild
$msbuild = Get-Command msbuild -ErrorAction SilentlyContinue
if (-not $msbuild) {
    Write-Host "MSBuild not found in PATH. Searching for VS installation..." -ForegroundColor Yellow
    
    $vswherePath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswherePath) {
        $vsPath = & $vswherePath -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
        if ($vsPath) {
            $msbuildPath = Join-Path $vsPath "MSBuild\Current\Bin\MSBuild.exe"
            if (Test-Path $msbuildPath) {
                $msbuild = Get-Command $msbuildPath
                Write-Host "Found MSBuild at: $msbuildPath" -ForegroundColor Green
            }
        }
    }
    
    if (-not $msbuild) {
        Write-Error "MSBuild not found. Please ensure Visual Studio or Build Tools are installed."
        exit 1
    }
}

# Find NuGet
$nuget = $null
$nugetPaths = @(
    (Get-Command nuget -ErrorAction SilentlyContinue),
    "${env:ProgramFiles(x86)}\NuGet\nuget.exe",
    "$env:LOCALAPPDATA\NuGet\nuget.exe"
)

foreach ($path in $nugetPaths) {
    if ($path -and (Test-Path $path.Source -ErrorAction SilentlyContinue)) {
        $nuget = $path
        Write-Host "Found NuGet at: $($nuget.Source)" -ForegroundColor Green
        break
    }
}

if (-not $nuget) {
    Write-Host "NuGet.exe not found in common locations." -ForegroundColor Yellow
}

Write-Host ""

# Clean obj and bin folders if force
if ($Force) {
    Write-Host "Force restore requested. Cleaning obj and bin folders..." -ForegroundColor Yellow
    
    $objFolders = Get-ChildItem -Path $solutionDir -Filter "obj" -Recurse -Directory -ErrorAction SilentlyContinue
    $binFolders = Get-ChildItem -Path $solutionDir -Filter "bin" -Recurse -Directory -ErrorAction SilentlyContinue
    
    $totalFolders = $objFolders.Count + $binFolders.Count
    Write-Host "Found $totalFolders folders to clean..." -ForegroundColor Gray
    
    foreach ($folder in $objFolders) {
        try {
            Remove-Item $folder.FullName -Recurse -Force -ErrorAction SilentlyContinue
        }
        catch {
            Write-Warning "Could not delete: $($folder.FullName)"
        }
    }
    
    foreach ($folder in $binFolders) {
        try {
            Remove-Item $folder.FullName -Recurse -Force -ErrorAction SilentlyContinue
        }
        catch {
            Write-Warning "Could not delete: $($folder.FullName)"
        }
    }
    
    Write-Host "Clean complete." -ForegroundColor Green
    Write-Host ""
}

# Method 1: MSBuild Restore (recommended for SDK-style projects)
Write-Host "Method 1: MSBuild Restore" -ForegroundColor Cyan
Write-Host "-------------------------" -ForegroundColor Cyan
& $msbuild.Source $solutionPath /t:Restore /p:RestorePackagesConfig=true /verbosity:normal

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✓ MSBuild restore succeeded!" -ForegroundColor Green
    $method1Success = $true
}
else {
    Write-Host ""
    Write-Host "✗ MSBuild restore failed" -ForegroundColor Red
    $method1Success = $false
}

Write-Host ""

# Method 2: NuGet.exe restore (for packages.config)
if ($nuget) {
    Write-Host "Method 2: NuGet.exe Restore" -ForegroundColor Cyan
    Write-Host "---------------------------" -ForegroundColor Cyan
    & $nuget.Source restore $solutionPath -NonInteractive
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✓ NuGet.exe restore succeeded!" -ForegroundColor Green
        $method2Success = $true
    }
    else {
        Write-Host ""
        Write-Host "✗ NuGet.exe restore failed" -ForegroundColor Red
        $method2Success = $false
    }
    Write-Host ""
}
else {
    $method2Success = $null
}

# Method 3: DotNet restore (for .NET Core/Standard projects)
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($dotnet) {
    Write-Host "Method 3: dotnet restore" -ForegroundColor Cyan
    Write-Host "------------------------" -ForegroundColor Cyan
    & dotnet restore $solutionPath
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✓ dotnet restore succeeded!" -ForegroundColor Green
        $method3Success = $true
    }
    else {
        Write-Host ""
        Write-Host "✗ dotnet restore failed" -ForegroundColor Red
        $method3Success = $false
    }
    Write-Host ""
}
else {
    $method3Success = $null
}

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Restore Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Method 1 (MSBuild):  " -NoNewline
if ($method1Success) { Write-Host "✓ Success" -ForegroundColor Green } else { Write-Host "✗ Failed" -ForegroundColor Red }

if ($method2Success -ne $null) {
    Write-Host "Method 2 (NuGet.exe): " -NoNewline
    if ($method2Success) { Write-Host "✓ Success" -ForegroundColor Green } else { Write-Host "✗ Failed" -ForegroundColor Red }
}
else {
    Write-Host "Method 2 (NuGet.exe): Skipped (nuget.exe not found)" -ForegroundColor Yellow
}

if ($method3Success -ne $null) {
    Write-Host "Method 3 (dotnet):   " -NoNewline
    if ($method3Success) { Write-Host "✓ Success" -ForegroundColor Green } else { Write-Host "✗ Failed" -ForegroundColor Red }
}
else {
    Write-Host "Method 3 (dotnet):   Skipped (dotnet not found)" -ForegroundColor Yellow
}

Write-Host ""

# Check for missing assets files
Write-Host "Checking for missing assets files..." -ForegroundColor Yellow
$buildObj = Join-Path $solutionDir "build\obj"
if (Test-Path $buildObj) {
    $assetsFiles = Get-ChildItem -Path $buildObj -Filter "project.assets.json" -Recurse -ErrorAction SilentlyContinue
    Write-Host "Found $($assetsFiles.Count) project.assets.json files" -ForegroundColor Gray
    
    if ($assetsFiles.Count -eq 0) {
        Write-Host ""
        Write-Host "⚠ Warning: No assets files found in build\obj\" -ForegroundColor Yellow
        Write-Host "This may indicate restore issues." -ForegroundColor Yellow
    }
}

Write-Host ""

# Recommendations
if ($method1Success -or $method2Success -or $method3Success) {
    Write-Host "✓ Package restore completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now build the solution:" -ForegroundColor Cyan
    Write-Host "  .\scripts\Build-Performance.ps1 -SkipRestore" -ForegroundColor White
    Write-Host "  Or: msbuild ACAT.sln" -ForegroundColor White
}
else {
    Write-Host "✗ All restore methods failed" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting steps:" -ForegroundColor Yellow
    Write-Host "  1. Check internet connection" -ForegroundColor White
    Write-Host "  2. Clear NuGet cache: nuget locals all -clear" -ForegroundColor White
    Write-Host "  3. Try force restore: .\scripts\Restore-Packages.ps1 -Force" -ForegroundColor White
    Write-Host "  4. Check NuGet.config for package sources" -ForegroundColor White
    Write-Host "  5. Ensure you have access to required package feeds" -ForegroundColor White
    Write-Host ""
    Write-Host "If using a corporate network, you may need to:" -ForegroundColor Yellow
    Write-Host "  - Configure proxy settings for NuGet" -ForegroundColor White
    Write-Host "  - Add corporate package sources to NuGet.config" -ForegroundColor White
    Write-Host ""
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Done!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
