namespace BeatTogether.Core.Security.Abstractions
{
    public interface ICertificateSigningService
    {
        byte[] Sign(byte[] data);
    }
}
