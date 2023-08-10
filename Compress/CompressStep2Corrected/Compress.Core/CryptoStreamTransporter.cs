﻿using System;
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
            byte[] buffer = new byte[32768];
            int read;
            while (length > 0)
            {
                read = input.Read(buffer, 0, buffer.Length);

                var packed = packer.Pack(buffer, 0, read);

                output.Write(packed, 0, packed.Length);

                length -= read;
            }

            var lastPacked = packer.GetLastSegment();
            if (lastPacked.Length > 0)
                output.Write(lastPacked, 0, lastPacked.Length);
        }

        public void Unpack(Stream input, Stream output, long length, ICryptoUnpacker unpacker)
        {
            byte[] buffer = new byte[32768];
            int read;
            while (length > 0)
            {
                read = input.Read(buffer, 0, buffer.Length);

                var unpacked = unpacker.Unpack(buffer, 0, read);

                output.Write(unpacked, 0, unpacked.Length);

                length -= read;
            }   
        }
    }
}
