@echo off
echo Starting post build batch file
echo Setting working directory
cd /d "%~dp0"

:: 1. Setup the release folder variable from the first argument (%1) passed by VS
set "releaseFolder=%~1"

:: Fallback fallback if running manually outside of VS (removes trailing slash if present)
if "%releaseFolder%"=="" set "releaseFolder=F:\FoneCall_15_OCT\FoneCall\bin\Release"
if "%releaseFolder:~-1%"=="\" set "releaseFolder=%releaseFolder:~0,-1%"

echo Release folder target is: %releaseFolder%

echo Windows path must include C:\Program Files\Inno Setup 7
:: echo Running ISCC FoneCallGold_Spon.iss
echo Running ISCC FoneCallGold.iss

:: 2. Verified Inno Setup execution line with clean outer-quoting syntax
:: ISCC FoneCallGold_Spon.iss "/DappName=FoneCall" "/DreleaseFolder=%releaseFolder%"
ISCC FoneCallGold.iss "/DappName=FoneCall" "/DreleaseFolder=%releaseFolder%"
pause
