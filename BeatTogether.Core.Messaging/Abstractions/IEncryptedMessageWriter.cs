using System.Security.Cryptography;
using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IEncryptedMessageWriter
    {
        /// <summary>
        /// Writes an encrypted message to the given buffer.
        /// This will include message headers.
        /// </summary>
        /// <typeparam name="T">The type of message to serialize.</typeparam>
        /// <param name="bufferWriter">The buffer to write to.</param>
        /// <param name="message">The message to serialize.</param>
        /// <param name="key">The encryption key.</param>
        /// <param name="hmac">HMAC hasher.</param>
        /// <param name="packetProperty">The LiteNetLib PacketProperty to write.</param>
        void WriteTo<T>(ref SpanBufferWriter bufferWriter, T message, byte[] key, HMAC hmac, byte? packetProperty = null)
            where T : class, IEncryptedMessage;
    }
}
