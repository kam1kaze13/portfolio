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
    public class AsyncServer<TRequest, TResponse> : IServer
    {
        private const int BufferSize = 10240;

        private readonly int port;
        private readonly int listenerCount;
        private readonly Func<TRequest, TResponse> handler;

        private readonly IProtocolExecutor<TRequest, TResponse> protocol;

        private Thread listenerThread;

        private ManualResetEvent socketAccepted = new ManualResetEvent(false);
        private bool stopped = false;

        public AsyncServer(int port, int listenerCount, IProtocolExecutor<TRequest, TResponse> protocol, Func<TRequest, TResponse> handler)
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
            this.stopped = true;
            this.socketAccepted.Set();
        }

        private void StartPortListener()
        {
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.port);

                using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                    SendTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                    Blocking = false,
                })
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(listenerCount);

                    Console.WriteLine("Waiting for a connection...");

                    do
                    {
                        this.socketAccepted.Reset();

                        listener.BeginAccept(this.AcceptCallback, listener);

                        this.socketAccepted.WaitOne();
                    }
                    while (!this.stopped);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            if (this.stopped)
                return;

            try
            {
                Console.WriteLine("Got a connection");

                this.socketAccepted.Set();

                var listener = (Socket)ar.AsyncState;
                var socket = listener.EndAccept(ar);

                var state = new ReceivingState(socket);

                socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, this.ReceiveCallback, state);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private class ReceivingState
        {
            public Socket Socket { get; set; }

            public byte[] Buffer { get; set; }

            public int Offset { get; set; }

            public ReceivingState(Socket socket)
            {
                this.Socket = socket;
                this.Buffer = new byte[BufferSize];
                this.Offset = 0;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {

            var state = (ReceivingState)ar.AsyncState;

            int bytesReceived = state.Socket.EndReceive(ar);

            if (bytesReceived > 0)
            {
                state.Offset += bytesReceived;

                var result = this.protocol.ParseInput(state.Buffer, 0, state.Offset);

                if (result.ParsedObjects.Count > 0)
                {
                    foreach (var request in result.ParsedObjects)
                    {
                        var response = this.handler(request);

                        this.SendResponse(state.Socket, protocol.CreateResponse(response));
                    }
                }

                if (result.IsClosed)
                {
                    state.Socket.Shutdown(SocketShutdown.Both);
                    return;
                }

                if (result.ProcessedBytes > 0)
                {
                    if (state.Offset > result.ProcessedBytes)
                        Array.Copy(state.Buffer, result.ProcessedBytes, state.Buffer, 0, state.Offset - result.ProcessedBytes);

                    state.Offset -= result.ProcessedBytes;
                }

                state.Socket.BeginReceive(
                    state.Buffer, state.Offset, state.Buffer.Length - state.Offset, SocketFlags.None, this.ReceiveCallback, state);
            }
            else
            {
                state.Socket.Shutdown(SocketShutdown.Both);
            }
        }

        private void SendResponse(Socket socket, byte[] response)
        {
            socket.BeginSend(response, 0, response.Length, SocketFlags.None, this.SendCallback, socket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var socket = (Socket)ar.AsyncState;

                socket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
