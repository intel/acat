@echo off

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


set SOURCEDIR=Extensions\Default\UI\Menus
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\Menus.dll %TARGETDIR%
copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

set SOURCEDIR=Extensions\Default\UI\Scanners
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
if exist .\%SOURCEDIR%\bin\%CONFIG%\*.dll copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%
if exist .\%SOURCEDIR%\Config\*.xml copy .\%SOURCEDIR%\Config\*.xml %TARGETDIR%

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

set SOURCEDIR=Extensions\Default\Actuators\Vision\VisionActuator
set TARGETDIR=%INSTALLDIR%\Extensions\Default\Actuators\VisionActuator
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\VisionActuator.dll %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\VisionUtils.dll %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\*.exe %TARGETDIR%
if exist .\%SOURCEDIR%\External goto CopyVisionExternal
echo *** ERROR *** Could not find External dependencies for the Vision Actuator (.\%SOURCEDIR%\External).
goto Next

:CopyVisionExternal
if not exist %TARGETDIR%\acat_gestures_dll.dll copy .\%SOURCEDIR%\External\*.* %TARGETDIR%
if not exist %INSTALLDIR%\shape_predictor_68_face_landmarks.dat copy .\%SOURCEDIR%\External\shape_predictor_68_face_landmarks.dat %INSTALLDIR%

:Next
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
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

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

set AGENT=DialogControlAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=FoxitReaderAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=MenuControlAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=DLLHostAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=ApplicationFrameHostAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=FireFoxAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=ChromeBrowserAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=UnsupportedAppAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=InternetExplorerAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=MSWordAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=NotepadAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=WordpadAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=OutlookAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=TalkWindowAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=MediaPlayerAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=WindowsExplorerAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
echo copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=CalculatorAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\AppAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\AppAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
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
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=NewFileAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=AbbreviationsAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=PhraseSpeakAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=VolumeSettingsAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=SwitchWindowsAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=LaunchAppAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

set AGENT=LectureManagerAgent
set EXTENSIONSBASE=Extensions\Default
set SOURCEDIR=%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
set TARGETDIR=%INSTALLDIR%\%EXTENSIONSBASE%\FunctionalAgents\%AGENT%
if not exist %TARGETDIR% mkdir %TARGETDIR%
@echo Copying .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll to %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\%AGENT%.dll %TARGETDIR%\%AGENT%.dll
if exist %SOURCEDIR%\*.xml copy %SOURCEDIR%\*.xml %TARGETDIR%
if exist %SOURCEDIR%\Config\*.xml copy %SOURCEDIR%\Config\*.xml %TARGETDIR%

rem ------------------------------------------------
@echo Deploying ACAT WordPredictor dlls
rem ------------------------------------------------

set LANGUAGE=en
set EXTENSIONBASE=Extensions\Default\WordPredictors
set SOURCEDIR=%EXTENSIONBASE%\%LANGUAGE%\Presage
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%\%EXTENSIONBASE%\Presage
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%

set SOURCEDIR=Extensions\Default\WordPredictors\Presage
set TARGETDIR=%INSTALLDIR%\%SOURCEDIR%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\bin\%CONFIG%\*.dll %TARGETDIR%

rem ------------------------------------------------
@echo Deploying Localization Resources
rem ------------------------------------------------

set LANGUAGE=en
set SOURCEDIR=ACATResources\bin\%CONFIG%\%LANGUAGE%
set TARGETDIR=%INSTALLDIR%\%LANGUAGE%
if not exist %TARGETDIR% mkdir %TARGETDIR%
copy .\%SOURCEDIR%\*.* %TARGETDIR%
set PRESAGEDIR=WordPredictors\Presage
if not exist %TARGETDIR%\%PRESAGEDIR% mkdir %TARGETDIR%\%PRESAGEDIR%
if not exist %TARGETDIR%\%PRESAGEDIR%\database.db copy .\%SOURCEDIR%\%PRESAGEDIR%\database.db %TARGETDIR%\%PRESAGEDIR%
if not exist %TARGETDIR%\%PRESAGEDIR%\15k_words_truecase.txt copy .\%SOURCEDIR%\%PRESAGEDIR%\15k_words_truecase.txt %TARGETDIR%\%PRESAGEDIR%
if not exist %TARGETDIR%\%PRESAGEDIR%\all_text_tokenized_truecase.lm copy .\%SOURCEDIR%\%PRESAGEDIR%\all_text_tokenized_truecase.lm %TARGETDIR%\%PRESAGEDIR%

