using System;
using System.Threading;
using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Delegates;
using BeatTogether.Core.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeatTogether.Core.Messaging.Implementations
{
    public abstract class BaseMessageHandler : IHostedService
    {
        private readonly IMessageSource _messageSource;
        private readonly IMessageDispatcher _messageDispatcher;

        public BaseMessageHandler(
            IMessageSource messageSource,
            IMessageDispatcher messageDispatcher)
        {
            _messageSource = messageSource;
            _messageDispatcher = messageDispatcher;
        }

        #region Public Methods

        public Task StartAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        #endregion

        #region Protected Methods

        protected void Register<TMessage>(MessageHandler<TMessage> messageHandler)
            where TMessage : class, IMessage =>
            _messageSource.Subscribe<TMessage>((session, message) =>
            {
                if (message is IReliableRequest request)
                    _messageDispatcher.Send(session, new AcknowledgeMessage()
                    {
                        ResponseId = request.RequestId,
                        MessageHandled = true
                    });
                return messageHandler(session, message);
            });

        protected void Register<TRequest, TResponse>(MessageHandler<TRequest, TResponse> messageHandler)
            where TRequest : class, IRequest
            where TResponse : class, IResponse =>
            Register<TRequest>(async (session, request) =>
            {
                var response = await messageHandler(session, request);
                if (response == null)
                    return;
                if (response.ResponseId == 0)
                    response.ResponseId = request.RequestId;
                if (response is IReliableRequest)
                    await _messageDispatcher.SendWithRetry(session, (IReliableRequest)response);
                else
                    _messageDispatcher.Send(session, response);
            });

        #endregion
    }

    public abstract class BaseMessageHandler<TService> : BaseMessageHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public BaseMessageHandler(
            IMessageSource messageSource,
            IMessageDispatcher messageDispatcher,
            IServiceProvider serviceProvider)
            : base(messageSource, messageDispatcher)
        {
            _serviceProvider = serviceProvider;
        }

        protected void Register<TMessage>(ServiceMessageHandler<TService, TMessage> messageHandler)
            where TMessage : class, IMessage =>
            Register<TMessage>((session, message) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<TService>();
                return messageHandler(service, session, message);
            });

        protected void Register<TRequest, TResponse>(ServiceMessageHandler<TService, TRequest, TResponse> messageHandler)
            where TRequest : class, IRequest
            where TResponse : class, IResponse =>
            Register<TRequest, TResponse>((session, request) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<TService>();
                return messageHandler(service, session, request);
            });
    }
}
