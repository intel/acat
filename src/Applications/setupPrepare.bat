set SETUPTARGETDIR=.\Setup
set SETUPCLEANDIR=.\SetupClean
set CONFIGURATION=Release
set SOURCEDIR=ACATApp\bin\%CONFIGURATION%
set XCOPYOPTIONS=/s /y /e /i
if not exist %SETUPTARGETDIR% mkdir %SETUPTARGETDIR%

@echo Copying SetupClean...
if exist %SETUPCLEANDIR% xcopy %XCOPYOPTIONS% %SETUPCLEANDIR% %SETUPTARGETDIR%
@echo Copying application files...
copy %SOURCEDIR%\*.exe %SETUPTARGETDIR%
copy %SOURCEDIR%\*.dll %SETUPTARGETDIR%
copy %SOURCEDIR%\*.bat %SETUPTARGETDIR%
copy %SOURCEDIR%\*.txt %SETUPTARGETDIR%
copy %SOURCEDIR%\*.config %SETUPTARGETDIR%
del %SETUPTARGETDIR%\*vshost*

@echo Copying Extensions...
xcopy %XCOPYOPTIONS% %SOURCEDIR%\Extensions\*.* %SETUPTARGETDIR%\Extensions

@echo Copying Assets...
xcopy %XCOPYOPTIONS% %SOURCEDIR%\Assets\*.* %SETUPTARGETDIR%\Assets

@echo Copying Install...
xcopy %XCOPYOPTIONS% %SOURCEDIR%\Install\*.* %SETUPTARGETDIR%\Install


