cls

dotnet clean
dotnet restore

rmdir /s /q c:\dumpit\InVivoStat-win

dotnet publish -c Release -r win-x64 -o c:\dumpit\InVivoStat-win
XCOPY ".\R-3.5.1\*.*" "c:\dumpit\InVivoStat-win\R-3.5.1\*.*" /d /y /s

erase c:\dumpit\InVivoStat-win\InVivoStat.db

XCOPY "C:\Users\robal\OneDrive\Documents\Visual Studio 2013\Projects\InVivoStat-Launcher\InVivoStat\bin\Release\*.*" -o c:\dumpit\InVivoStat-win
