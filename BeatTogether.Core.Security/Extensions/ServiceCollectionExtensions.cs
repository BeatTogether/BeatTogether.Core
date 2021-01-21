using System.Security.Cryptography.X509Certificates;
using BeatTogether.Core.Security.Abstractions;
using BeatTogether.Core.Security.Configuration;
using BeatTogether.Core.Security.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Security;

namespace BeatTogether.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreSecurity(this IServiceCollection services) =>
            services
                .AddConfiguration<SecurityConfiguration>("Security")
                .AddSingleton(serviceProvider => new X509Certificate2(
                    serviceProvider
                        .GetRequiredService<SecurityConfiguration>()
                        .CertificatePath
                ))
                .AddTransient<SecureRandom>()
                .AddSingleton<IDiffieHellmanService, DiffieHellmanService>()
                .AddSingleton<ICertificateSigningService, CertificateSigningService>();
    }
}
