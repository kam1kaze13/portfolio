using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public interface IHttpFilter
    {
        CustomHttpResponse Transform(CustomHttpRequest request, CustomHttpResponse response);
    }
}
