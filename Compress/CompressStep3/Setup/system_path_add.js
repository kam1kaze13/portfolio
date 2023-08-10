var wsh_shell = WScript.CreateObject("WScript.Shell");
var path = wsh_shell.ExpandEnvironmentStrings(WScript.Arguments.Unnamed(0));

var v = wsh_shell.RegRead("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment\\path");
for ( var i = new Enumerator(v.split(";")); !i.atEnd(); i.moveNext())
    if ( path.toUpperCase() == i.item().toUpperCase() )
WScript.Quit(0);
if ( v.charAt(v.length-1) != ";" )
    v += ";";
v += path;
if ( wsh_shell.Run("setx /M Path \"" + v + "\"",0,true) != 0 )
    WScript.Echo("setx failed.");