using HttpServer.Core;
using NUnit.Framework;
using System;
using System.IO;

namespace HttpServer.Test
{
    public class HttpServerEngineTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(200, response.StatusCode);

            Assert.AreEqual("bytes", response.Headers[HttpHeaders.AcceptRanges]);
            Assert.IsTrue(response.Headers.ContainsKey(HttpHeaders.Date));

            Assert.AreEqual("content of index.htm", response.Body);
        }

        [Test]
        public void RangeRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            request.Headers.Add("Range", "bytes=11-15");

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(206, response.StatusCode);

            Assert.AreEqual("index", response.Body);
        }

        [Test]
        public void RangeOpenLeftRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            request.Headers.Add("Range", "bytes=-9");

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(206, response.StatusCode);

            Assert.AreEqual("index.htm", response.Body);
        }

        [Test]
        public void RangeOpenRigntRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            request.Headers.Add("Range", "bytes=11-");

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(206, response.StatusCode);

            Assert.AreEqual("index.htm", response.Body);
        }

        [Test]
        public void OverflowRightRangeRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            request.Headers.Add("Range", "bytes=11-94");

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(206, response.StatusCode);

            Assert.AreEqual("index.htm", response.Body);
        }

        [Test]
        public void OverflowLeftRangeRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            request.Headers.Add("Range", "bytes=-69");

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(206, response.StatusCode);

            Assert.AreEqual("content of index.htm", response.Body);
        }

        [Test]
        public void MissingRangeRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            request.Headers.Add("Range", "bytes=27-69");

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(416, response.StatusCode);
        }

        [Test]
        public void BadRangeRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            request.Headers.Add("Range", "bytes=9-3");

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(416, response.StatusCode);
        }

        [Test]
        public void BadMethodRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "badmethod",
                RequestUri = "missing.htm",
                HttpVersion = "1.1",
            };

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(501, response.StatusCode);
        }

        [Test]
        public void HeadRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "HEAD",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
            };

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(null, response.Body);
        }

        [Test]
        public void ConditionalTrue1Request()
        {
            var store = new MemoryContentStore();
            var time = DateTime.Now - TimeSpan.FromHours(2);
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", time);

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
                IfModifiedSince = time,
            };

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(200, response.StatusCode);

            Assert.AreEqual("content of index.htm", response.Body);
        }

        [Test]
        public void ConditionalTrue2Request()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now - TimeSpan.FromHours(2));

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
                IfModifiedSince = DateTime.Now - TimeSpan.FromHours(3),
            };

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(200, response.StatusCode);

            Assert.AreEqual("content of index.htm", response.Body);
        }

        [Test]
        public void ConditionalFalseRequest()
        {
            var store = new MemoryContentStore();
            store.AddContentItem("simple.htm", "content of simple.htm", DateTime.Now);
            store.AddContentItem("index.htm", "content of index.htm", DateTime.Now - TimeSpan.FromHours(2));

            var httpEngine = new HttpServerEngine(store);
            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "index.htm",
                HttpVersion = "1.1",
                IfModifiedSince = DateTime.Now - TimeSpan.FromHours(1),
            };

            var response = httpEngine.GetResponse(request);

            Assert.AreEqual(304, response.StatusCode);
        }
    }
}