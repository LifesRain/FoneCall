; /DappName
; /DreleaseFolder

; #define appName "CustomizeMyWin10"
; #define releaseFolder "E:\CustomizeMyWin10_Solution\Release"

[Setup]
AppId={{5A5902CD-5640-45B1-A83A-BD1553920EA2}
AppName={#appName}
AppVersion=1.0
PrivilegesRequired=Lowest
AppCopyright=Copyright (C) 2025 its.moi@myteck.us
DefaultDirName={autopf}\{#appName}
DefaultGroupName={#appName}
UninstallDisplayIcon={app}\{#appName}\{#appName}.exe
DisableWelcomePage=yes
SetupIconFile="FoneCall.ico"
AppReadmeFile=README.txt
InfoBeforeFile=license.txt
WizardStyle=dark
Compression=lzma2
SolidCompression=yes
OutputDir=.\
OutputBaseFilename={#appName}_Setup
ArchitecturesInstallIn64BitMode=x64compatible
SetupLogging=yes

[Files]
; Install MyProg-x64.exe if running in 64-bit mode (see above), and MyProg.exe otherwise.
Source: {#releaseFolder}\*; DestDir: "{app}"; Excludes: "*.xml"; Flags: ignoreversion recursesubdirs solidbreak
Source: "license.txt"; DestDir: {app}
Source: "FoneCall.db"; DestDir: {localappdata}\{#appName}
Source: "FoneCall.log"; DestDir: {localappdata}\{#appName}

[Run]
Filename: "{app}\{#appName}.exe"; Description: "Launch application"; Flags: postinstall nowait skipifsilent

[Icons]
Name: "{group}\{#appName}"; Filename: "{app}\{#appName}.exe"
Name: "{group}\Uninstall {#appName}"; Filename: "{uninstallexe}"

[Code]
function InitializeSetup(): Boolean;
begin
  // built in function; if required .Net not installed wizard won''t be started
  Result := IsDotNetInstalled(net48, 0); //Returns True if .NET Framework version 4.6.2 is installed, or a compatible version such as 4.8.1
  if not Result then
      // Only display  msg if the system is lacking .NET 4.8
      MsgBox('You system does not appear to have the requisit .Net Framework 4.8 installled, ' +
      'Please install .Net Framework 4.8 before retrying FoneCall installation. Setup will exit.',  
          mbCriticalError, MB_OK);
End;
