using System.Security.Cryptography;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Configuration;
using BeatTogether.Core.Messaging.Implementations;
using BeatTogether.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BeatTogether.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreMessaging(this IServiceCollection services) =>
            services
                .AddConfiguration<MessagingConfiguration>("Messaging")
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
