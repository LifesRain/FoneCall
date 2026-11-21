; /DappName
; /DreleaseFolder

#define GitHubOrg "LifesRain"
#define GitHubRepo "FoneCall"

[Setup]
AppId={{5A5902CD-5640-45B1-A83A-BD1553920EA2}
AppName={#appName}
AppVersion=1.0.0
PrivilegesRequired=Lowest
AppCopyright=Copyright (C) 2025 its.moi@myteck.us
DefaultDirName={autopf}\{#appName}
DefaultGroupName={#appName}
UninstallDisplayIcon={app}\{#appName}.exe
DisableWelcomePage=yes
SetupIconFile="FoneCall.ico"
; AppReadmeFile=README.txt
InfoBeforeFile=InstallCombo.rtf
; LicenseFile=LICENSE.rtf
WizardStyle=dark
Compression=lzma2
SolidCompression=yes
OutputDir=.\Output
OutputBaseFilename={#appName}_Setup
ArchitecturesInstallIn64BitMode=x64compatible
SetupLogging=yes

[Files]
Source: "{#releaseFolder}\*"; DestDir: "{app}"; Excludes: "*.xml"; Flags: ignoreversion recursesubdirs solidbreak
Source: "FoneCall.db"; DestDir: "{localappdata}\{#appName}"; Flags: onlyifdoesntexist uninsneveruninstall
Source: "FoneCall.log"; DestDir: "{localappdata}\{#appName}"; Flags: onlyifdoesntexist
Source: "License.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\THIRD-PARTY-NOTICES.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#appName}"; Filename: "{app}\{#appName}.exe"
Name: "{group}\Uninstall {#appName}"; Filename: "{uninstallexe}"

[Registry]
Root: HKCU; Subkey: "Software\{#appName}"; Flags: uninsdeletekey

[Run]
Filename: "{app}\{#appName}.exe"; Description: "Launch FoneCall"; Flags: postinstall nowait skipifsilent

[Code]
var
  SuccessState: Boolean;
  DetectedScrcpyFQN: String;
  ScrcpyPage: TInputFileWizardPage;

// Searches immediate *scrcpy* subdirectories of BaseDir and returns full path to scrcpy.exe (or '')
function FindScrcpyInSubfolders(BaseDir: String): String;
var
  FindRec: TFindRec;
  SearchMask, CandidateExe: String;
begin
  Result := '';
  if not DirExists(BaseDir) then Exit;

  SearchMask := AddBackslash(BaseDir) + '*scrcpy*';
  if FindFirst(SearchMask, FindRec) then
  begin
    try
      repeat
        if (FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY) <> 0 then
        begin
          CandidateExe := AddBackslash(BaseDir) + FindRec.Name + '\scrcpy.exe';
          if FileExists(CandidateExe) then
          begin
            Result := CandidateExe;
            Break;
          end;
        end;
      until not FindNext(FindRec);
    finally
      FindClose(FindRec);
    end;
  end;
end;

// Returns full path to scrcpy.exe if found, or '' if not detected
function DetectScrcpyPath(): String;
var
  UserProfile, ScoopExe: String;
begin
  // 1. Check Windows PATH
  Result := FileSearch('scrcpy.exe', GetEnv('PATH'));
  if Result <> '' then Exit;

  // 2. Check common extraction roots (*scrcpy* subfolders)
  UserProfile := GetEnv('USERPROFILE');

  Result := FindScrcpyInSubfolders('C:\');
  if Result <> '' then Exit;

  Result := FindScrcpyInSubfolders(ExpandConstant('{autopf}'));
  if Result <> '' then Exit;

  Result := FindScrcpyInSubfolders(UserProfile);
  if Result <> '' then Exit;

  Result := FindScrcpyInSubfolders(UserProfile + '\Downloads');
  if Result <> '' then Exit;

  Result := FindScrcpyInSubfolders(UserProfile + '\Desktop');
  if Result <> '' then Exit;

  // 3. Check Scoop shim/current path
  ScoopExe := UserProfile + '\scoop\apps\scrcpy\current\scrcpy.exe';
  if FileExists(ScoopExe) then
    Result := ScoopExe;
end;

procedure DownloadScrcpyBtnClick(Sender: TObject);
var
  ErrorCode: Integer;
begin
  ShellExecAsOriginalUser(
    'open',
    'https://github.com/Genymobile/scrcpy/releases',
    '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode
  );
end;

function IsDotNetInstalled(): Boolean;
var
  ReleaseKey: Cardinal;
begin
  Result := False;
  if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', ReleaseKey) then
  begin
    if ReleaseKey >= 528040 then
      Result := True;
  end;
end;

function InitializeSetup(): Boolean;
begin
  // 1. Hard requirement: .NET Framework 4.8
  Result := IsDotNetInstalled();
  if not Result then
  begin
    MsgBox('Your system does not appear to have the requisite .NET Framework 4.8 installed. ' +
           'Please install .NET Framework 4.8 before retrying the FoneCall installation. Setup will exit.',
           mbCriticalError, MB_OK);
    Exit;
  end;

  // 2. Run auto-detection silently before wizard pages are built
  DetectedScrcpyFQN := DetectScrcpyPath();
end;

procedure InitializeWizard();
var
  DownloadBtn: TNewButton;
begin
  ScrcpyPage := CreateInputFilePage(
    wpSelectDir,
    'Locate scrcpy.exe',
    'scrcpy was not automatically detected on your system.',
    'FoneCall is a UI wrapper for scrcpy. If you have already downloaded and extracted scrcpy, ' +
    'browse to scrcpy.exe below so FoneCall can configure it automatically.' + #13#10 + #13#10 +
    'If you do not have scrcpy yet, click "Download scrcpy..." below or leave this blank and set it later inside FoneCall.'
  );

  ScrcpyPage.Add(
    'Full path to scrcpy.exe (optional):',
    'scrcpy executable (scrcpy.exe)|scrcpy.exe|Executable files (*.exe)|*.exe',
    '.exe'
  );

  DownloadBtn := TNewButton.Create(ScrcpyPage);
  DownloadBtn.Parent := ScrcpyPage.Surface;
  DownloadBtn.Left := 0;
  DownloadBtn.Top := ScrcpyPage.Edits[0].Top + ScrcpyPage.Edits[0].Height + ScaleY(12);
  DownloadBtn.Width := ScaleX(140);
  DownloadBtn.Height := ScaleY(24);
  DownloadBtn.Caption := 'Download scrcpy...';
  DownloadBtn.OnClick := @DownloadScrcpyBtnClick;
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  Result := False;
  // Skip the manual browse page if scrcpy.exe was already auto-detected
  if (PageID = ScrcpyPage.ID) and (DetectedScrcpyFQN <> '') then
    Result := True;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ChosenFile: String;
begin
  Result := True;

  if CurPageID = ScrcpyPage.ID then
  begin
  // RemoveQuotes strips surrounding double-quotes if the user used Explorer's "Copy as path"
    ChosenFile := Trim(RemoveQuotes(Trim(ScrcpyPage.Values[0])));
    ScrcpyPage.Values[0] := ChosenFile;
    
    ChosenFile := Trim(ScrcpyPage.Values[0]);
    if ChosenFile <> '' then
    begin
      if not FileExists(ChosenFile) then
      begin
        MsgBox('The selected file does not exist. Please browse to a valid scrcpy.exe or clear the field to configure it later.',
               mbError, MB_OK);
        Result := False;
        Exit;
      end;
      DetectedScrcpyFQN := ChosenFile;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  // Write either the auto-detected path or the user-browsed path to HKCU
  if (CurStep = ssPostInstall) and (DetectedScrcpyFQN <> '') then
  begin
    RegWriteStringValue(HKCU, 'Software\{#appName}', 'DetectedScrcpyFQN', DetectedScrcpyFQN);
  end;

  if CurStep = ssDone then
  begin
    SuccessState := True;
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
      ShellExecAsOriginalUser('open', 'https://github.com/{#GitHubOrg}/{#GitHubRepo}/issues', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end;
  end;
end;
