using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compress.Core
{
    public class LzwAlgo : ICryptoAlgo
    {
        public byte[] Pack(byte[] data)
        {
            var write = new BitWriter();
            var seqTable = new SequenceTable();
            seqTable.Init();
            var bitCount = LzwAlgoParams.Default;

            write.Writer((ulong)SpecialSeqCodes.Clear, seqTable.CurrentBitLength);

            ulong prevStr = (ulong)SpecialSeqCodes.Clear;
            ulong findSeq;

            foreach (byte elem in data)
            {
                if (seqTable.CurrentBitLength == bitCount.MaxCodeBitCount)
                {
                    write.Writer((ulong)SpecialSeqCodes.Clear, seqTable.CurrentBitLength);
                    seqTable.Init();
                }
                    
                findSeq = seqTable.FindSequence(prevStr, elem);

                if (findSeq != ulong.MaxValue)
                {
                    prevStr = findSeq;
                }
                else
                {
                    write.Writer(prevStr, seqTable.CurrentBitLength);
                    seqTable.tableCompress.Add((prevStr << 8) ^ elem, (ulong)(seqTable.tableCompress.Count + 2));
                    prevStr = elem;
                }
            }
            write.Writer(prevStr, seqTable.CurrentBitLength);

            return write.GetBytes();
        }

        public byte[] Unpack(byte[] data)
        {
            var reader = new BitReader();
            var sequenceTable = new SequenceTable();

            sequenceTable.Init();

            reader.PutBytes(data);
            ulong oldCode = 0;

            List<byte> decompressed = new List<byte>();        

            ulong code;
            while (reader.TryRead(sequenceTable.CurrentBitLength, out code))
            {
                if (code == (ulong)SpecialSeqCodes.Clear)
                {
                    sequenceTable.Init();
                    reader.TryRead(sequenceTable.CurrentBitLength, out oldCode);
                    decompressed.AddRange(sequenceTable.GetSequence(sequenceTable.tableDecompress[oldCode]));
                    continue;
                }

                if (code == (ulong)SpecialSeqCodes.Eof)
                {
                    break;
                }

                var newCode = new List<byte>();

                if (sequenceTable.tableDecompress.ContainsKey(code))
                {
                    newCode.AddRange(sequenceTable.GetSequence(sequenceTable.tableDecompress[code]));
                }
                else if (code == (ulong)(sequenceTable.tableDecompress.Count + 2))
                {
                    newCode.AddRange(sequenceTable.GetSequence(sequenceTable.tableDecompress[oldCode]));
                    newCode.Add(newCode[0]);
                }
                if (newCode.Count > 0)
                {
                    decompressed.AddRange(newCode);
                    sequenceTable.tableDecompress.Add((ulong)(sequenceTable.tableDecompress.Count + 2), sequenceTable.AddSequence(oldCode, newCode[0]));
                    oldCode = code;
                }
            }

            return decompressed.ToArray();
        }
    }
}
