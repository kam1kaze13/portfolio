using FastCgiNet;
using FastCgiNet.Streams;
using FastCgiNet.Requests;
using System.Net.Sockets;
using System.Net;

using (var listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
{
    listenSock.Bind(new IPEndPoint(IPAddress.Loopback, 9000));
    listenSock.Listen(1);

    // For simplicity, let's accept only one connection
    var sock = listenSock.Accept();
    using (var request = new ApplicationSocketRequest(sock))
    {
        // Now let's wait until we have received the Params and Stdin streams completely
        int bytesRead;
        byte[] buf = new byte[4096];
        while (!request.Params.IsComplete || !request.Stdin.IsComplete)
        {
            bytesRead = sock.Receive(buf, SocketFlags.None);
            request.FeedBytes(buf, 0, bytesRead).ToList();
        }

        // Let's look for the requested path and ignore everything else
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

        // Let's write a classic response
        using (var writer = new StreamWriter(request.Stdout))
        {
            // The headers first
            writer.NewLine = "\r\n";
            writer.Write("Status: 200 OK");
            writer.WriteLine("Content-Type: text/html");
            writer.WriteLine();

            // Now the body
            writer.Write("<html><head><title>Hello World</title></head><body><h1>Hello FastCgiNet!</h1>The requested path was {0}</body></html>", requestedPath);
        }

        // Our application status and end of request. The FastCgi Standard defines that returning 0 indicates there were no errors
        request.SendEndRequest(0, ProtocolStatus.RequestComplete);
    }

    // The connection socket and all other resources (except for the 
    // listen socket) are automatically disposed at this point. This
    // implies that ApplicationSocketRequest still doesn't multiplex
    // requests (not for long, hopefully - also, you can inherit from
    // this class and make Dispose() not call CloseSocket() if 
    // you want to multiplex requests).
}
