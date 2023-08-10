using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class PartialStream : Stream
    {
        private readonly Stream stream;
        private int start;
        private int length;

        public PartialStream(Stream stream, int start, int length)
        {
            this.stream = stream;
            this.start = start;
            this.length = Math.Min(length,(int)stream.Length-start);
            this.stream.Seek(start, SeekOrigin.Begin);
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => this.length;

        public override long Position { get => this.stream.Position; set => this.stream.Position = this.start + value; }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            
            if (this.Position < this.start+this.length)
            {
                bytesRead = this.stream.Read(buffer, offset, Math.Min(count,this.length));
                this.Position = bytesRead;
            }
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch(origin)
            {
                case SeekOrigin.Begin:
                    {
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        break;
                    }
                case SeekOrigin.End:
                    {
                        break;
                    }
            }
            return this.stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
