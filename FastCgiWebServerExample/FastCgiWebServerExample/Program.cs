using FastCgiNet;
using FastCgiNet.Streams;
using FastCgiNet.Requests;
using System.Net.Sockets;
using System.Net;

// Let's simulate a GET request to http://github.com/mzabani/FastCgiNet
var requestedUrl = new Uri("http://localhost/phpinfo.php");
string requestMethod = "GET";

// Suppose the FastCgi application is listening on 127.0.0.1, port 9000
var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
sock.Connect(new IPEndPoint(IPAddress.Loopback, 9000));

// There must be no two concurrent requests with the same requestid, even if in different sockets. For simplicity, this request will have request id equal to 1
ushort requestId = 5172;
using (var request = new WebServerSocketRequest(sock, requestId))
{
    // The BeginRequest Record defines how the application should respond. To know more read FastCgi's docs.
    request.SendBeginRequest(Role.Responder, true);

    // The Request Headers are sent with Params Records. You don't have to worry about the mechanisms, though: just write to the Params stream.
    using (var nvpWriter = new NvpWriter(request.Params))
    {
        // The WriteParamsFromUri is a helper method that writes the following Name-Value Pairs:
        // HTTP_HOST, HTTPS, SCRIPT_NAME, DOCUMENT_URI, REQUEST_METHOD, SERVER_NAME, QUERY_STRING, REQUEST_URI, SERVER_PROTOCOL, GATEWAY_INTERFACE
        //nvpWriter.WriteParamsFromUri(requestedUrl, requestMethod);

        // The other http request headers, e.g. User-Agent
        //nvpWriter.Write("HTTP_USER_AGENT", "Super cool Browser v1.0");

        nvpWriter.Write("SCRIPT_FILENAME", @"C:\inetpub\wwwroot\phpinfo.php");
    }

    // If there is any request body, send it through the Stdin stream. If there is nothing to send, send an End-Of-Request Record (an empty record)
    request.SendEmptyStdin();

    // At this point, the application is processing the request and cooking up a response for us, so let's welcome the incoming data until the response is over
    int bytesRead;
    byte[] buf = new byte[4096];
    while (!request.ResponseComplete)
    {
        bytesRead = sock.Receive(buf, SocketFlags.None);
        request.FeedBytes(buf, 0, bytesRead).ToList();
    }

    // All the application's response will be in the Stdout and/or Stderr streams
    // Don't forget that the very first line of the output is ASCII encoded text with the response status, such as "Status: 200 OK"
    using (var reader = new StreamReader(request.Stdout))
    {
        Console.Write(reader.ReadToEnd());
    }
}
// The socket and all other resources are automatically disposed at this point.
// This implies that WebServerSocketRequest still doesn't multiplex requests 
// (not for long, hopefully - also, you can inherit from this class and make
// Dispose() not call CloseSocket() if you want to multiplex requests)
