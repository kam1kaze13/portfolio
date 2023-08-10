using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientServer.Core
{
    public interface IProtocolExecutor<TRequest, TResponse>
    {
        byte[] CreateResponse(TResponse response);

        byte[] CreateRequest(TRequest request);

        ParsingResult<TRequest> ParseInput(byte[] data, int offset, int length);

        ParsingResult<TResponse> ParseOutput(byte[] data, int offset, int length);
    }
}
