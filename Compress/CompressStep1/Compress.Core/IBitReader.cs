using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    interface IBitReader
    {
        void PutBytes(byte[] data);
        
        bool TryRead(int bitCount, out ulong code);
    }
}
