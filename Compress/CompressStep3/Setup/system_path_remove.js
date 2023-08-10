var wsh_shell = WScript.CreateObject("WScript.Shell");
var path = wsh_shell.ExpandEnvironmentStrings(WScript.Arguments.Unnamed(0));

var newv = "";
var v = wsh_shell.RegRead("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment\\path");
for ( var i = new Enumerator(v.split(";")); !i.atEnd(); i.moveNext())
    if ( i.item().length && i.item().toUpperCase() != path.toUpperCase() )
        newv += i.item() + ";";

newv = newv.substring(0,newv.length-1);

if (newv.charAt(newv.length-1) == "\\")
    newv = newv.substring(0,newv.length-1);

if ( wsh_shell.Run("setx /M path \"" + newv + "\"",0,true) != 0 )
    WScript.Echo("setx failed.");