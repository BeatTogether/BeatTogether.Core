using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IMessageReader
    {
        /// <summary>
        /// Reads a message from the given buffer.
        /// It must include message headers.
        /// </summary>
        /// <param name="bufferReader">The buffer to read from.</param>
        /// <param name="packetProperty">The LiteNetLib PacketProperty to compare against.</param>
        /// <returns>The deserialized message.</returns>
        IMessage ReadFrom(ref SpanBufferReader bufferReader, byte? packetProperty = null);
    }
}
