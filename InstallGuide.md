## Installation Guide & Caveats
#### Why is it not distributed as a standalone "Setup.exe"?
Browsers, email clients and antivirus scanners frequently block direct downloads of .exe installers or flag them with aggressive SmartScreen warnings. Packaging the installer inside a `.ZIP archive` ensures you can download it successfully.
#### SmartScreen Considerations
FoneCall features a fully transparent development process with a public codebase repository. To keep the project free, it is distributed without an expensive commercial code-signing certificate. Because of this, Windows Defender SmartScreen will display a standard "Windows protected your PC" warning when you launch the installer. While this warning is a helpful security feature for truly unknown files, it is a false alarm here since the source is trusted.

*   ***The Root Cause:*** Windows SmartScreen makes no distinction between truly well known sources—such as GitHub—and other sources. 

    When you use the default Windows File Explorer to extract a downloaded ZIP file, Windows automatically propagates an internet tracking tag—called *Mark of the Web* (MotW)—to every single file extracted from the zip file. This often causes Windows Defender or SmartScreen to block the installer from launching or corrupts the extraction process.

  * ***The Solution:*** To avoid the unnecessary warning use `7-Zip`,  `.WinRAR` or any other unzipping tool to extract the FoneCall installer. (`7-Zip` is preferred by the author.)

    Using third-party tools like 7-Zip or WinRAR safely typically extracts the files without forcefully tagging the installer with the MotW, allowing the setup to run smoothly.

For details on our security architecture, see our [Security Policy](SECURITY.md).

> [!IMPORTANT]
> **TLDR:** Use `7-Zip`,  `.WinRAR` or other archiving utility to extract the FoneCall installer. ***Do not use*** Windows built-in unzip.

#### If you still see the Windows SmartScreen warning, follow these steps to bypass it:
![Instructions for Negotiating SmartScreen.](/assets/images/SmartScreenInstruct.png)


|Download File Name|.zip File Contents|
|----|----|
| FoneCall_Setup.zip|FoneCall_Setup (the installer executable[^1])|
| FoneCall_Setup+NDP481-Web.zip|FoneCall_Setup and NDP481-Web[^2] executables|
| FoneCall_Portable.zip|FoneCall [binaries](https://www.google.com/search?q=what+are+application+binaries) containing the FoneCall_Portable.exe and required supporting files.  Unzip while retaining folder structure.|
| FoneCall_Portable+NDP481-Web.zip| FoneCall binaries zip file and NDP481-Web[^2] executable|

[^1]: Setup makes the Registry entries to add FoneCall to the Start Menu and  support application add/uninstall.  No other modifications to the Registry are made
[^2]: NDP481-Web is Microsoft's official .Net Framework 4.8 runtime installer 

#### Download Integreity Verification
Both "scripted" and manual verification is performed using `PowerShell` on your computer.  The script automatically retrieves the GitHub generated checksum key and compares it to the one associated with the downloaded file.

For the manual approach first copy the **sha256** badge listed next to the file under **Assets**  and the appropriate PowerShell command.  Manually compare the output of the command to the copied key.

In both cases open PowerShell and change directory (cd ) to the folder where you saved your downloaded `.zip` file.  Then run the appropriate command as described below.

##### Automation "One-Liner" Scripts

***`FoneCall_Setup.zip`***
```powershell
$f = "FoneCall_Setup.zip"; $a = (Invoke-RestMethod "https://api.github.com/repos/LifesRain/FoneCall/releases/tags/v1.0.0-rc.1").assets | Where-Object name -eq $f; if (-not $a.digest) { Write-Host "ERROR: Release or SHA-256 digest not found." -ForegroundColor Red } elseif ((Get-FileHash $f -Algorithm SHA256).Hash -eq ($a.digest -replace '^sha256:','')) { Write-Host "VERIFIED: File matches official GitHub release." -ForegroundColor Green } else { Write-Host "WARNING: Checksum mismatch!" -ForegroundColor Red }
```

##### Manual Command for Verification

***`FoneCall_Setup.zip`***
```powershell
Get-FileHash .\FoneCall_Setup.zip -Algorithm SHA256`
```
---


Return to the FoneCall [README](README.md) for more information on the tool or proceed to our repository's **[Latest Releases Page](https://github.com/LifesRain/FoneCall/releases/latest)**.