using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IVersionedMessage
    {
        void WriteTo(ref SpanBufferWriter bufferWriter, uint protocolVersion);
        void ReadFrom(ref SpanBufferReader bufferReader, uint protocolVersion);
    }
}