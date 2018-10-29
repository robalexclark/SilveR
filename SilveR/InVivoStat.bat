@ECHO Launching InVivoStat...
@start "" SilveR.exe InVivoStat
@ping 127.0.0.1 -n 6 > nul
@start "" /B "http://localhost:5000"
