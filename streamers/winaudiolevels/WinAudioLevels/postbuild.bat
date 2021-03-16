@ECHO OFF
SETLOCAL
IF NOT EXIST "WinAudioLevels.exe" (
    GOTO :NotRightDirectory
) ELSE (
    GOTO :Start
)
:NotRightDirectory
ECHO The current directory ("%cd%")
ECHO does not contain "WinAudioLevels.exe". This is likely an error.
ECHO As such, the post build script will now terminate.
GOTO :EOF

:Start
ECHO ^>^>^> Current Configuration is: %1
IF "%1"=="Release" (
    GOTO :Release
) ELSE (
    GOTO :Debug
)

:Release
ECHO ^>^>^> Deleting debug files...
CALL :DeleteReleaseFiles
ECHO ^>^>^> Finished deleting debug files...
GOTO :BuildCommon

:Debug
ECHO ^>^>^> Not deleting debug files.
GOTO :BuildCommon

:BuildCommon
ECHO.
ECHO ^>^>^> Moving CefSharp files...
CALL :MoveCefSharp
ECHO ^>^>^> Creating Archive...
CALL :CreateArchive
ECHO ^>^>^> Done Creating Archive.
GOTO :EOF

:---------------------------------------------------------------
:-- DeleteReleaseFiles - Deletes the unwanted release files.  --
:---------------------------------------------------------------
:DeleteReleaseFiles
SETLOCAL
:-- DO NOT DELETE CONFIG FILE :(
:-- CALL :DeleteFile "WinAudioLevels.exe.config" 
CALL :DeleteFile "WinAudioLevels.pdb"
CALL :DeleteFile "CefSharp.Core.pdb"
CALL :DeleteFile "CefSharp.Core.Runtime.pdb"
CALL :DeleteFile "CefSharp.Core.Runtime.xml"
CALL :DeleteFile "CefSharp.Core.xml"
CALL :DeleteFile "CefSharp.pdb"
CALL :DeleteFile "CefSharp.WinForms.pdb"
CALL :DeleteFile "CefSharp.WinForms.xml"
CALL :DeleteFile "CefSharp.xml"
CALL :DeleteFile "IcronOcr.xml"
CALL :DeleteFile "Microsoft.Win32.Registry.xml"
CALL :DeleteFile "NAudio.xml"
CALL :DeleteFile "Newtonsoft.Json.xml"
CALL :DeleteFile "System.Security.AccessControl.xml"
CALL :DeleteFile "System.Security.Principal.Windows.xml"
CALL :DeleteFile "websocket-sharp.xml"
CALL :DeleteFile "x64\CefSharp.BrowserSubprocess.Core.pdb"
CALL :DeleteFile "x64\CefSharp.BrowserSubprocess.pdb"
CALL :DeleteFile "x64\CefSharp.Core.pdb"
CALL :DeleteFile "x64\CefSharp.Core.Runtime.pdb"
CALL :DeleteFile "x64\CefSharp.Core.Runtime.xml"
CALL :DeleteFile "x64\CefSharp.Core.xml"
CALL :DeleteFile "x64\CefSharp.pdb"
CALL :DeleteFile "x64\CefSharp.xml"
CALL :DeleteFile "x64\README.txt"
CALL :DeleteFile "x86\CefSharp.BrowserSubprocess.Core.pdb"
CALL :DeleteFile "x86\CefSharp.BrowserSubprocess.pdb"
CALL :DeleteFile "x86\CefSharp.Core.pdb"
CALL :DeleteFile "x86\CefSharp.Core.Runtime.pdb"
CALL :DeleteFile "x86\CefSharp.Core.Runtime.xml"
CALL :DeleteFile "x86\CefSharp.Core.xml"
CALL :DeleteFile "x86\CefSharp.pdb"
CALL :DeleteFile "x86\CefSharp.xml"
CALL :DeleteFile "x86\README.txt"

GOTO :EOF






:-----------------------------------------------
:-- DeleteFile - Deletes the specified file.  --
:-- @param {string} file - The file to delete --
:-----------------------------------------------
:DeleteFile
SETLOCAL
DEL "%~1" 2>&1 1>nul | FIND /V "" 1>nul 2>&1
IF "%errorlevel%"=="1" (
    GOTO :DeleteFileSuccess
) ELSE (
    GOTO :DeleteFileFailure
)
:DeleteFileSuccess
ECHO Removed "%~1"
GOTO :EOF
:DeleteFileFailure
ECHO Couldn't find "%~1"
GOTO :EOF

:-----------------------------------------------------------------
:-- CreateArchive - Creates an archive of the built application --
:-----------------------------------------------------------------
:CreateArchive
SETLOCAL
IF EXIST "WinAudioLevels.7z" (
    DEL "WinAudioLevels.7z"
)
"C:\Program Files\7-Zip\7z.exe" a -y WinAudioLevels.7z -x!settings.json -x!*.log -x!domain.* -x!WinAudioLevels.7z "*.*" "*\*.*" "*\*\*.*" "*\*\*\*.*"  >nul
GOTO :EOF


:---------------------------------------------------------------
:-- Move CefSharp - Moves CefSharp files to a subfolder       --
:---------------------------------------------------------------
:MoveCefSharp
SETLOCAL
MKDIR "cef" >nul
MKDIR "cef\locales" >nul
MKDIR "cef\swiftshader" >nul
DEL "cef\locales\*.*" >nul
DEL "cef\swiftshader\*.*" >nul
DEL "cef\*.*" >nul
MOVE "cef.pak" "cef" >nul
MOVE "cef_100_percent.pak" "cef" >nul
MOVE "cef_200_percent.pak" "cef" >nul
MOVE "cef_extensions.pak" "cef" >nul
MOVE "CefSharp.BrowserSubprocess.Core.*" "cef" >nul
MOVE "CefSharp.BrowserSubprocess.*" "cef" >nul
MOVE "CefSharp.Core.Runtime.*" "cef" >nul
MOVE "CefSharp.Core.*" "cef" >nul
MOVE "CefSharp.WinForms.*" "cef" >nul
MOVE "CefSharp.*" "cef" >nul
MOVE "chrome_elf.dll" "cef" >nul
MOVE "d3dcompiler_47.dll" "cef" >nul
MOVE "devtools_resources.pak" "cef" >nul
MOVE "icudtl.dat" "cef" >nul
MOVE "libcef.dll" "cef" >nul
MOVE "libEGL.dll" "cef" >nul
MOVE "libGLESv2.dll" "cef" >nul
MOVE "LICENSE.txt" "cef" >nul
MOVE "README.txt" "cef" >nul
MOVE "snapshot_blob.bin" "cef" >nul
MOVE "v8_context_snapshot.bin" "cef" >nul
MOVE "locales\*.*" "cef\locales" >nul
MOVE "swiftshader\*.*" "cef\swiftshader" >nul
RMDIR "locales" >nul
RMDIR "swiftshader" >nul

GOTO :EOF