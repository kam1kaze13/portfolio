using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGITestApplication
{
    class CGITest
    {
        [STAThread]
        static void Main(string[] args)
        {
            // *** Use this for debugging –

            //      Hit the link then attach debugger to this process

            //     and then pause to continue

            //System.Threading.Thread.Sleep(30000);
            // *** Loop through all the environement vars and write to string
            IDictionary Dict = Environment.GetEnvironmentVariables();

            StringBuilder sb = new System.Text.StringBuilder();

            foreach (DictionaryEntry Item in Dict)
            {
                sb.Append((string)Item.Key + " - " + (string)Item.Value + "\r\n");
            }

            // *** Read individual values
            string QueryString = Environment.GetEnvironmentVariable("QUERY_STRING");

            // *** Read all the incoming form data both text and binary
            string FormData = "";

            byte[] Data = null;

            if (Environment.GetEnvironmentVariable("REQUEST_METHOD") == "POST")
            {
                Stream s = Console.OpenStandardInput();

                BinaryReader br = new BinaryReader(s);

                string Length = Environment.GetEnvironmentVariable("CONTENT_LENGTH");

                int Size = Int32.Parse(Length);

                Data = new byte[Size];

                br.Read(Data, 0, Size);

                // *** don’t close the reader!
                FormData = System.Text.Encoding.Default.GetString(Data, 0, Size);
            }

            Console.Write(
                @"HTTP/1.1 200 OK
                Content-type: text/html

                <html>
                Hello World

                <pre>
                <b>Environment and Server Variables:</b>
                " + sb.ToString() + @"

                <b>Form Vars (if any):</b>
                " + FormData + @"
                </pre>
                </html>
                ");
        }
    }
}
