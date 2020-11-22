using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IMessage
    {
        void WriteTo(ref GrowingSpanBuffer buffer);
        void ReadFrom(ref SpanBufferReader bufferReader);
    }
}
