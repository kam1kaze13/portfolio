
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileDownloader.Core
{
    public enum HttpMethod
    {
        GET,
        HEAD
    }
    public record HttpRequest(HttpMethod HttpMethod, string URL, long Start = 0, long Length = 0);

    public record HttpResponse(byte[] Body, long ContentLength);

    public static class HttpProtocol
    {
        public static byte[] CreateRequest(HttpRequest request)
        {
            string byteRequest ="";

            var uri = new Uri(request.URL);

            switch(request.HttpMethod)
            {
                case HttpMethod.HEAD:
                    {
                        byteRequest = $"HEAD {request.URL} HTTP/1.1\r\nHost: {NetUtil.ResolveIpAddress(uri.Host)}\r\n\r\n";
                        break;
                    }
                case HttpMethod.GET:
                    {
                        byteRequest = $"GET {request.URL} HTTP/1.1\r\nHost: {NetUtil.ResolveIpAddress(uri.Host)}\r\nRange: bytes={request.Start}-{request.Start + request.Length - 1}\r\n\r\n";
                        break;
                    }
            }

            return Encoding.ASCII.GetBytes(byteRequest);
        }

        public static HttpResponse ParseOutput(byte[] buffer, int offset, int length)
        {
            var resultArray = new byte[0];
            long contentLength;

            var response = Encoding.ASCII.GetString(buffer, offset, length);
            var contentLengthStr = response.Substring(response.IndexOf("Content-Length:")).Split('\n');
            try
            {
                contentLength = long.Parse(contentLengthStr[0].Substring(16, contentLengthStr[0].Length - 17));
            }
            catch
            {
                contentLength = 0;
            }

            var emptyStr = response.IndexOf("\r\n\r\n") + 4;


            resultArray = new byte[length - emptyStr];
            Array.Copy(buffer, emptyStr, resultArray, 0, resultArray.Length);

            return new HttpResponse(resultArray, contentLength);
        }
    }
}
