using Calc.Core;
using NUnit.Framework;
using SocketClientServer.Core;
using System;
using System.Net;
using System.Threading;

namespace Calc.Tests
{
    public class IntegrationTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void OperationSyncServerSyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartSyncCalcServer(port);
            try
            {
                using (var connection = this.CreateSyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Add, Argument2 = 7 });
                    Assert.AreEqual(2 + 7, result.Result);
                }

                using (var connection = this.CreateSyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Sub, Argument2 = 7 });
                    Assert.AreEqual(2 - 7, result.Result);
                }

                using (var connection = this.CreateSyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Mult, Argument2 = 7 });
                    Assert.AreEqual(2 * 7, result.Result);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        [Test]
        public void OperationAsyncServerSyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartAsyncCalcServer(port);
            try
            {
                using (var connection = this.CreateSyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Add, Argument2 = 7 });
                    Assert.AreEqual(2 + 7, result.Result);
                }

                using (var connection = this.CreateSyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Sub, Argument2 = 7 });
                    Assert.AreEqual(2 - 7, result.Result);
                }

                using (var connection = this.CreateSyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Mult, Argument2 = 7 });
                    Assert.AreEqual(2 * 7, result.Result);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        [Test]
        public void OperationSyncServerAsyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartSyncCalcServer(port);
            try
            {
                using (var connection = this.CreateAsyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Add, Argument2 = 7 });
                    Assert.AreEqual(2 + 7, result.Result.Result);
                }

                using (var connection = this.CreateAsyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Sub, Argument2 = 7 });
                    Assert.AreEqual(2 - 7, result.Result.Result);
                }

                using (var connection = this.CreateAsyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Mult, Argument2 = 7 });
                    Assert.AreEqual(2 * 7, result.Result.Result);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        [Test]
        public void OperationAsyncServerAsyncClientTest()
        {
            int port = this.GetPort();
            var server = this.StartAsyncCalcServer(port);
            try
            {
                using (var connection = this.CreateAsyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 =2, OperationType = OperationType.Add, Argument2 = 7});
                    Assert.AreEqual(2 + 7, result.Result.Result);
                }

                using (var connection = this.CreateAsyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Sub, Argument2 = 7 });
                    Assert.AreEqual(2 - 7, result.Result.Result);
                }

                using (var connection = this.CreateAsyncCalcClient(port))
                {
                    var result = connection.DoOperation(new CalcRequest { Argument1 = 2, OperationType = OperationType.Mult, Argument2 = 7 });
                    Assert.AreEqual(2 * 7, result.Result.Result);
                }
            }
            finally
            {
                server.Stop();
            }
        }       

        private int portCounter = 300;

        private int GetPort()
        {
            return this.portCounter++;
        }

        private IServer StartSyncCalcServer(int port)
        {
            var calcProcessor = new CalcProcessor();
            var server = new SyncServer<CalcRequest, CalcResponse>(port, 10, new CalcProtocolExecutor(), calcProcessor.Process);
            server.Start();

            return server;
        }

        private IServer StartAsyncCalcServer(int port)
        {
            var calcProcessor = new CalcProcessor();
            var server = new AsyncServer<CalcRequest, CalcResponse>(port, 10, new CalcProtocolExecutor(), calcProcessor.Process);
            server.Start();

            return server;
        }

        private SyncClient<CalcRequest, CalcResponse> CreateSyncCalcClient(int port)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var localEndPoint = new IPEndPoint(ipAddress, port);

            return new SyncClient<CalcRequest, CalcResponse>(localEndPoint, new CalcProtocolExecutor());
        }

        private AsyncClient<CalcRequest, CalcResponse> CreateAsyncCalcClient(int port)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var localEndPoint = new IPEndPoint(ipAddress, port);

            return new AsyncClient<CalcRequest, CalcResponse>(localEndPoint, new CalcProtocolExecutor());
        }
    }
}