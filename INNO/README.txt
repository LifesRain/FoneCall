## License
This project is licensed under the **PolyForm Noncommercial License 1.0.0**. 
You are free to use, share, and modify both the source code and the installer 
for any personal or non-commercial purpose. Commercial use, commercial sale, 
or inclusion in paid products (including commercial AI training) is strictly prohibited.

### ;FoneCall
FoneCall makes it easier to start scrcpy with your preferred Arguments via the Start Menu or Taskbar while circumventing display of the command window.

The tool addresses some current constrsaints: [1] Pinning scrcpy.exe itself to the Start Menu or Taskbar creates a Windows comand window and doesn't accomodate Arguments. [2] Pinning the ".vbs" file distributed with scrcpy directly to the Start Menu or Taskbar is prohibited by Windows.  Pinning a shortcut to the .vbs file is increasingly problematic.

FoneCall is tested for use exclusively with the official version of scrcpy  (<https://github.com/Genymobile/scrcpy>).

When running FoneCall the first time enter the required info and Save to the app DB.  After saving scrcpy will startusing your provided Arguments.

When subsequently running FoneCall scrcpy will start after 5 seconds.  Clicking  anywhere on the app before the 5 seconds expires aborts scrcpy starting and enables making changes to data in the database.
[Download & Install](release/FoneCall_Setup.exe)
