using SocketClientServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.Core
{
    public class CalcProtocolExecutor : IProtocolExecutor<CalcRequest, CalcResponse>
    {
        public ParsingResult<CalcRequest> ParseInput(byte[] data,int offset, int length)
        {
            var result = new ParsingResult<CalcRequest>();
            int start = 0;
            for (int i = 0; i < length; i++)
            {
                var element = Encoding.ASCII.GetString(data, i, 1);

                if (element == ";")
                {
                    if (Encoding.ASCII.GetString(data, start, 1) == ";")
                    {
                        result.IsClosed = true;
                        break;
                    }

                    var arg1 = BitConverter.ToInt32(data, start);
                    var opType = OperationExtension.ParseProtocolString(Encoding.ASCII.GetString(data, start + 4, 1));
                    var arg2 = BitConverter.ToInt32(data, start + 5);

                    result.ParsedObjects.Add(new CalcRequest { Argument1 = arg1, Argument2 = arg2, OperationType = opType });

                    start = i + 1;
                }

            }

            result.ProcessedBytes = start;

            return result;
        }

        public ParsingResult<CalcResponse> ParseOutput(byte[] data, int offset, int length)
        {
            var result = new ParsingResult<CalcResponse>();
            int start = 0;
            for (int i = 0; i < data.Length; i++)
            {
                var element = Encoding.ASCII.GetString(data, i, 1);

                if (element == ";")
                {
                    if (Encoding.ASCII.GetString(data, start, 1) == ";")
                    {
                        result.IsClosed = true;
                        break;
                    }

                    var outputResult = BitConverter.ToInt64(data, start);

                    result.ParsedObjects.Add(new CalcResponse { Result = outputResult });

                    start = i + 1;
                }

            }

            result.ProcessedBytes = start;

            return result;
        }

        public byte[] CreateRequest(CalcRequest request)
        {
            // TODO: реализовать
            var arg1 = BitConverter.GetBytes(request.Argument1);
            var operation = Encoding.ASCII.GetBytes(OperationExtension.ToProtocolString(request.OperationType));
            var arg2 = BitConverter.GetBytes(request.Argument2);
            var end = Encoding.ASCII.GetBytes(";");


            return arg1.Concat(operation).Concat(arg2).Concat(end).ToArray();
        }

        public byte[] CreateResponse(CalcResponse response)
        {
            var res = BitConverter.GetBytes(response.Result);
            var end = Encoding.ASCII.GetBytes(";");

            return res.Concat(end).ToArray();
        }
    }
}
