using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class MyUnpacker : ICryptoUnpacker
    {
        public byte[] Unpack(byte[] data)
        {
            return this.Unpack(data, 0, data.Length);
        }

        public byte[] Unpack(byte[] packed, int offset, int length)
        {
            byte[] buffer_decode = new byte[length];

            BWT bwt = new BWT();

            int primary_index = 0;
            bwt.bwt_decode(packed, buffer_decode, length, primary_index);

            List<byte> dest = new List<byte>();
            byte runLength;

            for (int i = 1 + offset; i < length; i += 2)
            {
                runLength = buffer_decode[i - 1];

                while (runLength > 0)
                {
                    dest.Add(buffer_decode[i]);
                    runLength--;
                }
            }
            return dest.ToArray();

            /*List<byte> dest = new List<byte>();
            byte runLength;

            for (int i = 1+offset; i < length; i += 2)
            {
                runLength = packed[i - 1];

                while (runLength > 0)
                {
                    dest.Add(packed[i]);
                    runLength--;
                }
            }
            return dest.ToArray();*/
        }
    }
}
