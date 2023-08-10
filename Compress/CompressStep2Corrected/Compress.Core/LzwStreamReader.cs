using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compress.Core
{
    public class LzwStreamReader : Stream
    {
        public LzwStreamReader(Stream inner)
        {
            this.inner = inner;
            this.unpacker = new LzwUnpacker();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int retValue;
            if (cache.Length !=0)
            {
                retValue = cache.Length;
                Array.Copy(cache, buffer, retValue);
                cache = new byte[0];
            }
            else
            {
                retValue = this.inner.Read(buffer, offset, count);

                var unpacked = unpacker.Unpack(buffer, 0, retValue);

                if (retValue >= unpacked.Length)
                {
                    Array.Copy(unpacked, buffer, unpacked.Length);               
                }
                else
                {
                    Array.Copy(unpacked, buffer, retValue);

                    cache = new byte[unpacked.Length - retValue];
                    Array.Copy(unpacked, retValue, cache, 0, unpacked.Length - retValue);
                }
            }

            return retValue;
        }

        public override void Flush()
        {
            this.inner.Flush();
        }

        public override void Close()
        {
            this.inner.Close();
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            inner.Dispose();
            base.Dispose(disposing);
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        private Stream inner;
        private byte[] cache = new byte[0];
        private ICryptoUnpacker unpacker;
    }
}
