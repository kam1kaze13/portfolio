using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class LzwPacker : ICryptoPacker
    {
        public LzwPacker()
        {
            this.bitWriter = new BitWriter();
            this.table = new SequenceTable();
            this.currentSeq = (ulong)SpecialSeqCodes.Clear;
            this.table.Init();
            this.bitWriter.Writer((ulong)SpecialSeqCodes.Clear, table.CurrentBitLength);
        }

        public LzwPacker(LzwAlgoParams parameters)
            : this()
        {
            this.parameters = parameters;
        }

        public byte[] Pack(byte[] data)
        {
            var firstSegment = this.Pack(data, 0, data.Length);
            var lastSegment = this.GetLastSegment();

            if (lastSegment.Length == 0)
                return firstSegment;
            else
                return firstSegment.Concat(lastSegment).ToArray();
        }

        public byte[] Pack(byte[] data, int offset, int length)
        {
            ulong prevStr = currentSeq;
            ulong findSeq;

            for (int i = offset; i < length + offset; i++)
            {
                if (table.CurrentBitLength == parameters.MaxCodeBitCount + 1)
                {
                    bitWriter.Writer((ulong)SpecialSeqCodes.Clear, table.CurrentBitLength);
                    table.Init();
                }

                findSeq = table.FindSequence(prevStr, data[i]);

                if (findSeq != ulong.MaxValue)
                {
                    prevStr = findSeq;
                }
                else
                {
                    bitWriter.Writer(prevStr, table.CurrentBitLength);
                    table.tableCompress.Add((prevStr << 8) ^ data[i], (ulong)(table.tableCompress.Count + 2));
                    prevStr = data[i];
                }
            }
            currentSeq = prevStr;

            return bitWriter.GetRoundBytes();
        }

        public byte[] GetLastSegment()
        {
            bitWriter.Writer(currentSeq, table.CurrentBitLength);
            return bitWriter.GetAllBytes();
        }

        private LzwAlgoParams parameters = LzwAlgoParams.Default;
        private ulong currentSeq;
        private BitWriter bitWriter;
        private SequenceTable table;
    }
}
