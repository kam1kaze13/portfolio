using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public interface IContentStore
    {
        IContent GetContent(string path);
    }
}
