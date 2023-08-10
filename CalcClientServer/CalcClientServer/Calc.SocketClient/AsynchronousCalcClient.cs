namespace Calc.SocketClient
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    using AsyncResult;
    using Protocol;

    public class AsynchronousCalcClient : IAsynchronousCalcClient
    {
        private const int BufferSize = 10240;

        private readonly CalcProtocolExecutor protocol = new CalcProtocolExecutor();
        private Socket socket;

        public AsynchronousCalcClient()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private class OperationState
        {
            public AsyncResult<long> CommonAsyncResult { get; set; }

            public byte[] Buffer { get; set; }

            public int Offset { get; set; }

            public OperationState(AsyncResult<long> asyncResult)
            {
                this.CommonAsyncResult = asyncResult;
            }
        }

        public IAsyncResult BeginConnect(IPEndPoint endPoint, AsyncCallback asyncCallback, object state)
        {
            // присоединяемся к заданному Endpoint
            return this.socket.BeginConnect(endPoint, asyncCallback, state);
        }

        public void EndConnect(IAsyncResult asyncResult)
        {
            this.socket.EndConnect(asyncResult);
        }

        public IAsyncResult BeginDoOperation(Operation op, AsyncCallback asyncCallback, object state)
        {
            byte[] request = this.protocol.CreateRequest(
                new CalcRequest
                {
                    Argument1 = op.Argument1,
                    Argument2 = op.Argument2,
                    OperationType = op.OperationType,
                });

            // инициализируем наш собственный AsyncResult
            var commonAr = new AsyncResult<long>(asyncCallback, state);

            var operationState = new OperationState(commonAr);

            // начинаем посылать запрос
            this.socket.BeginSend(request, 0, request.Length, SocketFlags.None, this.SendCallback, operationState);

            return commonAr;
        }

        public long EndDoOperation(IAsyncResult ar)
        {
            return ((AsyncResult<long>)ar).EndInvoke();
        }

        public IAsyncResult BeginDisconnect(AsyncCallback asyncCallback, object state)
        {
            var commonAr = new AsyncResultNoResult(asyncCallback, state);

            commonAr.ExecuteInContext(
                () =>
                    {
                        // начинаем посылать финальный ";"
                        this.socket.BeginSend(new[] { (byte) ';' }, 0, 1, SocketFlags.None, this.SendEndCallback, commonAr);
                    });

            return commonAr;
        }

        public void EndDisconnect(IAsyncResult ar)
        {
            ((AsyncResultNoResult)ar).EndInvoke();

            this.socket.Shutdown(SocketShutdown.Both);
            this.socket.Close();
        }

        private void SendCallback(IAsyncResult ar)
        {
            var state = (OperationState) ar.AsyncState;
            state.CommonAsyncResult.ExecuteInContext(
                () =>
                {
                    this.socket.EndSend(ar);
                    state.Buffer = new byte[BufferSize];
                    state.Offset = 0;

                    // начинаем читать данные
                    this.socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, this.ReceiveCallback, state);
                });
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = (OperationState)ar.AsyncState;
            state.CommonAsyncResult.ExecuteInContext(
                () =>
                {
                    // получаем данные из сокета
                    int bytesReceived = this.socket.EndReceive(ar);
                    if (bytesReceived > 0)
                    {
                        state.Offset += bytesReceived;

                        // парсим полученные данные
                        var result = this.protocol.ParseOutput(state.Buffer, state.Offset);

                        // если какие-то запросы успешно распарсились
                        if (result.ParsedObjects.Count > 0)
                        {
                            // возвращаем первый
                            state.CommonAsyncResult.SetAsCompleted(result.ParsedObjects.First().Result);
                        }
                        else
                        {
                            // продолжаем, пока есть данные
                            this.socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, this.ReceiveCallback, null);
                        }
                    }
                });
        }

        private void SendEndCallback(IAsyncResult ar)
        {
            var commonAr = (AsyncResultNoResult)ar.AsyncState;
            commonAr.ExecuteInContext(
                () =>
                {
                    this.socket.EndSend(ar);

                    this.socket.BeginDisconnect(false, this.DisconnectCallback, commonAr);
                });
        }

        private void DisconnectCallback(IAsyncResult ar)
        {
            var commonAr = (AsyncResultNoResult)ar.AsyncState;
            commonAr.ExecuteInContext(
                () =>
                {
                    this.socket.EndDisconnect(ar);

                    commonAr.SetAsCompleted();
                });
        }
    }
}
