using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class MyPacker : WholeDataPacker
    {
        public override byte[] Pack(byte[] data)
        {
            byte[] buffer_out = new byte[data.Length];

            BWT bwt = new BWT();

            int primary_index = 0;
            bwt.bwt_encode(data, buffer_out, data.Length, ref primary_index);

            List<byte> dest = new List<byte>();
            byte runLength;

            for (int i = 0; i < buffer_out.Length; i++)
            {
                runLength = 1;
                while (runLength < byte.MaxValue
                    && i + 1 < buffer_out.Length
                    && buffer_out[i] == buffer_out[i + 1])
                {
                    runLength++;
                    i++;
                }
                dest.Add(runLength);
                dest.Add(buffer_out[i]);
            }

            return dest.ToArray();

            /*List<byte> dest = new List<byte>();
            byte runLength;

            for (int i = 0; i < data.Length; i++)
            {
                runLength = 1;
                while (runLength < byte.MaxValue
                    && i + 1 < data.Length
                    && data[i] == data[i + 1])
                {
                    runLength++;
                    i++;
                }
                dest.Add(runLength);
                dest.Add(data[i]);
            }

            return dest.ToArray();*/
        }
    }
}
