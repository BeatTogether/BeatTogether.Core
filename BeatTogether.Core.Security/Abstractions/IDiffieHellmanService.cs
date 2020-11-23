using BeatTogether.Core.Security.Models;
using Org.BouncyCastle.Crypto.Parameters;

namespace BeatTogether.Core.Security.Abstractions
{
    public interface IDiffieHellmanService
    {
        ECKeyPair GetECKeyPair();
        ECPublicKeyParameters DeserializeECPublicKey(byte[] publicKey);
        byte[] GetPreMasterSecret(
            ECPublicKeyParameters publicKeyParameters,
            ECPrivateKeyParameters privateKeyParameters);
    }
}
