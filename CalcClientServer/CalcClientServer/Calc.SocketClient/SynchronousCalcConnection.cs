namespace Calc.SocketClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    using Protocol;

    public class SynchronousCalcConnection<TRequest, TResponse> : IDisposable
    {
        private const int BufferSize = 10240;

        private readonly IProtocolExecutor<TRequest, TResponse> protocol;
        private readonly Socket socket;

        public SynchronousCalcConnection(IPEndPoint endPoint, IProtocolExecutor<TRequest, TResponse> protocol)
        {
            // присоединяемся к заданному Endpoint
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            this.socket.Connect(endPoint);
            this.protocol = protocol;
        }

        public TResponse DoOperation(TRequest request)
        {

            byte[] binaryRequest = this.protocol.CreateRequest(request);

            this.socket.Send(binaryRequest);

            byte[] buffer = new byte[BufferSize];

            while (this.socket.Receive(buffer, BufferSize, SocketFlags.None) > 0)
            {
                var result = this.protocol.ParseOutput(buffer, buffer.Length);

                if (result.ParsedObjects.Count > 0)
                {
                    return result.ParsedObjects.First();
                }               
            }

            throw new ApplicationException("Unexpected end of server response");
        }

        public void Dispose()
        {
            // посылаем финальный ";" для закрытия сессии
            this.socket.Send(new[] { (byte)';' });
            this.socket.Shutdown(SocketShutdown.Both);
            this.socket.Close();
        }
    }
}
