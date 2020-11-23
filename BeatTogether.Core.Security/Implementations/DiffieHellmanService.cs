using System.IO;
using BeatTogether.Core.Security.Abstractions;
using BeatTogether.Core.Security.Models;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;

namespace BeatTogether.Core.Security.Implementations
{
    public class DiffieHellmanService : IDiffieHellmanService
    {
        private static byte[] _ecPointFormats = new byte[] { 2 };
        private static ECDomainParameters _ecParameters = TlsEccUtilities.GetParametersForNamedCurve(24);

        private readonly SecureRandom _secureRandom;

        public DiffieHellmanService(SecureRandom secureRandom)
        {
            _secureRandom = secureRandom;
        }

        public ECKeyPair GetECKeyPair()
        {
            using var memoryStream = new MemoryStream();
            return new ECKeyPair
            {
                PrivateKeyParameters = TlsEccUtilities.GenerateEphemeralClientKeyExchange(
                    _secureRandom, _ecPointFormats,
                    _ecParameters, memoryStream
                ),
                PublicKey = memoryStream.ToArray()
            };
        }

        public ECPublicKeyParameters DeserializeECPublicKey(byte[] publicKey)
        {
            using var memoryStream = new MemoryStream(publicKey);
            return TlsEccUtilities.ValidateECPublicKey(
                TlsEccUtilities.DeserializeECPublicKey(
                    _ecPointFormats, _ecParameters,
                    TlsUtilities.ReadOpaque8(memoryStream)
                )
            );
        }

        public byte[] GetPreMasterSecret(
            ECPublicKeyParameters publicKeyParameters,
            ECPrivateKeyParameters privateKeyParameters)
            => TlsEccUtilities.CalculateECDHBasicAgreement(publicKeyParameters, privateKeyParameters);
    }
}
