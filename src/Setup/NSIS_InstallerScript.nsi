!include MUI2.nsh
!include LogicLib.nsh  ; Needed for ${If}, ${DoWhile}, etc.
!include "FileFunc.nsh"
!include nsDialogs.nsh
!insertmacro GetSize

RequestExecutionLevel admin ;Needed to install fonts

!define MUI_ICON ".\installer_icons\intel.ico"
!define MUI_UNICON ".\installer_icons\intel.ico"
!define NAME "ACAT"
!define LONG_NAME "Assistive Context-Aware Toolkit (ACAT)"
!define PUBLISHER "Intel Corporation"
!define COPYRIGHT "©2025 Intel Corporation"
!define VERSION "4.0.0.082225"
!define SETUPNAME "SetupACAT\SetupACAT64.exe"
!define ASSETS_FOLDER "Assets"
!define MUI_WARN_UNUSED_VARIABLES "false"

; Section: Installer Info
Name ${NAME}
OutFile ${SETUPNAME}
InstallDir "$PROGRAMFILES64\ACAT"
RequestExecutionLevel admin

Var CreateDesktopShortcuts
Var CreateStartMenu

VIProductVersion "${VERSION}"     ; must be 4 numbers
VIFileVersion    "${VERSION}"     ; optional, defaults to ProductVersion

VIAddVersionKey "ProductName"     "${LONG_NAME}"
VIAddVersionKey "FileDescription" "${Name} Installer"
VIAddVersionKey "CompanyName"     "${PUBLISHER}"
VIAddVersionKey "LegalCopyright"  "${COPYRIGHT}"
VIAddVersionKey "FileVersion"     "${VERSION}"
VIAddVersionKey "ProductVersion"  "${VERSION}"
VIAddVersionKey "OriginalFilename" "SetupACAT64.exe"

SetCompress auto
SetOverwrite ifnewer

LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"

LangString MUI_UNTEXT_UNINSTALLING_TITLE ${LANG_ENGLISH} "TODO"
LangString MUI_UNTEXT_UNINSTALLING_SUBTITLE ${LANG_ENGLISH} "TODO"
LangString MUI_UNTEXT_FINISH_TITLE ${LANG_ENGLISH} "TODO"
LangString MUI_UNTEXT_FINISH_SUBTITLE ${LANG_ENGLISH} "TODO"
LangString MUI_UNTEXT_CONFIRM_TITLE ${LANG_ENGLISH} "TODO"
LangString MUI_UNTEXT_CONFIRM_SUBTITLE ${LANG_ENGLISH} "TODO"
LangString MUI_UNTEXT_ABORT_TITLE ${LANG_ENGLISH} "TODO"
LangString MUI_UNTEXT_ABORT_SUBTITLE ${LANG_ENGLISH} "TODO"
LangString MUI_TEXT_WELCOME_INFO_TITLE ${LANG_ENGLISH} "Welcome to the ACAT Installer"
LangString MUI_TEXT_WELCOME_INFO_TEXT ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_WELCOME_INFO ${LANG_ENGLISH} "This installer will guide you through the installation of ACAT."
LangString MUI_TEXT_LICENSE_TITLE ${LANG_ENGLISH} "License Agreement"
LangString MUI_TEXT_LICENSE_SUBTITLE ${LANG_ENGLISH} "Please read the following terms before continuing."
LangString MUI_TEXT_INSTALLING_TITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_INSTALLING_SUBTITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_FINISH_TITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_FINISH_SUBTITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_FINISH_REBOOTNOW ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_FINISH_REBOOTLATER ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_FINISH_INFO_TITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_FINISH_INFO_TEXT ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_FINISH_INFO_REBOOT ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_DIRECTORY_TITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_DIRECTORY_SUBTITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_ABORT_TITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_TEXT_ABORT_SUBTITLE ${LANG_ENGLISH}  "TODO"
LangString MUI_INNERTEXT_LICENSE_TOP ${LANG_ENGLISH}  "TODO"
LangString MUI_INNERTEXT_LICENSE_BOTTOM ${LANG_ENGLISH}  "TODO"
LangString MUI_BUTTONTEXT_FINISH ${LANG_ENGLISH}  "TODO"

; !insertmacro MUI_LANGUAGE "English"

; Section: Pages (Modern UI)
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "license.txt"
!insertmacro MUI_PAGE_DIRECTORY

Page custom ShowShortcutOptions

!insertmacro MUI_PAGE_INSTFILES
; !insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Section: Main Installation
Section "Install"

  ; Ensure registry writes to 64-bit view
  SetRegView 64


  ; Install main application files from build output (compressed)
  SetOutPath "$INSTDIR"

  !define BUILD_OUTPUT "..\build\bin\Debug\"
  File /r /x Assets "${BUILD_OUTPUT}\*.*"

  SetOutPath "$INSTDIR\ConvAssistApp\_internal\Assets"
  File /r "${BUILD_OUTPUT}\ConvAssistApp\_internal\Assets\*.*"

  SetOutPath "$INSTDIR\Assets\Fonts"
  File /r "${BUILD_OUTPUT}\Assets\Fonts\*.*"

  SetOutPath "$INSTDIR\Assets\Images"
  File /r "${BUILD_OUTPUT}\Assets\Images\*.*"

  SetOutPath "$INSTDIR\Assets\Sounds"
  File /r "${BUILD_OUTPUT}\Assets\Sounds\*.*"

  SetOutPath "$INSTDIR\Assets\Themes"
  File /r "${BUILD_OUTPUT}\Assets\Themes\*.*"

  ; Install fonts
  Push "$INSTDIR\Assets\Fonts\ACAT Icon.ttf"
  Call InstallSystemFont
  
  Push "$INSTDIR\Assets\Fonts\ACATFONT1.otf"
  Call InstallSystemFont

  Push "$INSTDIR\Assets\Fonts\bootstrap-icons.otf"
  Call InstallSystemFont

  Push "$INSTDIR\Assets\Fonts\Montserrat-Italic-VariableFont_wght.ttf"
  Call InstallSystemFont

  Push "$INSTDIR\Assets\Fonts\Montserrat-VariableFont_wght.ttf"
  Call InstallSystemFont

  ;--------------------------------
  ; Create Start Menu entries
  ;--------------------------------
  ${NSD_GetState} $CreateStartMenu $0
  ${If} $0 == 1
    CreateDirectory "$SMPROGRAMS\ACAT"
    CreateShortcut  "$SMPROGRAMS\ACAT\ACAT Dashboard.lnk" "$INSTDIR\ACATApp.exe"
    CreateShortcut  "$SMPROGRAMS\ACAT\ACAT Talk.lnk" "$INSTDIR\ACATTalk.exe"
    CreateShortcut  "$SMPROGRAMS\ACAT\ACAT Config.lnk" "$INSTDIR\ACATConfig.exe"
    CreateShortcut  "$SMPROGRAMS\ACAT\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
  ${EndIf}

  ;--------------------------------
  ; Create Desktop shortcuts
  ;--------------------------------
  ${NSD_GetState} $CreateDesktopShortcuts $0
  ${If} $0 == 1
    CreateShortcut  "$DESKTOP\ACAT Dashboard.lnk" "$INSTDIR\ACATApp.exe"
    CreateShortcut  "$DESKTOP\ACAT Talk.lnk" "$INSTDIR\ACATTalk.exe"
    CreateShortcut  "$DESKTOP\ACAT Config.lnk" "$INSTDIR\ACATConfig.exe"
  ${EndIf}


;   ;--------------------------------
;   ; Write uninstaller
;   ;--------------------------------
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  StrCpy $R0 $0

  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "DisplayName" "${LONG_NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "DisplayVersion" "${VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "Publisher" "${PUBLISHER}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "DisplayIcon" "$INSTDIR\ACATApp.exe"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${Name}" "EstimatedSize" $R0

  ; Optional: prevent Modify/Repair buttons
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" "NoRepair" 1
SectionEnd

;--------------------------------
; Section: Uninstall
;--------------------------------
Section "Uninstall"
  SetRegView 64

  ; Remove installed files
  RMDir /r "$INSTDIR"

  ; Remove Start Menu entries
  RMDir /r "$SMPROGRAMS\ACAT"

  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}"
SectionEnd

;--------------------------------
; Function: InstallFont
; Expects: font file name (relative path or just name) on stack
Function InstallSystemFont
    Exch $0 ; pop font path

    ; Extract file name
    ${GetFileName} "$0" $1

    ; Determine type (TrueType vs OpenType)
    ${GetFileExt} "$0" $2
    StrCpy $3 "TrueType"
    ${If} $2 == "otf"
        StrCpy $3 "OpenType"
    ${EndIf}

    ; Check registry to see if font is already installed
    ClearErrors
    ReadRegStr $4 HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" "$1 ($3)"
    ${If} ${Errors}
        ; Font not installed, proceed
        StrCpy $5 "$WINDIR\Fonts\$1"

        ; ; Delete existing file just in case
        ; IfFileExists "$5" Delete "$5"

        ; Copy font
        CopyFiles "$0" "$5"

        ; Register in registry
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" "$1 ($3)" "$1"

        ; Load font immediately
        System::Call 'gdi32::AddFontResourceEx(t "$5", i 0x10, i 0)'

        ; Notify all apps
        System::Call 'user32::SendMessageTimeout(i 0xffff, i ${WM_FONTCHANGE}, i 0, i 0, i 0, i 1000, *i .r0)'
    ${Else}
        ; Font already installed, skip
        DetailPrint "Font $1 already installed, skipping."
    ${EndIf}
FunctionEnd

Function ShowShortcutOptions
  nsDialogs::Create 1018
  Pop $0
  ${If} $0 == error
      Abort
  ${EndIf}

  ${NSD_CreateCheckBox} 20u 20u 150u 12u "Create Desktop Shortcut"
  Pop $CreateDesktopShortcuts
  ${NSD_SetState} $CreateDesktopShortcuts 1 ; Default to checked

  ${NSD_CreateCheckBox} 20u 40u 150u 12u "Create Start Menu Shortcut"
  Pop $CreateStartMenu
  ${NSD_SetState} $CreateStartMenu 1 ; Default to checked

  nsDialogs::Show
FunctionEnd

; ;--------------------------------
; ; Function: UninstallFontHKLM
; ; Expects: font file name on stack
; Function UninstallFontHKLM
;     Exch $0 ; pop font file name into $0

;     StrCpy $1 "$WINDIR\Fonts"

;     ; Extract display name for registry key
;     ${GetFileName} "$0" $2
;     StrCpy $2 $2 0

;     ; Remove registry entry
;     DeleteRegKey /ifempty HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" "$2 (TrueType)"

;     ; Remove font from current session
;     System::Call 'gdi32::RemoveFontResourceEx(t "$1\$0", i 0x10, i 0)'

;     ; Delete the font file from system fonts folder
;     Delete "$1\$0"

;     ; Notify all applications that fonts have changed
;     System::Call 'user32::SendMessageTimeout(i 0xffff, i ${WM_FONTCHANGE}, i 0, i 0, i 0, i 1000, *i .r0)'

; FunctionEnd
