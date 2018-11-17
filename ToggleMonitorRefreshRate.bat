@echo off

echo --- Toggle Monitor Refresh Rate ---
echo.

"%~dp0ToggleMonitorRefreshRate.exe" 100

echo.
echo Launching app...
echo.

%1

echo App exited?
pause

"%~dp0ToggleMonitorRefreshRate.exe" 60

echo.
echo Done.
timeout /t 3 > nul
