Unicode true
RequestExecutionLevel user
SetCompressor /SOLID lzma
!include "MUI2.nsh"
!include "x64.nsh"
!include "WinVer.nsh"
!include "nsDialogs.nsh"
!include "WordFunc.nsh"
!ifndef VERSION
  !error "Define VERSION, PAYLOAD, OUTPUT, ICON, LICENSE y DELETE_LIST."
!endif
!ifndef FILE_VERSION
  !error "Define FILE_VERSION con la revisión numérica de cuatro componentes. VERSION es la versión pública."
!endif
!define APPKEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\AdaptaPE.Lite"
Name "Adapta PE Lite"
OutFile "${OUTPUT}"
InstallDir "$LOCALAPPDATA\Programs\AdaptaPELite"
Icon "${ICON}"
UninstallIcon "${ICON}"
VIProductVersion "${FILE_VERSION}"
VIAddVersionKey "ProductVersion" "${VERSION}"
VIAddVersionKey "ProductName" "Adapta PE Lite"
VIAddVersionKey "FileDescription" "Instalador de Adapta PE Lite para Windows x64"
VIAddVersionKey "FileVersion" "${FILE_VERSION}"
VIAddVersionKey "LegalCopyright" "© 2026 Rodrigo Alejandro Apcho Aliaga"
!define MUI_ICON "${ICON}"
!define MUI_UNICON "${ICON}"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_HEADERIMAGE_BITMAP "../assets/installer/header.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "../assets/installer/welcome.bmp"
!define MUI_TEXTCOLOR "17365B"
!define MUI_ABORTWARNING
!define MUI_WELCOMEPAGE_TITLE "Bienvenido a Adapta PE Lite"
!define MUI_WELCOMEPAGE_TEXT "Tu escritorio, a tu ritmo.$\r$\nAsistente de voz y siete filtros de daltonismo, con tu logo Lite.$\r$\n$\r$\nSe instalará para tu usuario. Podrás elegir accesos en escritorio e Inicio. Incluye .NET y reconocimiento español local.$\r$\n$\r$\nNo necesita cámara, cuenta ni servidor."
!define MUI_FINISHPAGE_RUN "$INSTDIR\AdaptaPELite.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Abrir Adapta PE Lite"
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "${LICENSE}"
Page custom OptionsPage OptionsLeave
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "Spanish"
Var Dialog
Var DesktopCheck
Var StartCheck
Var DesktopChoice
Var StartChoice
Var ExistingVersion
Var InstallAction
Var WindowsName

Function OptionsPage
  !insertmacro MUI_HEADER_TEXT "$InstallAction Adapta PE Lite ${VERSION}" "Elige cómo acceder a tu aplicación."
  nsDialogs::Create 1018
  Pop $Dialog
  ${If} $Dialog == error
    Abort
  ${EndIf}
  ${NSD_CreateLabel} 0 0 100% 20u "$WindowsName · x64 · edición Lite · voz y daltonismo"
  Pop $0
  ${NSD_CreateLabel} 0 24u 100% 28u "Se instalará sólo para tu usuario. Incluye .NET, voz local y filtros."
  Pop $0
  ${NSD_CreateLabel} 0 54u 100% 28u "Carpeta: $INSTDIR"
  Pop $0
  ${NSD_CreateCheckbox} 0 88u 100% 18u "Crear acceso directo en el escritorio"
  Pop $DesktopCheck
  ${NSD_SetState} $DesktopCheck $DesktopChoice
  ${NSD_CreateCheckbox} 0 110u 100% 18u "Crear acceso directo en el menú Inicio"
  Pop $StartCheck
  ${NSD_SetState} $StartCheck $StartChoice
  ${NSD_CreateLabel} 0 138u 100% 36u "Tus preferencias se conservan. Voz y filtros se activan desde la app. Windows 7/8 requieren paquetes Legacy separados."
  Pop $0
  nsDialogs::Show
FunctionEnd
Function OptionsLeave
  ${NSD_GetState} $DesktopCheck $DesktopChoice
  ${NSD_GetState} $StartCheck $StartChoice
FunctionEnd

!macro CheckClosed PREFIX
Function ${PREFIX}CheckClosed
  System::Call 'kernel32::CreateMutexW(p0, i0, w "Local\AdaptaPE.Lite.Instance") p.r0 ?e'
  Pop $1
  System::Call 'kernel32::CloseHandle(p r0)'
  ${If} $1 == 183
    MessageBox MB_ICONEXCLAMATION|MB_OK "Cierra Adapta PE Lite desde la bandeja junto al reloj y vuelve a intentar."
    Abort
  ${EndIf}
FunctionEnd
!macroend
!insertmacro CheckClosed ""
!insertmacro CheckClosed "un."

Function .onInit
  ${IfNot} ${IsNativeAMD64}
  ${OrIfNot} ${AtLeastWin10}
    MessageBox MB_ICONSTOP|MB_OK "Este instalador requiere Windows 10 u 11 x64."
    Abort
  ${EndIf}
  SetShellVarContext current
  SetRegView 64
  StrCpy $INSTDIR "$LOCALAPPDATA\Programs\AdaptaPELite"
  Call CheckClosed
  ReadRegStr $0 HKLM "Software\Microsoft\Windows NT\CurrentVersion" "CurrentBuildNumber"
  StrCpy $WindowsName "Windows 10"
  ${If} $0 >= 22000
    StrCpy $WindowsName "Windows 11"
  ${EndIf}
  StrCpy $InstallAction "Instalar"
  StrCpy $DesktopChoice ${BST_CHECKED}
  StrCpy $StartChoice ${BST_CHECKED}
  ReadRegStr $ExistingVersion HKCU "${APPKEY}" "BuildVersion"
  ${If} $ExistingVersion == ""
    ReadRegStr $ExistingVersion HKCU "${APPKEY}" "DisplayVersion"
  ${EndIf}
  ${If} $ExistingVersion == ""
    IfFileExists "$INSTDIR\AdaptaPELite.exe" 0 versions_done
    ClearErrors
    GetDLLVersion "$INSTDIR\AdaptaPELite.exe" $1 $2
    ${IfNot} ${Errors}
      IntOp $3 $1 >> 16
      IntOp $4 $1 & 0xFFFF
      IntOp $5 $2 >> 16
      IntOp $6 $2 & 0xFFFF
      StrCpy $ExistingVersion "$3.$4.$5.$6"
    ${EndIf}
  ${EndIf}
  ${If} $ExistingVersion != ""
    ${VersionCompare} "$ExistingVersion" "${FILE_VERSION}" $0
    ${If} $0 == 1
      MessageBox MB_ICONEXCLAMATION|MB_OK "Ya tienes una compilación de Adapta PE Lite más reciente que este instalador. Usa esa instalación."
      Abort
    ${EndIf}
    StrCpy $InstallAction "Actualizar"
    ${If} $0 == 0
      StrCpy $InstallAction "Reparar"
    ${EndIf}
    ClearErrors
    ReadRegDWORD $DesktopChoice HKCU "${APPKEY}" "DesktopShortcut"
    ${If} ${Errors}
      StrCpy $DesktopChoice ${BST_UNCHECKED}
      IfFileExists "$DESKTOP\Adapta PE Lite.lnk" 0 +2
        StrCpy $DesktopChoice ${BST_CHECKED}
    ${EndIf}
    ClearErrors
    ReadRegDWORD $StartChoice HKCU "${APPKEY}" "StartMenuShortcut"
    ${If} ${Errors}
      StrCpy $StartChoice ${BST_UNCHECKED}
      IfFileExists "$SMPROGRAMS\Adapta PE Lite\Adapta PE Lite.lnk" 0 +2
        StrCpy $StartChoice ${BST_CHECKED}
    ${EndIf}
  ${EndIf}
  versions_done:
FunctionEnd

Function un.onInit
  SetShellVarContext current
  SetRegView 64
  StrCmp $INSTDIR "$LOCALAPPDATA\Programs\AdaptaPELite" +3
    MessageBox MB_ICONSTOP|MB_OK "La ubicación no coincide con esta instalación. No se borrarán archivos."
    Abort
  Call un.CheckClosed
FunctionEnd

Section "Adapta PE Lite" Main
  Call CheckClosed
  SetRegView 64
  SetOutPath "$INSTDIR"
  ClearErrors
  File /r "${PAYLOAD}/*"
  IfErrors 0 +3
    MessageBox MB_ICONSTOP|MB_OK "No se pudieron copiar todos los archivos. Cierra la app y vuelve a ejecutar el instalador."
    Abort
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  WriteRegStr HKCU "${APPKEY}" "DisplayName" "Adapta PE Lite"
  WriteRegStr HKCU "${APPKEY}" "DisplayVersion" "${VERSION}"
  WriteRegStr HKCU "${APPKEY}" "BuildVersion" "${FILE_VERSION}"
  WriteRegStr HKCU "${APPKEY}" "Publisher" "Rodrigo Alejandro Apcho Aliaga — TheAlejo160"
  WriteRegStr HKCU "${APPKEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${APPKEY}" "UninstallString" '$\"$INSTDIR\Uninstall.exe$\"'
  WriteRegStr HKCU "${APPKEY}" "DisplayIcon" "$INSTDIR\AdaptaPELite.exe,0"
  WriteRegDWORD HKCU "${APPKEY}" "NoModify" 1
  WriteRegDWORD HKCU "${APPKEY}" "NoRepair" 1
  WriteRegDWORD HKCU "${APPKEY}" "DesktopShortcut" $DesktopChoice
  WriteRegDWORD HKCU "${APPKEY}" "StartMenuShortcut" $StartChoice
  ClearErrors
  ${If} $StartChoice == ${BST_CHECKED}
    CreateDirectory "$SMPROGRAMS\Adapta PE Lite"
    CreateShortCut "$SMPROGRAMS\Adapta PE Lite\Adapta PE Lite.lnk" "$INSTDIR\AdaptaPELite.exe" "" "$INSTDIR\AdaptaPELite.ico"
  ${Else}
    Delete "$SMPROGRAMS\Adapta PE Lite\Adapta PE Lite.lnk"
    RMDir "$SMPROGRAMS\Adapta PE Lite"
  ${EndIf}
  ${If} $DesktopChoice == ${BST_CHECKED}
    CreateShortCut "$DESKTOP\Adapta PE Lite.lnk" "$INSTDIR\AdaptaPELite.exe" "" "$INSTDIR\AdaptaPELite.ico"
  ${Else}
    Delete "$DESKTOP\Adapta PE Lite.lnk"
  ${EndIf}
  IfErrors 0 +2
    MessageBox MB_ICONEXCLAMATION|MB_OK "Adapta PE Lite está instalado; Windows no permitió crear alguno de los accesos directos. Abre la app desde $INSTDIR."
SectionEnd

Section "Uninstall"
  Call un.CheckClosed
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "AdaptaPELite"
  ${If} $0 == '$\"$INSTDIR\AdaptaPELite.exe$\" --background'
    DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "AdaptaPELite"
  ${EndIf}
  Delete "$DESKTOP\Adapta PE Lite.lnk"
  Delete "$SMPROGRAMS\Adapta PE Lite\Adapta PE Lite.lnk"
  RMDir "$SMPROGRAMS\Adapta PE Lite"
  DeleteRegKey HKCU "${APPKEY}"
  SetOutPath "$TEMP"
  ; Lista de archivos de esta compilación: conserva archivos ajenos y preferencias.
  !include "${DELETE_LIST}"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
SectionEnd
