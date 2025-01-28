rem @echo off

set CONFIG=%2
set INSTALLDIR=.\Applications\ACATApp\bin\%CONFIG%

cd %1

rem ------------------------------------------------
@echo Deploying Install files
rem ------------------------------------------------

set SOURCEDIR=Applications\Install\Users
set TARGETDIR=%INSTALLDIR%\Install\Users
xcopy /s /y /e /i %SOURCEDIR%\*.* %TARGETDIR% 

set LANGUAGE=en

set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\WordPredictors\ConvAssist
set SOURCEDIR=Applications\Install\%LANGUAGE%\WordPredictors\ConvAssist
if not exist %SOURCEDIR% (
	echo ERROR: %SOURCEDIR% does not exist
	goto DeployConvAssist
)
xcopy /s /y /e /i %SOURCEDIR%\*.* %TARGETDIR% 

rem ------------------------------------------------
@echo Deploying ConvAssist
rem ------------------------------------------------

:DeployConvAssist
set SOURCEDIR=Applications\Install\ConvAssistApp
set TARGETDIR=%INSTALLDIR%\ConvAssistApp
if not exist %TARGETDIR% mkdir %TARGETDIR%
if not exist %SOURCEDIR%\ConvAssist\ (
	powershell -Command "Expand-Archive -Path %SOURCEDIR%\ConvAssist.zip -Destination %SOURCEDIR%\ConvAssist"
)
xcopy /s /y /e /i %SOURCEDIR%\ConvAssist\*.* %TARGETDIR%

:DeployAssets
rem ------------------------------------------------
@echo Deploying Assets
rem ------------------------------------------------
set SOURCEDIR=Assets
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
xcopy /s /y /e /i %SOURCEDIR%\*.* %TARGETDIR% 

rem ------------------------------------------------
@echo Deploying UI dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\UI\Scanners
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

set SOURCEDIR=Extensions\Default\UI\Menus
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\Menus.dll %TARGETDIR%
copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

rem ------------------------------------------------
@echo Deploying English Language UI DLL's
rem ------------------------------------------------

set LANGUAGE=en
set BASEDIR=Extensions\Default\UI
set SOURCEDIR=%BASEDIR%\%LANGUAGE%\Scanners
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\Scanners
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

set SOURCEDIR=%BASEDIR%\%LANGUAGE%\UserControls
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\UserControls
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

rem goto Next

rem ------------------------------------------------
@echo Actuators
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\Actuators\CameraActuator
set TARGETDIR=%INSTALLDIR%\Extensions\Default\Actuators\Camera
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\CameraActuator.dll %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\*.exe %TARGETDIR%
if exist .\%SOURCEDIR%\External goto CopyVisionExternal
echo *** ERROR *** Could not find External dependencies for the Vision Actuator (.\%SOURCEDIR%\External).
rem goto Next

:CopyVisionExternal
if not exist %TARGETDIR%\acat_gestures_dll.dll copy .\%SOURCEDIR%\External\*.* %TARGETDIR%
if not exist %INSTALLDIR%\shape_predictor_68_face_landmarks.dat copy .\%SOURCEDIR%\External\shape_predictor_68_face_landmarks.dat %INSTALLDIR%


rem ------------------------------------------------
@echo Deploying TTSEngine dlls
rem ------------------------------------------------
:Next
set SOURCEDIR=Extensions\Default\TTSEngines\SAPIEngine
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\SAPIEngine.dll %TARGETDIR%

set SOURCEDIR=Extensions\Default\TTSEngines\TTSClient
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\TTSClient.dll %TARGETDIR%

rem ------------------------------------------------
@echo Deploying SpellChecker dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\SpellCheckers\SpellCheck
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\SpellCheck.dll %TARGETDIR%

rem ------------------------------------------------
@echo Deploying AppAgents dlls
rem ------------------------------------------------

set AGENT=ACATAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=TalkApplicationScannerAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

rem ------------------------------------------------
@echo Deploying ACAT WordPredictor dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\WordPredictors\ConvAssist
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%

rem ------------------------------------------------
@echo Deploying ACAT UserControls
rem ------------------------------------------------


set SOURCEDIR=%BASEDIR%\%LANGUAGE%\UserControls
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\UserControls
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%


rem ------------------------------------------------
@echo Deploying Localization Resources
rem ------------------------------------------------

set LANGUAGE=en
set SOURCEDIR=ACATResources\bin\%CONFIG%\%LANGUAGE%
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\*.* %TARGETDIR%


rem ------------------------------------------------
@echo Deploying Redistributables
rem ------------------------------------------------
set SHARED_LIB_DIR=%INSTALLDIR%\SharedLibs
if not exist %SHARED_LIB_DIR% mkdir %SHARED_LIB_DIR%
copy Redistributable\*.*  %SHARED_LIB_DIR%

rem ------------------------------------------------
@echo Deploying Docs
rem ------------------------------------------------
set TARGETDIR=%INSTALLDIR%\Docs
if not exist %TARGETDIR% mkdir %TARGETDIR%
xcopy /s /y /e /i Docs\*.* %TARGETDIR% 

rem ------------------------------------------------
@echo Deploying BCI Extensions
rem ------------------------------------------------

set SOURCEDIR=Extensions\BCI\Actuators\BCIActuator
set BCIEXTERNALSRCDIR=Extensions\BCI\Actuators\External
set TARGETDIR=%INSTALLDIR%\Extensions\BCI\Actuators\BCIActuator
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
copy .\%BCIEXTERNALSRCDIR%\brainflow.5.5.0\*.dll %TARGETDIR%

set LANGUAGE=en
set BASEDIR=Extensions\BCI\UI
set SOURCEDIR=%BASEDIR%\%LANGUAGE%\Scanners
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\Scanners
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

set SOURCEDIR=%BASEDIR%\%LANGUAGE%\UserControls
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%BASEDIR%\UserControls
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%
