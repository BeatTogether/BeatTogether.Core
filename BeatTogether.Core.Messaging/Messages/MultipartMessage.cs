using System.Runtime.Serialization;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Extensions;
using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Messages
{
    public class MultipartMessage : IEncryptedMessage, IReliableRequest
    {
        public uint SequenceId { get; set; }
        public uint RequestId { get; set; }
        public uint MultipartMessageId { get; set; }
        public uint Offset { get; set; }
        public uint Length { get; set; }
        public uint TotalLength { get; set; }
        public byte[] Data { get; set; } = null!;

        private const uint _maximumLength = 384;
        private const uint _maximumTotalLength = 0x7FFF;

        public void WriteTo(ref SpanBufferWriter bufferWriter)
        {
            bufferWriter.WriteUInt32(MultipartMessageId);
            bufferWriter.WriteVarUInt(Offset);
            bufferWriter.WriteVarUInt(Length);
            bufferWriter.WriteVarUInt(TotalLength);
            bufferWriter.WriteBytes(Data);
        }

        public void ReadFrom(ref SpanBufferReader bufferReader)
        {
            MultipartMessageId = bufferReader.ReadUInt32();
            Offset = bufferReader.ReadVarUInt();
            Length = bufferReader.ReadVarUInt();
            TotalLength = bufferReader.ReadVarUInt();
            if (Length > _maximumLength)
                throw new InvalidDataContractException($"Length must not surpass {_maximumLength} bytes.");
            if (TotalLength > _maximumTotalLength)
                throw new InvalidDataContractException($"Length must not surpass {_maximumTotalLength} bytes.");
            Data = bufferReader.ReadBytes((int)Length).ToArray();
        }
    }
}
