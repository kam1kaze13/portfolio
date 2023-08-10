using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gryphon.HttpServer.Core
{
    public class ScriptEngine
    {
        private V8ScriptEngine jsEngine;

        private string execCommand =
"""
var Response = {
    body : "",
    Write : function (str) {
        Response.body += str;
    },
    Output : function () {
        return Response.body;
    }
}
function GetTitle() {
    return "Apptime";
}
""";

        public ScriptEngine() 
        { 
            this.jsEngine = new V8ScriptEngine();
        }

        public string RunScript(string template)
        {
            var temp = template.Replace("\r\n", "");
            var script = execCommand + "\r\n" + temp;
            jsEngine.ExecuteCommand(script);
            return jsEngine.Script.Response.Output();
        }
    }
}
