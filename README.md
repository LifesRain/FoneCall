## FoneCall
FoneCall is a very simple scrcpy UI. It makes it easier to access scrcpy with your preferred Arguments via the Start Menu or Taskbar while avoiding display of the command window.  It is standard Windows desktop application alternative to the ".vbs" script file distributed with scrcpy.

The tool addresses some current constraints: 
1. Pinning scrcpy.exe itself to the Start Menu or Taskbar creates a Windows command window and doesn't accommodate Arguments
2. Pinning the ".vbs" file to the Start Menu or Taskbar is prohibited by Windows
3. Pinning a shortcut to the .vbs file is increasingly problematic

### What’s in the Box?
![Welcome Window](/assets/images/Intro.png)
![Data Entry Window](/assets/images/DataEntry.png)
![scrcpy Execution Window](/assets/images/execution.png)
![Options Reference and Argument Update Window](/assets/images/ArgRef.png)

### Prerequisites
FoneCall is a Windows desktop application that requires  NET Framework 4.8 (or higher) to be installed and the official scrcpy.exe to be available on your system to function as designed.

FoneCall will not install if .NET Framework 4.8 or higher is installed. .NET Framework 4.8 is included in the following:  
> + Windows 10: Version 1903 (May 2019 Update) and all later Windows 10 versions.
> + Windows 11: Included in the initial release (Version 22000) and later.
> + Windows Server: Windows Server 2019 (starting with version 1903 updates), Windows Server 2022, and Windows Server 2025.
*(Source: Google search)*

If your OS doesn’t already include support for .NET Framework 4.8 the installer is available [on this site](https://github.com/ddDerelict/FoneCall/releases) or if you prefer you can [download  from Microsoft]( https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)

FoneCall will install and will start to run but will not proceed to scrcpy execution if scrcpy.exe is not available  If you have arrived here from the official scrcpy project page, you probably already have scrcpy. If you do not have the official scrcpy.exe, it can be downloaded from the [official scrcpy project page](https://github.com/Genymobile/scrcpy).

FoneCall is tested for use exclusively with the official version of scrcpy.

### Get FoneCall
[Download FoneCall](https://github.com/ddDerelict/FoneCall/releases)

### Acknowledgements
scrcpy is a Marvelous open-source project developed by Genymobile. FoneCall is a third-party application that provides a user-friendly interface for scrcpy. The developer of FoneCall would like to acknowledge the contributions of the scrcpy team and the open-source community.
