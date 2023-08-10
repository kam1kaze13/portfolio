using Gryphon.HttpServer.Core;
using System.Text;

var pathTranslated = Environment.GetEnvironmentVariable("PATH_TRANSLATED");
var scriptName = Environment.GetEnvironmentVariable("SCRIPT_NAME");

var fileUrl = pathTranslated + scriptName;

StreamReader stream = new StreamReader(System.IO.File.OpenRead(fileUrl), Encoding.UTF8);

var response = new Response();
var templateEngine = new KantTemplateEngine { 
    OuterStart = "Response.Write(\"",
    OuterEnd = "\")\n",
    InnerStart = "Response.Write(",
    InnerEnd = ")\n" };

var template = templateEngine.Process(stream.ReadToEnd());

var scriptEngine = new ScriptEngine();

var script = scriptEngine.RunScript(template);

var httpResponse = ServiceObjects.CreateHttpResponse(script.Length);

response.Write(httpResponse.Length.ToString());
response.Write(httpResponse);
response.Write(script);

response.GetResponse();