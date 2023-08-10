using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public static class ContentRules
    {
        public static Func<string, bool> Extension(string ext)
        {
            return path => Path.GetExtension(path) == ext;
        }

        public static Func<string, bool> Directory(string dir)
        {
            return path => path.StartsWith(dir.ToLower());
        }
    }
}
