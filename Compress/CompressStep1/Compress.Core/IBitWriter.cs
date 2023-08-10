using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    interface IBitWriter
    {
        void Writer(ulong code, int bitCount);

        byte[] GetBytes();
    }
}
