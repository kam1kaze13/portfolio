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
    public class PartialStreamTest
    {
        [Test]
        public void WholeStringTest()
        {
            string str = "test string for partial stream";
            using (var stream = StreamUtil.StreamFromString(str))
            {
                var partialStream = new PartialStream(stream, 0, str.Length);
                var reader = new StreamReader(partialStream);
                string actual = reader.ReadToEnd();

                Assert.AreEqual(actual, str);
            }
        }

        [Test]
        public void BeginStringTest()
        {
            string str = "test\r\nstring\r\n for partial stream";
            using (var stream = StreamUtil.StreamFromString(str))
            {
                var partialStream = new PartialStream(stream, 0, 4);
                var partialReader = new StreamReader(partialStream);
                string actual = partialReader.ReadToEnd();

                Assert.AreEqual("test", actual);
            }
        }

        [Test]
        public void MiddleStringTest()
        {
            string str = "test\r\nstring\r\n for partial stream";
            using (var stream = StreamUtil.StreamFromString(str))
            {
                var partialStream = new PartialStream(stream, 6, 6);
                var partialReader = new StreamReader(partialStream);
                string actual = partialReader.ReadToEnd();

                Assert.AreEqual("string", actual);
            }
        }

        [Test]
        public void EndStringTest()
        {
            string str = "test\r\nstring\r\n for partial stream";
            using (var stream = StreamUtil.StreamFromString(str))
            {
                var partialStream = new PartialStream(stream, 27, 500);
                var partialReader = new StreamReader(partialStream);
                string actual = partialReader.ReadToEnd();

                Assert.AreEqual("stream", actual);
            }
        }
    }
}
