cls

dotnet clean
dotnet restore

rmdir /s /q c:\dumpit\Silver-win
rmdir /s /q c:\dumpit\Silver-linux
rmdir /s /q c:\dumpit\Silver-osx

dotnet publish -c Release -r win-x64 -o c:\dumpit\Silver-win
REM XCOPY ".\R-3.5.1\*.*" "c:\dumpit\Silver-win\R-3.5.1\*.*" /d /y /s

dotnet publish -c Release -r linux-x64 -o c:\dumpit\Silver-linux
dotnet publish -c Release -r osx-x64 -o c:\dumpit\Silver-osx

start "" "c:\dumpit\Silver-win\InvivoStat.exe"
ping 127.0.0.1 -n 6 > nul
start "" "http://localhost:5000"

7z a -tzip -mx9 c:\dumpit\InVivoStat-Win.zip c:\dumpit\Silver-win\*
7z a -tzip -mx9 c:\dumpit\InVivoStat-Linux.zip c:\dumpit\Silver-linux\*
7z a -tzip -mx9 c:\dumpit\InVivoStat-OSx.zip c:\dumpit\Silver-osx\*



pause
xcopy /y c:\dumpit\InVivoStat-Win.zip \\floodiis\wwwroot
xcopy /y c:\dumpit\InVivoStat-Linux.zip \\floodiis\wwwroot
xcopy /y c:\dumpit\InVivoStat-OSx.zip \\floodiis\wwwroot

xcopy /y c:\dumpit\InVivoStat-Win.zip \\floodiis\wwwroot\FloodLive\wwwroot\fileshare
xcopy /y c:\dumpit\InVivoStat-Linux.zip \\floodiis\wwwroot\FloodLive\wwwroot\fileshare
xcopy /y c:\dumpit\InVivoStat-OSx.zip \\floodiis\wwwroot\FloodLive\wwwroot\fileshare
