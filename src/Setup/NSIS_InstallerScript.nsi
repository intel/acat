!include MUI2.nsh
!include LogicLib.nsh  ; Needed for ${If}, ${DoWhile}, etc.
!include "FileFunc.nsh"
!insertmacro GetSize

!define MUI_ICON ".\installer_icons\intel.ico"
!define MUI_UNICON ".\installer_icons\intel.ico"
!define NAME "ACAT"
!define SETUPNAME "SetupACAT\SetupACAT64.exe"
!define ASSETS_FOLDER "Assets"
!define MUI_WARN_UNUSED_VARIABLES "false"

; Section: Installer Info
Name ${NAME}
OutFile ${SETUPNAME}
InstallDir "$PROGRAMFILES64\ACAT"
RequestExecutionLevel admin

SetCompress auto

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

  !define BUILD_OUTPUT "..\build\bin\Debug"
  File /r /x ConvAssistApp /x *.dat "${BUILD_OUTPUT}\*.*"

  CopyFiles /SILENT "$EXEDIR\${ASSETS_FOLDER}\*.*" "$INSTDIR"

  ; --------------------------------
  ; Install fonts from Assets\Fonts (uncompressed)
  ; --------------------------------
  ; SetOutPath "$FONTS"
  ; File /r /UNCOMPRESS "${BUILD_OUTPUT}\Assets\Fonts\*.*"

  ;  Register TTF fonts
  ; ClearErrors
  ; FindFirst $0 $1 "Assets\Fonts\*.ttf"
  ; ${DoWhile} ${Errors}
  ;   CopyFiles /SILENT "Assets\Fonts\$1" "$FONTS"
  ;   ; WriteRegStr HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" "$1" "$FONTS\$1"
  ;   FindNext $0 $1
  ; ${Loop}
  ; FindClose $0

  ; ; Register OTF fonts
  ; ; ClearErrors
  ; FindFirst $0 $1 "Assets\Fonts\*.otf"
  ; ${DoWhile} ${Errors}
  ;   CopyFiles /SILENT "Assets\Fonts\$1" "$FONTS"
  ;   ; WriteRegStr HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" "$1" "$FONTS\$1"
  ;   FindNext $0 $1
  ; ${Loop}
  ; FindClose $0

  ;--------------------------------
  ; Create Start Menu entries
  ;--------------------------------
  CreateDirectory "$SMPROGRAMS\ACAT"
  CreateShortcut  "$SMPROGRAMS\ACAT\ACAT Dashboard.lnk" "$INSTDIR\ACATApp.exe"
  CreateShortcut  "$SMPROGRAMS\ACAT\ACAT Talk.lnk" "$INSTDIR\ACATTalk.exe"
  CreateShortcut  "$SMPROGRAMS\ACAT\ACAT Config.lnk" "$INSTDIR\ACATConfigNext.exe"
  CreateShortcut  "$SMPROGRAMS\ACAT\Uninstall.lnk" "$INSTDIR\Uninstall.exe"

;   ;--------------------------------
;   ; Write uninstaller
;   ;--------------------------------
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2

  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \
    "DisplayName" "${NAME}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \
    "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${Name}" \
    "EstimatedSize" $0

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

;   ; Optional: remove installed fonts
;   ; WARNING: Only safe if these fonts are guaranteed to be app-specific
;   ClearErrors
;   FindFirst $0 $1 "Assets\Fonts\*.ttf"
;   ${DoWhile} ${Errors} == 0
;     Delete "$FONTS\$1"
;     DeleteRegValue HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" "$1"
;     FindNext $0 $1
;   ${Loop}
;   FindClose $0

;   ClearErrors
;   FindFirst $0 $1 "Assets\Fonts\*.otf"
;   ${DoWhile} ${Errors} == 0
;     Delete "$FONTS\$1"
;     DeleteRegValue HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" "$1"
;     FindNext $0 $1
;   ${Loop}
;   FindClose $0
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}" \

SectionEnd