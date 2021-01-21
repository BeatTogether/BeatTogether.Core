using System;
using System.Security.Cryptography;
using BeatTogether.Core.Messaging.Abstractions;
using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Implementations
{
    public class EncryptedMessageWriter : IEncryptedMessageWriter
    {
        private readonly RNGCryptoServiceProvider _rngCryptoServiceProvider;
        private readonly AesCryptoServiceProvider _aesCryptoServiceProvider;
        private readonly IMessageWriter _messageWriter;

        public EncryptedMessageWriter(
            RNGCryptoServiceProvider rngCryptoServiceProvider,
            AesCryptoServiceProvider aesCryptoServiceProvider,
            IMessageWriter messageWriter)
        {
            _rngCryptoServiceProvider = rngCryptoServiceProvider;
            _aesCryptoServiceProvider = aesCryptoServiceProvider;
            _messageWriter = messageWriter;
        }

        /// <inheritdoc cref="IEncryptedMessageWriter.WriteTo"/>
        public void WriteTo<T>(ref SpanBufferWriter bufferWriter, T message, byte[] key, HMAC hmac, byte? packetProperty)
            where T : class, IEncryptedMessage
        {
            var unencryptedBufferWriter = new SpanBufferWriter(stackalloc byte[412]);
            _messageWriter.WriteTo(ref unencryptedBufferWriter, message, packetProperty);

            var hashBufferWriter = new SpanBufferWriter(stackalloc byte[unencryptedBufferWriter.Size + 4]);
            hashBufferWriter.WriteBytes(unencryptedBufferWriter.Data);
            hashBufferWriter.WriteUInt32(message.SequenceId);
            Span<byte> hash = stackalloc byte[32];
            if (!hmac.TryComputeHash(hashBufferWriter.Data, hash, out _))
                throw new Exception("Failed to compute message hash.");
            unencryptedBufferWriter.WriteBytes(hash.Slice(0, 10));

            var iv = new byte[16];
            _rngCryptoServiceProvider.GetBytes(iv);

            var paddingByteCount = (byte)((16 - ((unencryptedBufferWriter.Size + 1) & 15)) & 15);
            for (var i = 0; i < paddingByteCount + 1; i++)
                unencryptedBufferWriter.WriteUInt8(paddingByteCount);

            var encryptedBuffer = unencryptedBufferWriter.Data.ToArray();
            using (var cryptoTransform = _aesCryptoServiceProvider.CreateEncryptor(key, iv))
            {
                var bytesWritten = 0;
                for (var i = encryptedBuffer.Length; i >= cryptoTransform.InputBlockSize; i -= bytesWritten)
                {
                    var inputCount = cryptoTransform.CanTransformMultipleBlocks
                        ? (i / cryptoTransform.InputBlockSize * cryptoTransform.InputBlockSize)
                        : cryptoTransform.InputBlockSize;
                    bytesWritten = cryptoTransform.TransformBlock(
                        encryptedBuffer, bytesWritten, inputCount,
                        encryptedBuffer, bytesWritten
                    );
                }
            }

            bufferWriter.WriteUInt32(message.SequenceId);
            bufferWriter.WriteBytes(iv);
            bufferWriter.WriteBytes(encryptedBuffer);
        }
    }
}
