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
    public class FileDirectoryContentStoreTest
    {
        [Test]
        public void BaseContentTest()
        {
            var store = new FileDirectoryContentStore("Content");

            var content = store.GetContent("index.htm");

            string body = content.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("content of index.htm", body);
        }

        [Test]
        public void CaseInsensitiveContentTest()
        {
            var store = new FileDirectoryContentStore("Content");

            var content = store.GetContent("iNdeX.hTm");

            string body = content.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("content of index.htm", body);
        }

        [Test]
        public void SubdirContentTest()
        {
            var store = new FileDirectoryContentStore("Content");

            var content = store.GetContent("/subdir/simple.htm");

            string body = content.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("content of simple.htm", body);       
        }

        [Test]
        public void NoContentTest()
        {
            var store = new FileDirectoryContentStore("Content");

            var content = store.GetContent("subdir/simple2.htm");

            Assert.AreEqual(null, content);
        }

        [Test]
        public void NotLocalContentTest()
        {
            var store = new FileDirectoryContentStore("Content");

            var content = store.GetContent("../../subdir/simple.htm");

            Assert.AreEqual(null, content);
        }
    }
}
