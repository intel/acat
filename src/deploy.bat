rem @echo off


set CONFIG=%2
set INSTALLDIR=.\Applications\ACATApp\bin\%CONFIG%

cd %1


rem ------------------------------------------------
@echo Deploying Assets
rem ------------------------------------------------
set SOURCEDIR=Assets
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
xcopy /s /y /e /i %SOURCEDIR%\*.* %TARGETDIR% 

rem ------------------------------------------------
@echo Deploying UI dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\UI\Dialogs
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\Dialogs.dll %TARGETDIR%
copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%


set SOURCEDIR=Extensions\Default\UI\ContextMenus
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\ContextMenus.dll %TARGETDIR%
copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

set SOURCEDIR=Extensions\Default\UI\Scanners
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

rem ------------------------------------------------
@echo Deploying Actuator dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\Actuators\CameraActuator
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\CameraActuator.dll %TARGETDIR%

set SOURCEDIR=Extensions\Default\Actuators\WordsPlusActuator
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\WordsPlusActuator.dll %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\USBHidInterface.dll %TARGETDIR%



rem ------------------------------------------------
@echo Deploying TTSEngine dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\TTSEngines\SAPIEngine
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\SAPIEngine.dll %TARGETDIR%

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

set AGENT=AcrobatReaderAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=ACATAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=DialogControlAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=MenuControlAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=DLLHostAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%


set AGENT=FireFoxAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=ChromeBrowserAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=UnsupportedAppAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=InternetExplorerAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=MSWordAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=NotepadAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=WordpadAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=EudoraAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

rem ------------------------------------------------
@echo Deploying Functional Agent dlls
rem ------------------------------------------------

set AGENT=FileBrowserAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=NewFileAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=AbbreviationsAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=VolumeSettingsAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=SwitchWindowsAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=LaunchAppAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=LectureManagerAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
@echo Copying .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll to %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%\%AGENT%.dll
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

rem ------------------------------------------------
@echo Deploying ACAT WordPredictor dlls
rem ------------------------------------------------

set SOURCEDIR=Extensions\Default\WordPredictors\Presage
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\PresageWordPredictor.dll %TARGETDIR%

:end
