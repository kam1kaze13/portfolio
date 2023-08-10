using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class LzwUnpacker : ICryptoUnpacker
    {
        public LzwUnpacker()
        {
            this.bitReader = new BitReader();
            this.table = new SequenceTable();
            this.table.Init();
            this.oldCode = 0;
        }

        public byte[] Unpack(byte[] data)
        {
            return this.Unpack(data, 0, data.Length);
        }

        public byte[] Unpack(byte[] packed, int offset, int length)
        { 
            byte[] data = new byte[length];

            Array.Copy(packed, offset, data, 0, length);

            bitReader.PutBytes(data);

            List<byte> decompressed = new List<byte>();

            ulong code;   

            while (bitReader.TryRead(table.CurrentBitLength, out code))
            {
                if (code == (ulong)SpecialSeqCodes.Clear)
                {
                    table.Init();
                    bitReader.TryRead(table.CurrentBitLength, out oldCode);
                    decompressed.AddRange(table.GetSequence(table.tableDecompress[oldCode]));
                    continue;
                }

                if (code == (ulong)SpecialSeqCodes.Eof)
                {
                    return decompressed.ToArray();
                }

                var newCode = new List<byte>();

                if (table.tableDecompress.ContainsKey(code))
                {
                    newCode.AddRange(table.GetSequence(table.tableDecompress[code]));
                }
                else if (code == (ulong)(table.tableDecompress.Count + 2))
                {
                    newCode.AddRange(table.GetSequence(table.tableDecompress[oldCode]));
                    newCode.Add(newCode[0]);
                }
                if (newCode.Count > 0)
                {
                    decompressed.AddRange(newCode);
                    table.tableDecompress.Add((ulong)(table.tableDecompress.Count + 2), table.AddSequence(oldCode, newCode[0]));
                    oldCode = code;
                }
            }

            return decompressed.ToArray();
        }

        private ulong oldCode;
        private BitReader bitReader;
        private SequenceTable table;
    }
}
