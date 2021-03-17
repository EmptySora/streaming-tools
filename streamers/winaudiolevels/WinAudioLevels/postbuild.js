//How do I JScript again...?
//All I remember is console cscript objects and activex controls that are so robust
//that you can actually load a dotnet app from jScript.
//Oh, and that JScript is so horribly out of date to the point that you can't even
//index a string.
//Oh, and a lot of the script objects use ".item(n)" instead of "[n]"

//IF SCRIPT FAILS TO RUN DUE TO INVALID CHAR AT 1,1, GO IN HXD AND REMOVE BOM
/**
 * @param {string} self
 * @param {string} search
 * @param {number} rawPos
 * @returns {boolean}
 */
function startsWith(self, search, rawPos) {
    var pos = rawPos > 0 ? rawPos | 0 : 0;
    return self.substring(pos, pos + search.length) === search;
}
/**
 * @param {string} self
 * @param {string} search
 * @param {number} this_len
 * @returns {boolean}
 */
function endsWith(self, search, this_len) {
    if (this_len === undefined || this_len > self.length) {
        this_len = self.length;
    }
    return self.substring(this_len - search.length, this_len) === search;
}
/**
 * @param {string} self
 * @returns {string}
 */
function trim(self) {
    return self.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
}
/**
 * @param {string} text
 *     The text to output
 * @param {boolean} nl
 *     False or omitted to write the current line terminator as well.
 */
function echo(text, nl) {
    if (!nl) {
        WScript.StdOut.WriteLine(text);
    } else {
        WScript.StdOut.Write(text);
    }
}

/**
 * @typedef {object} BuildSettings
 * @property {string} configuration
 *     The configuration, eg: "debug", "release"
 * @property {string} platform
 *     The platform, eg: "x86", "x64", or "any cpu"
 * @property {string} backpath
 *     The string that can be used to resolve to the
 *     directory above the "bin" directory.
 * @property {string} [version]
 *     The detected version of the application.
 *     If an error occurred, this will be null.
 */
/** 
 * @param {string} path
 *     The current directory
 * @returns {BuildSettings}
 *     The detected settings.
 */
function detectBuildSettings(path, fso) {
    var parts = path.split("\\");
    var config = parts[parts.length - 1];
    var platform = parts[parts.length - 2];
    var backpath = "..\\..\\";
    var version = null;
    if (platform.toLowerCase() === "bin") {
        platform = "Any CPU";
    } else {
        backpath += "..\\";
    }
    var asmFile = combinePath(combinePath(path, backpath), "Properties\\AssemblyInfo.cs");
    try {
        if (fso.FileExists(asmFile)) {
            var stream = fso.OpenTextFile(asmFile, 1, -2); //read, useDefault
            var line;
            while (!stream.AtEndOfStream) {
                line = trim(stream.ReadLine());
                if (startsWith(line, "[assembly: AssemblyVersion(\"")) {
                    line = line.substr("[assembly: AssemblyVersion(\"".length);
                    line = line.substr(0, line.length - 3);
                    version = line;
                    break;
                }
            }
        }
    } catch (e) {
        echo("ERROR DURING VERSION LOOKUP: " + e.message);
    }
    return {
        "configuration": config.toLowerCase(),
        "platform": platform.toLowerCase(),
        "backpath": combinePath(path, backpath),
        "version": version
    };
}
/**
 * @param {string} a
 * @param {string} b
 * @returns {string}
 */
function combinePath(a, b) {
    if (!endsWith(a, "\\")) {
        return a + ((!startsWith(b, "\\")) ? "\\" : "") + b;
    } else {
        return a + (startsWith(b, "\\") ? b.substr(1) : b);
    }
}
/**
 * @param {string} path
 *     The file to delete
 * @param {ActiveXObject} fso
 *     The FileSystemObject ActiveXObject.
 * @param {boolean} silent
 *     Whether or not to output messages to the console.
 * @returns {boolean}
 *     A value that indicates whether or not the file was successfully deleted.
 */
function deleteFile(path, fso, silent) {
    try {
        if (!fso.FileExists(path)) {
            if (!silent) {
                echo("Couldn't find " + path);
            }
            return false;
        }
        fso.DeleteFile(path, true);
        if (!silent) {
            echo("Removed " + path);
        }
        return true;
    } catch (e) {
        echo("ERROR DURING DELETE: " + e.message);
        return false;
    }
}
/**
 * @param {string} path
 *     The file to move
 * @param {string} dest
 *     The destination file name.
 * @param {ActiveXObject} fso
 *     The FileSystemObject ActiveXObject..
 * @param {boolean} silent
 *     Whether or not to output messages to the console.
 * @returns {boolean}
 *     A value that indicates whether or not the file was successfully moved.
 */
function moveFile(path, dest, fso, silent) {
    try {
        if (!fso.FileExists(path)) {
            if (!silent) {
                echo("Couldn't find " + path);
            }
            return false;
        }
        if (fso.FileExists(dest)) {
            deleteFile(dest, fso, true);
        }
        fso.MoveFile(path, dest);
        if (!silent) {
            echo("Moved " + path);
        }
        return true;
    } catch (e) {
        echo("ERROR DURING MOVE: " + e.message);
        return false;
    }
}

function main() {
    var appver = "1.0.0.0";
    echo("PostBuild.js (" + appver + ")");
    var releaseFiles = [
        "WinAudioLevels.pdb", "CefSharp.Core.pdb", "CefSharp.Core.Runtime.pdb",
        "CefSharp.Core.Runtime.xml", "CefSharp.Core.xml", "CefSharp.pdb",
        "CefSharp.WinForms.pdb", "CefSharp.WinForms.xml", "CefSharp.xml",
        "Microsoft.Win32.Registry.xml", "NAudio.xml",
        "Newtonsoft.Json.xml", "System.Security.AccessControl.xml",
        "System.Security.Principal.Windows.xml", "websocket-sharp.xml",
        "x64\\CefSharp.BrowserSubprocess.Core.pdb", "x64\\CefSharp.BrowserSubprocess.pdb",
        "x64\\CefSharp.Core.pdb", "x64\\CefSharp.Core.Runtime.pdb", "x64\\CefSharp.Core.Runtime.xml",
        "x64\\CefSharp.Core.xml", "x64\\CefSharp.pdb", "x64\\CefSharp.xml", "x64\\README.txt",
        "x86\\CefSharp.BrowserSubprocess.Core.pdb", "x86\\CefSharp.BrowserSubprocess.pdb",
        "x86\\CefSharp.Core.pdb", "x86\\CefSharp.Core.Runtime.pdb", "x86\\CefSharp.Core.Runtime.xml",
        "x86\\CefSharp.Core.xml", "x86\\CefSharp.pdb", "x86\\CefSharp.xml", "x86\\README.txt"
    ];
    var cefPath = "cef";
    var cefFiles = ["cef.pak", "cef_100_percent.pak", "cef_200_percent.pak", "cef_extensions.pak",
        "CefSharp.BrowserSubprocess.Core.*", "CefSharp.BrowserSubprocess.*", "CefSharp.Core.Runtime.*",
        "CefSharp.Core.*", "CefSharp.WinForms.*", "CefSharp.*", "chrome_elf.dll", "d3dcompiler_47.dll",
        "devtools_resources.pak", "icudtl.dat", "libcef.dll", "libEGL.dll", "libGLESv2.dll", "LICENSE.txt",
        "README.txt", "snapshot_blob.bin", "v8_context_snapshot.bin"
    ];
    var cefFolders = ["locales", "swiftshader"];
    var cefWildcard = ["exe", "dll", "xml", "pdb"];
    var appname = "WinAudioLevels";
    var app = appname + ".exe";
    var fso = WScript.CreateObject("Scripting.FileSystemObject");
    var wsh = WScript.CreateObject("WScript.Shell");
    //C:\Users\Astaroth\source\repos\WinAudioLevels\WinAudioLevels\bin\x64\Debug
    var cd = wsh.CurrentDirectory;
    var i;
    var j;
    var f;
    var settings = detectBuildSettings(wsh.CurrentDirectory, fso);
    var ver = (settings.version === null ? "unknown" : settings.version);
    var szFile = "[" + settings.configuration.toUpperCase() + "][" + ver + "] " + appname + ".7z";
    var szArgs = "a -y \"" + szFile + "\" -x!settings.json -x!*.log -x!domain.* -x!*.7z \"*.*\" \"*\\*.*\" \"*\\*\\*.*\" \"*\\*\\*\\*.*\"";
    var szPath = "\"C:\\Program Files\\7-Zip\\7z.exe\"";
    var cmdBatch = "CMD /C DEL /F /Q \"" + combinePath(cd, "*.7z") + "\"";

    if (!fso.FileExists(app)) {
        echo("The current directory (" + cd + ")");
        echo("does not contain \"" + app + "\". This is likely an error.");
        echo("As such, the post build script will now terminate.");
        return;
    }
    echo(">>> Current configuration is:");
    echo("        Platform: " + settings.platform);
    echo("        Configuration: " + settings.configuration);
    echo("        Assembly Version: " + ver);
    if (settings.configuration === "release") {
        echo(">>> Deleting debug files...");
        for (i = 0; i < releaseFiles.length; i += 1) {
            f = releaseFiles[i];
            echo(">>>     Removing " + f, true);
            echo(" [" +
                (deleteFile(combinePath(cd, f), fso, true) ? "SUCCESS" : "FAILURE") +
                "]");
        }
        echo(">>> Finished deleting debug files...");
    } else {
        echo(">>> Not deleting debug files (current configuration is debug).");
    }
    echo(">>> Moving CefSharp files...");
    if (fso.FolderExists(combinePath(cd, cefPath))) {
        fso.DeleteFolder(combinePath(cd, cefPath), true);
    }
    fso.CreateFolder(combinePath(cd, cefPath));
    for (i = 0; i < cefFiles.length; i += 1) {
        f = cefFiles[i];
        if (endsWith(f, "*")) {
            f = f.substr(0, f.length - 1);
            for (j = 0; j < cefWildcard.length; j += 1) {
                if (fso.FileExists(f + cefWildcard[j])) {
                    echo(">>>    Moving " + f + cefWildcard[j], true);
                    echo(" [" +
                        (moveFile(combinePath(cd, f + cefWildcard[j]), combinePath(combinePath(cd, cefPath), f + cefWildcard[j]), fso, true) ? "SUCCESS" : "FAILURE") +
                        "]");
                }
            }
        } else {
            echo(">>>    Moving " + f, true);
            echo(" [" +
                (moveFile(combinePath(cd, f), combinePath(combinePath(cd, cefPath), f), fso, true) ? "SUCCESS" : "FAILURE") +
                "]");
        }
    }
    for (i = 0; i < cefFolders.length; i += 1) {
        f = cefFolders[i];
        if (fso.FolderExists(combinePath(combinePath(cd, cefPath), f))) {
            fso.DeleteFolder(combinePath(combinePath(cd, cefPath), f), true);
        }
        if (fso.FolderExists(combinePath(cd, f))) {
            echo(">>>    Moving " + f);
            fso.MoveFolder(combinePath(cd, f), combinePath(combinePath(cd, cefPath), f));
        }
    }
    echo(">>> Creating archive...");
    wsh.Run(cmdBatch, 0, true);
    var szRetCode = wsh.Run(szPath + " " + szArgs, 0, true);
    if (szRetCode !== 0) {
        echo(">>> Failed to create archive.");
    } else {
        echo(">>> Done creating archive.");
    }
    echo("Ignore the following \"Files were not copied\" error. They were copied, postbuild.js just moved them.");
    //WScript.FullName
    //WScript.Echo("arg","arg","arg")
    //WScript.Arguments.item(n)
    //WScript.Std[Err|Out|In].[Write[Line]|Read[All|Line]]

    //WScript.CreateObject("Scripting.FileSystemObject")
    //vs
    //new ActiveXObject("Scripting.FileSystemObject")
}






main();