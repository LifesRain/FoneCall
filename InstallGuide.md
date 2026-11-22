<p align="center">
  <a href="./https://github.com/LifesRain/FoneCall">Home</a> •
  <a href="./InstallGuide.md">Installation Guide</a> •
  <a href="./SECURITY.md">Security</a> •
  <a href="https://github.com">Latest Releases</a> •
  <a href="https://github.com">GitHub Repo</a>
</p>

---
## Installation Guide & Caveats
#### Why is it not distributed as a standalone "Setup.exe"?
Browsers, email clients and antivirus scanners frequently block direct downloads of .exe installers or flag them with aggressive SmartScreen warnings. Packaging the installer inside a `.ZIP archive` ensures you can download it successfully.
#### SmartScreen Considerations
FoneCall features a fully transparent development process with a public codebase repository. To keep the project free, it is distributed without an expensive commercial code-signing certificate. Because of this, Windows Defender SmartScreen will display a standard "Windows protected your PC" warning when you launch the installer. While this warning is a helpful security feature for truly unknown files, it is a false alarm here since the source is trusted.

*   ***The Root Cause:*** Windows SmartScreen makes no distinction between truly well known sources—such as GitHub—and other sources. 

    When you use the default Windows File Explorer to extract a downloaded ZIP file, Windows automatically propagates an internet tracking tag—called *Mark of the Web* (MotW)—to every single file extracted from the zip file. This often causes Windows Defender or SmartScreen to block the installer from launching or corrupts the extraction process.

  * ***The Solution:*** To avoid the unnecessary warning use `7-Zip`,  `.WinRAR` or any other unzipping tool to extract the FoneCall installer. (`7-Zip` is preferred by the author.)

    Using third-party tools like 7-Zip or WinRAR typically extracts the files without forcefully tagging the installer with the MotW, allowing the setup to run smoothly.

#### If you still see the Windows SmartScreen warning

<details>
<summary>Click here for SmartScreen navigation instructions</summary>
<br>

![Instructions for Negotiating SmartScreen.](/assets/images/SmartScreenInstruct.png)

</details>

#### scrcpy and ADB.exe

<details>
<summary>Click here for ADB.exe and Windows Firewall explanation</summary>
<br>

`scrcpy.exe` uses ADB (Android Debug Bridge) to communicate with your phone. If executing scrcpy via FoneCall is the first time scrcpy is run on your computer, Windows Firewall may ask permission to let `adb.exe` through the firewall. You can leave **Private networks** checked and click **Allow access**.

</details>



> [!IMPORTANT]
> **TLDR:** Use `7-Zip`,  `.WinRAR` or other archiving utility to extract the FoneCall installer. ***Do not use*** Windows built-in unzip. Right-click the downloaded `.zip` file and select `7-Zip > Extract` or `WinRAR > Extract` to avoid the SmartScreen warning.

For details on our security architecture, see our [Security Policy](SECURITY.md).


|Download File Name|.zip File Contents|
|----|----|
| [1-FoneCall_Setup.zip](https://github.com/LifesRain/FoneCall/releases/download/v1.0.0-rc.2/1-FoneCall_Setup.zip) | `FoneCall_Setup` — the installer executable[^1] which includes a "get scrcpy" option.|
| [2-FoneCall_Setup_and_NDP481-Web.zip](https://github.com/LifesRain/FoneCall/releases/download/v1.0.0-rc.2/2-FoneCall_Setup_and_NDP481-Web.zip) | `FoneCall_Setup` and `NDP481-Web`[^2] executables.|
| [3-FoneCall_Portable.zip](https://github.com/LifesRain/FoneCall/releases/download/v1.0.0-rc.2/3-FoneCall_Portable.zip) | FoneCall binaries containing `FoneCall_Portable.exe` and the required supporting files. Unzip while retaining the folder structure. |
| [4-FoneCall_Portable_and_NDP481-Web.zip](https://github.com/LifesRain/FoneCall/releases/download/v1.0.0-rc.2/4-FoneCall_Portable_and_NDP481-Web.zip) | FoneCall binaries and the `NDP481-Web`[^2] executable.|


[^1]: Setup creates the Registry entries required to  
(1) add FoneCall to the Start Menu and register it with Windows Apps & features for easy uninstallation  
(2) save scrcpy location via Computer\HKEY_CURRENT_USER\SOFTWARE\FoneCall  
No other modifications to the Registry are made.  
[^2]: NDP481-Web is Microsoft's official .Net Framework 4.8 runtime installer 

#### Download Integrity Verification
Both "scripted" and manual verification are performed using `PowerShell` on your computer. The script automatically retrieves the GitHub generated checksum key and compares it to the key associated with the downloaded file.

For the manual approach first copy the **sha256** badge listed next to the file under **Assets** and the PowerShell command. Manually compare the output of the command to the copied key.

In both cases open PowerShell and change directory (cd ) to the folder where you saved your downloaded `.zip` file(s).  Then run the appropriate command as described below.

##### Automated "One-Liner" Script

***`Requires internet connection`***
```powershell
$files = Get-ChildItem -File | Where-Object Name -in '1-FoneCall_Setup.zip','2-FoneCall_Setup_and_NDP481-Web.zip','3-FoneCall_Portable.zip','4-FoneCall_Portable_and_NDP481-Web.zip'; if (-not $files) { Write-Host "ERROR: No FoneCall .zip package found in this folder." -ForegroundColor Red } else { $assets = (Invoke-RestMethod "https://api.github.com/repos/LifesRain/FoneCall/releases/tags/v1.0.0-rc.2").assets; foreach ($f in $files.Name) { $a = $assets | Where-Object name -eq $f; if (-not $a.digest) { Write-Host "ERROR: Release or SHA-256 digest not found for $f." -ForegroundColor Red } elseif ((Get-FileHash $f -Algorithm SHA256).Hash -eq ($a.digest -replace '^sha256:','')) { Write-Host "VERIFIED: $f matches official GitHub release." -ForegroundColor Green } else { Write-Host "WARNING: Checksum mismatch for $f!" -ForegroundColor Red } } }
```

##### Manual Command for Verification

***`Can be run locally after downloading`***
```powershell
Get-FileHash .\*FoneCall*.zip -Algorithm SHA256 | Select-Object @{Name="File";Expression={Split-Path $_.Path -Leaf}}, Hash

```
---

Return to the FoneCall **[Repository Home](https://github.com/LifesRain/FoneCall)** for more information on the tool or proceed to our repository's **[Latest Releases Page](https://github.com/LifesRain/FoneCall/releases/latest)**.