using Gryphon.HttpServer.Core;
using Microsoft.ClearScript.V8;
using System.Text;

namespace Gryphon.HttpServer.Test
{
    public class KantTemplateEngineTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InCorrectGetTitleTest()
        {            
            var templateEngine = new KantTemplateEngine
            {
                OuterStart = "Response.Write(\"",
                OuterEnd = "\");\n",
                InnerStart = "Response.Write(",
                InnerEnd = ");\n"
            };

            string data =
"""
<html>
<@=GetTitle()@
<html>
""";

            var template = templateEngine.Process(data);

            data =
"""
<html>
<@=GetTitle()>
<html>
""";
            
            template = templateEngine.Process(data);
        }
    }
}