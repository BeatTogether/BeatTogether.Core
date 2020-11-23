using System;
using System.IO;
using BeatTogether.Core.Security.Abstractions;
using BeatTogether.Core.Security.Configuration;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace BeatTogether.Core.Security.Implementations
{
    public class CertificateSigningService : ICertificateSigningService
    {
        private readonly RsaPrivateCrtKeyParameters _privateKey;

        public CertificateSigningService(SecurityConfiguration configuration)
        {
            using var streamReader = File.OpenText(configuration.PrivateKeyPath);
            var pemReader = new PemReader(streamReader);
            var @object = pemReader.ReadObject();
            var asymmetricCipherKeyPair = @object as AsymmetricCipherKeyPair;
            if (asymmetricCipherKeyPair != null)
                @object = asymmetricCipherKeyPair.Private;
            _privateKey = @object as RsaPrivateCrtKeyParameters;
            if (_privateKey is null)
                throw new Exception($"Invalid RSA private key (Path='{configuration.PrivateKeyPath}').");
        }

        public byte[] Sign(byte[] data)
        {
            var signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            signer.Init(true, _privateKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }
    }
}
