@echo off

set SOURCEFILE=%1
set TARGETDIR=%2\Redistributable

if not exist %TARGETDIR% mkdir %TARGETDIR%

if exist %SOURCEFILE% copy /y %SOURCEFILE% %TARGETDIR%

