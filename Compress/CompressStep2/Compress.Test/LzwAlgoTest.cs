using Compress.Core;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    public class LzwAlgoTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BookTest()
        {
            var data = new byte[] { 45, 55, 55, 151, 55, 55, 55 };
            var lzw = new LzwAlgo();

            var packed = lzw.Pack(data);

            var unpacked = lzw.Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void BasicCodesTest()
        {
            var data = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
            var lzw = new LzwAlgo();

            var packed = lzw.Pack(data);

            var unpacked = lzw.Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void RepeatBasicCodesTest()
        {
            var data = Enumerable.Range(0, 1000000).Select(i => (byte)i).ToArray();
            var lzw = new LzwAlgo();

            var packed = lzw.Pack(data);

            var unpacked = lzw.Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void LinearCodesTest()
        {
            var data = Enumerable.Range(0, 100000).SelectMany(i => BitConverter.GetBytes(i)).ToArray();
            var lzw = new LzwAlgo();

            var packed = lzw.Pack(data);

            var unpacked = lzw.Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void StepsCodesTest()
        {
            var data = Enumerable.Range(0, 100000).SelectMany(i => Enumerable.Range(1, 10).Select(j =>(byte)i)).ToArray();
            var lzw = new LzwAlgo();

            var packed = lzw.Pack(data);

            var unpacked = lzw.Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void RepeatCodesTest()
        {
            var data = Enumerable.Range(0, 1000).SelectMany(i => Enumerable.Range(1, 10).Select(j => (byte)j)).ToArray();
            var lzw = new LzwAlgo();

            var packed = lzw.Pack(data, new LzwAlgoParams { MaxCodeBitCount = 9 });

            var unpacked = lzw.Unpack(packed);

            CollectionAssert.AreEqual(data, unpacked);
        }

        [Test]
        public void RandomCheckTest()
        {
            int repeats = 50;
            int minLength = 1;
            int maxLength = 200000;

            for (int i = 0; i < repeats; i++)
            {
                var lzw = new LzwAlgo();
                var data = generateRandomBytes(minLength, maxLength);
                var packed = lzw.Pack(data, new LzwAlgoParams { MaxCodeBitCount = 12 });
                var unpacked = lzw.Unpack(packed);

                CollectionAssert.AreEqual(data, unpacked);
            }
        }

        private byte[] generateRandomBytes(int minLength, int maxLength)
        {
            var r = new Random();
            var res = new byte[r.Next(minLength, maxLength + 1)];
            for (int i = 0; i < res.Length; i++)
                res[i] = (byte)r.Next(256);

            return res;
        }
    }
}