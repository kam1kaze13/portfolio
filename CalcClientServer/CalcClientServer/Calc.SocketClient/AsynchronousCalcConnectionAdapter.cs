namespace Calc.SocketClient
{
    using System.Net;

    /// <summary>
    /// Адаптер AsynchronousCalcClient к ICalcConnection
    /// Необходим для того, чтобы синхронные операции могли быть сделаны с помощью асинхронного клиента
    /// </summary>
    public class AsynchronousCalcConnectionAdapter : ICalcConnection
    {
        private readonly AsynchronousCalcClient client;

        public AsynchronousCalcConnectionAdapter(AsynchronousCalcClient client, IPEndPoint endpoint)
        {
            this.client = client;
            this.Connect(endpoint);
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        public long DoOperation(Operation op)
        {
            return this.client.EndDoOperation(this.client.BeginDoOperation(op, null, null));
        }

        private void Connect(IPEndPoint endPoint)
        {
            this.client.EndConnect(this.client.BeginConnect(endPoint, null, null));
        }

        private void Disconnect()
        {
            this.client.EndDisconnect(this.client.BeginDisconnect(null, null));
        }
    }
}
