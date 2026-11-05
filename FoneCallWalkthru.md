## FoneCall Walkthrough

The tool is minimalist with two small-footprint core windows and two subsidiary windows. The windows are:
* Welcome — subsidiary
* Data Entry — Core
* Execution — Core
* Options Reference and Argument Update — subsidiary

### Welcome Window
The Welcome window describes tool usage. It is only shown the first time FoneCall is launched after installation. The text is also included in the application log for future reference.  
![Welcome Window describes tool usage. It is shown the first time FoneCall is launched after installation. The text is included in the log for future reference.](/assets/images/Intro.png)

### Data Entry Window
Data Entry captures execution parameters which are saved in an embedded SQLite database (DB). The `scrcpy.exe` file location is verified and command arguments are validated prior to saving.  
![Data Entry captures execution parameters which are saved in an embedded SQLite DB. The scrcpy.exe file location is verified and Arguments are validated prior to saving.](/assets/images/DataEntry.png)

### scrcpy Execution Window
The Execution window starts scrcpy, displays startup events, and logs execution status. On the initial launch of FoneCall after installation, this window is triggered automatically after successfully saving your parameters. In subsequent launches, the Execution window acts as the initial startup screen if valid parameters exist in the DB. If parameters are missing or invalid, the user is returned to the *Data Entry* window to correct the issue.

There is a built-in seven-second delay between the launch of FoneCall and the start of scrcpy. During this countdown, the user can:
* Start scrcpy immediately
* Pause the countdown
* Cancel the execution and switch to *Data Entry*

When your phone's screen is successfully displayed on the computer, FoneCall automatically exits. If a launch error occurs, the user is returned to the *Data Entry* window to fix the configuration.  
![Execution starts scrcpy and provides & logs execution status. This is the initial window after execution parameters have been saved. When the phone is displayed on the computer screen FoneCall exits. If an error occurs, the user is returned to the Data Entry window to correct the issue.](/assets/images/execution.png)

### Options Reference and Argument Update Window
The Options Reference and Update window displays native scrcpy Help outputs and provides the opportunity to add new scrcpy options when they are introduced upstream. This serves as the data validation source for the *Data Entry* window.  
![Options Reference and Update displays scrcpy Help and provides the opportunity to add new scrcpy Options when they are introduced. This is the source for the Arguments validation in Data Entry.](/assets/images/ArgRef.png)

---
Return to the FoneCall [README](README.md) for more information on the tool, its features, and installation instructions.
