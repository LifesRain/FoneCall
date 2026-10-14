# FoneCall — Windows GUI for scrcpy (Android Screen Mirroring)

FoneCall is a simple, lightweight **scrcpy UI** designed for Windows. It serves as a standard native Windows desktop application alternative to the default `.vbs` script file distributed with scrcpy.

The tool overcomes scrcpy constraints:
*   **Command Window Launch:** Command window must stay active or scrcpy exits.
*   **Delayed Launch Confirmation:** There is a significant delay between `.vbs` execution initiation and display of the mirrored phone screen on the computer screen.

The tool directly resolves common Windows shortcut limitations:
*   **Taskbar Pinning:** Pinning `scrcpy.exe` natively opens an unwanted Windows command prompt window and cannot accommodate custom arguments.
*   **VBS Constraints:** Pinning the default `.vbs` script directly to the Start Menu or Taskbar is natively blocked by Windows.
*   **Shortcut Issues:** Pinning manual Windows shortcuts targeting a `.vbs` source file is increasingly problematic in modern Windows builds.
	
---


## Core Features & Interface Windows

FoneCall wraps your entire screen-mirroring workflow into a streamlined graphical workflow. It provides a clean graphical user interface to launch scrcpy with custom arguments directly from your Start Menu or Taskbar while hiding the background command window. 

### Welcome Window
Welcome describes tool usage. It is shown the first time FoneCall is launched after installation. The text is included in the  log for future reference.  
![Welcome Window describes tool usage. It is shown the first time FoneCall is launched after installation. The text is included in the  log for future reference.](/assets/images/Intro.png)

### Data Entry Window
Data Entry captures execution parameters which are saved in an embedded SQLite DB. The `scrcpy.exe` file location is verified and `Arguments` are validated prior to saving.  
![Data Entry captures execution parameters which are saved in an embedded SQLite DB. The scrcpy.exe file location is verified and Arguments are validated prior to saving.](/assets/images/DataEntry.png)

### zcrcpy Execution Window
Execution starts scrcpy and provides & logs execution status. This is the initial window after execution parameters have been saved When the phone is displayed on the computer screen FoneCall exits.  If an error occurs, the user is returned to the Data Entry window to correct the issue.  
![Execution starts scrcpy and provides & logs execution status. This is the initial window after execution parameters have been saved When the phone is displayed on the computer screen FoneCall exits.  If an error occurs, the user is returned to the Data Entry window to correct the issue.](/assets/images/execution.png)

### scrcpy Options Reference and Argument Update Window
Options Reference and Update displays scrcpy Help and provides the opportunity to add new scrcpy Options when they are introduced. This is the source for the Arguments validation in Data Entry.  
![Options Reference and Update displays scrcpy Help and provides the opportunity to add new scrcpy Options when they are introduced. This is the source for the Arguments validation in Data Entry.](/assets/images/ArgRef.png)

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
