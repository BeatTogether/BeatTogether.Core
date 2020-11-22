using System.Security.Cryptography;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeatTogether.Core.Messaging.Bootstrap
{
    public static class CoreMessagingBootstrapper
    {
        public static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) =>
            services
                .AddTransient<RNGCryptoServiceProvider>()
                .AddTransient(serviceProvider =>
                    new AesCryptoServiceProvider()
                    {
                        Mode = CipherMode.CBC,
                        Padding = PaddingMode.None
                    }
                )
                .AddSingleton<IMessageReader, MessageReader>()
                .AddSingleton<IMessageWriter, MessageWriter>()
                .AddSingleton<IEncryptedMessageReader, EncryptedMessageReader>()
                .AddSingleton<IEncryptedMessageWriter, EncryptedMessageWriter>();
    }
}
