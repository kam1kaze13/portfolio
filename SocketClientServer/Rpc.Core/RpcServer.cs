using SocketClientServer.BinaryEnvelopeProtocol;
using SocketClientServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rpc.Core
{
    public class RpcServer
    {
        private readonly IServer socketServer;
        private readonly RpcProtocol rpcProtocol;
        private Dictionary<string, object> implementationDir = new Dictionary<string, object>();

        public RpcServer(int port, int listenerCount)
        {
            this.socketServer = new AsyncServer<Envelope, Envelope>(port,listenerCount, new BinaryEnvelopeProtocolExecutor(), this.ProcessRequest);
            this.rpcProtocol = new RpcProtocol();
        }       

        public void Start()
        {
            this.socketServer.Start();
        }

        public void Stop()
        {
            this.socketServer.Stop();
        }

        public Envelope ProcessRequest(Envelope input)
        {
            try
            {
                var rpcRequest = this.rpcProtocol.ParseRpcRequest(input);

                if (!this.implementationDir.TryGetValue(rpcRequest.TypeName, out var impl))
                    return this.rpcProtocol.CreateErrorResponse($"Implementation of {rpcRequest.TypeName} was not found");

                var argTypes = rpcRequest.Arguments.Select(i => i.Type).ToArray();
                var method = this.GetMethod(impl, rpcRequest.MethodName, argTypes);
                if (method == null)
                    return this.rpcProtocol.CreateErrorResponse(
                        $"Method {rpcRequest.MethodName} with arguments {string.Concat(", ", argTypes)} was not found in implementation of {rpcRequest.TypeName}");

                var result = method.Invoke(impl, rpcRequest.Arguments.Select(i => i.Content).ToArray());
                return this.rpcProtocol.CreateResponse(result, method.ReturnType, false);
            }
            catch (Exception e)
            {
                return this.rpcProtocol.CreateResponse(e);
            }
        }

        private MethodInfo? GetMethod(object impl, string methodName, Type[] args)
        {
            return impl.GetType().GetMethod(methodName, args);
        }

        public void Register<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : TInterface, new()
        {
            this.Register<TInterface>(new TImplementation());
        }

        public void Register<TInterface>(TInterface implementation) where TInterface : class
        {
            this.Register(typeof(TInterface), implementation);
        }

        private void Register(Type interfaceType, object implementation)
        {
            this.implementationDir.Add(interfaceType.AssemblyQualifiedName, implementation);
        }
    }
}
