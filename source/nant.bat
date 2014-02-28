@echo off
"tools\nant\NAnt.exe" -D:platform=win -targetframework:net-4.0 %*
