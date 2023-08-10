using Calc.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calc.Protocol
{
     public interface IProtocolExecutor<TRequest, TResponse>
    {
        byte[] CreateResponse(TResponse response);

        byte[] CreateRequest(TRequest request);

        ParsingResult<TRequest> ParseInput(byte[] data, int length);

        ParsingResult<TResponse> ParseOutput(byte[] data, int length);
    }
}
