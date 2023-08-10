using HttpServer.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Test
{
    public class CgiContentStoreTest
    {
        [Test]
        public void BaseContentTest()
        {
            var store = new CgiContentStore();

            var content = store.GetContent("\\ourfile.jsk");

            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "\\ourfile.jsk",
                HttpVersion = "1.1",
                Host = "localhost:300",
                Connection = "Keep-Alive"

            };

            string body = content.GetResponse(request).Body;
            Assert.AreEqual("<html><head>\t<title>\t\tApptime\t</title></head><body><p>\t0</p><p>\t1</p><p>\t2</p><p>\t3</p><p>\t4</p><p>\t5</p><p>\t6</p><p>\t7</p><p>\t8</p><p>\t9</p></body><div style=\"sdsd\"></html>\r\n", body);
        }

        [Test]
        public void SubdirContentTest()
        {
            var store = new CgiContentStore();

            var content = store.GetContent("\\Content\\ourfile.jsk");

            var request = new CustomHttpRequest
            {
                Method = "GET",
                RequestUri = "\\Content\\ourfile.jsk",
                HttpVersion = "1.1",
                Host = "localhost:300",
                Connection = "Keep-Alive"

            };

            string body = content.GetResponse(request).Body;
            Assert.AreEqual("<html><head>\t<title>\t\tApptime\t</title></head><body><p>\t0</p><p>\t1</p><p>\t2</p><p>\t3</p><p>\t4</p><p>\t5</p><p>\t6</p><p>\t7</p><p>\t8</p><p>\t9</p></body><div style=\"sdsd\"></html>\r\n", body);
        }

        [Test]
        public void NoContentTest()
        {
            var store = new CgiContentStore();

            var content = store.GetContent("\\temp.jsk");

            Assert.AreEqual(null, content);
        }
    }
}
