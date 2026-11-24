<p align="center">
  <strong>Overview</strong> •
  <a href="./InstallGuide.md">Installation Guide</a> •
  <a href="./FoneCallWalkthru.md">App Walkthrough</a> •
  <a href="https://github.com/LifesRain/FoneCall/releases/latest">Latest Releases</a>
</p>

---
# FoneCall — Windows GUI for scrcpy (Android Screen Mirroring)

FoneCall is a simple, lightweight **scrcpy UI**. It is a standard native Windows desktop application. Functionally it provides an alternative to the `.vbs` script file distributed with scrcpy.

The tool overcomes scrcpy constraints:
*   **Command Prompt Launch:** When using the Command Prompt window to launch scrcpy with execution arguments that window must stay active or scrcpy exits.
*   **Delayed Launch Confirmation:** When using the provided `vbs` script to launch scrcpy there is a significant delay between `.vbs` execution initiation and display of the mirrored phone screen on the computer screen.

The tool directly resolves common Windows shortcut limitations:
*   **Taskbar Pinning:** Pinning `scrcpy.exe` natively opens an unwanted Windows command prompt window and cannot accommodate custom arguments.
*   **VBS Constraints:** Pinning the default `.vbs` script directly to the Start Menu or Taskbar is natively blocked by Windows.
*   **Shortcut Issues:** Pinning manual Windows shortcuts targeting a `.vbs` source file is increasingly problematic in modern Windows builds.
	
---


## Core Features & Interface Windows

FoneCall launches scrcpy from your Start Menu, Taskbar or the FoneCall executable. The location of scrcpy on your system and your preferred execution arguments are saved in a lightweight embedded SQLLite database.

During execution, FoneCall displays and logs scrcpy startup status. When your phone's screen is displayed on your computer, FoneCall automatically exits, leaving only the mirrored phone screen visible and active.

The [FoneCall Walkthrough](FoneCallWalkthru.md) shows and describes the tool's interface windows.

---

## System Requirements & Prerequisites
FoneCall is a native Windows desktop application built on .NET technology; installation will fail if your system does not meet the requirements provided below.  As a scrcpy UI wrapper FoneCall will provide little value without access to a fully functional scrcpy package.

### Native Runtime Environment
**Framework Requirement:** Requires .NET Framework **4.8 or higher**. Many recent Windows builds include this runtime natively[^1]. If your system does not include it, the runtime installer package can be downloaded with the FoneCall_Setup executable or if you prefer you can acquire it directly from Microsoft.
[^1]: 	<ins>**Windows 10:**</ins> Included natively in Version 1903 (May 2019 Update) and all later builds.    
    <ins>**Windows 11:**</ins> Natively included in the initial release (Version 22000) and later.  
    <ins>**Windows Server:**</ins> Natively included in Windows Server 2019 (build 1903+), 2022, and 2025.

> ⚠️ **Installation Notice:** FoneCall will fail to install if your system lacks .NET Framework 4.8 or higher.

### Dependency Configuration
**scrcpy Requirement:** Theoretically FoneCall should function with any scrcpy version but it has only been tested with version 4 and later.  If you do not have the official scrcpy package and you
+ use FoneCall_Setup the installer supports acquisition of the package
+ use the portable version scrcpy can be downloaded from the [official scrcpy project](https://github.com/Genymobile/scrcpy/releases).
---

## Download and Installation
Because FoneCall connects with an individual's smartphone via scrcpy our installer is configured for "User-Mode" which does not require Administrator privileges.

Whether you've installed numerous applications or this is your first time, we strongly recommend reading the [Installation Guide & Caveats](InstallGuide.md). The guide describes the download options and provides important information about the installation process, including potential issues with Windows SmartScreen and how to avoid them.

### 1. Download FoneCall

Get the latest installation package directly from  our repository **[Latest Releases Page](https://github.com/LifesRain/FoneCall/releases/latest)**.


### Verify Your Download (Optional but Recommended)

GitHub automatically generates a cryptographic **SHA-256 checksum** for every release file so you can confirm your download was not corrupted or tampered with. Instructions for verifying the integrity of your download are in the [Installation Guide & Caveats](InstallGuide.md#download-integreity-verification).

## Acknowledgements

*   **scrcpy:** This project relies on the open-source engine developed by Genymobile.
*   **Development:** FoneCall functions as a standalone third-party interface utility. The developer highly appreciates the contributions made by the scrcpy development team.
*   **Official Compatibility:** This tool is built and tested exclusively for use with the official version of scrcpy.
 
## ⚖️ Licensing & Commercial Use

This software is distributed under the [PolyForm Noncommercial License 1.0.0](LICENSE.md). 

* **Commercial Entities & Businesses:** Any use of this software to generate revenue, conduct commercial operations, or use within a corporate/business environment requires a paid **Commercial License**.
* **Everybody Else:** You are free to use, modify, and distribute this software for personal, educational, research, or charitable purposes.
* **Third-Party Open Source Components:** This application utilizes open-source runtime binary libraries (such as Dapper and SQLite). These dependencies are distributed under their own permissive terms (MIT, Apache 2.0, and Public Domain) and are not subject to the non-commercial restriction. For the full legal text of these licenses, see [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt).

##### 💼 Commercial Licensing & Enterprise Support
If you wish to use this application for commercial purposes contact us at:

📧  [fonecall@lifesrain.com](mailto:fonecall@lifesrain.com)


---

 
<p align="center">
  <strong>Overview</strong> •
  <a href="./InstallGuide.md">Installation Guide</a> •
  <a href="./FoneCallWalkthru.md">App Walkthrough</a> •
  <a href="https://github.com/LifesRain/FoneCall/releases/latest">Latest Releases</a>
</p>
