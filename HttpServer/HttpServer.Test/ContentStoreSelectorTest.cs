using HttpServer.Core;
using NUnit.Framework;
using System;
using System.IO;

namespace HttpServer.Test
{
    public class ContentStoreSelectorTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TwoStoreTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var store2 = new MemoryContentStore();
            store2.AddContentItem("simple.htm", "store2 simple.htm", DateTime.Now);
            store2.AddContentItem("index.htm", "store2 index.htm", DateTime.Now);

            var contentStoreSelector = new ContentStoreSelector();
            contentStoreSelector.AddRule(ContentRules.Directory("/dir1"), store1);
            contentStoreSelector.AddRule(ContentRules.Directory("/dir2"), store2);

            var store1sample = contentStoreSelector.GetContent("dir1/simple.htm");

            string body = store1sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);

            var store2sample = contentStoreSelector.GetContent("dir2/simple.htm");

            body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store2 simple.htm", body);
        }

        [Test]
        public void CustomPredicateTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var contentStoreSelector = new ContentStoreSelector();
            contentStoreSelector.AddRule(CustomPredicate.Or(ContentRules.Directory("/dir1"), ContentRules.Extension("htm")), store1);

            var store1sample = contentStoreSelector.GetContent("dir1/simple.htm");

            string body = store1sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);

            var store2 = new MemoryContentStore();
            store2.AddContentItem("simple.htm", "store2 simple.htm", DateTime.Now);
            store2.AddContentItem("index.htm", "store2 index.htm", DateTime.Now);

            contentStoreSelector.AddRule(CustomPredicate.And(ContentRules.Directory("/dir2"), ContentRules.Extension(".htm")), store2);

            var store2sample = contentStoreSelector.GetContent("dir2/simple.htm");

            body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store2 simple.htm", body);
        }

        [Test]
        public void FileDirStoreTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var store2 = new MemoryContentStore();
            store2.AddContentItem("simple.htm", "store2 simple.htm", DateTime.Now);
            store2.AddContentItem("index.htm", "store2 index.htm", DateTime.Now);

            var store3 = new FileDirectoryContentStore("Content");

            var contentStoreSelector = new ContentStoreSelector();
            contentStoreSelector.AddRule(ContentRules.Directory("/dir1"), store1);
            contentStoreSelector.AddRule(ContentRules.Directory("/dir2"), store2);
            contentStoreSelector.AddRule(ContentRules.Directory("/dir3"), store3);


            var store1sample = contentStoreSelector.GetContent("dir1/simple.htm");
            string body = store1sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);

            var store2sample = contentStoreSelector.GetContent("/dir2/simple.htm");

            body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store2 simple.htm", body);


            var store3sample = contentStoreSelector.GetContent("/dir3/subdir/simple.htm");

            body = store3sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("content of simple.htm", body);
        }

        [Test]
        public void NoContentTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("subdir/simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var store2 = new MemoryContentStore();
            store2.AddContentItem("simple.htm", "store2 simple.htm", DateTime.Now);
            store2.AddContentItem("index.htm", "store2 index.htm", DateTime.Now);

            var store3 = new FileDirectoryContentStore("Content");

            var contentStoreSelector = new ContentStoreSelector();
            contentStoreSelector.AddRule(ContentRules.Directory("/dir1"), store1);
            contentStoreSelector.AddRule(ContentRules.Directory("/dir2"), store2);
            contentStoreSelector.AddRule(ContentRules.Directory("/dir3"), store3);

            var content1 = contentStoreSelector.GetContent("/simple.htm");
            Assert.AreEqual(null, content1);

            var content2 = contentStoreSelector.GetContent("dir1/subdir2/simple.htm");
            Assert.AreEqual(null, content2);

            var content3 = contentStoreSelector.GetContent("/dir1/subdir3/subdir/simple2.htm");
            Assert.AreEqual(null, content3);
        }
    }
}