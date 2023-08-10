using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class LzwAlgo : ICryptoAlgo
    {
        public byte[] Pack(byte[] data, LzwAlgoParams param = null)
        {
            var write = new BitWriter();
            var seqTable = new SequenceTable();

            Pack(data, 0, data.Length, seqTable, write, param);

            return write.GetAllBytes();
        }

        public void Pack(byte[] data, int offset, int length, ISequenceTable table, IBitWriter bitWriter, LzwAlgoParams parameters = null)
        {
            SequenceTable tableCompress = (SequenceTable)table;
            if (offset == 0)
            {
                tableCompress.Init();
                bitWriter.Writer((ulong)SpecialSeqCodes.Clear, tableCompress.CurrentBitLength);
            }

            ulong prevStr = (ulong)SpecialSeqCodes.Clear;
            ulong findSeq;

            for (int i=offset;i<length+offset;i++)
            {
                if (parameters != null)
                {
                    if (tableCompress.CurrentBitLength == parameters.MaxCodeBitCount+1)
                    {
                        bitWriter.Writer((ulong)SpecialSeqCodes.Clear, tableCompress.CurrentBitLength);
                        tableCompress.Init();
                    }
                }

                findSeq = tableCompress.FindSequence(prevStr, data[i]);

                if (findSeq != ulong.MaxValue)
                {
                    prevStr = findSeq;
                }
                else
                {
                    bitWriter.Writer(prevStr, table.CurrentBitLength);
                    tableCompress.tableCompress.Add((prevStr << 8) ^ data[i], (ulong)(tableCompress.tableCompress.Count + 2));
                    prevStr = data[i];
                }
            }
            if ((offset+length) == data.Length)
                bitWriter.Writer(prevStr, tableCompress.CurrentBitLength);
            table = tableCompress;
        }


        public byte[] Unpack(byte[] data)
        {
            var reader = new BitReader();
            var sequenceTable = new SequenceTable();

            return Unpack(data, 0, data.Length, sequenceTable, reader);
        }       

        public byte[] Unpack(byte[] packed, int offset, int length, ISequenceTable table, IBitReader bitReader)
        {
            SequenceTable tableDecompress = (SequenceTable)table;
            if (offset == 0)
            {
                tableDecompress.Init();
            }

            byte[] data = new byte[length];
            for (int i=offset,j=0;i<length+offset;i++,j++)
            {
                data[j] = packed[i];
            }

            bitReader.PutBytes(data);

            ulong oldCode = 0;

            List<byte> decompressed = new List<byte>();

            ulong code;
            while (bitReader.TryRead(tableDecompress.CurrentBitLength, out code))
            {
                if (code == (ulong)SpecialSeqCodes.Clear)
                {
                    tableDecompress.Init();
                    bitReader.TryRead(tableDecompress.CurrentBitLength, out oldCode);
                    decompressed.AddRange(tableDecompress.GetSequence(tableDecompress.tableDecompress[oldCode]));
                    continue;
                }

                if (code == (ulong)SpecialSeqCodes.Eof)
                {
                    break;
                }

                var newCode = new List<byte>();

                if (tableDecompress.tableDecompress.ContainsKey(code))
                {
                    newCode.AddRange(tableDecompress.GetSequence(tableDecompress.tableDecompress[code]));
                }
                else if (code == (ulong)(tableDecompress.tableDecompress.Count + 2))
                {
                    newCode.AddRange(tableDecompress.GetSequence(tableDecompress.tableDecompress[oldCode]));
                    newCode.Add(newCode[0]);
                }
                if (newCode.Count > 0)
                {
                    decompressed.AddRange(newCode);
                    tableDecompress.tableDecompress.Add((ulong)(tableDecompress.tableDecompress.Count + 2), tableDecompress.AddSequence(oldCode, newCode[0]));
                    oldCode = code;
                }
            }
            table = tableDecompress;

            return decompressed.ToArray();
        }
    }
}
