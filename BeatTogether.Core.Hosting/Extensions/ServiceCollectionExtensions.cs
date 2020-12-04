using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeatTogether.Core.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguration<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionKey = default)
            where T : class, new()
        {
            if (!string.IsNullOrEmpty(sectionKey))
                configuration = configuration.GetSection(sectionKey);
            var instance = configuration.Get<T>();
            if (instance is null)
                instance = new T();
            return services.AddSingleton(instance);
        }
    }
}
