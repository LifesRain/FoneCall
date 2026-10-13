# FoneCall — Windows GUI for scrcpy (Android Screen Mirroring)

FoneCall is a simple, lightweight **scrcpy UI** designed for Windows. It provides a clean graphical user interface to launch scrcpy with custom arguments directly from your Start Menu or Taskbar while hiding the background command window. It serves as a standard native Windows desktop application alternative to the default ".vbs" script file distributed with scrcpy.

The tool directly resolves common Windows shortcut limitations:
*   **Taskbar Pinning:** Pinning `scrcpy.exe` natively opens an unwanted Windows command prompt window and cannot accommodate custom arguments.
*   **VBS Constraints:** Pinning the default `.vbs` script directly to the Start Menu or Taskbar is natively blocked by Windows.
*   **Shortcut Issues:** Pinning manual Windows shortcuts targeting a `.vbs` source file is increasingly problematic in modern Windows builds.


	
---

## Core Features & Interface Windows

FoneCall wraps your entire screen-mirroring workflow into a clean graphical workflow:
*   **Welcome Window:** Disposable entry screen for fast launching.
<br>
![Welcome Window](/assets/images/Intro.png)
*   **Data Entry Window:** Configuration fields for targeted mobile connection profile.
<br>
![Data Entry Window](/assets/images/DataEntry.png)
*   **scrcpy Execution Window:** process executiion that suppresses command prompt popups.
<br>
![scrcpy Execution Window](/assets/images/execution.png)

*   **Options Reference Window:** Interface to manage and save persistent command-line arguments.
<br>
![Options Reference and Argument Update Window](/assets/images/ArgRef.png)
---

## System Requirements & Prerequisites
FoneCall is a native Windows desktop application built to bridge the gap between your OS and your phone's display.

### Native Runtime Environment
*   **Framework Requirement:** Requires .NET Framework 4.8 or higher.
*   **Windows 10 Compatibility:** Included natively in Version 1903 (May 2019 Update) and all later builds.
*   **Windows 11 Compatibility:** Natively included in the initial release (Version 22000) and later.
*   **Windows Server Compatibility:** Natively included in Windows Server 2019 (build 1903+), 2022, and 2025.

> ⚠️ **Installation Notice:** FoneCall will fail to install if your system lacks .NET Framework 4.8 or higher. If your operating system does not include it, the runtime installer package can be acquired [on this site](https://github.com/ddDerelict/FoneCall/releases) or if you prefer you can download it directly from Microsoft.

### Dependency Configuration
*   **scrcpy Requirement:** FoneCall functions strictly as a user interface wrapper and will not run properly if the underlying scrcpy package (i.e.; the contents of installer zip file) is absent.  If you do not have the official scrcpy package, it can be downloaded from the [official scrcpy project page](https://github.com/Genymobile/scrcpy).
*   **Official Compatibility:** This tool is built and tested exclusively for use with the official version of scrcpy.


---

## Download and Installation

### 1. Download FoneCall
Get the latest installation package directly from our repository releases page:
[Download FoneCall](https://github.com/ddDerelict/FoneCall/releases)

### 2. Basic Setup Steps
1. Ensure the official `scrcpy.package` is installed on your computer.
2. Run the FoneCall installer and follow the desktop setup prompts.
3. Launch FoneCall, configure your preferred arguments, and pin the app to your Taskbar or Start Menu for one-click access.

---

## Acknowledgements & Open Source Credits

*   **scrcpy:** This project relies on the open-source engine developed by Genymobile.
*   **Development:** FoneCall functions as a standalone third-party interface utility. The developer highly appreciates the contributions made by the scrcpy development team and the global open-source community.
