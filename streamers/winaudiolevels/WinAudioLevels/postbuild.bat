IF EXIST "..\..\..\postbuild.js" (
    CSCRIPT //nologo ..\..\..\postbuild.js
)
IF EXIST "..\..\postbuild.js" (
    CSCRIPT //nologo ..\..\postbuild.js
)