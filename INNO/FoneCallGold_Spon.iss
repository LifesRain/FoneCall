; /DappName
; /DreleaseFolder

; #define appName "CustomizeMyWin10"
; #define releaseFolder "E:\CustomizeMyWin10_Solution\Release"
#define GitHubOrg "LifesRain"
#define GitHubRepo "FoneCall"

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
InfoBeforeFile=InstallCombo.rtf
; LicenseFile=LICENSE.rtf
WizardStyle=dark
Compression=lzma2
SolidCompression=yes
OutputDir=.\
OutputBaseFilename={#appName}_Setup
ArchitecturesInstallIn64BitMode=x64compatible
SetupLogging=yes

[Files]
Source: {#releaseFolder}\*; DestDir: "{app}"; Excludes: "*.xml"; Flags: ignoreversion recursesubdirs solidbreak
Source: "FoneCall.db"; DestDir: {localappdata}\{#appName}
Source: "FoneCall.log"; DestDir: {localappdata}\{#appName}

[Run]
Filename: "{app}\{#appName}.exe"; Description: "Launch FoneCall"; Flags: postinstall nowait skipifsilent

[Icons]
Name: "{group}\{#appName}"; Filename: "{app}\{#appName}.exe"
Name: "{group}\Uninstall {#appName}"; Filename: "{uninstallexe}"

[Code]
var
  SuccessState: Boolean; 
  SponsorCheckBox: TNewCheckBox;

// Robust function to check for specific .NET Framework versions via Windows Registry
function IsDotNetInstalled(): Boolean;
var
  ReleaseKey: Cardinal;
begin
  Result := False;
  // Check the standard release key path for .NET 4.5+ versions
  if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', ReleaseKey) then
  begin
    // A release key >= 528040 corresponds to .NET Framework 4.8 or newer (4.8.1, etc.)
    if ReleaseKey >= 528040 then
    begin
      Result := True;
    end;
  end;
end;

function InitializeSetup(): Boolean;
begin
  // Execute the registry check when the installer initializes
  Result := IsDotNetInstalled(); 
  if not Result then
  begin
    MsgBox('Your system does not appear to have the requisite .NET Framework 4.8 installed. ' +
           'Please install .NET Framework 4.8 before retrying the FoneCall installation. Setup will exit.',  
           mbCriticalError, MB_OK);
  end;
end;

procedure InitializeWizard();
begin
  { Create the checkbox on the Finished Page }
  SponsorCheckBox := TNewCheckBox.Create(WizardForm);
  SponsorCheckBox.Parent := WizardForm.FinishedPage;
  
  { Align it neatly underneath the RunList container area }
  SponsorCheckBox.Left := WizardForm.RunList.Left;
  SponsorCheckBox.Width := WizardForm.RunList.Width;
  SponsorCheckBox.Height := ScaleY(20);
  
  { Explicitly place it safely near the bottom of the FinishedPage area }
  SponsorCheckBox.Top := WizardForm.FinishedPage.Height - ScaleY(35);
  
  SponsorCheckBox.Caption := '💖 Enjoying the app? Consider sponsoring the project on GitHub!';
  SponsorCheckBox.Checked := False;
end;

procedure ChangeFinishedPageLines;
var
  ErrorCode: Integer;
begin
  if (SponsorCheckBox <> nil) and SponsorCheckBox.Checked then
  begin
    ShellExec('open', 'https://github.com{#GitHubOrg}', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  { Trigger: Installation fully completed through all steps successfully }
  if CurStep = ssDone then
  begin
    SuccessState := True;
    ChangeFinishedPageLines;
  end;
end;

procedure DeinitializeSetup();
var
  ErrorCode: Integer;
begin
  if not SuccessState then
  begin
    if MsgBox('Installation failed to complete. Would you like to open our GitHub Issues page to check for a solution or report this problem?', 
              mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExecAsOriginalUser('open', 'https://github.com{#GitHubOrg}/{#GitHubRepo}/issues', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end;
  end;
end;
