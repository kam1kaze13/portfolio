using FastCgiNet;
using FastCgiNet.Streams;
using FastCgiNet.Requests;
using System.Net.Sockets;
using System.Net;
using System.Text;
using FastCGI.DynamicHttpServer.Core;
using System.IO;

using (var listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
{
    listenSock.Bind(new IPEndPoint(IPAddress.Loopback, 9000));
    listenSock.Listen(10);

    while (true)
    {
        var sock = listenSock.Accept();
        using (var request = new ApplicationSocketRequest(sock))
        {
            int bytesRead;
            byte[] buf = new byte[4096];
            while (!request.Params.IsComplete || !request.Stdin.IsComplete)
            {
                bytesRead = sock.Receive(buf, SocketFlags.None);
                request.FeedBytes(buf, 0, bytesRead).ToList();
            }

            string requestedPath = null;
            using (var nvpReader = new NvpReader(request.Params))
            {
                NameValuePair nvp;
                while ((nvp = nvpReader.Read()) != null)
                {
                    if (nvp.Name == "DOCUMENT_URI")
                        requestedPath = nvp.Value;
                }
            }

            using (var writer = new StreamWriter(request.Stdout))
            {
                writer.NewLine = "\r\n";
                writer.Write("Status: 200 OK");
                writer.WriteLine("Content-Type: text/html");
                writer.WriteLine();

                //StreamReader stream = new StreamReader(System.IO.File.OpenRead("C:\\Users\\Александр\\Downloads\\test.js"), Encoding.UTF8);

                var jsExecutor = new JSExecutor();

                //writer.Write(jsExecutor.RunScript(stream.ReadToEnd()));
                writer.Write(jsExecutor.RunScript("out = 5+15; out;"));
            }

            request.SendEndRequest(0, ProtocolStatus.RequestComplete);
        }
    }   
}
