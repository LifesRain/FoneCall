[Code]
/////// Add IsScrcpyDetected() to your [Code] section and call it inside InitializeSetup() right after the .NET 4.8 check passes:

function FindScrcpyInSubfolders(BaseDir: String): Boolean;
var
  FindRec: TFindRec;
  SearchMask: String;
begin
  Result := False;
  if not DirExists(BaseDir) then Exit;

  SearchMask := AddBackslash(BaseDir) + '*scrcpy*';
  if FindFirst(SearchMask, FindRec) then
  begin
    try
      repeat
        // Check if it's a directory (e.g. scrcpy-win64-v3.1) containing scrcpy.exe
        if (FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY) <> 0 then
        begin
          if FileExists(AddBackslash(BaseDir) + FindRec.Name + '\scrcpy.exe') then
          begin
            Result := True;
            Break;
          end;
        end;
      until not FindNext(FindRec);
    finally
      FindClose(FindRec);
    end;
  end;
end;

function IsScrcpyDetected(): Boolean;
var
  UserProfile: String;
begin
  Result := False;

  // 1. Check PATH (handles winget, scoop, chocolatey, or manual PATH entries)
  if FileSearch('scrcpy.exe', GetEnv('PATH')) <> '' then
  begin
    Result := True;
    Exit;
  end;

  // 2. Shallow wildcard check (*scrcpy*) in common extraction roots (< 5ms)
  UserProfile := GetEnv('USERPROFILE');
  if FindScrcpyInSubfolders('C:\') or
     FindScrcpyInSubfolders(ExpandConstant('{autopf}')) or
     FindScrcpyInSubfolders(UserProfile) or
     FindScrcpyInSubfolders(UserProfile + '\Downloads') or
     FindScrcpyInSubfolders(UserProfile + '\Desktop') or
     FileExists(UserProfile + '\scoop\apps\scrcpy\current\scrcpy.exe') then
  begin
    Result := True;
  end;
end;

////Then update InitializeSetup() to warn—without returning False—when scrcpy.exe isn't found:

function InitializeSetup(): Boolean;
var
  ErrorCode: Integer;
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

  // 2. Soft check: scrcpy.exe (warn only, do not block setup)
  if not IsScrcpyDetected() then
  begin
    if MsgBox('scrcpy.exe was not detected in your PATH or standard folders.' + #13#10 + #13#10 +
              'FoneCall requires scrcpy to function. If you have already unzipped it to a custom folder, ' +
              'you can point FoneCall to it after installation.' + #13#10 + #13#10 +
              'Would you like to open the official scrcpy releases page to download it now?',
              mbInformation, MB_YESNO) = IDYES then
    begin
      ShellExecAsOriginalUser('open', 'https://github.com', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end;
  end;
end;
