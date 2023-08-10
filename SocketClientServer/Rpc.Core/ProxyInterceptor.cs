using Castle.DynamicProxy;
using SocketClientServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SocketClientServer.BinaryEnvelopeProtocol;

namespace Rpc.Core
{
    public class ProxyInterceptor : IInterceptor
    {
        private readonly IPEndPoint endPoint;
        private readonly RpcProtocol rpcProtocol;

        public ProxyInterceptor(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            this.rpcProtocol = new RpcProtocol();
        }
        public void Intercept(IInvocation invocation)
        {
            using (var socketClient = new AsyncClient<Envelope,Envelope>(this.endPoint, new BinaryEnvelopeProtocolExecutor()))
            {
                var input = this.rpcProtocol.CreateRpcRequest(
                    invocation.Method.DeclaringType.AssemblyQualifiedName, invocation.Method.Name, invocation.Arguments.ToList());
                var output = socketClient.DoOperation(input).Result;
                var response = this.rpcProtocol.ParseRpcResponse(output);
                object result = response.Result.Content;
                if (response.IsException)
                    throw result as Exception;

                if (!invocation.Method.ReturnType.IsInstanceOfType(result))
                    throw new Exception($"Return value of type {response.Result.Type.AssemblyQualifiedName} is not instance of {invocation.Method.ReturnType}");

                invocation.ReturnValue = result;
            }
        }
    }
}
