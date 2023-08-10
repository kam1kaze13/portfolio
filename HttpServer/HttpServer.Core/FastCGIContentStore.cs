using System;
using System.IO;
using System.Linq;
using FastCgiNet;
using FastCgiNet.Streams;
using FastCgiNet.Requests;
using System.Net.Sockets;
using System.Net;

namespace HttpServer.Core
{
    public class FastCGIContentStore : IContentStore
    {
        public IContent GetContent(string path)
        {
            return new FastCGIContent();
        }

        private class FastCGIContent : IContent
        {
            private CustomHttpResponse response;

            public CustomHttpResponse GetResponse(CustomHttpRequest request)
            {
                this.response = new CustomHttpResponse();

                var requestedUrl = new Uri(request.Host + request.RequestUri);
                string requestMethod = request.Method;

                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(new IPEndPoint(IPAddress.Loopback, 9000));

                ushort requestId = 5172;
                using (var webRequest = new WebServerSocketRequest(sock, requestId))
                {
                    webRequest.SendBeginRequest(Role.Responder, true);

                    using (var nvpWriter = new NvpWriter(webRequest.Params))
                    {
                        nvpWriter.WriteParamsFromUri(requestedUrl, requestMethod);
                    }

                    webRequest.SendEmptyStdin();

                    int bytesRead;
                    byte[] buf = new byte[4096];
                    while (!webRequest.ResponseComplete)
                    {
                        bytesRead = sock.Receive(buf, SocketFlags.None);
                        webRequest.FeedBytes(buf, 0, bytesRead).ToList();
                    }

                    using (var reader = new StreamReader(webRequest.Stdout))
                    {
                        this.response.Body = reader.ReadToEnd();
                    }
                }

                return this.response;
            }
        }
    }
}
