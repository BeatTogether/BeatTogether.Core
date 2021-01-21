using System;
using System.Net;
using System.Text;
using Krypton.Buffers;

namespace BeatTogether.Extensions
{
    public static class SpanBufferWriterExtensions
    {
        public static void WriteVarULong(this ref SpanBufferWriter bufferWriter, ulong value)
        {
            do
            {
                var b = (byte)(value & 127UL);
                value >>= 7;
                if (value != 0UL)
                    b |= 128;
                bufferWriter.WriteUInt8(b);
            } while (value != 0UL);
        }

        public static void WriteVarLong(this ref SpanBufferWriter bufferWriter, long value)
            => bufferWriter.WriteVarULong((value < 0L ? (ulong)((-value << 1) - 1L) : (ulong)(value << 1)));

        public static void WriteVarUInt(this ref SpanBufferWriter buffer, uint value)
            => buffer.WriteVarULong(value);

        public static void WriteVarInt(this ref SpanBufferWriter bufferWriter, int value)
            => bufferWriter.WriteVarLong(value);

        public static void WriteVarBytes(this ref SpanBufferWriter bufferWriter, ReadOnlySpan<byte> bytes)
        {
            bufferWriter.WriteVarUInt((uint)bytes.Length);
            bufferWriter.WriteBytes(bytes);
        }

        public static void WriteString(this ref SpanBufferWriter bufferWriter, string value)
        {
            bufferWriter.WriteInt32(value.Length);
            bufferWriter.WriteBytes(Encoding.UTF8.GetBytes(value));
        }

        public static void WriteIPEndPoint(this ref SpanBufferWriter bufferWriter, IPEndPoint ipEndPoint)
        {
            bufferWriter.WriteString(ipEndPoint.Address.ToString());
            bufferWriter.WriteInt32(ipEndPoint.Port);
        }
    }
}
