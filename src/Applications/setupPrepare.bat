set SETUPTARGETDIR=.\SetupFiles
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
copy %SOURCEDIR%\DashboardSettings.xml %SETUPTARGETDIR%
copy %SOURCEDIR%\*.dat %SETUPTARGETDIR%
del %SETUPTARGETDIR%\*vshost*

@echo Copying Extensions...
xcopy %XCOPYOPTIONS% %SOURCEDIR%\Extensions\*.* %SETUPTARGETDIR%\Extensions

@echo Copying Assets...
xcopy %XCOPYOPTIONS% %SOURCEDIR%\Assets\*.* %SETUPTARGETDIR%\Assets

@echo Copying Install...
xcopy %XCOPYOPTIONS% %SOURCEDIR%\Install\*.* %SETUPTARGETDIR%\Install

@echo Copying English language localization resources
set LANGUAGE=en
xcopy %XCOPYOPTIONS% %SOURCEDIR%\%LANGUAGE%\*.* %SETUPTARGETDIR%\%LANGUAGE%

@echo Copying French language localization resources
set LANGUAGE=fr
xcopy %XCOPYOPTIONS% %SOURCEDIR%\%LANGUAGE%\*.* %SETUPTARGETDIR%\%LANGUAGE%

@echo Copying Spanish language localization resources
set LANGUAGE=es
xcopy %XCOPYOPTIONS% %SOURCEDIR%\%LANGUAGE%\*.* %SETUPTARGETDIR%\%LANGUAGE%

@echo Copying Portuguese language localization resources
set LANGUAGE=pt
xcopy %XCOPYOPTIONS% %SOURCEDIR%\%LANGUAGE%\*.* %SETUPTARGETDIR%\%LANGUAGE%
