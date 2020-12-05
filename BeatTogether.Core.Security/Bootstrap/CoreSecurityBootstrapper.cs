using System.Security.Cryptography.X509Certificates;
using BeatTogether.Core.Hosting.Extensions;
using BeatTogether.Core.Security.Abstractions;
using BeatTogether.Core.Security.Configuration;
using BeatTogether.Core.Security.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Security;

namespace BeatTogether.Core.Security.Bootstrap
{
    public static class CoreSecurityBootstrapper
    {
        public static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services.AddConfiguration<SecurityConfiguration>(hostBuilderContext.Configuration, "Security");
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<SecurityConfiguration>();
                return new X509Certificate2(configuration.CertificatePath);
            });
            services.AddTransient<SecureRandom>();
            services.AddSingleton<IDiffieHellmanService, DiffieHellmanService>();
            services.AddSingleton<ICertificateSigningService, CertificateSigningService>();
        }
    }
}
