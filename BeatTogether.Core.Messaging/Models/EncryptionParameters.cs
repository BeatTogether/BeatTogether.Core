using System.Security.Cryptography;

namespace BeatTogether.Core.Messaging.Models
{
    public record EncryptionParameters(byte[] ReceiveKey, byte[] SendKey, HMAC ReceiveMac, HMAC SendMac);
}
