namespace Calc.SocketServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Protocol;

    public class SynchronousCalcServer<TRequest, TResponse> : ICalcServer
    {
        private const int BufferSize = 10240;

        private static readonly TimeSpan SocketTimeout = TimeSpan.FromSeconds(5);

        private readonly int port;
        private readonly int listenerCount;
        private readonly Func<TRequest, TResponse> handler;

        private readonly IProtocolExecutor<TRequest, TResponse> protocol;

        private Thread listenerThread;

        public SynchronousCalcServer(int port, int listenerCount, IProtocolExecutor<TRequest, TResponse> protocol, Func<TRequest, TResponse> handler)
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
                                              // Внимание: неблокирующийся сокет!
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
                    while (true)
                    {
                        // исполнение прерывается, пока не будет получено соединение от пользователя

                        // для того, чтобы unit тесты могли завершаться
                        ////var sockets = new [] { listener.Accept() };

                        //var sockets = new[] { listener.Accept() };
                        var sockets = NonblockingSocketUtil.Accept(listener);                        

                        foreach (var socket in sockets)
                        {
                            Console.WriteLine("Got a connection");

                            // необходимо инициализировать локальную переменную, чтобы в замыкание попало значение из цикла
                            // не является необходимым для .net 4.5
                            var localSocket = socket;

                            // TODO: для каждого сокета запускаем нить обработки
                            // надо запустить и стартовать  нить () => this.ProcessClient(localSocket)

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
                    // читаем данные из сокета
                    int bytesReceived = NonblockingSocketUtil.Receive(socket, buffer, offset, buffer.Length - offset, SocketFlags.None, SocketTimeout);

                    offset += bytesReceived;

                    // парсим полученные данные
                    var result = this.protocol.ParseInput(buffer, offset);

                    // если какие-то запросы успешно распарсились
                    if (result.ParsedObjects.Count > 0)
                    {
                        foreach (var request in result.ParsedObjects)
                        {
                            // то получаем ответ для каждого запроса и отправляем его обратно
                            var response = this.handler(request);

                            // TODO: посылаем ответ
                            // ProtocolExecutor.CreateResponse
                            // socket.Send

                            socket.Send(this.protocol.CreateResponse(response));
                        }
                    }

                    // если сессия закрыта, то уходим
                    if (result.IsClosed)
                    {
                        break;
                    }

                    // удаляем те данные, которые уже были распарсены
                    if (result.ProcessedBytes > 0)
                    {
                        if (offset > result.ProcessedBytes)
                            Array.Copy(buffer, result.ProcessedBytes, buffer, 0, offset - result.ProcessedBytes);

                        offset -= result.ProcessedBytes;
                    }
                }
                while (true);


                // TODO: закрываем клиентский сокет
                // socket.Shutdown
                socket.Shutdown(SocketShutdown.Both);
            }
        }
    }
}
