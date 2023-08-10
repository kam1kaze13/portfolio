using Compress.Core;
using NUnit.Framework;
using System;

namespace Tests
{
    public class PrototypeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ByteTest()
        {
            var data = new byte[256] ;

            for (int i = 0; i < 256; i++)
                data[i] = (byte)i;

            var lzw = new LzwAlgo();

            var packed = lzw.Pack(data);

            var unpacked = lzw.Unpack(packed);
           
            Assert.AreEqual(data, unpacked);
        }

        [Test]
        public void BookTest()
        {
            var data = new byte[] { 45, 55, 55, 151, 55, 55, 55 };
            var lzw = new LzwAlgo();

            var packed = lzw.Pack(data);
          
            var unpacked = lzw.Unpack(packed);

            Assert.AreEqual(data, unpacked);
        }
        
        [Test]
        public void RandomCheckTest()
        {
            int repeats = 100;
            int minLength = 1;
            int maxLength = 100000;

            for (int i = 0; i < repeats; i++)
            {
                var lzw = new LzwAlgo();
                var data = generateRandomBytes(minLength, maxLength);
                var packed = lzw.Pack(data);
                var unpacked = lzw.Unpack(packed);

                CollectionAssert.AreEqual(data, unpacked);
            }
        }

        [Test]
        public void BitCountTest()
        {
            var data = generateBytes(300000); 

            var lzw = new LzwAlgo();
            var packed = lzw.Pack(data);
            var unpacked = lzw.Unpack(packed);

            Assert.AreEqual(data, unpacked);
        }

        private byte[] generateBytes(int length)
        {
            var rand = new Random();
            var bytes = new byte[length];
            rand.NextBytes(bytes);
            return bytes;
        }

        private byte[] generateRandomBytes(int minLength, int maxLength)
        {
            var rand = new Random();
            var length = rand.Next(minLength, maxLength);
            var bytes = new byte[length];
            rand.NextBytes(bytes);
            return bytes;
        }
    }

    class BitReaderWriterTest
    {

        [Test]
        public void BitWriterTest1()
        {
            var bitWriter = new BitWriter();
            bitWriter.Writer(0, 3);
            bitWriter.Writer(0b1111111, 7);
            var res = bitWriter.GetBytes();
        }

        private ulong GetAllBitsFor(int bitCount)
        {
            ulong code = 1;
            if (bitCount > 1)
            {
                for (int i = 0; i < bitCount-1; i++)
                {
                    code <<= 1;
                    code |= 1;                   
                }
            }      
            return code;
        }

        [Test]
        public void BitWriterTest2()
        {
            var bitWriter = new BitWriter();
            bitWriter.Writer(0, 1);
            bitWriter.Writer(GetAllBitsFor(24), 24);
            var res = bitWriter.GetBytes();
        }

        [Test]
        public void BitWriterTest3()
        {
            var bitWriter = new BitWriter();
            bitWriter.Writer(0, 3);
            bitWriter.Writer(GetAllBitsFor(37), 37);
            var res = bitWriter.GetBytes();
        }

        [Test]
        public void BitWriterTest4()
        {
            var bitWriter = new BitWriter();
            bitWriter.Writer(GetAllBitsFor(16), 16);
            var res = bitWriter.GetBytes();
        }


        [Test]
        public void BitWriterTest5()
        {
            var bitWriter = new BitWriter();
            bitWriter.Writer(0, 1);
            bitWriter.Writer(GetAllBitsFor(2), 2);
            bitWriter.Writer(GetAllBitsFor(2), 2);
            bitWriter.Writer(GetAllBitsFor(2), 2);
            bitWriter.Writer(GetAllBitsFor(2), 2);
            var res = bitWriter.GetBytes();
        }

        [Test]
        public void BitReaderTest1()
        {
            var bitReader = new BitReader();
            bitReader.PutBytes(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, });
            ulong l = 0;
            bitReader.TryRead(1, out l);
            bitReader.TryRead(1, out l);
            bitReader.TryRead(2, out l);
            bitReader.TryRead(2, out l);
            bitReader.TryRead(4, out l);
            bitReader.TryRead(4, out l);
            bitReader.TryRead(6, out l);
            bitReader.TryRead(6, out l);
            bitReader.TryRead(8, out l);
            bitReader.TryRead(8, out l);
        }

        [Test]
        public void BitReaderTest2()
        {
            var bitReader = new BitReader();
            bitReader.PutBytes(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, });
            ulong l = 0;
            bitReader.TryRead(2, out l);
            Assert.AreEqual(l, GetAllBitsFor(2));
            bitReader.TryRead(14, out l);
            Assert.AreEqual(l, GetAllBitsFor(14));
            bitReader.TryRead(14, out l);
            Assert.AreEqual(l, GetAllBitsFor(14));
            bitReader.TryRead(44, out l);
            Assert.AreEqual(l, GetAllBitsFor(44));
        }


        [Test]
        public void BitReaderTest3()
        {
            var bitReader = new BitReader();
            bitReader.PutBytes(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, });
            ulong l = 0;
            bitReader.TryRead(74, out l);
            Assert.AreEqual(l, GetAllBitsFor(74));
            bool b = bitReader.TryRead(74, out l);
            Assert.AreEqual(l, 0);
            Assert.AreEqual(b, false);
        }
    }
}