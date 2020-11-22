using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IMessageWriter
    {
        /// <summary>
        /// Writes a message to the given buffer.
        /// This will include message headers.
        /// </summary>
        /// <typeparam name="T">The type of message to serialize.</typeparam>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="message">The message to serialize.</param>
        /// <param name="packetProperty">The LiteNetLib PacketProperty to compare against.</param>
        void WriteTo<T>(ref GrowingSpanBuffer buffer, T message, byte? packetProperty = null)
            where T : class, IMessage;
    }
}
