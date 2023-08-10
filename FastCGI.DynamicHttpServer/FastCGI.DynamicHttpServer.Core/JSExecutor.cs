using Microsoft.ClearScript.V8;

namespace FastCGI.DynamicHttpServer.Core
{
    public class JSExecutor
    {
        private V8ScriptEngine jsEngine;

        public JSExecutor()
        {
            this.jsEngine = new V8ScriptEngine();
        }

        public string RunScript(string script)
        {
            return jsEngine.ExecuteCommand(script);
        }
    }
}