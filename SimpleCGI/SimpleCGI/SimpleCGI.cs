using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCGI
{
    class SimpleCGI
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\r\n\r\n");
            Console.WriteLine("<h1>Environment Variables</h1>");
            foreach (DictionaryEntry var in Environment.GetEnvironmentVariables())
                Console.WriteLine("<hr><b>{0}</b>: {1}", var.Key, var.Value);
        }
    }
}
