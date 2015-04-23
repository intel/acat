if %1a==a goto default
set ACATUSER=%1
goto create
:default
set ACATUSER=Default
:create
if not exist Users mkdir Users
if not exist Users\%ACATUSER% mkdir Users\%ACATUSER%
xcopy /s /i /e /y install\Users\Default Users\%ACATUSER%
