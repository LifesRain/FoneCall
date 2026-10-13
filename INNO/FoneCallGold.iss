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
var
  SuccessState: Boolean; // Global variable to track if setup reached the final 

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


// The most robust way to catch an actual system/extraction failure in Inno Setup
procedure CurStepChanged(CurStep: TSetupStep);
begin
  // This step is ONLY reached if the actual installation process finishes successfully
  if CurStep = ssDone then
  begin
    SuccessState := True;
  end;
end;

procedure DeinitializeSetup();
var
  ErrorCode: Integer;
begin
  // If SuccessState is false, the user either cancelled or a fatal extraction error occurred
  if not SuccessState then
  begin
    // Ask the user if they want to report the error
    if MsgBox('Installation failed to complete. Would you like to open our GitHub Issues page to check for a solution or report this problem?', 
              mbConfirmation, MB_YESNO) = IDYES then
    begin
      // ShellExecAsOriginalUser securely launches the link in the user's default browser
      ShellExecAsOriginalUser('open', 'https://github.com/ddDerelict/FoneCall/issues', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end;
  end;
end;