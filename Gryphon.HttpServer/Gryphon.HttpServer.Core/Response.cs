using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gryphon.HttpServer.Core
{
    public class Response
    {
        StringBuilder sb;
        public Response()
        {
            this.sb = new StringBuilder();
        }
        public void Write(string message)
        {
            sb.Append(message);
        }

        public void GetResponse()
        {
            Console.WriteLine(sb.ToString());
        }
    }
}
