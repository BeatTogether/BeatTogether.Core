namespace BeatTogether.Core.Security.Configuration
{
    public class SecurityConfiguration
    {
        public string PrivateKeyPath { get; set; } = "key.pem";
        public string CertificatePath { get; set; } = "cert.pem";
    }
}
