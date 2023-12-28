@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

:: Size of your program in MB
SET ProgramSize=%2
SET Drive=%1

:: Compute the free disk space in MB
FOR /F "tokens=1,3" %%B IN ('DIR /-C /S "%Drive%"') DO (
	IF %%B NEQ 0 SET FreeSpace=%%C
)
IF !FreeSpace! GTR 0 SET FreeSpace=!FreeSpace:~0,-6!
IF "!FreeSpace!"=="" SET FreeSpace=0
ECHO Free space on %Drive% !FreeSpace! MB

:: Compare the free space with the program size and the max 32-bit integer size
IF !FreeSpace! LSS 2048 (
	ECHO Disk space is smaller than the max size of a 32-bit integer. Calculating more accurately within the installer.
	EXIT /B 1
) ELSE IF !FreeSpace! GEQ !ProgramSize! (
	ECHO Disk space is larger than a 32bit integer in bytes, and so we can assume that there is enough space to run.
	EXIT /B 0
) ELSE (
	ECHO This realistically should NEVER be hit. Unless of course there is an error or ACAT somehow becomes a large program size.
	EXIT /B 2
)