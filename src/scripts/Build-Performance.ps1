# Build ACATTalk with Performance Monitoring
# This script builds the entire ACAT solution and ACATTalk with PERFORMANCE symbol defined

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [switch]$Clean,
    
    [Parameter(Mandatory=$false)]
    [switch]$Run,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipRestore
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ACATTalk Performance Build Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Resolve paths relative to script location
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionDir = Split-Path -Parent $scriptPath
$solutionPath = Join-Path $solutionDir "ACAT.sln"
$projectPath = Join-Path $solutionDir "Applications\ACATTalk\ACATTalk.csproj"
$outputPath = Join-Path $solutionDir "build\bin\$Configuration\ACATTalk.exe"

# Verify paths
if (-not (Test-Path $solutionPath)) {
    Write-Error "ACAT.sln not found at: $solutionPath"
    exit 1
}

if (-not (Test-Path $projectPath)) {
    Write-Error "ACATTalk.csproj not found at: $projectPath"
    exit 1
}

Write-Host "Solution: $solutionPath" -ForegroundColor Gray
Write-Host "Project: ACATTalk" -ForegroundColor Gray
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
        break
    }
}

if (-not $nuget -and -not $SkipRestore) {
    Write-Host "NuGet not found in PATH. Will use MSBuild restore instead." -ForegroundColor Yellow
}

Write-Host "Configuration: $Configuration" -ForegroundColor White
Write-Host "Performance Monitoring: ENABLED" -ForegroundColor Green
Write-Host ""

# Clean if requested
if ($Clean) {
    Write-Host "Cleaning solution..." -ForegroundColor Yellow
    & $msbuild.Source $solutionPath /t:Clean /p:Configuration=$Configuration /verbosity:minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Clean had some issues but continuing..."
    }
    else {
        Write-Host "Clean complete." -ForegroundColor Green
    }
    Write-Host ""
}

# Restore NuGet packages
if (-not $SkipRestore) {
    Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow

    # Use MSBuild restore with the same configuration as the build
    # This ensures assets files are in the correct location
    Write-Host "Using MSBuild to restore packages..." -ForegroundColor Gray
    & $msbuild.Source $solutionPath /t:Restore /p:Configuration=$Configuration /p:RestorePackagesConfig=true /verbosity:minimal

    if ($LASTEXITCODE -ne 0) {
        Write-Host "MSBuild restore failed. Trying NuGet.exe..." -ForegroundColor Yellow

        if ($nuget) {
            & $nuget.Source restore $solutionPath -NonInteractive
            if ($LASTEXITCODE -ne 0) {
                Write-Error "NuGet restore failed. Please restore packages manually:"
                Write-Host "  nuget restore ACAT.sln" -ForegroundColor White
                Write-Host "  Or: msbuild ACAT.sln /t:Restore /p:Configuration=$Configuration" -ForegroundColor White
                Write-Host "  Or: .\scripts\Restore-Packages.ps1" -ForegroundColor White
                exit 1
            }
            Write-Host "NuGet restore complete." -ForegroundColor Green
        }
        else {
            Write-Error "Package restore failed and nuget.exe not found."
            Write-Host ""
            Write-Host "Please restore packages manually:" -ForegroundColor Yellow
            Write-Host "  .\scripts\Restore-Packages.ps1" -ForegroundColor White
            Write-Host "  Or: msbuild ACAT.sln /t:Restore /p:Configuration=$Configuration" -ForegroundColor White
            exit 1
        }
    }
    else {
        Write-Host "Package restore complete." -ForegroundColor Green
    }
    Write-Host ""
}

# Build solution with PERFORMANCE defined for ACATTalk
Write-Host "Building solution..." -ForegroundColor Yellow
Write-Host "(This may take a few minutes as all dependencies are built)" -ForegroundColor Gray
Write-Host ""

# Build the entire solution normally first with x64 platform
Write-Host "Building with Platform=x64 (64-bit)..." -ForegroundColor Gray
& $msbuild.Source $solutionPath /t:Build /p:Configuration=$Configuration /p:Platform=x64 /verbosity:minimal /maxcpucount
if ($LASTEXITCODE -ne 0) {
    Write-Error "Solution build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Rebuilding ACATTalk with PERFORMANCE monitoring..." -ForegroundColor Yellow

# Restore ACATTalk for x64 platform
Write-Host "Restoring ACATTalk for x64 platform..." -ForegroundColor Gray
& $msbuild.Source $projectPath /t:Restore /p:Configuration=$Configuration /p:Platform=x64 /verbosity:quiet
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Restore with Platform=x64 failed, but continuing..."
}

# Build ACATTalk with x64 platform and PERFORMANCE symbol
# Use /property: syntax with quoted value and %3B for semicolon
& $msbuild.Source $projectPath /t:Build /property:Configuration=$Configuration /property:Platform=x64 "/property:DefineConstants=TRACE%3BPERFORMANCE" /verbosity:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "ACATTalk build with PERFORMANCE failed with exit code $LASTEXITCODE"
    Write-Host ""
    Write-Host "Try manually:" -ForegroundColor Yellow
    Write-Host "  msbuild Applications\ACATTalk\ACATTalk.csproj /t:Build /property:Configuration=$Configuration /property:Platform=x64 `/property:DefineConstants=`"TRACE%3BPERFORMANCE`"" -ForegroundColor White
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""
Write-Host "Output: $outputPath" -ForegroundColor Cyan

# Run if requested
if ($Run) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Launching ACATTalk..." -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    if (Test-Path $outputPath) {
        Write-Host "Performance reports will be saved to:" -ForegroundColor Yellow
        Write-Host "  $env:USERPROFILE\ACATTalk_PerformanceReports\" -ForegroundColor White
        Write-Host ""
        
        Start-Process -FilePath $outputPath -WorkingDirectory (Split-Path $outputPath)
        
        Write-Host "Application launched. Performance report will be generated on exit." -ForegroundColor Green
    }
    else {
        Write-Error "Executable not found at: $outputPath"
        exit 1
    }
}
else {
    Write-Host ""
    Write-Host "To run ACATTalk with performance monitoring:" -ForegroundColor Yellow
    Write-Host "  $outputPath" -ForegroundColor White
    Write-Host ""
    Write-Host "Or use: .\scripts\Build-Performance.ps1 -Run" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Performance reports will be saved to:" -ForegroundColor Yellow
    Write-Host "  $env:USERPROFILE\ACATTalk_PerformanceReports\" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Done!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
