using System;
using System.Collections.Generic;
using System.Linq;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Extensions;
using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Implementations
{
    public class MessageWriter : IMessageWriter
    {
        protected virtual uint ProtocolVersion => 1;

        private readonly Dictionary<uint, IMessageRegistry> _messageRegistries;

        public MessageWriter(IEnumerable<IMessageRegistry> messageRegistries)
        {
            _messageRegistries = messageRegistries.ToDictionary(
                messageRegistry => messageRegistry.MessageGroup
            );
        }

        /// <inheritdoc cref="IMessageWriter.WriteTo"/>
        public void WriteTo<T>(ref GrowingSpanBuffer buffer, T message, byte? packetProperty)
            where T : class, IMessage
        {
            var messageGroup = 0U;
            var messageId = 0U;
            try
            {
                var messageType = message.GetType();
                messageGroup = _messageRegistries
                    .First(kvp => kvp.Value.TryGetMessageId(messageType, out messageId))
                    .Key;
            }
            catch (InvalidOperationException)
            {
                throw new Exception(
                    "Failed to retrieve identifier for message of type " +
                    $"'{typeof(T).Name}'."
                );
            }

            if (packetProperty.HasValue)
                buffer.WriteUInt8(packetProperty.Value);
            buffer.WriteUInt32(messageGroup);
            buffer.WriteVarUInt(ProtocolVersion);

            var messageBuffer = new GrowingSpanBuffer(stackalloc byte[412]);
            messageBuffer.WriteVarUInt(messageId);
            if (message is IReliableRequest reliableRequest)
                messageBuffer.WriteUInt32(reliableRequest.RequestId);
            if (message is IReliableResponse reliableResponse)
                messageBuffer.WriteUInt32(reliableResponse.ResponseId);
            message.WriteTo(ref messageBuffer);
            buffer.WriteVarUInt((uint)messageBuffer.Size);
            // TODO: Remove byte array allocation
            buffer.WriteBytes(messageBuffer.Data.ToArray());
        }
    }
}
