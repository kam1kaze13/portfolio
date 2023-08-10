using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public static class HttpResponseFactory
    {
        public static CustomHttpResponse CreateResponse(IContent content, CustomHttpRequest request, CustomHttpRequest.RangeInfo range, bool noBody)
        {
            CustomHttpResponse response;

            if (request.RequestUri.Contains(".jsk"))
                response = content.GetResponse(request);
            else
                response = new CustomHttpResponse();
                
            response.HttpVersion = request.HttpVersion;
            response.StatusCode = 200;
            response.ReasonPhrase = "OK";
            response.Date = DateTime.UtcNow;
            response.AcceptRanges = "bytes";

            if (!noBody)
            {
                if (range == null)
                    response.Body = content.GetResponse(request).Body;
                else
                {
                    var partialStream = new PartialStream(StreamUtil.StreamFromString(content.GetResponse(request).Body), (int)range.Begin, (int)(range.End - range.Begin + 1));
                    response.Body = StreamUtil.StreamToString(partialStream);

                    if (request.HttpVersion == "1.1")
                        response.StatusCode = 206;
                }

                response.ContentLength = response.Body.Length;
            }

            return response;
        }

        public static CustomHttpResponse CreateResponse404(string httpVersion)
        {
            var response = new CustomHttpResponse();

            response.HttpVersion = httpVersion;
            response.StatusCode = 404;
            response.ReasonPhrase = "Not Found";
            response.Date = DateTime.UtcNow;
            response.AcceptRanges = "bytes";

            return response;
        }

        public static CustomHttpResponse CreateResponse304(string httpVersion)
        {
            var response = new CustomHttpResponse();

            response.HttpVersion = httpVersion;
            response.StatusCode = 304;
            response.ReasonPhrase = "Not Modified";
            response.Date = DateTime.UtcNow;
            response.AcceptRanges = "bytes";

            return response;
        }

        public static CustomHttpResponse CreateResponse416(string httpVersion)
        {
            var response = new CustomHttpResponse();

            response.HttpVersion = httpVersion;
            response.StatusCode = 416;
            response.ReasonPhrase = "Requested Range Not Satisfiable";
            response.Date = DateTime.UtcNow;
            response.AcceptRanges = "bytes";

            return response;
        }

        public static CustomHttpResponse CreateResponse400(string httpVersion)
        {
            var response = new CustomHttpResponse();

            response.HttpVersion = httpVersion;
            response.StatusCode = 400;
            response.ReasonPhrase = "Bad Request";
            response.Date = DateTime.UtcNow;
            response.AcceptRanges = "bytes";

            return response;
        }

        public static CustomHttpResponse CreateResponse501(string httpVersion)
        {
            var response = new CustomHttpResponse();

            response.HttpVersion = httpVersion;
            response.StatusCode = 501;
            response.ReasonPhrase = "Not Implemented";
            response.Date = DateTime.UtcNow;
            response.AcceptRanges = "bytes";

            return response;
        }

        public static CustomHttpResponse CreateResponse408(string httpVersion)
        {
            var response = new CustomHttpResponse();

            response.HttpVersion = httpVersion;
            response.StatusCode = 408;
            response.ReasonPhrase = "Request Timeout";
            response.Date = DateTime.UtcNow;
            response.AcceptRanges = "bytes";

            return response;
        }

        public static CustomHttpResponse CreateResponse500(string httpVersion)
        {
            var response = new CustomHttpResponse();

            response.HttpVersion = httpVersion;
            response.StatusCode = 500;
            response.ReasonPhrase = "Internal Server Error";
            response.Date = DateTime.UtcNow;
            response.AcceptRanges = "bytes";

            return response;
        }
    }
}
