using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IMessage
    {
        void WriteTo(ref SpanBufferWriter bufferWriter);
        void ReadFrom(ref SpanBufferReader bufferReader);
    }
}
