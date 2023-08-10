using Newtonsoft.Json;
using SocketClientServer.BinaryEnvelopeProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpc.Core
{
    public record RpcRequestProtocol(string TypeName, string MethodName, List<ArgumentInfoProtocol> Arguments);

    public record ArgumentInfoProtocol(string TypeName, string Content);

    public record RpcResponseProtocol(ArgumentInfoProtocol Result, bool IsException);

    public record RpcRequest(string TypeName, string MethodName, List<ArgumentInfo> Arguments);

    public record ArgumentInfo(Type Type, object Content);

    public record RpcResponse(ArgumentInfo Result, bool IsException);

    public class RpcProtocol
    {
        public Envelope CreateRpcRequest(string typeName, string methodName, List<object> arguments)
        {
            var argInfos = arguments.Select(obj =>
            {
                string content = JsonConvert.SerializeObject(obj);
                return new ArgumentInfoProtocol(obj.GetType().AssemblyQualifiedName, content);
            })
                .ToList();
            var rpcRequest = new RpcRequestProtocol(typeName, methodName, argInfos);
            return new Envelope(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rpcRequest)));
        }

        public RpcRequest ParseRpcRequest(Envelope envelope)
        {
            var rpcRequestProtocol =
                JsonConvert.DeserializeObject<RpcRequestProtocol>(Encoding.UTF8.GetString(envelope.Data));
            if (rpcRequestProtocol == null)
                throw new Exception($"Bad request (RpcRequest can't be deserialized)");

            var args = rpcRequestProtocol.Arguments
                .Select(i => (i.Content, Type: Type.GetType(i.TypeName)))
                .Select(i => new ArgumentInfo(i.Type, JsonConvert.DeserializeObject(i.Content, i.Type)))
                .ToList();

            return new RpcRequest(rpcRequestProtocol.TypeName, rpcRequestProtocol.MethodName, args);
        }

        public Envelope CreateErrorResponse(string error)
        {
            return this.CreateResponse(new Exception(error));
        }

        public Envelope CreateResponse(Exception e)
        {
            return this.CreateResponse(e, e.GetType(), true);
        }

        public Envelope CreateResponse(object? result,Type type,bool isException)
        {
            string content = JsonConvert.SerializeObject(result);
            var rpcResponse = new RpcResponseProtocol(new ArgumentInfoProtocol(type.AssemblyQualifiedName, content), isException);
            return new Envelope(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rpcResponse)));
        }

        public RpcResponse ParseRpcResponse(Envelope envelope)
        {
            var rpcResponseProtocol =
                JsonConvert.DeserializeObject<RpcResponseProtocol>(Encoding.UTF8.GetString(envelope.Data))
                ?? throw new Exception($"Bad response (RpcResponseProtocol can't be deserialized from JSON)");

            return new RpcResponse(this.ParseArgumentInfo(rpcResponseProtocol.Result), rpcResponseProtocol.IsException);
        }

        private ArgumentInfo ParseArgumentInfo(ArgumentInfoProtocol arg)
        {
            var type = Type.GetType(arg.TypeName)
                ?? throw new Exception($"Instance of type {arg.TypeName} can't be created");

            var content = JsonConvert.DeserializeObject(arg.Content, type)
                ?? throw new Exception($"Argument of type {arg.TypeName} can't be deserialized from JSON");

            return new ArgumentInfo(type, content);
        }
    }
}
