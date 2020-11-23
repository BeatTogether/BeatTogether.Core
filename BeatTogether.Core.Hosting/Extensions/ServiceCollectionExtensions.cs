using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeatTogether.Core.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguration<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionKey) where T : class =>
            services.AddSingleton(configuration.GetSection(sectionKey).Get<T>());
    }
}
