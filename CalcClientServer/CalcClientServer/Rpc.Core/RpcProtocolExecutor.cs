using Calc.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpc.Core
{
    public class RpcProtocolExecutor : IProtocolExecutor<RpcRequest, RpcResponse>
    {
        public byte[] CreateRequest(RpcRequest request)
        {
            var binaryRequest = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));

            var length = BitConverter.GetBytes(binaryRequest.Length);

            return length.Concat(binaryRequest).ToArray();
        }

        public byte[] CreateResponse(RpcResponse response)
        {
            var binaryResponse = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));

            var length = BitConverter.GetBytes(binaryResponse.Length);

            return length.Concat(binaryResponse).ToArray();
        }

        public ParsingResult<RpcRequest> ParseInput(byte[] data, int length)
        {
            var result = new ParsingResult<RpcRequest>();
            int start = 0;
            for (int i = 0; i < length; i++)
            {
                if (Encoding.ASCII.GetString(data, start, 1) == ";")
                {
                    result.IsClosed = true;
                    break;
                }

                var lengthArray = BitConverter.ToInt32(data, start);

                if (lengthArray > 0)
                {
                    start += 4;

                    var request = JsonConvert.DeserializeObject<RpcRequest>(Encoding.UTF8.GetString(data, start, lengthArray));

                    result.ParsedObjects.Add(request);

                    start += lengthArray;

                    i = start;
                }                
            }

            result.ProcessedBytes = start;

            return result;
        }

        public ParsingResult<RpcResponse> ParseOutput(byte[] data, int length)
        {
            var result = new ParsingResult<RpcResponse>();
            int start = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (Encoding.ASCII.GetString(data, start, 1) == ";")
                {
                    result.IsClosed = true;
                    break;
                }

                var lengthArray = BitConverter.ToInt32(data, start);

                if (lengthArray > 0)
                {
                    start += 4;

                    var response = JsonConvert.DeserializeObject<RpcResponse>(Encoding.UTF8.GetString(data, start, lengthArray));

                    result.ParsedObjects.Add(response);

                    start += lengthArray;

                    i = start;
                }                
            }

            result.ProcessedBytes = start;

            return result;
        }
    }
}
