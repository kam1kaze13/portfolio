using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class LzwAlgoParams
    {
        public int MaxCodeBitCount { get; set; }

        public static LzwAlgoParams Default = new LzwAlgoParams
        {
            MaxCodeBitCount = 20
        };
    }
}
