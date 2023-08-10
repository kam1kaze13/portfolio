using HttpServer.Core;
using NUnit.Framework;
using System;
using System.IO;

namespace HttpServer.Test
{
    public class CompositeContentStoreTest
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

            var compositeStore = new CompositeContentStore();
            compositeStore.AddAssociation("dir1", store1);
            compositeStore.AddAssociation("dir2", store2);

            var store1sample = compositeStore.GetContent("dir1/simple.htm");

            string body = store1sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);

            var store2sample = compositeStore.GetContent("dir2/simple.htm");

            body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store2 simple.htm", body);

        }

        [Test]
        public void CaseInsensitiveStoreTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var compositeStore = new CompositeContentStore();
            compositeStore.AddAssociation("dIR1", store1);

            var store1sample = compositeStore.GetContent("dir1/simple.htm");

            string body = store1sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);

            var store2sample = compositeStore.GetContent("Dir1/Simple.htm");
            body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);
        }

        [Test]
        public void RootStoreTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var store2 = new MemoryContentStore();
            store2.AddContentItem("simple.htm", "store2 simple.htm", DateTime.Now);
            store2.AddContentItem("index.htm", "store2 index.htm", DateTime.Now);

            var compositeStore = new CompositeContentStore();
            compositeStore.AddAssociation("dir1", store1);
            compositeStore.AddAssociation("/", store2);

            var store1sample = compositeStore.GetContent("dir1/simple.htm");

            string body = store1sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);

            var store2sample = compositeStore.GetContent("simple.htm");

            body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store2 simple.htm", body);

        }

        [Test]
        public void Subst1StoreTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var store2 = new MemoryContentStore();
            store2.AddContentItem("simple.htm", "store2 simple.htm", DateTime.Now);
            store2.AddContentItem("index.htm", "store2 index.htm", DateTime.Now);

            var compositeStore = new CompositeContentStore();
            compositeStore.AddAssociation("dir1", store1);
            compositeStore.AddAssociation("/dir1/subdir", store2);

            var store1sample = compositeStore.GetContent("dir1/simple.htm");

            string body = store1sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);

            var store2sample = compositeStore.GetContent("/dir1/subdir/simple.htm");

            body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store2 simple.htm", body);
        }

        [Test]
        public void Subst2StoreTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("subdir/simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var store2 = new MemoryContentStore();
            store2.AddContentItem("simple.htm", "store2 simple.htm", DateTime.Now);
            store2.AddContentItem("index.htm", "store2 index.htm", DateTime.Now);

            var compositeStore = new CompositeContentStore();
            compositeStore.AddAssociation("dir1", store1);
            compositeStore.AddAssociation("/dir1/subdir", store2);

            var store2sample = compositeStore.GetContent("/dir1/subdir/simple.htm");

            string body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);
        }

        [Test]
        public void FileDirStoreTest()
        {
            var store1 = new MemoryContentStore();
            store1.AddContentItem("subdir/simple.htm", "store1 simple.htm", DateTime.Now);
            store1.AddContentItem("index.htm", "store1 index.htm", DateTime.Now);

            var store2 = new MemoryContentStore();
            store2.AddContentItem("simple.htm", "store2 simple.htm", DateTime.Now);
            store2.AddContentItem("index.htm", "store2 index.htm", DateTime.Now);

            var store3 = new FileDirectoryContentStore("Content");

            var compositeStore = new CompositeContentStore();
            compositeStore.AddAssociation("dir1", store1);
            compositeStore.AddAssociation("/dir1/subdir2", store2);
            compositeStore.AddAssociation("/dir1/subdir3", store3);

            var store1sample = compositeStore.GetContent("dir1/subdir/simple.htm");
            string body = store1sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store1 simple.htm", body);

            var store2sample = compositeStore.GetContent("/dir1/subdir2/simple.htm");

            body = store2sample.GetResponse(new CustomHttpRequest()).Body;
            Assert.AreEqual("store2 simple.htm", body);


            var store3sample = compositeStore.GetContent("/dir1/subdir3/subdir/simple.htm");

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

            var compositeStore = new CompositeContentStore();
            compositeStore.AddAssociation("/", store1);
            compositeStore.AddAssociation("/dir1/subdir2", store3);
            compositeStore.AddAssociation("/dir1/subdir3", store3);

            var content1 = compositeStore.GetContent("/simple.htm");
            Assert.AreEqual(null, content1);

            var content2 = compositeStore.GetContent("dir1/subdir2/simple.htm");
            Assert.AreEqual(null, content2);

            var content3 = compositeStore.GetContent("/dir1/subdir3/subdir/simple2.htm");
            Assert.AreEqual(null, content3);
        }
    }
}