@echo off

set CONFIG=%2

if "%CONFIG%"=="" (
	echo "ERROR: No configuration specified. Please specify Debug or Release."
	echo "Usage: deploy.bat <path to ACAT root> <Debug|Release>"
	exit /b 1
)
if "%CONFIG%"=="Debug" (
	echo Deploying Debug configuration
) else if "%CONFIG%"=="Release" (
	echo Deploying Release configuration
) else (
	echo "ERROR: Invalid configuration specified. Please specify Debug or Release."
	echo "Usage: deploy.bat <path to ACAT root> <Debug|Release>"
	exit /b 1
)
set INSTALLDIR=.\Applications\ACATApp\bin\%CONFIG%

echo Deploying ACAT to %INSTALLDIR%
cd %1

echo Current Directory is %CD%


rem ------------------------------------------------
@echo Deploying Install files
rem ------------------------------------------------

set SOURCEDIR=Applications\Install\Users\
set TARGETDIR=%INSTALLDIR%\Install\Users\
call :safe_xcopy %SOURCEDIR%\*.* %TARGETDIR% 
if errorlevel 1 exit /b 1

set LANGUAGE=en

set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\WordPredictors\ConvAssist
set SOURCEDIR=Applications\Install\%LANGUAGE%\WordPredictors\ConvAssist
if not exist %SOURCEDIR% (
	echo ERROR: %SOURCEDIR% does not exist
	exit /b 1
)
call :safe_xcopy %SOURCEDIR%\*.* %TARGETDIR% 
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying ConvAssist
rem ------------------------------------------------

:DeployConvAssist
set SOURCEDIR=Applications\Install\ConvAssistApp
set TARGETDIR=%INSTALLDIR%\ConvAssistApp
if not exist %TARGETDIR% (
	mkdir %TARGETDIR%
) else (
	del /s /q %TARGETDIR%\*
)
if not exist %SOURCEDIR%\ConvAssist\ (
	powershell -Command "Expand-Archive -Force -Path %SOURCEDIR%\ConvAssist.zip -Destination %SOURCEDIR%\ConvAssist"
)
call :safe_xcopy /s /y /e /i %SOURCEDIR%\ConvAssist\* %TARGETDIR%
if errorlevel 1 exit /b 1

:DeployAssets
rem ------------------------------------------------
@echo Deploying Assets
rem ------------------------------------------------
set SOURCEDIR=Assets\
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
call :safe_xcopy %SOURCEDIR%\*.* %TARGETDIR% 
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying UI dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\UI\Scanners
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if errorlevel 1 exit /b 1
if exist .\%SOURCEDIR%\Config\*.xml call :safe_copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1

set SOURCEDIR=Extensions\Default\UI\Menus
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\Menus.dll %TARGETDIR%
if errorlevel 1 exit /b 1

call :safe_copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying English Language UI DLL's
rem ------------------------------------------------

set LANGUAGE=en
set BASEDIR=Extensions\Default\UI
set SOURCEDIR=%BASEDIR%\%LANGUAGE%\Scanners
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\Scanners
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if errorlevel 1 exit /b 1

if exist .\%SOURCEDIR%\Config\*.xml call :safe_copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1

set SOURCEDIR=%BASEDIR%\%LANGUAGE%\UserControls
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\UserControls
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if errorlevel 1 exit /b 1

if exist .\%SOURCEDIR%\Config\*.xml call :safe_copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1
rem ------------------------------------------------
@echo Deploying Camera Actuator
rem ------------------------------------------------
set SOURCEDIR=Extensions\Default\Actuators\CameraActuator\bin\%CONFIG%\
set TARGETDIR=%INSTALLDIR%\Extensions\Default\Actuators\Camera
echo TargetDir is %TARGETDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy %SOURCEDIR%\CameraActuator.dll %TARGETDIR%
if errorlevel 1 exit /b 1

set SOURCEDIR=Extensions\Default\Actuators\CameraActuator\External\
call :safe_xcopy %SOURCEDIR% %TARGETDIR%
if errorlevel 1 exit /b 1
call :safe_copy .\%SOURCEDIR%shape_predictor_68_face_landmarks.dat %INSTALLDIR%
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying TTSEngine dlls
rem ------------------------------------------------
set SOURCEDIR=Extensions\Default\TTSEngines\SAPIEngine
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\SAPIEngine.dll %TARGETDIR%
if errorlevel 1 exit /b 1

set SOURCEDIR=Extensions\Default\TTSEngines\TTSClient
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\TTSClient.dll %TARGETDIR%
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying SpellChecker dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\SpellCheckers\SpellCheck
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\SpellCheck.dll %TARGETDIR%
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying AppAgents dlls
rem ------------------------------------------------

set AGENT=ACATAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if errorlevel 1 exit /b 1
if exist %SOURCEDIR%\*.xml call :safe_copy %SOURCEDIR%\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1
if exist %SOURCEDIR%\Config\*.xml call :safe_copy %SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1

set AGENT=TalkApplicationScannerAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if errorlevel 1 exit /b 1
if exist %SOURCEDIR%\*.xml call :safe_copy %SOURCEDIR%\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1
if exist %SOURCEDIR%\Config\*.xml call :safe_copy %SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying ACAT WordPredictor dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\WordPredictors\ConvAssist
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying ACAT UserControls
rem ------------------------------------------------


set SOURCEDIR=%BASEDIR%\%LANGUAGE%\UserControls
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\UserControls
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if errorlevel 1 exit /b 1
if exist .\%SOURCEDIR%\Config\*.xml call :safe_copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1


rem ------------------------------------------------
@echo Deploying Localization Resources
rem ------------------------------------------------

set LANGUAGE=en
set SOURCEDIR=ACATResources\bin\%CONFIG%\%LANGUAGE%
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\*.* %TARGETDIR%
if errorlevel 1 exit /b 1


rem ------------------------------------------------
@echo Deploying Redistributables
rem ------------------------------------------------
set SHARED_LIB_DIR=%INSTALLDIR%\SharedLibs
if not exist %SHARED_LIB_DIR% mkdir %SHARED_LIB_DIR%
call :safe_copy Redistributable\*.*  %SHARED_LIB_DIR%
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying Docs
rem ------------------------------------------------
set TARGETDIR=%INSTALLDIR%\Docs
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_xcopy Docs\*.* %TARGETDIR% 
if errorlevel 1 exit /b 1

rem ------------------------------------------------
@echo Deploying BCI Extensions
rem ------------------------------------------------

set SOURCEDIR=Extensions\BCI\Actuators\BCIActuator
set BCIEXTERNALSRCDIR=Extensions\BCI\Actuators\External
echo Install Dir is %INSTALLDIR%
set TARGETDIR=%INSTALLDIR%\Extensions\BCI\Actuators\BCIActuator
echo Target Dir is %TARGETDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
call :safe_copy .\%BCIEXTERNALSRCDIR%\brainflow.5.5.0\*.dll %TARGETDIR%
if errorlevel 1 exit /b 1

set LANGUAGE=en
set BASEDIR=Extensions\BCI\UI
set SOURCEDIR=%BASEDIR%\%LANGUAGE%\Scanners
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\Scanners
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml call :safe_copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1

set SOURCEDIR=%BASEDIR%\%LANGUAGE%\UserControls
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\UserControls
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll call :safe_copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml call :safe_copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%
if errorlevel 1 exit /b 1

echo Completed deploying ACAT to %INSTALLDIR%
exit /b 0

:safe_copy
copy %1 %2
echo ERRORLEVEL is %errorlevel%
if errorlevel 1 (
    echo Failed to copy %1 to %2
    exit /b 1
)
goto :eof

:safe_xcopy
xcopy /E /Y %1 %2
echo ERRORLEVEL is %errorlevel%
if errorlevel 1 (
    echo Failed to xcopy from %1 to %2
    exit /b 1
)
goto :eof
