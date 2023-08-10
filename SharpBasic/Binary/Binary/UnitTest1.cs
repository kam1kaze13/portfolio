using NUnit.Framework;

namespace Binary
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            long longToByte = 0x7F9B72F394568C1F;
            byte[] byteArray = LongToByte(longToByte);
            long byteToLong = ByteToLong(byteArray);
            Assert.AreEqual(longToByte, byteToLong);
        }

        public byte[] LongToByte(long num)
        {
            byte[] byteArray = new byte[8];

            for (int i=0; i<8; i++)
            {
                byteArray[i] = (byte)((num >> 8 * (7 - i)) & 0xFF);
            }

            return byteArray;
        }

        public long ByteToLong(byte[] array)
        {
            long byteToLong = 0;

            for (int i=0; i<8; i++)
            {
                byteToLong = (byteToLong << 8) | array[i];
            }
            return byteToLong;
        }
    }
}