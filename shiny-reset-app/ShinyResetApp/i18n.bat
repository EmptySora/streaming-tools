cd en-GB
SET "bindir=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools"
"%bindir%\resgen" Resources.en-GB.resx
"%bindir%\al" /embed:Resources.en-GB.resources /c:en-GB /out:ShinyResetApp.resources.dll
del Resources.en-GB.Resources  2>&1 >nul
mkdir "../%1/en-GB/" 2>&1 >nul
move "ShinyResetApp.resources.dll" "../%1/en-GB/ShinyResetApp.resources.dll"  2>&1 >nul