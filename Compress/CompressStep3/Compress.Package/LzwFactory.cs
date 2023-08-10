using Compress.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class LzwFactory : ICryptoFactory
    {
        public LzwFactory(int MaxCodeBitCount = 20)
        {
            this.MaxCodeBitCount = MaxCodeBitCount;
        }

        public ICryptoPacker CreatePacker()
        {
            return new LzwPacker(new LzwAlgoParams() { MaxCodeBitCount = this.MaxCodeBitCount, });
        }

        public ICryptoUnpacker CreateUnpacker()
        {
            return new LzwUnpacker();
        }

        private int MaxCodeBitCount;
    }
}
