using System;
using System.Net;
using System.Text;
using Krypton.Buffers;

namespace BeatTogether.Extensions
{
    public static class SpanBufferReaderExtensions
    {
        public static ulong ReadVarULong(this ref SpanBufferReader bufferReader)
        {
            var value = 0UL;
            var shift = 0;
            var b = bufferReader.ReadByte();
            while ((b & 128UL) != 0UL)
            {
                value |= (b & 127UL) << shift;
                shift += 7;
                b = bufferReader.ReadByte();
            }
            return value | (ulong)b << shift;
        }

        public static long ReadVarLong(this ref SpanBufferReader bufferReader)
        {
            var varULong = (long)bufferReader.ReadVarULong();
            if ((varULong & 1L) != 1L)
                return varULong >> 1;
            return -(varULong >> 1) + 1L;
        }

        public static uint ReadVarUInt(this ref SpanBufferReader bufferReader)
            => (uint)bufferReader.ReadVarULong();

        public static int ReadVarInt(this ref SpanBufferReader bufferReader)
            => (int)bufferReader.ReadVarLong();

        public static bool TryReadVarULong(this ref SpanBufferReader bufferReader, out ulong value)
        {
            value = 0UL;
            var shift = 0;
            while (shift <= 63 && bufferReader.RemainingSize >= 1)
            {
                var b = bufferReader.ReadByte();
                value |= (ulong)(b & 127) << shift;
                shift += 7;
                if ((b & 128) == 0)
                    return true;
            }

            value = 0UL;
            return false;
        }

        public static bool TryReadVarUInt(this ref SpanBufferReader bufferReader, out uint value)
        {
            ulong num;
            if (bufferReader.TryReadVarULong(out num) && (num >> 32) == 0UL)
            {
                value = (uint)num;
                return true;
            }

            value = 0U;
            return false;
        }

        public static ReadOnlySpan<byte> ReadVarBytes(this ref SpanBufferReader bufferReader)
        {
            var length = bufferReader.ReadVarUInt();
            return bufferReader.ReadBytes((int)length);
        }

        public static string ReadString(this ref SpanBufferReader bufferReader, int maxLength = 65535)
        {
            var length = bufferReader.ReadInt32();
            if (length <= 0 | length > maxLength)
                return string.Empty;
            var bytes = bufferReader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        public static IPEndPoint ReadIPEndPoint(this ref SpanBufferReader bufferReader)
        {
            if (!IPAddress.TryParse(bufferReader.ReadString(512), out var address))
                throw new ArgumentException("Failed to parse IP address");
            var port = bufferReader.ReadInt32();
            return new IPEndPoint(address, port);
        }
    }
}
