cls

mkdir c:\dumpit\SilveR-Electron
xcopy SilveR\*.* c:\dumpit\SilveR-Electron /sy

cd c:\dumpit\SilveR-Electron

electronize build /target win /electron-params "--icon=\"C:\Dumpit\SilveR-Electron\IVS.ico\" --name=InVivoStat"
ren c:\dumpit\SilveR-Electron\bin\desktop\electron.net.host-win32-x64\electron.net.host.exe InVivoStat.exe
pause
7z a -tzip -mx9 c:\dumpit\InVivoStat-Win-Full.zip c:\dumpit\SilveR-Electron\bin\desktop\electron.net.host-win32-x64\*

c:\dumpit\SilveR-Electron\bin\desktop\electron.net.host-win32-x64\InVivoStat.exe

pause
xcopy /y c:\dumpit\InVivoStat-Win-Full.zip \\floodiis\wwwroot\FloodLive\wwwroot\fileshare
erase c:\dumpit\InVivoStat-Win-Full.zip



electronize build /target linux /electron-params "--name=InVivoStat --icon=\"./bin/IVS.ico\""
ren c:\dumpit\SilveR-Electron\bin\desktop\electron.net.host-linux-x64\electron.net.host InVivoStat
7z a -tzip -mx9 c:\dumpit\InVivoStat-Linux-Full.zip c:\dumpit\SilveR-Electron\bin\desktop\electron.net.host-linux-x64\*

xcopy /y c:\dumpit\InVivoStat-Linux-Full.zip \\floodiis\wwwroot\FloodLive\wwwroot\fileshare
erase c:\dumpit\InVivoStat-Linux-Full.zip

pause
rem rmdir /s /q c:\dumpit\SilveR-Electron
rem rmdir /s /q c:\dumpit\SilveR-Electron



