using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClientServer.Core
{
    public class SyncServer<TRequest, TResponse> : IServer
    {
        private const int BufferSize = 10240;

        private static readonly TimeSpan SocketTimeout = TimeSpan.FromSeconds(5);

        private readonly int port;
        private readonly int listenerCount;
        private readonly Func<TRequest, TResponse> handler;

        private readonly IProtocolExecutor<TRequest, TResponse> protocol;

        private Thread listenerThread;

        public SyncServer(int port, int listenerCount, IProtocolExecutor<TRequest, TResponse> protocol, Func<TRequest, TResponse> handler)
        {
            this.port = port;
            this.listenerCount = listenerCount;
            this.protocol = protocol;
            this.handler = handler;
        }

        public void Start()
        {
            this.listenerThread = new Thread(this.StartPortListener);
            this.listenerThread.Start();
        }

        public void Stop()
        {
        }

        private void StartPortListener()
        {
            try
            {

                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.port);

                using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    Blocking = false,
                })
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(listenerCount);

                    Console.WriteLine("Waiting for a connection...");

                    while (true)
                    {
                        var sockets = NonblockingSocketUtil.Accept(listener);

                        foreach (var socket in sockets)
                        {
                            Console.WriteLine("Got a connection");

                            var localSocket = socket;

                            this.ProcessClient(localSocket);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ProcessClient(Socket socket)
        {
            using (socket)
            {
                var buffer = new byte[BufferSize];

                int offset = 0;

                do
                {
                    int bytesReceived = NonblockingSocketUtil.Receive(socket, buffer, offset, buffer.Length - offset, SocketFlags.None, SocketTimeout);

                    offset += bytesReceived;

                    var result = this.protocol.ParseInput(buffer, offset, buffer.Length - offset);

                    if (result.ParsedObjects.Count > 0)
                    {
                        foreach (var request in result.ParsedObjects)
                        {
                            var response = this.handler(request);

                            socket.Send(this.protocol.CreateResponse(response));
                        }
                    }

                    if (result.IsClosed)
                    {
                        break;
                    }

                    if (result.ProcessedBytes > 0)
                    {
                        if (offset > result.ProcessedBytes)
                            Array.Copy(buffer, result.ProcessedBytes, buffer, 0, offset - result.ProcessedBytes);

                        offset -= result.ProcessedBytes;
                    }
                }
                while (true);

                socket.Shutdown(SocketShutdown.Both);
            }
        }
    }
}
