using System.Security.Cryptography;
using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IEncryptedMessageReader
    {
        /// <summary>
        /// Reads an encrypted message from the given buffer.
        /// It must include message headers.
        /// </summary>
        /// <param name="bufferReader">The buffer to read from.</param>
        /// <param name="key">The decryption key.</param>
        /// <param name="hmac">HMAC hasher.</param>
        /// <returns>The deserialized message.</returns>
        IEncryptedMessage ReadFrom(ref SpanBufferReader bufferReader, byte[] key, HMAC hmac);
    }
}
