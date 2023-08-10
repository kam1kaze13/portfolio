namespace Calc.SocketClient
{
    using System;
    using System.Net;

    /// <summary>
    /// Асинхронный интерфейс клиента
    /// </summary>
    public interface IAsynchronousCalcClient
    {
        IAsyncResult BeginConnect(IPEndPoint endPoint, AsyncCallback asyncCallback, object state);

        void EndConnect(IAsyncResult asyncResult);

        IAsyncResult BeginDoOperation(Operation op, AsyncCallback asyncCallback, object state);

        long EndDoOperation(IAsyncResult ar);

        IAsyncResult BeginDisconnect(AsyncCallback asyncCallback, object state);

        void EndDisconnect(IAsyncResult asyncResult);
    }
}