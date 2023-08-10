using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public abstract class WholeDataPacker : ICryptoPacker
    {
        public byte[] GetLastSegment()
        {
            return this.Pack(this.dataPack.ToArray());
        }

        abstract public byte[] Pack(byte[] data);
        

        public byte[] Pack(byte[] data, int offset, int length)
        {
            this.dataPack.Write(data, offset, length);
            return new byte[0];
        }

        private MemoryStream dataPack = new MemoryStream();
    }
}
