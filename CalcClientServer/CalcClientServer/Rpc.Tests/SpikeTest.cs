using Rpc.Core;

namespace Rpc.Tests
{
    using Calc.SocketClient;
    using Calc.SocketServer;
    using Castle.DynamicProxy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;
    using System.Runtime.CompilerServices;

    [TestClass]
    public class SpikeTest
    {
        [TestMethod]
        public void SimpleTest()
        {
            int port = this.GetPort();
            var server = this.StartSyncCalcServer(port);

            try
            {
                var generator = new ProxyGenerator();
                var connection = this.CreateCalcConnection(port);
                var t = generator.CreateInterfaceProxyWithoutTarget<ICall>(new ProcessInterceptor(connection));
                var result = t.Call("abc");
                Assert.AreEqual("cba", result);
            }
            finally
            {
                server.Stop();
            }
        }

        private ICalcServer StartSyncCalcServer(int port)
        {
            var rpcProcessor = new RpcProcessor();
            var server = new SynchronousCalcServer<RpcRequest, RpcResponse>(port, 10, new RpcProtocolExecutor(), rpcProcessor.Process);
            server.Start();

            return server;
        }

        private ICalcServer StartAsyncCalcServer(int port)
        {
            var rpcProcessor = new RpcProcessor();
            var server = new AsynchronousCalcServer<RpcRequest, RpcResponse>(port, 10, new RpcProtocolExecutor(), rpcProcessor.Process);
            server.Start();

            return server;
        }

        private SynchronousCalcConnection<RpcRequest, RpcResponse> CreateCalcConnection(int port)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var localEndPoint = new IPEndPoint(ipAddress, port);

            return new SynchronousCalcConnection<RpcRequest, RpcResponse>(localEndPoint, new RpcProtocolExecutor());
        }

        private int portCounter = 300;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int GetPort()
        {
            return this.portCounter++;
        }
    }
}
