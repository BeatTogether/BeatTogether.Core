using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IMessageWriter
    {
        /// <summary>
        /// Writes a message to the given buffer.
        /// This will include message headers.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="message">The message to serialize.</param>
        /// <param name="packetProperty">The LiteNetLib PacketProperty to write.</param>
        void WriteTo(ref GrowingSpanBuffer buffer, IMessage message, byte? packetProperty = null);
    }
}
