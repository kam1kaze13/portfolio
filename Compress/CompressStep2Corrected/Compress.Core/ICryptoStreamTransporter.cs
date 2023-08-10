using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{

    public interface ICryptoStreamTransporter
    {
        void Pack(Stream input, Stream output, long length, ICryptoPacker packer);

        void Unpack(Stream input, Stream output, long length, ICryptoUnpacker unpacker);

        event EventHandler<DataProcessedEventArgs> DataProcessed;
    }
}
