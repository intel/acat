-------------------------------------------------------
                  ACAT Installer
-------------------------------------------------------

Generates the installer for ACAT. Uses NSIS installer to generate ACATSetup.exe.

REQUIREMENTS
------------

1. Install Python: https://www.python.org/ (*note:* any python version 3+ should work)

2. Install NSIS: https://nsis.sourceforge.io/Download

3. Install a .nsi editor. Recommended editor is HM Edit: https://hmne.sourceforge.net/

HOW IT WORKS
------------

Run the python script installGenerator.py with a command line argument which points to the folder where the ACAT binaries are located 
when you build ACAT with Visual Studio.

For example:

$ python installGenerator.py C:\Github2\acat\src\Applications\ACATApp\bin\Release

This will generate the NSIS install script NSIS_InstallerScript.nsi.

Open this script file using either the NSIS compiler from Step 2 above, or the NSIS editor from Step 3 above.

Compile the script.

This will create the ACAT installer ACATSetup.exe



