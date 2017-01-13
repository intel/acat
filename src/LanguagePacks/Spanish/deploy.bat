@echo on
if not exist %1\config.bat exit
cd %1
call config.bat
set SOLUTIONDIR=%2
set CONFIG=%3
set INSTALLDIR=%ACAT_APPDIR%\bin\%CONFIG%
set LANGUAGE=es

set SOURCEDIR=%SOLUTIONDIR%\Resources\bin\%CONFIG%
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%
if not exist %TARGETDIR% mkdir %TARGETDIR%

copy %SOURCEDIR%\%LANGUAGE%\ACATResources.resources.dll %TARGETDIR%
copy %SOURCEDIR%\UI\*.* %TARGETDIR%

set SOURCEDIR=%SOLUTIONDIR%\Presage\bin\%CONFIG%
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%ACAT_EXTENSIONDIR%\WordPredictors\Presage
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy %SOURCEDIR%\*.dll %TARGETDIR%

set SOURCEDIR=%SOLUTIONDIR%Presage\Database
set PRESAGEDIR=WordPredictors\Presage
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%PRESAGEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
rem for /f %%f in ('dir /b %SOURCEDIR%\*.*') do if not exist %TARGETDIR%\%%f copy %%f %TARGETDIR%
for /f %%f in ('dir /b %SOURCEDIR%\*.*') do if not exist %TARGETDIR%\%%f copy %SOURCEDIR%\%%f %TARGETDIR%


if not exist %TARGETDIR%\database.db copy %SOURCEDIR%\database.db %TARGETDIR%

set SOURCEDIR=%SOLUTIONDIR%\Resources\Install\Users\DefaultUser\%LANGUAGE%
set TARGETDIR=%INSTALLDIR%\%ACAT_USERINSTALLDIR%\%LANGUAGE%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy %SOURCEDIR%\*.* %TARGETDIR%

