using HttpServer.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Test
{
    public class HttpServerTest
    {
        [Test]
        public void SimpleTest()
        {
            int port = this.GetPort();
            var server = this.StartHttpServer(port);
            try
            {
                var request = WebRequest.Create(string.Format("http://localhost:{0}/Content/index.htm", port));

                var response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string body = reader.ReadToEnd();
                    Assert.AreEqual("content of index.htm", body);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        [Test]
        public void Get404Test()
        {
            int port = this.GetPort();
            var server = this.StartHttpServer(port);
            try
            {
                var request = WebRequest.Create(string.Format("http://localhost:{0}/Content/missing.htm", port));

                try
                {
                    request.GetResponse();
                }
                catch (WebException e)
                {
                    Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
                    Assert.AreEqual(HttpStatusCode.NotFound, (e.Response as HttpWebResponse).StatusCode);
                }
            }
            finally
            {
                server.Stop();
            }
        }


        [Test]
        public void Get304Test()
        {
            int port = this.GetPort();
            var server = this.StartHttpServer(port);
            try
            {
                var request = WebRequest.Create(string.Format("http://localhost:{0}/memory/subdir/sample.htm", port));

                var response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string body = reader.ReadToEnd();
                    Assert.AreEqual("memory store simple.htm", body);
                }

                request = WebRequest.Create(string.Format("http://localhost:{0}/memory/subdir/sample.htm", port));
                request.Headers.Add(HttpHeaders.IfModifiedSince, (new DateTime()).ToString("r"));

                try
                {
                    request.GetResponse();
                }
                catch (WebException e)
                {
                    Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
                    Assert.AreEqual(HttpStatusCode.NotModified, (e.Response as HttpWebResponse).StatusCode);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        [Test]
        public void RangeTest()
        {
            int port = this.GetPort();
            var server = this.StartHttpServer(port);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format("http://localhost:{0}/Content/index.htm", port));
                request.AddRange(8, 15);

                var response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string body = reader.ReadToEnd();
                    Assert.AreEqual("of index", body);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        private int portCounter = 300;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int GetPort()
        {
            return this.portCounter++;
        }

        private Core.HttpServer StartHttpServer(int port)
        {
            var server = new Core.HttpServer(port, 10, 2000, new HttpProtocolExecutor(), new HttpServerEngine(this.GetContentStore()));
            server.Start();

            return server;
        }

        private IContentStore GetContentStore()
        {
            var memoryStore = new MemoryContentStore();
            memoryStore.AddContentItem("subdir/sample.htm", "memory store simple.htm", DateTime.Now - TimeSpan.FromHours(2));
            memoryStore.AddContentItem("index.htm", "memory store index.htm", DateTime.Now);

            var fileStore = new FileDirectoryContentStore("Content");

            var compositeStore = new CompositeContentStore();
            compositeStore.AddAssociation("/Content", fileStore);
            compositeStore.AddAssociation("/memory", memoryStore);

            return compositeStore;
        }
    }
}
