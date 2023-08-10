using SocketClientServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class HttpServer
    {
        private readonly IServer socketServer;
        private readonly HttpProtocolExecutor httpProtocol;
        private readonly int port;
        private readonly int listenerCount;
        private readonly int timeout;
        private readonly HttpServerEngine serverEngine;

        public HttpServer(int port, int listenerCount,int timeout, HttpProtocolExecutor httpProtocol, HttpServerEngine serverEngine)
        {
            this.socketServer = new AsyncServer<CustomHttpRequest, CustomHttpResponse>(port, listenerCount, httpProtocol, this.ProcessRequest);
            this.port = port;
            this.listenerCount = listenerCount;
            this.timeout = timeout;
            this.httpProtocol = httpProtocol;
            this.serverEngine = serverEngine;
        }

        public void Start()
        {
            this.socketServer.Start();
        }

        public void Stop()
        {
            this.socketServer.Stop();
        }

        public CustomHttpResponse ProcessRequest(CustomHttpRequest request)
        {
            return this.serverEngine.GetResponse(request);
        }
    }
}
