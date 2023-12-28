#Generator created by Joshua Hunter, Intel Labs Intern

# This is a NSIS script generator. How it works: 
# -There is a ton of hard coded stuff specially created for the ACAT Installer.
# - There are plenty of dynamic elements that are added here that will help keep ACAT up to date no matter what version changes happen in the future.
# - If you need to change any core elements within this script, please either follow the flow of the script and know what you are doing, or run the hardCodeGen.py script to generate new hard coded elements and then paste them in.
# - script generated from hardCodeGen.py will be placed in _installGenerator.txt and then you can copy and paste it into this script.
# - the output of this script will be placed in installer.nsi

########################################################################

# imports
import os
import sys

if len(sys.argv) <= 1 :
    print("ERROR: installGenerator.py requires argument <ACAT Install Files Folder Location>")
    sys.exit(1)
elif len(sys.argv) > 2 :
    print("ERROR: installGenerator.py requires only a single argument -> <ACAT Install Files Folder Location>")
    sys.exit(1)


# Installer Definitions

InstallerCaption = "Assistive Context-Aware Toolkit (ACAT)" # Capton Title

appName = "ACAT" # This is the name of the main application. We use this for registry keys and other things.

appVersion = "" # This is the version of the application. We use this for registry keys and other things.

appPublisherName = "Intel Corporation" # This is the publisher name. We use this for registry keys and other things.

appWebsiteLink = "https://www.intel.com/content/www/us/en/developer/tools/open/acat/overview.html" # This is the website link. We use this for registry keys and other things.

# installLocation = "C:\Intel\ACAT" 
installLocation = "$PROGRAMFILES\\ACAT" # This should be what the default install location is:"$PROGRAMFILES\\" + appName | "$PROGRAMFILES\\ACAT"

#List of shortcuts to be made within the program
additionalShortcuts = ["ACATTalk", "ACATConfig"]

# The icons for the installer can be changed by replacing the current ones in the icons folder. Make sure to keep the same name and file type

# TODO:


########################################################################

#Functions

# This function is used to get the size of the application

appFileSize = os.path.getsize("C:")

if len(sys.argv) > 1:
    directory = sys.argv[1]
    total_size_bytes = 0

    # Use os.walk to traverse the file tree
    for dirpath, dirnames, filenames in os.walk(directory):
        for f in filenames:
            fp = os.path.join(dirpath, f)
            # Skip if it is symbolic link
            if not os.path.islink(fp):
                total_size_bytes += os.path.getsize(fp)
    
    appFileSize = total_size_bytes / (1024 ** 2)  # Convert bytes to gigabytes
    
    # floor the size to no decimal places
    appFileSize = int(appFileSize)
    print(f"The total size of directory {directory} and all its subdirectories is {appFileSize} MB")


# This function is used to generate installer Section

FontUninstallList = []

def nsis_script_generator(source_dir):
    with open('NSIS_InstallerScript.nsi', 'a') as f:
        for root, dirs, files in os.walk(source_dir):
            # Write SetOutPath command
            nsis_path = root.replace(source_dir, '$INSTDIR', 1)
            f.write(f'  SetOutPath "{nsis_path}"\n')

            # Write File commands
            for file in files:
                full_path = os.path.join(root, file)
                f.write(f'  File "{full_path}"\n')

                # If file is a font file, add a call to FontInstall and WriteRegStr to add to the Windows Registry
                if file.lower().endswith(('.ttf', '.otf', '.ttc')):
                    nsis_font_path = full_path.replace(source_dir, '$INSTDIR', 1)
                    # f.write(f'  Sleep 500\n')
                    f.write(f'  Push "{nsis_font_path}"\n')
                    # f.write(f'  Sleep 500\n')
                    f.write(f'  Call FontInstall\n')
                    # f.write(f'  Sleep 500\n')

                    # Write to the registry
                    f.write(f'  WriteRegStr HKLM "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts" "{file}" "{nsis_font_path}"\n')
                    FontUninstallList.append(f'  DeleteRegValue HKLM "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts" "{file}"\n')
    f.close()



# Main Body


with open('NSIS_InstallerScript.nsi', 'w') as f:
    f.write(f'; Script influenced by the HM NIS Edit Script Wizard.\n')
    f.write(f'; Custom script by Joshua Hunter, Intel Labs Intern\n')
    f.write(f'\n')
    f.write(f'; !IMPORTANT TO NOTE: MOST MESSAGE BOXES SHOULD NOT RUN WHEN TRANSFERRED TO PRODUCTION READY INSTALLER. PLEASE ONLY KEEP THEM FOR DEBUGGING.\n')
    f.write(f'; Make sure to comment out the Message boxes that are obviously for debugging, but keep all of the ones that should pop up on error. I will mostly likely do this but if I don\'t get to it whoever is looking at this must make sure to do that before we push to open source.\n')
    f.write(f'\n')
    f.write(f';Executes installer at an Admin level\n')
    f.write(f'RequestExecutionLevel admin\n')
    f.write(f'\n')
    f.write(f'ShowInstDetails show\n')
    f.write(f'\n')

    #Installer Variables

    f.write(f'; Application Definitions\n')
    f.write(f'!define PRODUCT_NAME \"{appName}\"\n')
    f.write(f'!define PRODUCT_VERSION \"{appVersion}\"\n')
    f.write(f'!define PRODUCT_PUBLISHER \"{appPublisherName}\"\n')
    f.write(f'!define PRODUCT_WEB_SITE \"{appWebsiteLink}\"\n')
    f.write(f'!define PRODUCT_DIR_REGKEY \"Software\\Microsoft\\Windows\\CurrentVersion\\App Paths\\{appName}.exe\"\n')
    f.write(f'!define PRODUCT_UNINST_KEY \"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\${{PRODUCT_NAME}}\"\n')
    f.write(f'!define PRODUCT_UNINST_ROOT_KEY \"HKLM\"\n')

    # Compression Type

    f.write(f'\n')
    f.write(f'SetCompressor lzma\n')

    # Plugins

    f.write(f'\n')
    f.write(f'; Plugins\n')
    f.write(f'\n')
    f.write(f'  !include LogicLib.nsh\n')
    f.write(f'\n')
    f.write(f'; Plugins End --------\n')

    # MUI Settings

    f.write(f'\n')
    f.write(f'; MUI 1.67 compatible ------\n')
    f.write(f'!include MUI.nsh\n')
    f.write(f'\n')
    f.write(f'; THESE ARE THE MUI FILES. THIS IS A NATIVE-PLUGIN! This is separate from native NSIS\n')
    f.write(f'; MUI Settings\n')
    f.write(f'!define MUI_ABORTWARNING\n')
    f.write(f'\n')
    f.write(f';INTEL ICONS LOCATION NEEDS TO BE CHANGED WHEN ADDING TO DEVKIT!\n')
    f.write(f'!define MUI_ICON \".\\installer_icons\\intel.ico\"\n')
    f.write(f'!define MUI_UNICON \".\\installer_icons\\intel.ico\"\n')
    f.write(f'\n')
    f.write(f'; Welcome page\n')
    f.write(f'!define MUI_PAGE_CUSTOMFUNCTION_PRE  initMain\n')
    f.write(f'!insertmacro MUI_PAGE_WELCOME\n')
    f.write(f'; License page\n')
    f.write(f';!define MUI_PAGE_CUSTOMFUNCTION_PRE\n')
    f.write(f';!insertmacro MUI_PAGE_LICENSE \"{sys.argv[1]}\\License.txt\"\n')
    f.write(f'; Instfiles page\n')
    f.write(f';!define MUI_PAGE_CUSTOMFUNCTION_PRE InstallationChecks\n')
    f.write(f'!insertmacro MUI_PAGE_INSTFILES\n')
    f.write(f'; Finish page\n')
    f.write(f'!define MUI_FINISHPAGE_RUN \"$INSTDIR\\ACATTalk.exe\"\n')
    f.write(f'!insertmacro MUI_PAGE_FINISH\n')
    f.write(f'\n')
    f.write(f'; Uninstaller pages\n')
    f.write(f'!insertmacro MUI_UNPAGE_INSTFILES\n')
    f.write(f'\n')
    f.write(f'; Language files\n')
    f.write(f'!insertmacro MUI_LANGUAGE \"English\"\n')
    f.write(f'\n')
    f.write(f'; Reserve files\n')
    f.write(f'!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS\n')
    f.write(f'; MUI end ----------------\n')
    f.write(f'\n')

    #NSIS Specific Definitions

    f.write(f'\n')
    f.write(f'; NSIS Definitions\n')
    f.write(f'Caption "{InstallerCaption}"\n')
    f.write(f'Name \"${{PRODUCT_NAME}} ${{PRODUCT_VERSION}}\"\n')
    f.write(f'OutFile \"ACATSetup.exe\"\n')
    f.write(f'InstallDir \"{installLocation}\"\n')
    f.write(f'InstallDirRegKey HKLM \"${{PRODUCT_DIR_REGKEY}}\" \"\"\n')
    f.write(f'ShowInstDetails show\n')
    f.write(f'ShowUnInstDetails show\n')
    f.write(f'\n')
    f.write(f';This removes the NSoft branding at the bottom of the installer. You can replace it with Intel if needed\n')
    f.write(f'BrandingText \" \"\n')
    f.write(f'\n')
    f.write(f';NSIS Definitions End -\n')

    ### Start of the Function Definitions ----------

    f.write(f'\n')
    f.write(f';Functions\n')
    f.write(f'; Installer Functions +++\n')
    f.write(f'\n')

    # Installs Necessary .bat Files

    f.write(f'Function InstallBatFiles\n')
    f.write(f'  SetOutPath $PLUGINSDIR\n')
    f.write(f'  File /oname=diskSpace.bat \".\\bats\\diskSpace.bat\"\n')
    f.write(f'  File /oname=dotnet.exe \".\\ndp481-web.exe\"\n')
    f.write(f'\n')
    f.write(f'FunctionEnd\n')

    # Installation Checks Function

    f.write(f'Function InstallationChecks\n')
    f.write(f'\n')

    # ACAT portion of the install check
    f.write(f'  ${{If}} ${{FileExists}} \"$INSTDIR\"\n')
    f.write(f'    ;MessageBox MB_OK "ACAT detected, moving forward"\n')
    f.write(f'    goto detected\n')
    f.write(f'  ${{Else}}\n')
    f.write(f'    ;MessageBox MB_OK "You should see this if ACAT is uninstalled and you rebooted."\n')
    f.write(f'    goto undetected\n')
    f.write(f'  ${{EndIf}}\n')
    f.write(f'\n')
    f.write(f'  detected:\n')
    f.write(f'  ;MessageBox MB_OK "Triggering detected code block"\n')
    f.write(f'\n')
    f.write(f'  ReadRegStr $9 HKLM \"${{PRODUCT_DIR_REGKEY}}\" \"\"\n')
    f.write(f'\n')
    f.write(f'  ;MessageBox MB_OK "Registry: $9"\n')
    f.write(f'\n')
    f.write(f'  ${{If}} $9 != \"\"\n')
    f.write(f'    MessageBox MB_YESNO \"ACAT is already installed, would you like to uninstall? If not, please uninstall ACAT manually and run the installer again\" IDYES initiate IDNO quitout\n')
    f.write(f'    initiate:\n')
    f.write(f'    ;MessageBox MB_OK "Initiating uninstall..."\n')
    f.write(f'    ;do the uninstall stuff\n')
    f.write(f'    ExecWait \'\"$INSTDIR\\uninst.exe\"\'\n')
    f.write(f'    ; quit due to needing to reboot\n')
    f.write(f'    Quit\n')
    f.write(f'\n')
    f.write(f'    quitout:\n')
    f.write(f'    ;MessageBox MB_OK "Quitting install..."\n')
    f.write(f'    Quit\n')
    f.write(f'\n')
    f.write(f'\n')
    f.write(f'  ${{ElseIf}} \"$9\" == \"\"\n')
    f.write(f'    MessageBox MB_OK \"You MUST REBOOT to finish uninstalling ACAT. If this problem persists after rebooting, please delete ACAT file directory located at $INSTDIR\"\n')
    f.write(f'    Quit\n')
    f.write(f'  ${{EndIf}}\n')
    f.write(f'\n')
    f.write(f'\n')
    f.write(f'  undetected:')

    # .NET portion of the installation check

    f.write(f'  ;MessageBox MB_OK "Initializing ACAT and .NET checks..."  ;COMMENT OUT ON PROD!\n')
    f.write(f'  ;MessageBox MB_OK "This should be the productRegKey: ${{PRODUCT_DIR_REGKEY}}"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'  ;DWORD Registry search for .NET Frameword 4.8 or later\n')
    f.write(f'  ReadRegDWORD $0 HKLM \"SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\" \"Release\"\n')
    f.write(f'  ${{If}} $0 >= 0x82348 ;0x80FC ; 528040 in hexadecimal!!! This was for 4.8, we are using 4.8.1 we should be using the value 0x82348 which is 533320 in hex This is the release that .NET uses to verify if V4.8 or later is installed.\n')
    f.write(f'    ;MessageBox MB_OK ".NET Framework 4.8 or higher is installed."  ;COMMENT OUT ON PROD!\n')
    f.write(f'  ${{Else}}\n')
    f.write(f'\n')
    f.write(f'    loopInit:\n')
    f.write(f'    MessageBox MB_YESNO \".NET Framework 4.8 or higher is required. Do you want to install it?\" IDYES yes IDNO no\n')
    f.write(f'\n')
    f.write(f'    yes:\n')
    f.write(f'\n')
    f.write(f'    ;MessageBox MB_OK "Initiating Install..."\n')
    f.write(f'    ExecWait \'\"$PLUGINSDIR\\dotnet.exe\"\' $1\n')
    f.write(f'    ;MessageBox MB_OK "Install Complete. Continuing with ACAT installation..."\n')
    f.write(f'\n')
    f.write(f'    ${{If}} $1 == 0\n')
    f.write(f'      MessageBox MB_OK \"Installation of .NET Framework 4.8 or higher was successful.\"\n')
    f.write(f'      goto Installed\n')
    f.write(f'    ${{Else}}\n')
    f.write(f'      MessageBox MB_OK \"Installation of .NET Framework 4.8 or higher failed. Please again or try manually installing.\"\n')
    f.write(f'      goto loopInit\n')
    f.write(f'    ${{Endif}}\n')
    f.write(f'\n')
    f.write(f'    no:\n')
    f.write(f'    MessageBox MB_OK \"This software is required to run ACAT. Please install it before proceeding by going to this website: https://dotnet.microsoft.com/en-us/download/dotnet-framework/net481\"  ;COMMENT OUT ON PROD!\n')
    f.write(f'    Quit\n')
    f.write(f'\n')
    f.write(f'  ${{EndIf}}\n')
    f.write(f'\n')
    f.write(f'  Installed:\n')
    f.write(f'\n')
    f.write(f'FunctionEnd\n')

    # Check Space Function

    f.write(f'\n')
    f.write(f'Function CheckSpace\n')
    f.write(f'  ; Size of the program to be installed (in MB)\n')
    f.write(f'  Var /GLOBAL RequiredSize\n')
    f.write(f'  StrCpy $RequiredSize {appFileSize} ; This variable should be dynamically included when you generate using the Python script\n')
    f.write(f'  ;MessageBox MB_OK "Size of the program in Megabytes: $RequiredSize"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'  ; Check the free disk space using the .Bat\n')
    f.write(f'  ; IMPORTANT! IF ACAT INTEL EVER BECOMES LARGE ENOUGH OR CLOSE TO BEING BIG ENOUGH TO AMOUNT TO A 64BIT INTEGER IN BYTES, THEN THIS NEEDS TO BE REWORKED!!!\n')
    f.write(f'  ; Issue: We cannot compare a 64 bit integer with a 32 bit integer. So we will assume that if the free space on the disk is near the maximum of a 32bit integer, that there is indeed enough room on the disk.\n')
    f.write(f'  ; If the disk space is a 32 bit integer, we proceed with the comparison.\n')
    f.write(f'\n')
    f.write(f'  ; Get the current install directory and only take the first 2 characters')
    f.write(f'  StrCpy $0 $INSTDIR 2\n')
    f.write(f'  ;MessageBox MB_OK "directory: $0"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'  ;Find Plugindir for debugging purposes\n')
    f.write(f'  ;MessageBox MB_OK "About to run the .bat. Make sure it is installed correctly within: $PLUGINSDIR"\n')
    f.write(f'\n')
    f.write(f'  ExecWait \'\"$PLUGINSDIR\\diskSpace.bat\" $0 $RequiredSize\' $R9\n')
    f.write(f'  Sleep 500\n')
    f.write(f'  ;MessageBox MB_OK "Exit code: $R9"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'  StrCmp $R9 0 LargerThanThirtyTwoBitOperation\n')
    f.write(f'  StrCmp $R9 1 ThirtyTwoBitOperation\n')
    f.write(f'  StrCmp $R9 2 Error\n')
    f.write(f'  goto End\n')
    f.write(f'\n')
    f.write(f'  LargerThanThirtyTwoBitOperation:\n')
    f.write(f'  ;MessageBox MB_OK "System has more than enough storage space to install program. Forgoing calculations and proceeding with installation"  ;COMMENT OUT ON PROD!\n')
    f.write(f'  goto End\n')
    f.write(f'\n')
    f.write(f'  ThirtyTwoBitOperation:\n')
    f.write(f'  ;creating a mock storage value for debugging purposes.\n')
    f.write(f'  ;Var /GLOBAL MockStorageSpace\n')
    f.write(f'  ;StrCpy $MockStorageSpace 304593452 ;Mock storage size in bytes for debugging\n')
    f.write(f'  ;MessageBox MB_OK "Mock StorageSpace $MockStorageSpace"\n')
    f.write(f'\n')
    f.write(f'  ; Convert the program size to bytes (1 MB = 1048576 bytes)\n')
    f.write(f'  IntOp $RequiredSize $RequiredSize * 1048576\n')
    f.write(f'  ;MessageBox MB_OK "RequiredSize: $RequiredSize" ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'  ;Looking within whatever the drive that has the program files is named, and checking the space in btyes. Normally it is C: but since some people name their main drives other things this keeps it dynamic.\n')
    f.write(f'  System::Call \'KERNEL32::GetDiskFreeSpaceEx(t \"$0\", *l .R1, *l, *l)i.r0\'\n')
    f.write(f'  ;MessageBox MB_OK "Free space: $R1"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'  ;This should be self explanitory, if you need to debug this then uncomment all of the $MockSotrageSpace variables\n')
    f.write(f'  ;IntOp $R8 $MockStorageSpace - $RequiredSize\n')
    f.write(f'\n')
    f.write(f'  IntOp $R8 $R1 - $RequiredSize\n')
    f.write(f'\n')
    f.write(f'  ;MessageBox MB_OK "Space free after install: $R8"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'  ;The following is a workaround for a bug in the free space function which returns 0 if the disk space is large (say, 250 GB)\n')
    f.write(f'  StrCpy $R8 1255\n')
    f.write(f'  ${{If}} $R8 > 0\n')
    f.write(f'    ;MessageBox MB_OK "$R8 is a positive number."\n')
    f.write(f'    ;MessageBox MB_OK "There is enough space to install ACAT. Proceeding with the install" ;COMMENT OUT ON PROD!\n')
    f.write(f'    goto End\n')
    f.write(f'  ${{ElseIf}} $R8 < 0\n')
    f.write(f'    ;MessageBox MB_OK "$R8 is a negative number."\n')
    f.write(f'    MessageBox MB_OK \"ACAT cannot be installed. Please free some room on your disk drive\"\n')
    f.write(f'    Quit\n')
    f.write(f'  ${{Else}}\n')
    f.write(f'     MessageBox MB_OK \"It is not reccomended to install ACAT while you have exactly enough room to install. Please free some room on your disk drive\"\n')
    f.write(f'     Quit\n')
    f.write(f'  ${{EndIf}}\n')
    f.write(f'  goto End\n')
    f.write(f'\n')
    f.write(f'\n')
    f.write(f'  ; If all else fails, give the user a warning that we cannot guarentee that there is enough space to install.\n')
    f.write(f'  Error:\n')
    f.write(f'  MessageBox MB_ICONEXCLAMATION \"ERROR! Disk Space calculation encountered an error. We cannot gaurentee that you have enough disk space to install this program. Please manually check if your system has room for $RequiredSize MB\"\n')
    f.write(f'  MessageBox MB_YESNO \"Proceed with the install?\" IDYES End\n')
    f.write(f'  Quit\n')
    f.write(f'\n')
    f.write(f'  End:\n')
    f.write(f'FunctionEnd\n')
    f.write(f'\n')

    #Font Installer Function

    f.write(f'Function FontInstall\n')
    f.write(f'    ; The stack contains the arguments passed to the function\n')
    f.write(f'    Pop $1 ; font file path\n')
    f.write(f'\n')
    f.write(f'    ; Check if the file exists\n')
    f.write(f'    IfFileExists $1 +3\n')
    f.write(f'        MessageBox MB_OK \"Error: Font file $1 does not exist\"\n')
    f.write(f'        Return\n')
    f.write(f'\n')
    f.write(f'    ; Add the font resource\n')
    f.write(f'    System::Call \'gdi32::AddFontResource(t r1) i .r0\'\n')
    f.write(f'    ; Check if the function succeeded\n')
    f.write(f'    IntCmp $0 0 showError\n')
    f.write(f'\n')
    f.write(f'    ; Update the system about the font change\n')
    f.write(f'    SendMessage ${{HWND_BROADCAST}} ${{WM_FONTCHANGE}} 0 0 /TIMEOUT=5000\n')
    f.write(f'\n')
    f.write(f'    Return\n')
    f.write(f'\n')
    f.write(f'    showError:\n')
    f.write(f'        MessageBox MB_OK \"Error: Failed to add font resource $1\"\n')
    f.write(f'    Return\n')
    f.write(f'FunctionEnd\n')

    #Multi Function Call

    f.write(f'\n')
    f.write(f';Allows us to call multiple functions at the start\n')
    f.write(f'Function initMain\n')
    f.write(f'  call InstallBatFiles\n')
    f.write(f'  call InstallationChecks\n')
    f.write(f'  call CheckSpace\n')
    f.write(f'FunctionEnd\n')
    f.write(f'\n')

    # Uninstall Functions

    f.write(f'\n')
    f.write(f'; Uninstall Functions +++\n')
    f.write(f'\n')
    f.write(f'; Runs when the uninstaller is finished\n')
    f.write(f'Function un.onUninstSuccess\n')
    f.write(f'  HideWindow\n')
    f.write(f'  ;MessageBox MB_ICONINFORMATION|MB_OK "App name is:$(^Name). Functionally works different for MessageBox Functions" ;Whatever the heck I did with this, probably remove on PROD release\n')
    f.write(f'FunctionEnd\n')
    f.write(f'\n')

    # Uninstall Init Section

    f.write(f'\n')
    f.write(f';Runs as soon as the Uninstaller initiates\n')
    f.write(f'Function un.onInit\n')
    f.write(f'\n')
    f.write(f'FunctionEnd\n')
    f.write(f'\n')
    f.write(f'; Functions End -\n')


    # Main Body Dynamic Install Section

    f.write(f'\n')
    f.write(f'; Install Sections\n')
    f.write(f'\n')
    f.write(f'; Installer Main Files\n')
    f.write(f'Section \"MainSection\" SEC01\n')
    f.write(f'  ;MessageBox MB_OK \"This is where the installation of files would happen\"  ;COMMENT OUT ON PROD!\n')
    f.close()

    if len(sys.argv) != 2:
        print("Usage: python nsis_script_generator.py <source_directory>")
    else:
        nsis_script_generator(sys.argv[1])

with open('NSIS_InstallerScript.nsi', 'a') as f:
    f.write(f'SectionEnd\n')
    f.write(f'\n')

    f.write(f'; Icon Installer\n')
    f.write(f'Section -AdditionalIcons\n')
    f.write(f'  ;MessageBox MB_OK \"This is where we would install the icons\"  ;COMMENT OUT ON PROD!\n')
    f.write(f'  WriteIniStr \"$INSTDIR\\${{PRODUCT_NAME}}.url\" \"InternetShortcut\" \"URL\" \"${{PRODUCT_WEB_SITE}}\"\n')
    f.write(f'  CreateShortCut \"$SMPROGRAMS\\{appName}\\Website.lnk\" \"$INSTDIR\\${{PRODUCT_NAME}}.url\"\n')
    f.write(f'  CreateShortCut \"$SMPROGRAMS\\{appName}\\Uninstall.lnk\" \"$INSTDIR\\uninst.exe\"\n')
    for i, shortcut in enumerate(additionalShortcuts):
        counter = i
        f.write(f'  IfFileExists \"$INSTDIR\\{shortcut}.exe\" 0 NoShortcut{counter}\n')
        f.write(f'    SetOutPath \"$INSTDIR\"\n')
        f.write(f'    CreateShortCut \"$DESKTOP\\{shortcut}.lnk\" \"$INSTDIR\\{shortcut}.exe\"\n')
        f.write(f'  NoShortcut{counter}:\n')
    
    counter += 1
    f.write(f'  IfFileExists \"$INSTDIR\\Docs\\en\\ACAT User Guide.pdf\" 0 NoShortcut{counter}\n')
    f.write(f'    SetOutPath \"$INSTDIR\"\n')
    f.write(f'    CreateShortCut \"$DESKTOP\\ACAT User Guide.lnk\" \"$INSTDIR\\Docs\\en\\ACAT User Guide.pdf\"\n')
    f.write(f'  NoShortcut{counter}:\n')

    f.write(f'SectionEnd\n')






    #Post install Section

    f.write(f'\n')
    f.write(f'; Post process Installer\n')
    f.write(f'Section -Post\n')
    f.write(f'  ;MessageBox MB_OK "This is where we would write the apps to the registry, and create the uninstaller"  ;COMMENT OUT ON PROD!\n')
    f.write(f'  WriteUninstaller \"$INSTDIR\\uninst.exe\"\n')
    f.write(f'  WriteRegStr HKLM \"${{PRODUCT_DIR_REGKEY}}\" \"\" \"$INSTDIR\\{appName}.exe\"\n')
    f.write(f'  WriteRegStr ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"DisplayName\" \"$(^Name)\"\n')
    f.write(f'  WriteRegStr ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"UninstallString\" \"$INSTDIR\\uninst.exe\"\n')
    f.write(f'  WriteRegStr ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"DisplayIcon\" \"$INSTDIR\\{appName}.exe\"\n')
    f.write(f'  WriteRegStr ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"DisplayVersion\" \"${{PRODUCT_VERSION}}\"\n')
    f.write(f'  WriteRegStr ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"URLInfoAbout\" \"${{PRODUCT_WEB_SITE}}\"\n')
    f.write(f'  WriteRegStr ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"Publisher\" \"${{PRODUCT_PUBLISHER}}\"\n')
    f.write(f'SectionEnd\n')
    f.write(f'\n')
    f.write(f'; Install Section End -\n')
    f.write(f'\n')

    # Uninstall Section

    f.write(f'Section Uninstall\n')
    f.write(f'\n')
    f.write(f'  ExecWait \'\"$INSTDIR\\ConvAssistTerminate.exe\"\' $5\n')
    f.write(f'\n')
    f.write(f'  ${{if}} $5 == 0\n')
    f.write(f'    ;MessageBox MB_OK \"ConvAssist has been stopped\"\n')
    f.write(f'    goto done\n')
    f.write(f'\n')
    f.write(f'  ${{Elseif}} $5 == 1\n')
    f.write(f'    ;MessageBox MB_OK \"ConvAssist was not running\"\n')
    f.write(f'    goto done\n')
    f.write(f'\n')
    f.write(f'  ${{Elseif}} $5 == 2\n')
    f.write(f'    MessageBox MB_OK \"The exit application has run into an error. Please make sure to manually stop the prcoess ConvAssist.exe before uninstalling for a proper uninstall process\"\n')
    f.write(f'    goto done\n')
    f.write(f'\n')
    f.write(f'  ${{Else}}\n')
    f.write(f'    MessageBox MB_OK \"The exit application could not be run. Please make sure to manually stop the prcoess ConvAssist.exe before uninstalling for a proper uninstall process\"\n')
    f.write(f'  ${{Endif}}\n')
    f.write(f'\n')
    f.write(f'  done:\n')
    f.write(f'  ; Delete the registry entries\n')
    for font in FontUninstallList:
        f.write(font)
    f.write(f'  DeleteRegValue ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"DisplayName\"\n')
    f.write(f'  DeleteRegValue ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"UninstallString\"\n')
    f.write(f'  DeleteRegValue ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"DisplayIcon\"\n')
    f.write(f'  DeleteRegValue ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"DisplayVersion\"\n')
    f.write(f'  DeleteRegValue ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"URLInfoAbout\"\n')
    f.write(f'  DeleteRegValue ${{PRODUCT_UNINST_ROOT_KEY}} \"${{PRODUCT_UNINST_KEY}}\" \"Publisher\"\n')
    f.write(f'  DeleteRegKey HKLM \"${{PRODUCT_DIR_REGKEY}}\"\n')
    f.write(f'\n')
    f.write(f'  ; Remove all files and directories\n')
    f.write(f'\n')
    f.write(f'  RMDir /r \"$INSTDIR\"\n')
    f.write(f'  RMDir \"$INSTDIR\"\n')
    f.write(f'\n')
    f.write(f'  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 \"Do you want to remove the ACAT Users folder? This folder contains your personalized ACAT settings, phrases, and language models. If you are going to reinstall ACAT, select NO. \" IDYES yes IDNO no\n')
    f.write(f'  yes:\n')
    f.write(f'  ;MessageBox MB_OK "You have clicked YES"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'      ;Functionality goes here between the message box and the end. The Message box is only here for dev testing purposes\n')
    f.write(f'    RMDir /r \"$DOCUMENTS\\ACAT\"\n')
    f.write(f'\n')
    f.write(f'  goto end\n')
    f.write(f'  MessageBox MB_OK \"UHOH there was an error. You should NEVER see this message box\"\n')
    f.write(f'\n')
    f.write(f'  no:\n')
    f.write(f'  ;MessageBox MB_OK "You have clicked NO"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'      ;Functionality goes here between the message box and the end. The Message box is only here for dev testing purposes\n')
    f.write(f'\n')
    f.write(f'  goto end\n')
    f.write(f'  MessageBox MB_OK \"UHOH there was an error. You should NEVER see this message box\"\n')
    f.write(f'\n')
    f.write(f'  end:\n')
    f.write(f'  ;MessageBox MB_OK "You should be routed to the end now here with the correct earlier prompt"  ;COMMENT OUT ON PROD!\n')
    f.write(f'\n')
    f.write(f'\n')
    f.write(f'  ; Delete the shortcuts\n')
    for i, shortcut in enumerate(additionalShortcuts):
        counter = i
        f.write(f'  IfFileExists \"$DESKTOP\\{shortcut}.lnk\" 0 NoShortcut{counter}\n')
        f.write(f'    Delete \"$DESKTOP\\{shortcut}.lnk\"\n')
        f.write(f'  NoShortcut{counter}:\n')
    f.write(f'\n')
    counter += 1
    f.write(f'  IfFileExists \"$DESKTOP\\ACAT User Guide.lnk\" 0 NoShortcut{counter}\n')
    f.write(f'    Delete \"$DESKTOP\\ACAT User Guide.lnk\"\n')
    f.write(f'  NoShortcut{counter}:\n')
    f.write(f'  ; Read the current value of PendingFileRenameOperations\n')
    f.write(f'  ReadRegStr $0 HKLM \"SYSTEM\\CurrentControlSet\\Control\\Session Manager\" \"PendingFileRenameOperations\"\n')
    f.write(f'\n')
    f.write(f'  ; Append your files to this value. This uses a special character (`$R0`) to separate the strings in the REG_MULTI_SZ format\n')
    f.write(f'  StrCpy $0 \"$0$R0\\??\\$INSTDIR\\Assets\\Fonts\\ACATFONT1.otf$R0\"\n')
    f.write(f'  StrCpy $0 \"$0\\??\\$INSTDIR\\Assets\\Fonts\\Montserrat-VariableFont_wght.ttf$R0\"\n')
    f.write(f'  StrCpy $0 \"$0\\??\\$INSTDIR\\Assets\\Fonts\\Montserrat-Italic-VariableFont_wght.ttf$R0\"\n')
    f.write(f'\n')
    f.write(f'  ; Write the combined value back to the registry\n')
    f.write(f'  WriteRegExpandStr HKLM \"SYSTEM\\CurrentControlSet\\Control\\Session Manager\" \"PendingFileRenameOperations\" $0\n')
    f.write(f'\n')
    f.write(f'  RMDir /r /REBOOTOK \"$INSTDIR\\Assets\\Fonts\"\n')
    f.write(f'  RMDir /r /REBOOTOK \"$INSTDIR\\Assets\"\n')
    f.write(f'  RMDir /r /REBOOTOK \"$INSTDIR\"\n')
    f.write(f'  RMDir \"$INSTDIR\"\n')
    f.write(f'\n')
    f.write(f'\n')
    f.write(f'  Messagebox MB_OK \"You MUST REBOOT your computer to finish uninstall process. After rebooting, run the ACAT installer to install ACAT.\"\n')
    f.write(f'  quit\n')
    f.write(f'\n')
    f.write(f'SectionEnd\n')
