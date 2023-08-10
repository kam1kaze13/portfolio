using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gryphon.HttpServer.Core
{
    public static class ServiceObjects
    {
        public static string CreateServiceObjects()
        {
            string request = @"{
    Request: {
        Method : " + Environment.GetEnvironmentVariable("REQUEST_METHOD") + @"
        URL: " + Environment.GetEnvironmentVariable("HTTP_HOST") + Environment.GetEnvironmentVariable("SCRIPT_NAME") + @"
    }
}";

            string server = @"{
    Server: {
        Name : " + Environment.GetEnvironmentVariable("SERVER_NAME") + @"
        Protocol : " + Environment.GetEnvironmentVariable("SERVER_PROTOCOL") + @"
        Software: " + Environment.GetEnvironmentVariable("SERVER_SOFTWARE") + @"
    }
}";

            return request + "\n" + server;
        }

        public static string CreateHttpResponse(int contentLength)
        {
            string request =
    @"
Connection: close
Content-Length: " + contentLength + @"
Server: " + Environment.GetEnvironmentVariable("SERVER_SOFTWARE") + @"
";

            return request;
        }
    }  
}
