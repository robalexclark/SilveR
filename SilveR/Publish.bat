cls

dotnet clean
dotnet restore

rmdir /s /q c:\dumpit\Silver-win
rmdir /s /q c:\dumpit\Silver-linux
rmdir /s /q c:\dumpit\Silver-osx

dotnet publish -c Release -r win-x64 -o c:\dumpit\Silver-win
XCOPY ".\R-3.1.2\*.*" "c:\dumpit\Silver-win\R-3.1.2\*.*" /d /y /s

dotnet publish -c Release -r linux-x64 -o c:\dumpit\Silver-linux
dotnet publish -c Release -r osx-x64 -o c:\dumpit\Silver-osx

7z a -tzip -mx9 c:\dumpit\InVivoStat-Win.zip c:\dumpit\Silver-win\*
7z a -tzip -mx9 c:\dumpit\InVivoStat-Linux.zip c:\dumpit\Silver-linux\*
7z a -tzip -mx9 c:\dumpit\InVivoStat-OSx.zip c:\dumpit\Silver-osx\*

start "" "c:\dumpit\Silver-win\InvivoStat.exe"
ping 127.0.0.1 -n 6 > nul
start "" "http://localhost:5000"




