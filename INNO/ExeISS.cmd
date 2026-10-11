echo starting post build batch file
echo set working directory
echo cd /d "%~dp0"
cd /d "%~dp0"

echo Windows path must include C:\Program Files\Inno Setup 7\
echo running ISCC FoneCallGold.iss "/DappName=FoneCall" "/DreleaseFolder="E:\FoneCall\FoneCall\bin\Release""

ISCC FoneCallGold.iss "/DappName=FoneCall" "/DreleaseFolder="E:\FoneCall\FoneCall\bin\Release""
pause
