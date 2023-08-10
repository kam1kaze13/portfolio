using HttpServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class HttpServerEngine
    {
        private IContentStore contentStore;

        public HttpServerEngine(IContentStore contentStore)
        {
            this.contentStore = contentStore;
        }

        public CustomHttpResponse GetResponse(CustomHttpRequest request)
        {
            try
            {
                CustomHttpResponse response;
                bool noBody = request.Method == "HEAD";

                if (request.Method == "GET" || request.Method == "HEAD")
                {
                    IContent content = this.contentStore.GetContent(request.RequestUri);
                    if (content == null)
                    {
                        response = HttpResponseFactory.CreateResponse404(request.HttpVersion);
                    }
                    else if (request.IfModifiedSince.HasValue && request.IfModifiedSince > content.GetResponse(request).IfModifiedSince)
                    {
                        response = HttpResponseFactory.CreateResponse304(request.HttpVersion);
                    }
                    else
                    {
                        var range = request.Range;
                        if (this.RangeIsValid(content, request))
                        {
                            if (range != null)
                                range = this.NormalizeRange(content, request);

                            response = HttpResponseFactory.CreateResponse(content, request, range, noBody);
                        }
                        else
                        {
                            response = HttpResponseFactory.CreateResponse416(request.HttpVersion);
                        }
                    }

                    return response;
                }

                return HttpResponseFactory.CreateResponse501(request.HttpVersion);
            }
            catch (Exception)
            {
                throw;
            }
            throw new Exception();
        }

        public CustomHttpResponse GetResponse(BadRequestException e)
        {
            return HttpResponseFactory.CreateResponse400("1.1");
        }

        public CustomHttpResponse GetResponse(RequestTimeoutException e)
        {
            return HttpResponseFactory.CreateResponse408("1.1");
        }

        public CustomHttpResponse GetResponse(Exception e)
        {
            return HttpResponseFactory.CreateResponse500("1.1");
        }

        private bool RangeIsValid(IContent content, CustomHttpRequest request)
        {
            return
                request.Range == null || (request.Range.Begin < content.GetResponse(request).Body.Length && (request.Range.End == -1 || request.Range.End >= request.Range.Begin));
        }

        private CustomHttpRequest.RangeInfo NormalizeRange(IContent content, CustomHttpRequest request)
        {
            if (request.Range.Begin == -1)
            {
                if (content.GetResponse(request).Body.Length > request.Range.End)
                {
                    return new CustomHttpRequest.RangeInfo { Begin = content.GetResponse(request).Body.Length - request.Range.End, End = content.GetResponse(request).Body.Length - 1 };
                }
                else
                {
                    return new CustomHttpRequest.RangeInfo { Begin = 0, End = content.GetResponse(request).Body.Length - 1 };
                }
            }
            else if (request.Range.End == -1)
            {
                if (content.GetResponse(request).Body.Length > request.Range.Begin)
                {
                    return new CustomHttpRequest.RangeInfo { Begin = request.Range.Begin, End = content.GetResponse(request).Body.Length - 1 };
                }
                else
                {
                    return new CustomHttpRequest.RangeInfo { Begin = content.GetResponse(request).Body.Length - 1, End = content.GetResponse(request).Body.Length - 1 };
                }
            }

            return new CustomHttpRequest.RangeInfo { Begin = request.Range.Begin, End = request.Range.End };
        }
    }
}
