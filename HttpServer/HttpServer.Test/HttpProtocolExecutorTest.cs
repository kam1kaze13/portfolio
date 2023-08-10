using HttpServer.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Test
{
    public class HttpProtocolExecutorTest
    {
        [Test]
        public void SimpleRequestTest()
        {
            string requestStr =
@"GET /wiki/HTTP HTTP/1.0
";
            var byteRequest = Encoding.ASCII.GetBytes(requestStr);

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var requests = httpProtocolExecutor.ParseInput(byteRequest, 0, byteRequest.Length);

            Assert.IsTrue(requests.ParsedObjects.Count == 1);
            var request = requests.ParsedObjects[0];

            Assert.AreEqual(request.Method, "GET");
            Assert.AreEqual(request.RequestUri, "/wiki/http");
            Assert.AreEqual(request.HttpVersion, "1.0");
            Assert.IsTrue(request.Headers.Count == 0);
            Assert.AreEqual(request.ContentLength, -1);
            Assert.AreEqual(request.Body, null);
            Assert.AreEqual(request.Accept, null);
            Assert.AreEqual(request.Host, null);
            Assert.AreEqual(request.IfModifiedSince, null);
            Assert.AreEqual(request.KeepAlive, false);
            Assert.AreEqual(request.TransferEncoding, null);
            Assert.AreEqual(request.UserAgent, null);
        }

        [Test]
        public void RequestWithHeaderTest()
        {
            string requestStr =
@"GET /wiki/HTTP HTTP/1.0
Host: www.codemastersintl.com
Connection: Keep-Alive
User-Agent: Mozilla
";
            var byteRequest = Encoding.ASCII.GetBytes(requestStr);

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var requests = httpProtocolExecutor.ParseInput(byteRequest, 0, byteRequest.Length);

            Assert.IsTrue(requests.ParsedObjects.Count == 1);
            var request = requests.ParsedObjects[0];

            Assert.AreEqual(request.Method, "GET");
            Assert.AreEqual(request.RequestUri, "/wiki/http");
            Assert.AreEqual(request.HttpVersion, "1.0");
            Assert.IsTrue(request.Headers.Count == 3);
            Assert.AreEqual(request.ContentLength, -1);
            Assert.AreEqual(request.Body, null);
            Assert.AreEqual(request.Accept, null);
            Assert.AreEqual(request.Host, "www.codemastersintl.com");
            Assert.AreEqual(request.IfModifiedSince, null);
            Assert.AreEqual(request.KeepAlive, true);
            Assert.AreEqual(request.TransferEncoding, null);
            Assert.AreEqual(request.UserAgent, "Mozilla");
        }

        [Test]
        public void RequestWithHeaderHttp11Test()
        {
            string requestStr =
@"GET /wiki/HTTP?action=getData&data=123 HTTP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html
";
            var byteRequest = Encoding.ASCII.GetBytes(requestStr);

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var requests = httpProtocolExecutor.ParseInput(byteRequest, 0, byteRequest.Length);

            Assert.IsTrue(requests.ParsedObjects.Count == 1);
            var request = requests.ParsedObjects[0];

            Assert.AreEqual(request.Method, "GET");
            Assert.AreEqual(request.RequestUri, "/wiki/http?action=getdata&data=123");
            Assert.AreEqual(request.HttpVersion, "1.1");
            Assert.IsTrue(request.Headers.Count == 5);
            Assert.AreEqual(request.ContentLength, -1);
            Assert.AreEqual(request.Body, null);
            Assert.AreEqual(request.Accept, "text/html");
            Assert.AreEqual(request.Host, "www.codemastersintl.com");
            Assert.AreEqual(request.IfModifiedSince.Value.ToUniversalTime(), new DateTime(1994, 10, 29, 19, 43, 31, DateTimeKind.Utc));
            Assert.AreEqual(request.KeepAlive, true);
            Assert.AreEqual(request.TransferEncoding, "UTF8");
            Assert.AreEqual(request.UserAgent, "Mozilla");
        }

        [Test]
        public void RequestWithHeaderAndBodyTest()
        {
            string requestStr =
@"GET /wiki/HTTP?action=getData&data=123 HTTP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html
Content-Length: 9

some body";
            var byteRequest = Encoding.ASCII.GetBytes(requestStr);

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var requests = httpProtocolExecutor.ParseInput(byteRequest, 0, byteRequest.Length);

            Assert.IsTrue(requests.ParsedObjects.Count == 1);
            var request = requests.ParsedObjects[0];

            Assert.AreEqual(request.Method, "GET");
            Assert.AreEqual(request.RequestUri, "/wiki/http?action=getdata&data=123");
            Assert.AreEqual(request.HttpVersion, "1.1");
            Assert.IsTrue(request.Headers.Count == 6);
            Assert.AreEqual(request.ContentLength, 9);
            Assert.AreEqual(request.Body, "some body");
            Assert.AreEqual(request.Accept, "text/html");
            Assert.AreEqual(request.Host, "www.codemastersintl.com");
            Assert.AreEqual(request.IfModifiedSince.Value.ToUniversalTime(), new DateTime(1994, 10, 29, 19, 43, 31, DateTimeKind.Utc));
            Assert.AreEqual(request.KeepAlive, true);
            Assert.AreEqual(request.TransferEncoding, "UTF8");
            Assert.AreEqual(request.UserAgent, "Mozilla");
        }

        [Test]
        public void MultipleRequestTest()
        {
            string requestStr =
@"GET /wiki/HTTP?action=getData&data=123 HTTP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html
Content-Length: 9

some bodyGET /wiki/HTTP?action=getData&data=123 HTTP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html

GET /wiki/HTTP?action=getData&data=123 HTTP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html
Content-Length: 9

some bodyGET /wiki/HTTP?action=getData&data=123 HTTP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html
Content-Length: 9

some body";
            var byteRequest = Encoding.ASCII.GetBytes(requestStr);

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var requests = httpProtocolExecutor.ParseInput(byteRequest, 0, byteRequest.Length);

            Assert.IsTrue(requests.ParsedObjects.Count == 4);
            var request = requests.ParsedObjects[3];

            Assert.AreEqual(request.Method, "GET");
            Assert.AreEqual(request.RequestUri, "/wiki/http?action=getdata&data=123");
            Assert.AreEqual(request.HttpVersion, "1.1");
            Assert.IsTrue(request.Headers.Count == 6);
            Assert.AreEqual(request.ContentLength, 9);
            Assert.AreEqual(request.Body, "some body");
            Assert.AreEqual(request.Accept, "text/html");
            Assert.AreEqual(request.Host, "www.codemastersintl.com");
            Assert.AreEqual(request.IfModifiedSince.Value.ToUniversalTime(), new DateTime(1994, 10, 29, 19, 43, 31, DateTimeKind.Utc));
            Assert.AreEqual(request.KeepAlive, true);
            Assert.AreEqual(request.TransferEncoding, "UTF8");
            Assert.AreEqual(request.UserAgent, "Mozilla");
        }

        /*[Test]
        public void BadRequestTest()
        {
            string requestStr =
@"GET /wiki/HTTP?action=getData&data=123 HT_TP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html
Content-Length: 9

some body";

            var byteRequest = Encoding.ASCII.GetBytes(requestStr);

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var requests = httpProtocolExecutor.ParseInput(byteRequest, 0, byteRequest.Length);
        }

        [Test]
        public void SecondBadRequestTest()
        {
            string requestStr =
@"GET /wiki/HTTP?action=getData&data=123 HTTP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html
Content-Length: 9

some bodyGET /wiki/HTTP?action=getData&data=123 HT_TP/1.1
Host: www.codemastersintl.com
User-Agent: Mozilla
Transfer-Encoding: UTF8
If-Modified-Since: Sat, 29 Oct 1994 19:43:31 GMT
Accept: text/html
Content-Length: 9

some body";
            var byteRequest = Encoding.ASCII.GetBytes(requestStr);

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var requests = httpProtocolExecutor.ParseInput(byteRequest, 0, byteRequest.Length);

            foreach (var request in requests.ParsedObjects)
            {
                Assert.AreEqual(request.Method, "get");
                Assert.AreEqual(request.RequestUri, "/wiki/http?action=getdata&data=123");
                Assert.AreEqual(request.HttpVersion, "1.1");
                Assert.IsTrue(request.Headers.Count == 6);
                Assert.AreEqual(request.ContentLength, 9);
                Assert.AreEqual(request.Body, "some body");
                Assert.AreEqual(request.Accept, "text/html");
                Assert.AreEqual(request.Host, "www.codemastersintl.com");
                Assert.AreEqual(request.IfModifiedSince.Value.ToUniversalTime(), new DateTime(1994, 10, 29, 19, 43, 31, DateTimeKind.Utc));
                Assert.AreEqual(request.KeepAlive, true);
                Assert.AreEqual(request.TransferEncoding, "UTF8");
                Assert.AreEqual(request.UserAgent, "Mozilla");
            }
        }*/

        [Test]
        public void SimpleResponseTest()
        {
            var response = new CustomHttpResponse
            {
                HttpVersion = "1.1",
                StatusCode = 200,
                ReasonPhrase = "OK"
            };

            string expectedStr =
@"HTTP/1.1 200 OK

";
            var httpProtocolExecutor = new HttpProtocolExecutor();

            var byteResponse = httpProtocolExecutor.CreateResponse(response);

            var responseStr = Encoding.ASCII.GetString(byteResponse);
            Assert.AreEqual(expectedStr, responseStr);
        }
        
        [Test]
        public void HeadersResponseTest()
        {
            var response = new CustomHttpResponse
            {
                HttpVersion = "1.1",
                StatusCode = 200,
                ReasonPhrase = "OK"
            };

            response.Headers.Add(HttpHeaders.ContentLength, "0");
            response.Headers.Add(HttpHeaders.Host, "localhost");

            string expectedStr =
@"HTTP/1.1 200 OK
Content-Length: 0
Host: localhost

";

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var byteResponse = httpProtocolExecutor.CreateResponse(response);

            var responseStr = Encoding.ASCII.GetString(byteResponse);
            Assert.AreEqual(expectedStr, responseStr);
        }

        [Test]
        public void BodyResponseTest()
        {
            var response = new CustomHttpResponse
            {
                HttpVersion = "1.1",
                StatusCode = 200,
                ReasonPhrase = "OK",
                Body = "Some content"
            };

            response.Headers.Add(HttpHeaders.ContentLength, "12");
            response.Headers.Add(HttpHeaders.Host, "localhost");

            string expectedStr =
@"HTTP/1.1 200 OK
Content-Length: 12
Host: localhost

Some content";

            var httpProtocolExecutor = new HttpProtocolExecutor();

            var byteResponse = httpProtocolExecutor.CreateResponse(response);

            var responseStr = Encoding.ASCII.GetString(byteResponse);
            Assert.AreEqual(expectedStr, responseStr);
        }
    }
}
