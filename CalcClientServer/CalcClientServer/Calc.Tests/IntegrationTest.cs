namespace Calc.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Protocol;
    using SocketClient;
    using SocketServer;

    [TestClass]
    public class IntegrationTest
    {
        [TestMethod]
        public void OperationSyncServerSyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartSyncCalcServer(port);
            try
            {
                //this.AddOperationTest(port, this.CreateCalcConnection);
                //this.SubOperationTest(port, this.CreateCalcConnection);
                //this.MultOperationTest(port, this.CreateCalcConnection);
                //this.BatchOperationTest(port, this.CreateCalcConnection);
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        public void OperationAsyncServerSyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartAsyncCalcServer(port);
            try
            {
                //this.AddOperationTest(port, this.CreateCalcConnection);
                //this.SubOperationTest(port, this.CreateCalcConnection);
                //this.MultOperationTest(port, this.CreateCalcConnection);
                //this.BatchOperationTest(port, this.CreateCalcConnection);
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        public void OperationSyncServerAsyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartSyncCalcServer(port);
            try
            {
                this.AddOperationTest(port, this.CreateAsyncCalcClientAdapter);
                this.SubOperationTest(port, this.CreateAsyncCalcClientAdapter);
                this.MultOperationTest(port, this.CreateAsyncCalcClientAdapter);
                this.BatchOperationTest(port, this.CreateAsyncCalcClientAdapter);
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        public void OperationAsyncServerAsyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartAsyncCalcServer(port);
            try
            {
                this.AddOperationTest(port, this.CreateAsyncCalcClientAdapter);
                this.SubOperationTest(port, this.CreateAsyncCalcClientAdapter);
                this.MultOperationTest(port, this.CreateAsyncCalcClientAdapter);
                this.BatchOperationTest(port, this.CreateAsyncCalcClientAdapter);
            }
            finally
            {
                server.Stop();
            }
        }

        public void AddOperationTest(int port, Func<int, ICalcConnection> connectionFactory)
        {
            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Add(2, 7));
                Assert.AreEqual(2 + 7, result);
            }

            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Add(int.MinValue, int.MaxValue));
                Assert.AreEqual((long)int.MinValue + int.MaxValue, result);
            }

            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Add(-34763456, -94345789));
                Assert.AreEqual(-34763456L + -94345789L, result);
            }
        }

        public void SubOperationTest(int port, Func<int, ICalcConnection> connectionFactory)
        {
            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Sub(2, 7));
                Assert.AreEqual(2 - 7, result);
            }

            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Sub(int.MinValue, int.MaxValue));
                Assert.AreEqual((long)int.MinValue - int.MaxValue, result);
            }

            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Sub(-34763456, -94345789));
                Assert.AreEqual(-34763456L - -94345789L, result);
            }
        }

        public void MultOperationTest(int port, Func<int, ICalcConnection> connectionFactory)
        {
            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Mult(2, 7));
                Assert.AreEqual(2 * 7, result);
            }

            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Mult(int.MinValue, int.MaxValue));
                Assert.AreEqual((long)int.MinValue * int.MaxValue, result);
            }

            using (var connection = connectionFactory(port))
            {
                long result = connection.DoOperation(Operation.Mult(-34763456, -94345789));
                Assert.AreEqual(-34763456L * -94345789L, result);
            }
        }

        public void BatchOperationTest(int port, Func<int, ICalcConnection> connectionFactory)
        {
            using (var connection = connectionFactory(port))
            {
                for (int i = 0; i < 15; i++)
                {
                    long result = connection.DoOperation(Operation.Add(2, 7));
                    Assert.AreEqual(2 + 7, result);

                    long result2 = connection.DoOperation(Operation.Sub(int.MinValue, int.MaxValue));
                    Assert.AreEqual((long)int.MinValue - int.MaxValue, result2);

                    long result3 = connection.DoOperation(Operation.Mult(-34763456, -94345789));
                    Assert.AreEqual(-34763456L * -94345789L, result3);
                }
            }
        }

        [TestMethod]
        public void SimpleAsyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartAsyncCalcServer(port);

            try
            {
                var ipAddress = IPAddress.Parse("127.0.0.1");
                var localEndPoint = new IPEndPoint(ipAddress, port);

                var asyncClient = new AsynchronousCalcClient();

                var mre = new ManualResetEvent(false);

                asyncClient.BeginConnect(
                    localEndPoint,
                    ar1 =>
                    {
                        asyncClient.EndConnect(ar1);

                        asyncClient.BeginDoOperation(
                            Operation.Mult(-34763456, -94345789),
                            ar2 =>
                            {
                                long result = asyncClient.EndDoOperation(ar2);
                                Assert.AreEqual(-34763456L * -94345789L, result);

                                asyncClient.BeginDisconnect(
                                    ar3 =>
                                    {
                                        asyncClient.EndDisconnect(ar3);
                                        mre.Set();
                                    },
                                    null);
                            },
                            null);
                    },
                    null);

                mre.WaitOne();
            }
            finally
            {
                server.Stop();
            }
        }

        [TestMethod]
        public void BatchAsyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartAsyncCalcServer(port);

            try
            {
                var ipAddress = IPAddress.Parse("127.0.0.1");
                var localEndPoint = new IPEndPoint(ipAddress, port);

                var asyncClient = new AsynchronousCalcClient();

                var mre = new ManualResetEvent(false);

                int counter = 0;

                var disconnectCallback = new AsyncCallback(
                    ar =>
                    {
                        asyncClient.EndDisconnect(ar);
                        mre.Set();
                    });

                AsyncCallback doOperationCallback = null;
                doOperationCallback = new AsyncCallback(
                    ar =>
                    {
                        long result = asyncClient.EndDoOperation(ar);
                        Assert.AreEqual(-34763456L * -94345789L, result);

                        if (counter++ <= 15)
                        {
                            asyncClient.BeginDoOperation(
                                Operation.Mult(-34763456, -94345789),
                                doOperationCallback,
                                null);
                        }
                        else
                        {
                            asyncClient.BeginDisconnect(disconnectCallback, null);
                        }
                    });

                var connectCallback = new AsyncCallback(
                    ar =>
                    {
                        asyncClient.EndConnect(ar);

                        asyncClient.BeginDoOperation(
                            Operation.Mult(-34763456, -94345789),
                            doOperationCallback,
                            null);
                    });

                asyncClient.BeginConnect(localEndPoint, connectCallback, null);

                mre.WaitOne();
            }
            finally
            {
                server.Stop();
            }
        }

        private int portCounter = 300;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int GetPort()
        {
            return this.portCounter++;
        }

        private ICalcServer StartSyncCalcServer(int port)
        {
            var calcProcessor = new CalcProcessor();
            var server = new SynchronousCalcServer<CalcRequest, CalcResponse>(port, 10, new CalcProtocolExecutor(), calcProcessor.Process);
            server.Start();

            return server;
        }

        private ICalcServer StartAsyncCalcServer(int port)
        {
            var calcProcessor = new CalcProcessor();
            var server = new AsynchronousCalcServer<CalcRequest, CalcResponse>(port, 10, new CalcProtocolExecutor(), calcProcessor.Process);
            server.Start();

            return server;
        }

        private SynchronousCalcConnection<CalcRequest,CalcResponse> CreateCalcConnection(int port)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var localEndPoint = new IPEndPoint(ipAddress, port);

            return new SynchronousCalcConnection<CalcRequest,CalcResponse>(localEndPoint, new CalcProtocolExecutor());
        }

        private ICalcConnection CreateAsyncCalcClientAdapter(int port)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var localEndPoint = new IPEndPoint(ipAddress, port);

            return new AsynchronousCalcConnectionAdapter(new AsynchronousCalcClient(), localEndPoint);
        }
    }
}
