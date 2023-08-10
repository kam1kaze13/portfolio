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
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var lzw = new LzwAlgo();

            var unpacked = lzw.Unpack(buffer, offset, count, sequenceTable, bitReader);

            int retValue = inner.Read(unpacked, 0, unpacked.Length);

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
        private BitReader bitReader = new BitReader();
        private SequenceTable sequenceTable = new SequenceTable();
    }
}
