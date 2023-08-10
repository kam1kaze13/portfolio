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
            if (cache.Length >= count)
            {
                Array.Copy(cache, buffer, count);
                var newCache = new byte[cache.Length - count];
                Array.Copy(cache, count, newCache, 0, cache.Length - count);
                cache = new byte[newCache.Length];
                Array.Copy(newCache, cache, newCache.Length);

                return count;
            }

            var readCount = this.inner.Read(buffer, offset, count);

            if (readCount > 0)
            {
                var unpacked = unpacker.Unpack(buffer, 0, readCount);
                Array.Clear(buffer, 0, buffer.Length);
                Array.Copy(cache, buffer, cache.Length);

                var bytesCount = buffer.Length - cache.Length;
                if (bytesCount > unpacked.Length)
                {
                    Array.Copy(unpacked, 0, buffer, cache.Length, unpacked.Length);
                    count = cache.Length + unpacked.Length;
                    cache = new byte[0];
                }
                else
                {
                    Array.Copy(unpacked, 0, buffer, cache.Length, bytesCount);
                    var newCache = new byte[unpacked.Length - bytesCount];
                    Array.Copy(unpacked, bytesCount, newCache, 0, unpacked.Length - bytesCount);
                    cache = new byte[newCache.Length];
                    Array.Copy(newCache, cache, newCache.Length);
                }

                return count;
            }
            else
            {
                Array.Copy(cache, buffer, cache.Length);
                cache = new byte[0];
                return cache.Length;
            }            
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
            get { return true; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.length = value;
        }

        public override long Length
        {
            get
            {
                return this.length;
            }
        }

        public override long Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        private Stream inner;
        private byte[] cache = new byte[0];
        private long position;
        private long length;
        private ICryptoUnpacker unpacker;
    }
}
