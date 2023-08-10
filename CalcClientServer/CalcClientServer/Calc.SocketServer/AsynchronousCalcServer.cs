namespace Calc.SocketServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using Protocol;

    public class AsynchronousCalcServer<TRequest, TResponse> : ICalcServer
    {
        private const int BufferSize = 10240;

        private readonly int port;
        private readonly int listenerCount;
        private readonly Func<TRequest, TResponse> handler;

        private readonly IProtocolExecutor<TRequest, TResponse> protocol;

        private Thread listenerThread;

        private ManualResetEvent socketAccepted = new ManualResetEvent(false);
        private bool stopped = false;

        public AsynchronousCalcServer(int port, int listenerCount, IProtocolExecutor<TRequest, TResponse> protocol, Func<TRequest, TResponse> handler)
        {
            this.port = port;
            this.listenerCount = listenerCount;
            this.protocol = protocol;
            this.handler = handler;
        }

        public void Start()
        {
            // стартуем нить, которая будет принимать соединения от клиентов
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
                // TODO: конструируем endpoint для локальной машины
                // IPAddress.Parse
                // new IPEndPoint(ip, port -> инициализируется в конструкторе)
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.port);

                using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                                          {
                                              ReceiveTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                                              SendTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                                              Blocking = false,
                                          })
                {
                    // TODO: просоединяем сокет для локальной машины
                    // Socket.Bind
                    // Socket.Listen -> количество слушателей инициализируется в конструкторе
                    listener.Bind(localEndPoint);
                    listener.Listen(listenerCount);

                    Console.WriteLine("Waiting for a connection...");

                    // в бесконечном цикле ожидаем соединения
                    do
                    {
                        // сбрасываем флаг перед получением клиентских соединений
                        this.socketAccepted.Reset();

                        // TODO: начинаем получать клиентские соединения
                        // listener.BeginAccept
                        // обрабатываем окончание получения в this.AcceptCallback
                        listener.BeginAccept(this.AcceptCallback, listener);

                        // исполнение прерывается, пока не будет получено соединение от пользователя
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

                // сигнал для нити прослушивания продолжить работу
                this.socketAccepted.Set();

                // получаем сокет, который вызвал callback
                var listener = (Socket)ar.AsyncState;
                var socket = listener.EndAccept(ar);

                // создаем объект состояния для работы с сокетом
                var state = new ReceivingState(socket);

                // TODO: начинаем получать данные из сокета
                // socket.BeginReceive
                // обрабатываем окончание получения в this.ReceiveCallback
                // как объект состояния передаем state

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

            // TODO: заканчиваем читать данные из сокета
            // int bytesReceived = state.Socket.EndReceive;
            int bytesReceived = state.Socket.EndReceive(ar);

            if (bytesReceived > 0)
            {
                state.Offset += bytesReceived;

                // парсим полученные данные
                var result = this.protocol.ParseInput(state.Buffer, state.Offset);

                // если какие-то запросы успешно распарсились
                if (result.ParsedObjects.Count > 0)
                {
                    foreach (var request in result.ParsedObjects)
                    {
                        // то получаем ответ для каждого запроса и отправляем его обратно
                        var response = this.handler(request);

                        // TODO: посылаем ответ
                        // ProtocolExecutor.CreateResponse
                        // this.SendResponse
                        this.SendResponse(state.Socket, protocol.CreateResponse(response));
                    }
                }

                // если сессия закрыта, то уходим
                if (result.IsClosed)
                {
                    // TODO: закрываем клиентский сокет
                    // state.Socket.Shutdown
                    state.Socket.Shutdown(SocketShutdown.Both);
                    return;
                }

                // удаляем те данные, которые уже были распарсены
                if (result.ProcessedBytes > 0)
                {
                    if (state.Offset > result.ProcessedBytes)
                        Array.Copy(state.Buffer, result.ProcessedBytes, state.Buffer, 0, state.Offset - result.ProcessedBytes);

                    state.Offset -= result.ProcessedBytes;
                }

                // продолжаем получать данные, если сессия не закрыта
                state.Socket.BeginReceive(
                    state.Buffer, state.Offset, state.Buffer.Length - state.Offset, SocketFlags.None, this.ReceiveCallback, state);
            }
            else
            {
                // TODO: закрываем клиентский сокет
                // state.Socket.Shutdown
                state.Socket.Shutdown(SocketShutdown.Both);
            }
        }

        private void SendResponse(Socket socket, byte[] response)
        {
            // TODO: начинаем посылать ответ
            // socket.BeginSend
            // обрабатываем окончание получения в this.SendCallback
            // как объект состояния передаем сам сокет
            socket.BeginSend(response, 0, response.Length, SocketFlags.None, this.SendCallback, socket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // получаем сокет как объект состояния
                var socket = (Socket)ar.AsyncState;

                // TODO: завершаем посылку ответа с сервера
                // socket.EndSend
                socket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
