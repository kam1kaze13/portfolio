using Calc.SocketClient;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpc.Core
{
    public class ProcessInterceptor : IInterceptor
    {
        private readonly SynchronousCalcConnection<RpcRequest, RpcResponse> connection;

        public ProcessInterceptor(SynchronousCalcConnection<RpcRequest,RpcResponse> connection)
        {
            this.connection = connection;
        }
        public void Intercept(IInvocation invocation)
        {
            var request = new RpcRequest
            {
                TypeName = invocation.Method.DeclaringType.FullName,
                MethodName = invocation.Method.Name,
                Args = invocation.Arguments
                    .Select(i => new Arguments { FullTypeName = JsonConvert.SerializeObject(i.GetType(), Formatting.Indented), Content = JsonConvert.SerializeObject(i, Formatting.Indented) })
                    .ToList()
            };

            var result = this.connection.DoOperation(request);

            if (result.Exception != null)
            {
                invocation.ReturnValue = result.Exception;
            }
            else
            {
                invocation.ReturnValue = result.Result;
            }
        }
    }
}
