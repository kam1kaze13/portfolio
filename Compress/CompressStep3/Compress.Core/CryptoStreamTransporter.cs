using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class CryptoStreamTransporter : ICryptoStreamTransporter
    {
        public event EventHandler<DataProcessedEventArgs> DataProcessed;

        public void Pack(Stream input, Stream output, long length, ICryptoPacker packer)
        {
            int n = (int)Math.Min(32768, length);

            byte[] buffer = new byte[n];

            int read;

            var e = new DataProcessedEventArgs
            {
                Total = length,
                TotalProcessed = 0
            };

            while (length > 0)
            {
                read = input.Read(buffer, 0, (int)Math.Min(buffer.Length, length));

                var packed = packer.Pack(buffer, 0, read);

                output.Write(packed, 0, packed.Length);

                length -= read;

                e.TotalProcessed += read;

                this.DataProcessed?.Invoke(this, e);
            }

            var lastPacked = packer.GetLastSegment();
            if (lastPacked.Length > 0)
                output.Write(lastPacked, 0, lastPacked.Length);
                
        }

        public void Unpack(Stream input, Stream output, long length, ICryptoUnpacker unpacker)
        {
            int n = (int)Math.Min(32768, length);

            byte[] buffer = new byte[n];

            int read;

            var e = new DataProcessedEventArgs();
            e.Total = length;
            e.TotalProcessed = 0;

            while (length > 0)
            {
                read = input.Read(buffer, 0, (int)Math.Min(buffer.Length, length));

                var unpacked = unpacker.Unpack(buffer, 0, read);

                output.Write(unpacked, 0, unpacked.Length);

                length -= read;

                e.TotalProcessed += read;

                this.DataProcessed?.Invoke(this, e);
            }   
        }
    }
}
