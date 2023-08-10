using NUnit.Framework;
using Rpc.Core;
using SocketClientServer.BinaryEnvelopeProtocol;
using SocketClientServer.Core;
using System;
using System.Net;

namespace Rpc.Tests
{
    public class SpikeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleTest()
        {
            int port = this.GetPort();

            var server = this.StartRpcServer(port);
            server.Register<ICall>(new CallImpl());
            var client = new RpcClient(this.CreateEndPoint(port));

            var proxy = client.GetProxy<ICall>();

            var result = proxy.Call("abc");
            Assert.AreEqual("cba", result);
        }

        public interface ICall
        {
            string Call(string value);
        }

        class CallImpl : ICall
        {
            public string Call(string value)
            {
                char[] charArray = value.ToCharArray();

                Array.Reverse(charArray);

                return new string(charArray);
            }
        }

        private RpcServer StartRpcServer(int port)
        {
            var server = new RpcServer(port, 10);
            server.Start();

            return server;
        }

        private IPEndPoint CreateEndPoint(int port)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            return new IPEndPoint(ipAddress, port);
        }

        private int portCounter = 300;
        private Func<Envelope, Envelope> function = arr => arr;

        private int GetPort()
        {
            return this.portCounter++;
        }
    }
}