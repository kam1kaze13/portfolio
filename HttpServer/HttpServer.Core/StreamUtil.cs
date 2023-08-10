using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public static class StreamUtil
    {
        public static Stream StreamFromString(string body)
        {           
            return new MemoryStream(Encoding.UTF8.GetBytes(body));
        }

        public static string StreamToString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
