using Gryphon.HttpServer.Core;
using Microsoft.ClearScript.V8;
using System.Text;

namespace Gryphon.HttpServer.Test
{
    public class ScriptEngineTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleScriptTest()
        {
            var jsEngine = new V8ScriptEngine();

            string exec =
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
Response.Write("<html><head>	<title>		");
Response.Write("	</title></head><body>");
for (let i=0;i<10;i++){Response.Write("<p>	");
Response.Write(i);
Response.Write("</p>");
}Response.Write("</body><div style=\"sdsd\"></html>");
""";

            jsEngine.ExecuteCommand(exec);

            var x = jsEngine.Script.Response.Output();
        }

        [Test]
        public void GetTitleTest()
        {
            var jsEngine = new V8ScriptEngine();

            string exec =
"""
function GetTitle() {
    return "title";
}
""";

            jsEngine.ExecuteCommand(exec);

            var x = jsEngine.Script.GetTitle();
        }

        [Test]
        public void ReadFromFileTest()
        {
            var jsEngine = new V8ScriptEngine();

            StreamReader stream = new StreamReader(System.IO.File.OpenRead("ourfile.xxx"), Encoding.UTF8);

            var templateEngine = new KantTemplateEngine
            {
                OuterStart = "Response.Write(\"",
                OuterEnd = "\");\n",
                InnerStart = "Response.Write(",
                InnerEnd = ");\n"
            };

            var template = templateEngine.Process(stream.ReadToEnd());

            string exec =
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
    return "title";
}
""";
            var temp = template.Replace("\r\n", "");
            var script = exec + "\r\n" + temp;
            jsEngine.ExecuteCommand(script);

            var x = jsEngine.Script.Response.Output();
        }

        [Test]
        public void ReadyScriptTest()
        {
            var jsEngine = new ScriptEngine();

            StreamReader stream = new StreamReader(System.IO.File.OpenRead("ourfile.xxx"), Encoding.UTF8);

            var templateEngine = new KantTemplateEngine
            {
                OuterStart = "Response.Write(\"",
                OuterEnd = "\");\n",
                InnerStart = "Response.Write(",
                InnerEnd = ");\n"
            };

            var template = templateEngine.Process(stream.ReadToEnd());

            var x = jsEngine.RunScript(template);
        }
    }
}