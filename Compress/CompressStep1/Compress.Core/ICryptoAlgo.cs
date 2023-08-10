using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    interface ICryptoAlgo
    {
        byte[] Pack(byte[] data);

        byte[] Unpack(byte[] data);
    }
}
