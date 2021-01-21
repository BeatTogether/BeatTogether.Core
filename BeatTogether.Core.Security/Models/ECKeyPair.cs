using Org.BouncyCastle.Crypto.Parameters;

namespace BeatTogether.Core.Security.Models
{
    public record ECKeyPair(ECPrivateKeyParameters PrivateKeyParameters, byte[] PublicKey);
}
