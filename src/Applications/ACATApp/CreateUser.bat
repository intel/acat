rem @echo off
if not exist %WINDIR%\system32\xcopy.exe goto checkWow64
set XCOPYEXE=%WINDIR%\system32\xcopy.exe
goto next
:checkWow64
if not exist %WINDIR%\SYSWow64\xcopy.exe goto error
set XCOPYEXE=%WINDIR%\SYSWow64\xcopy.exe
goto next
:error
echo "*** ERROR *** Xcopy not found on your machine
pause
exit 2
:next
if %1a==a goto default
set ACATUSER=%1
goto create
:default
set ACATUSER=ACAT
:create
if not exist Users mkdir Users
if not exist Users\%ACATUSER% mkdir Users\%ACATUSER%
%XCOPYEXE% /s /i /e /y install\Users\ACAT Users\%ACATUSER%
exit 0
