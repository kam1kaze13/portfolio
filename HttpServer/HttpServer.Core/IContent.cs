using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public interface IContent
    {
        CustomHttpResponse GetResponse(CustomHttpRequest request);
    }
}
