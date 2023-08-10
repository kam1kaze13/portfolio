using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    interface ICryptoAlgo
    {
        byte[] Pack(byte[] data, LzwAlgoParams parameters = null);

        void Pack(byte[] data, int offset, int length, ISequenceTable table, IBitWriter bitWriter, LzwAlgoParams parameters = null);

        byte[] Unpack(byte[] data);

        byte[] Unpack(byte[] packed, int offset, int length, ISequenceTable table, IBitReader bitReader);
    }
}
