@echo off
REM Build ACAT solution and ACATTalk with performance monitoring

echo ========================================
echo ACATTalk Performance Build
echo ========================================
echo.

REM Find MSBuild
set MSBUILD=
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
    set MSBUILD=%%i
)

if not defined MSBUILD (
    echo ERROR: MSBuild not found. Please ensure Visual Studio is installed.
    pause
    exit /b 1
)

echo Using MSBuild: %MSBUILD%
echo.

REM Get paths
cd /d %~dp0..
set SOLUTION=ACAT.sln
set PROJECT=Applications\ACATTalk\ACATTalk.csproj

REM Restore NuGet packages
echo Restoring NuGet packages...
"%MSBUILD%" "%SOLUTION%" /t:Restore /p:RestorePackagesConfig=true /verbosity:minimal

if errorlevel 1 (
    echo.
    echo WARNING: Package restore had issues. Trying to continue...
    echo.
    echo If build fails, try running:
    echo   scripts\Restore-Packages.ps1
    echo.
)

echo.

REM Build solution first to ensure all dependencies are built
echo Building solution (this may take a few minutes)...
"%MSBUILD%" "%SOLUTION%" /t:Build /p:Configuration=Release /verbosity:minimal /maxcpucount

if errorlevel 1 (
    echo.
    echo ERROR: Solution build failed!
    pause
    exit /b 1
)

echo.
echo Rebuilding ACATTalk with performance monitoring enabled...
"%MSBUILD%" "%PROJECT%" /t:Rebuild /p:Configuration=Release /p:DefineConstants="TRACE;PERFORMANCE" /verbosity:minimal

if errorlevel 1 (
    echo.
    echo ERROR: ACATTalk rebuild failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build Successful!
echo ========================================
echo.

echo Performance reports will be saved to:
echo   %USERPROFILE%\ACATTalk_PerformanceReports\
echo.

echo Press any key to run ACATTalk...
pause > nul

start "" "build\bin\Release\ACATTalk.exe"

echo.
echo ACATTalk launched. Performance report will be generated on exit.
echo.
pause
