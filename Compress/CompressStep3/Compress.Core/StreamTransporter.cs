using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class StreamTransporter
    {
        public event EventHandler<DataProcessedEventArgs> DataProcessed;

        public void Copy(Stream input, Stream output, long length)
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

                output.Write(buffer, 0, read);

                length -= read;

                e.TotalProcessed += read;

                this.DataProcessed?.Invoke(this, e);
            }
        }
    }
}
