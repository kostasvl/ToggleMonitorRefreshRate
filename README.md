# ToggleMonitorRefreshRate
Utility for automatically toggling your monitor's refresh rate / frequency between 60Hz and some max value (defaults to 100Hz).

Example usage:

    ToggleMonitorRefreshRate.exe 144   (<-- will toggle between 60 and 144)
    ToggleMonitorRefreshRate.exe       (<-- will toggle between 60 and 100)

Just download the prebuilt exe, or build it yourself.

You can also download the batch file (ToggleMonitorRefreshRate.bat) and keep it next to the executable. This will allow you to launch most games/programs, while first automatically switching to the higher refresh rate. Simply drag the shortcut to the game and drop it on the batch file. You should probably modify the batch file first (it's just text), to adjust the specified refresh rate to match the one you want. Unfortunately, a lot of game shortcuts go through various kinds of launchers these days, so the batch file cannot know for sure when the game has exited. Therefore, it pauses after launching the game. When you exit the game, click on the command prompt and press any key to have it switch you back to 60Hz.
