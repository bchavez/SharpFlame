!include "FileFunc.nsh"
!include "MUI2.nsh"

;General
    CRCCheck on   ;make sure this isn't corrupted

    ;Name and file
    Name "${PACKAGE_NAME}"
    OutFile "${OUTFILE}"

    !define MUI_ICON "flaME.ico"

    ; define installation directory
    InstallDir "$LOCALAPPDATA\${PACKAGE_NAME}"

    RequestExecutionLevel user

;--------------------------------
;Versioninfo
    VIProductVersion                    "${PACKAGE_VERSION}"
    VIAddVersionKey "CompanyName"       "The SharpFlame Authors"
    VIAddVersionKey "FileDescription"   "${PACKAGE_NAME} Installer"
    VIAddVersionKey "FileVersion"       "${PACKAGE_VERSION}"
    VIAddVersionKey "InternalName"      "${PACKAGE_NAME}"
    VIAddVersionKey "LegalCopyright"    "Copyright (c) 2014 The SharpFlame Authors"
    VIAddVersionKey "ProductName"       "${PACKAGE_NAME}"
    VIAddVersionKey "ProductVersion"    "${PACKAGE_VERSION}"
 
;--------------------------------
;Variables
  Var MUI_TEMP
  Var STARTMENU_FOLDER

;--------------------------------
;Pages
;  !define MUI_PAGE_CUSTOMFUNCTION_PRE WelcomePageSetupLinkPre
;  !define MUI_PAGE_CUSTOMFUNCTION_SHOW WelcomePageSetupLinkShow
;  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "${TOP_SRCDIR}\LICENSE"
;  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_STARTMENU "Application" $STARTMENU_FOLDER
  !insertmacro MUI_PAGE_INSTFILES
;  !insertmacro MUI_PAGE_FINISH

;  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
;  !insertmacro MUI_UNPAGE_FINISH

# start default section
Section
    # set the installation directory as the destination for the following actions
    SetOutPath $INSTDIR

    # specify file to go in output path
    File SharpFlame.exe

    # The License File
    File LICENSE.txt
 
    # create the uninstaller
    WriteUninstaller "$INSTDIR\uninstall.exe"
 
    # create a shortcut named "new shortcut" in the start menu programs directory
    # point the new shortcut at the program uninstaller
    CreateDirectory "$SMPROGRAMS\$STARTMENU_FOLDER"
    CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall.lnk" "$INSTDIR\uninstall.exe"
    CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\${PACKAGE_NAME}.lnk" "$INSTDIR\SharpFlame.exe"

    ; Add/Remove Software entry
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PACKAGE_NAME}" \
                 "DisplayName" "${PACKAGE_NAME}"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PACKAGE_NAME}" \
                 "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PACKAGE_NAME}" \
                "Publisher" "Stiftung Maria Ebene"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PACKAGE_NAME}" \    
                 "DisplayVersion" "${PACKAGE_VERSION}"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PACKAGE_NAME}" \                 
                 "URLInfoAbout" "https://github.com/bchavez/SharpFlame/"

    ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
    IntFmt $0 "0x%08X" $0
    WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PACKAGE_NAME}" "EstimatedSize" "$0"                 
SectionEnd
 
# uninstaller section start
Section "uninstall"
 
    # first, delete the uninstaller
    Delete "$INSTDIR\uninstall.exe"

    Delete "$INSTDIR\SharpFlame.exe"

    Delete "$INSTDIR\LICENSE.txt"
 
    # second, remove the link from the start menu
    Delete "$SMPROGRAMS\${PACKAGE_NAME}\${PACKAGE_NAME}.lnk"
    Delete "$SMPROGRAMS\${PACKAGE_NAME}\Uninstall.lnk"
    RMDir "$SMPROGRAMS\$STARTMENU_FOLDER"
 
    DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PACKAGE_NAME}"

# uninstaller section end
SectionEnd