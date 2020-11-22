using BeatTogether.Core.Data.Abstractions;
using BeatTogether.Core.Data.Configuration;
using BeatTogether.Core.Data.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeatTogether.Core.Data.Bootstrap
{
    public static class CoreDataBootstrapper
    {
        public static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services.AddSingleton(
                hostBuilderContext
                    .Configuration
                    .GetSection("Data")
                    .Get<DataConfiguration>()
            );
            services.AddSingleton(
                hostBuilderContext
                    .Configuration
                    .GetSection("Data:Redis")
                    .Get<RedisConfiguration>()
            );
            services.AddSingleton<IConnectionMultiplexerPool, ConnectionMultiplexerPool>();
            services.AddScoped(serviceProvider =>
                serviceProvider
                    .GetRequiredService<IConnectionMultiplexerPool>()
                    .GetConnection()
            );
        }
    }
}
