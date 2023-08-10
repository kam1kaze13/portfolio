using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.Core
{
    public class CalcRequest
    {
        public int Argument1 { get; set; }

        public int Argument2 { get; set; }

        public OperationType OperationType { get; set; }
    }
}
