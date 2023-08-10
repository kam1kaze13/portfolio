using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rpc.Core
{
    public class RpcClient
    {
        private readonly IPEndPoint endPoint;

        public RpcClient(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        public TInterface GetProxy<TInterface>() where TInterface : class
        {
            return new ProxyGenerator().CreateInterfaceProxyWithoutTarget<TInterface>(new ProxyInterceptor(this.endPoint));
        }
    }
}
