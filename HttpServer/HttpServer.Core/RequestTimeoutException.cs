using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class RequestTimeoutException : Exception
    {
        public RequestTimeoutException() { }

        public RequestTimeoutException(string message)
            : base(message) { }
    }
}
