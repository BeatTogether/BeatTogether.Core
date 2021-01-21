using System;
using System.Collections.Generic;
using System.Linq;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Extensions;
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
        public void WriteTo(ref SpanBufferWriter bufferWriter, IMessage message, byte? packetProperty)
        {
            var messageGroup = 0U;
            var messageId = 0U;
            var messageType = message.GetType();
            try
            {
                messageGroup = _messageRegistries
                    .First(kvp => kvp.Value.TryGetMessageId(messageType, out messageId))
                    .Key;
            }
            catch (InvalidOperationException)
            {
                throw new Exception(
                    "Failed to retrieve identifier for message of type " +
                    $"'{messageType.Name}'."
                );
            }

            if (packetProperty.HasValue)
                bufferWriter.WriteUInt8(packetProperty.Value);
            bufferWriter.WriteUInt32(messageGroup);
            bufferWriter.WriteVarUInt(ProtocolVersion);

            var messageBufferWriter = new SpanBufferWriter(stackalloc byte[412]);
            messageBufferWriter.WriteVarUInt(messageId);
            if (message is IRequest request)
                messageBufferWriter.WriteUInt32(request.RequestId);
            if (message is IResponse response)
                messageBufferWriter.WriteUInt32(response.ResponseId);
            message.WriteTo(ref messageBufferWriter);
            bufferWriter.WriteVarUInt((uint)messageBufferWriter.Size);
            // TODO: Remove byte array allocation
            bufferWriter.WriteBytes(messageBufferWriter.Data.ToArray());
        }
    }
}
