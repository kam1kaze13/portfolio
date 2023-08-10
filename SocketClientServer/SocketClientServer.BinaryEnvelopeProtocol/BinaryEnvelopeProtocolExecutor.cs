using SocketClientServer.Core;
using System;
using System.IO;

namespace SocketClientServer.BinaryEnvelopeProtocol
{
    public record Envelope(byte[] Data);

    public class BinaryEnvelopeProtocolExecutor : IProtocolExecutor<Envelope, Envelope>
    {
        public byte[] CreateRequest(Envelope request)
        {
            return this.SerializeEnvelope(request);
        }

        public byte[] CreateResponse(Envelope response)
        {
            return this.SerializeEnvelope(response);
        }

        private byte[] SerializeEnvelope(Envelope envelope)
        {
            using (var memStream = new MemoryStream())
            {
                memStream.Write(BitConverter.GetBytes(envelope.Data.Length));
                memStream.Write(envelope.Data);
                return memStream.ToArray();
            }
        }

        public ParsingResult<Envelope> ParseInput(byte[] data, int offset, int length)
        {
            return this.DeserializeEnvelope(data, offset, length);
        }

        public ParsingResult<Envelope> ParseOutput(byte[] data, int offset, int length)
        {
            return this.DeserializeEnvelope(data, offset, length);
        }

        private ParsingResult<Envelope> DeserializeEnvelope(byte[] data, int offset, int length)
        {
            var res = new ParsingResult<Envelope>();
            using (var memStream = new MemoryStream(data))
            {
                while (length >= sizeof(int))
                {
                    var bytes = new byte[sizeof(int)];
                    memStream.Read(bytes, 0, bytes.Length);
                    var envLength = BitConverter.ToInt32(bytes, 0);
                    if (envLength == 0)
                    {
                        res.IsClosed = true;
                        break;
                    }
                    if (length - sizeof(int) >= envLength)
                    {
                        var envelopeBinary = new byte[envLength];
                        memStream.Read(envelopeBinary, 0, envLength);
                        res.ParsedObjects.Add(new Envelope(envelopeBinary));

                        offset += sizeof(int) + envLength;
                        length -= sizeof(int) + envLength;
                    }
                }
            }

            return res;
        }
    }
}
