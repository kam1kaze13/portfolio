using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpc.Core
{
    public class RpcRequest
    {
        public List<Arguments> Args { get; set; }

        public string MethodName { get; set; }

        public string TypeName { get; set; }
    }
}
