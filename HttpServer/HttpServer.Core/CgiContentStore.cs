using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class CgiContentStore : IContentStore
    {
        public IContent GetContent(string path)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + path))
            {
                return new CgiContent();
            }
            else
            {
                return null;
            }
        }

        private class CgiContent : IContent
        {
            private CustomHttpResponse response;

            public CustomHttpResponse GetResponse(CustomHttpRequest request)
            {
                this.response = new CustomHttpResponse();

                var sb = new StringBuilder();
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "C:\\Users\\Александр\\Desktop\\apptime\\school2021.nyrkov\\Gryphon.HttpServer\\Gryphon.HttpServer.Console\\bin\\Debug\\net7.0\\Gryphon.HttpServer.Console.exe",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    Environment.SetEnvironmentVariable("PATH_TRANSLATED", Directory.GetCurrentDirectory());
                    Environment.SetEnvironmentVariable("SCRIPT_NAME", request.RequestUri);
                    Environment.SetEnvironmentVariable("SERVER_NAME", request.Host.Substring(0, request.Host.IndexOf(':')));
                    Environment.SetEnvironmentVariable("SERVER_PORT", request.Host.Substring(request.Host.IndexOf(':') + 1));
                    Environment.SetEnvironmentVariable("HTTP_CONNECTION", request.Connection);
                    Environment.SetEnvironmentVariable("SERVER_PROTOCOL", request.HttpVersion);
                    Environment.SetEnvironmentVariable("HTTP_HOST", request.Host);
                    Environment.SetEnvironmentVariable("REQUEST_METHOD", request.Method);
                    Environment.SetEnvironmentVariable("SERVER_SOFTWARE", "Apptime/1.0");

                    process.Start();

                    while (!process.StandardOutput.EndOfStream)
                    {
                        sb.AppendLine(process.StandardOutput.ReadLine());
                    }

                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                var httpProtocolExecutor = new HttpProtocolExecutor();
                var httpResponse = httpProtocolExecutor.ParseOutput(Encoding.UTF8.GetBytes(sb.ToString()), 0, sb.Length);

                this.response.Headers = httpResponse.ParsedObjects[0].Headers;
                this.response.Body = httpResponse.ParsedObjects[0].Body;

                return this.response;
            }
        }
    }
}
