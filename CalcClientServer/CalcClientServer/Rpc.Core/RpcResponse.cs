using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpc.Core
{
    public class RpcResponse
    {
        public object Result { get; set; }

        public string Exception { get; set; }
    }
}
