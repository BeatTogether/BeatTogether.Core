using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Extensions;
using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Implementations
{
    public class MessageReader : IMessageReader
    {
        protected virtual uint MinimumProtocolVersion => 1;
        protected virtual uint MaximumProtocolVersion => 3;

        private readonly Dictionary<uint, IMessageRegistry> _messageRegistries;

        public MessageReader(IEnumerable<IMessageRegistry> messageRegistries)
        {
            _messageRegistries = messageRegistries.ToDictionary(
                messageRegistry => messageRegistry.MessageGroup
            );
        }

        /// <inheritdoc cref="IMessageReader.ReadFrom"/>
        public IMessage ReadFrom(ref SpanBufferReader bufferReader, byte? packetProperty)
        {
            if (packetProperty.HasValue)
            {
                var readPacketProperty = bufferReader.ReadUInt8();
                if (readPacketProperty != packetProperty.Value)
                    throw new InvalidDataContractException(
                        "Invalid packet property " +
                        $"(PacketProperty={readPacketProperty}, Expected={packetProperty.Value})."
                    );
            }
            var messageGroup = bufferReader.ReadUInt32();
            if (!_messageRegistries.TryGetValue(messageGroup, out var messageRegistry))
                throw new InvalidDataContractException($"Invalid message group (MessageGroup={messageGroup}).");
            var protocolVersion = bufferReader.ReadVarUInt();
            if (protocolVersion < MinimumProtocolVersion || protocolVersion > MaximumProtocolVersion)
                throw new InvalidDataContractException($"Invalid message protocol version (ProtocolVersion={protocolVersion}).");
            var length = bufferReader.ReadVarUInt();
            if (bufferReader.RemainingSize < length)
                throw new InvalidDataContractException($"Message truncated (RemainingSize={bufferReader.RemainingSize}, Expected={length}).");
            var messageId = bufferReader.ReadVarUInt();
            if (!messageRegistry.TryCreateMessage(messageId, out var message))
                throw new InvalidDataContractException($"Invalid message identifier (MessageId={messageId}).");
            if (message is IRequest request)
                request.RequestId = bufferReader.ReadUInt32();
            if (message is IResponse response)
                response.ResponseId = bufferReader.ReadUInt32();
            message.ReadFrom(ref bufferReader);
            return message;
        }
    }
}
