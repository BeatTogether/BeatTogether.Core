using BeatTogether.Core.Data.Abstractions;
using BeatTogether.Core.Data.Configuration;
using BeatTogether.Core.Data.Implementations;
using BeatTogether.Core.Hosting.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeatTogether.Core.Data.Bootstrap
{
    public static class CoreDataBootstrapper
    {
        public static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services.AddConfiguration<DataConfiguration>(hostBuilderContext.Configuration, "Data");
            services.AddConfiguration<RedisConfiguration>(hostBuilderContext.Configuration, "Data:Redis");
            services.AddSingleton<IConnectionMultiplexerPool, ConnectionMultiplexerPool>();
            services.AddScoped(serviceProvider =>
                serviceProvider
                    .GetRequiredService<IConnectionMultiplexerPool>()
                    .GetConnection()
            );
        }
    }
}
