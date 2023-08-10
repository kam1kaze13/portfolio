using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.Core
{
    public class CalcProcessor
    {
        public CalcResponse Process(CalcRequest request)
        {
            long result = 0;

            switch (request.OperationType)
            {
                case OperationType.Add:
                    {
                        result = (long)request.Argument1 + request.Argument2;
                        break;
                    }
                case OperationType.Sub:
                    {
                        result = (long)request.Argument1 - request.Argument2;
                        break;
                    }
                case OperationType.Mult:
                    {
                        result = (long)request.Argument1 * request.Argument2;
                        break;
                    }
            }

            return new CalcResponse { Result = result };
        }
    }
}
