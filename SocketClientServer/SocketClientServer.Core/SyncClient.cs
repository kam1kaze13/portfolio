using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientServer.Core
{
    public class SyncClient<TRequest, TResponse> : IDisposable
    {
        private const int BufferSize = 10240;

        private readonly IProtocolExecutor<TRequest, TResponse> protocol;
        private readonly Socket socket;
        private readonly IPEndPoint endPoint;
        private bool isConnected = false;

        public SyncClient(IPEndPoint endPoint, IProtocolExecutor<TRequest, TResponse> protocol)
        {
            this.protocol = protocol;
            this.endPoint = endPoint;

            // присоединяемся к заданному Endpoint
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);            
        }

        public TResponse DoOperation(TRequest request)
        {
            if (!this.isConnected)
            {
                this.socket.Connect(this.endPoint);
                this.isConnected = true;
            }

            byte[] binaryRequest = this.protocol.CreateRequest(request);

            this.socket.Send(binaryRequest);

            var buffer = new byte[BufferSize];

            int offset = 0;
            int bytesReceived;

            do
            {
                bytesReceived = this.socket.Receive(buffer, BufferSize, SocketFlags.None);

                var result = this.protocol.ParseOutput(buffer, offset, offset + bytesReceived);

                offset += result.ProcessedBytes;

                if (result.ParsedObjects.Count > 0)
                {
                    return result.ParsedObjects.First();
                }
            }
            while (bytesReceived > 0);

            throw new ApplicationException("Unexpected end of server response");
        }

        public void Dispose()
        {
            this.socket.Send(new[] { (byte)';' });
            this.socket.Shutdown(SocketShutdown.Both);
            this.socket.Close();
        }
    }
}
