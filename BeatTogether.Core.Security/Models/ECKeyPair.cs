using Org.BouncyCastle.Crypto.Parameters;

namespace BeatTogether.Core.Security.Models
{
    public record ECKeyPair
    {
        public ECPrivateKeyParameters PrivateKeyParameters;
        public byte[] PublicKey;
    }
}
