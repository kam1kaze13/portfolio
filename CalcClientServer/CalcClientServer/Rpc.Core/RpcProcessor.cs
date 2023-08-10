using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rpc.Core
{
    public class RpcProcessor
    {
        public RpcResponse Process(RpcRequest request)
        {
            object value;
            var hasValue = implementations.TryGetValue(request.TypeName, out value);
            if (hasValue)
            {
                Type type = value.GetType();

                var allMethods = type.GetMethods();

                var methodsInfo = allMethods.Where(i => i.Name == request.MethodName);

                var (method, args) = CheckMethod(methodsInfo, request.Args.ToArray());

                if (method != null)
                {
                    var result = method.Invoke(value, args);

                    return new RpcResponse { Result = result, };
                }
                else
                {
                    return new RpcResponse { Exception = $"Exception! Server don't have {request.MethodName} method in registered implementations for this interface!" };
                }
            }
            else
            {
                return new RpcResponse { Exception = $"Exception! Server don't have registered implementation for {request.TypeName} interface!" };
            }
        }

        private (MethodInfo, object[]) CheckMethod(IEnumerable<MethodInfo> methods, Arguments[] requestArgs)
        {
            foreach(var method in methods)
            {
                var arguments = new List<object>();
                var methodParams = method.GetParameters();

                for (int i=0; i < methodParams.Length; i++)
                {
                    if (methodParams[i].ParameterType.Equals(JsonConvert.DeserializeObject(requestArgs[i].FullTypeName).GetType()))
                        arguments.Add(JsonConvert.DeserializeObject(requestArgs[i].Content));
                }

                if (arguments.Count == methodParams.Length)
                    return (method, arguments.ToArray());
            }

            return (null, null);
        }

        private readonly Dictionary<string, object> implementations = new Dictionary<string, object>() { {"Rpc.Core.ICall", new CallImpl() }, };
    }
}
