@echo off
setlocal
cd /d "%~dp0"

set "LOG_FILE=%~dp0build_iss.log"

:: Overwrite log file at start of run
echo ================================================== > "%LOG_FILE%"
echo Starting post build batch file: >> "%LOG_FILE%"
echo %DATE% %TIME% >> "%LOG_FILE%"
echo ================================================== >> "%LOG_FILE%"

:: 1. Setup the release folder variable from the first argument (%1) passed by VS
set "releaseFolder=%~1"

:: Fallback if running manually outside of VS (removes trailing slash if present)
if "%releaseFolder%"=="" set "releaseFolder=F:\FoneCall_15_OCT\FoneCall\bin\Release"
if "%releaseFolder:~-1%"=="\" set "releaseFolder=%releaseFolder:~0,-1%"
echo Release folder target is: %releaseFolder% >> "%LOG_FILE%"

set "ISCC_EXE=ISCC.exe"
if exist "%ProgramFiles(x86)%\Inno Setup 7\ISCC.exe" set "ISCC_EXE=%ProgramFiles(x86)%\Inno Setup 7\ISCC.exe"
if exist "%ProgramFiles%\Inno Setup 7\ISCC.exe" set "ISCC_EXE=%ProgramFiles%\Inno Setup 7\ISCC.exe"
if not exist "%ISCC_EXE%" (
    if exist "%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" set "ISCC_EXE=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
)
echo Running ISCC FoneCallGold.iss... >> "%LOG_FILE%"

:: 2. Run ISCC and redirect both stdout (1) and stderr (2) to the log file
"%ISCC_EXE%" FoneCallGold.iss "/DappName=FoneCall" "/DreleaseFolder=%releaseFolder%" >> "%LOG_FILE%" 2>&1

set "ISCC_EXIT=%ERRORLEVEL%"
echo ISCC finished with exit code: %ISCC_EXIT% >> "%LOG_FILE%"

:: Display the log contents in the console window so you can still read it live
type "%LOG_FILE%"

:: Only pause when running locally (GitHub Actions automatically sets GITHUB_ACTIONS=true)
if /i not "%GITHUB_ACTIONS%"=="true" (
    pause
)

exit /b %ISCC_EXIT%
