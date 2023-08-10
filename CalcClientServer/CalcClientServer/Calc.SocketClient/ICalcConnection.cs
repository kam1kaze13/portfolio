namespace Calc.SocketClient
{
    using System;

    /// <summary>
    /// Интерфейс клиентского соединения
    /// </summary>
    public interface ICalcConnection : IDisposable
    {
        long DoOperation(Operation op);
    }
}