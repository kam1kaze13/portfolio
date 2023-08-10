using SocketClientServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class HttpProtocolExecutor : IProtocolExecutor<CustomHttpRequest,CustomHttpResponse>
    {
        private static readonly Regex RegexStartingLine = new Regex(@"([A-Za-z]+)\s+(\/?[\w\.?=%&=\-@/S,]*)\s+HTTP/(\d+\.\d+)", RegexOptions.Compiled);
        private static readonly Regex RegexHeaderLine = new Regex(@"(.+):\s+([\x20-\x7E]+)", RegexOptions.Compiled);

        private enum InputState
        {
            StartingLine,
            Headers
        }

        public ParsingResult<CustomHttpRequest> ParseInput(byte[] data, int offset, int length)
        {
            var state = InputState.StartingLine;
            var result = new ParsingResult<CustomHttpRequest>();

            var httpRequest = new CustomHttpRequest();
            int start = offset;
            for (int i = offset; i < length; i++)
            {
                var element = Encoding.UTF8.GetString(data, i, 2);

                if (element == "\r\n")
                {                   
                    string line = Encoding.UTF8.GetString(data, start, i-start);

                    switch (state)
                    {
                        case InputState.StartingLine:
                            {
                                var match = RegexStartingLine.Match(line);
                                if (!match.Success)
                                    throw new BadRequestException("Bad request");

                                httpRequest.Method = match.Groups[1].Value;
                                httpRequest.RequestUri = match.Groups[2].Value.ToLower();
                                httpRequest.HttpVersion = match.Groups[3].Value;

                                if (i + 1 == length - 1)
                                {
                                    result.ParsedObjects.Add(httpRequest);
                                    break;
                                }

                                state = InputState.Headers;
                                break;
                            }
                        case InputState.Headers:
                            {
                                var match = RegexHeaderLine.Match(line);
                                if (!match.Success)
                                    throw new BadRequestException("Bad request");

                                httpRequest.Headers.Add(match.Groups[1].Value, match.Groups[2].Value);


                                if (i + 1 == length - 1)
                                {
                                    result.ParsedObjects.Add(httpRequest);
                                    break;
                                }                                   
                                if (match.Groups[1].Value == HttpHeaders.ContentLength)
                                {
                                    httpRequest.Body = Encoding.UTF8.GetString(data, i + 4, Convert.ToInt32(match.Groups[2].Value));
                                    i += 3 + Convert.ToInt32(match.Groups[2].Value);
                                    result.ParsedObjects.Add(httpRequest);
                                    httpRequest = new CustomHttpRequest();
                                    state = InputState.StartingLine;
                                }
                                else if (Encoding.UTF8.GetString(data, i + 2, 2) == "\r\n")
                                {
                                    result.ParsedObjects.Add(httpRequest);
                                    state = InputState.StartingLine;
                                    httpRequest = new CustomHttpRequest();
                                    i += 4;
                                    continue;
                                }
                                break;
                            }
                    }

                    start = i + 1;
                    i++;
                }

            }

            result.ProcessedBytes = start;
            result.IsClosed = true;

            return result;
        }

        public byte[] CreateResponse(CustomHttpResponse response)
        {
            var memStream = new MemoryStream();
            memStream.Write(Encoding.UTF8.GetBytes($"HTTP/{response.HttpVersion} {response.StatusCode} {response.ReasonPhrase}\r\n"));
            
            if (response.Headers.Count > 0)
            {
                foreach (var header in response.Headers)
                {
                    memStream.Write(Encoding.UTF8.GetBytes($"{header.Key}: {header.Value}\r\n"));
                }
            }

            memStream.Write(Encoding.UTF8.GetBytes("\r\n"));

            if (response.Body != null)
                memStream.Write(Encoding.UTF8.GetBytes(response.Body));

            return memStream.ToArray();
        }

        public ParsingResult<CustomHttpResponse> ParseOutput(byte[] data, int offset, int length)
        {
            var result = new ParsingResult<CustomHttpResponse>();

            var httpResponse = new CustomHttpResponse();

            var headersLength = Int32.Parse(Encoding.UTF8.GetString(data, offset, 2));

            int start = offset + 4;
            for (int i = offset + 4; i < headersLength; i++)
            {
                var element = Encoding.UTF8.GetString(data, i, 2);

                if (element == "\r\n")
                {
                    string line = Encoding.UTF8.GetString(data, start, i - start);

                    var match = RegexHeaderLine.Match(line);
                    if (!match.Success)
                        throw new BadRequestException("Bad response");

                    httpResponse.Headers.Add(match.Groups[1].Value, match.Groups[2].Value);

                    start = i + 1;
                    i++;
                }

            }

            httpResponse.Body = Encoding.UTF8.GetString(data, headersLength + 2, length - 2 - headersLength);
            result.ParsedObjects.Add(httpResponse);

            result.ProcessedBytes = start;
            result.IsClosed = true;

            return result;
        }

        public byte[] CreateRequest(CustomHttpRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
