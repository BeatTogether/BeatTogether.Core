using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Configuration;
using BeatTogether.Core.Messaging.Delegates;
using BeatTogether.Core.Messaging.Messages;
using Krypton.Buffers;
using Serilog;

namespace BeatTogether.Core.Messaging.Implementations
{
    public abstract class BaseMessageDispatcher : IMessageDispatcher
    {
        private class AcknowledgementWaiter
        {
            private TaskCompletionSource<bool> _taskCompletionSource;

            public AcknowledgementWaiter()
            {
                _taskCompletionSource = new TaskCompletionSource<bool>();
            }

            public Task<bool> Wait() => _taskCompletionSource.Task;

            public void Complete(bool messageHandled)
            {
                if (_taskCompletionSource != null)
                    _taskCompletionSource.TrySetResult(messageHandled);
            }
        }

        public event MessageDispatchHandler OnSent;

        protected abstract byte PacketProperty { get; }

        private readonly MessagingConfiguration _configuration;
        private readonly IMessageSource _messageSource;
        private readonly IMessageWriter _messageWriter;
        private readonly IEncryptedMessageWriter _encryptedMessageWriter;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<uint, AcknowledgementWaiter> _acknowledgementWaiters;
        private readonly List<MessageDispatchHandler> _messageDispatchHandlers;

        public BaseMessageDispatcher(
            MessagingConfiguration configuration,
            IMessageSource messageSource,
            IMessageWriter messageWriter,
            IEncryptedMessageWriter encryptedMessageWriter)
        {
            _configuration = configuration;
            _messageSource = messageSource;
            _messageWriter = messageWriter;
            _encryptedMessageWriter = encryptedMessageWriter;
            _logger = Log.ForContext<BaseMessageDispatcher>();
            _acknowledgementWaiters = new ConcurrentDictionary<uint, AcknowledgementWaiter>();
            _messageDispatchHandlers = new List<MessageDispatchHandler>();
        }

        #region Public Methods

        public void Acknowledge(uint requestId, bool messageHandled)
        {
            if (_acknowledgementWaiters.TryRemove(requestId, out var acknowledgementWaiter))
            {
                _logger.Verbose(
                    "Received acknowledgement for request " +
                    $"(RequestId={requestId}, " +
                    $"MessageHandled={messageHandled})."
                );
                acknowledgementWaiter.Complete(messageHandled);
            }
        }

        public async Task SendWithRetry(ISession session, IReliableRequest request)
        {
            if (request.RequestId == 0)
                request.RequestId = session.GetNextRequestId();

            var retryCount = 0;
            while (retryCount++ < _configuration.MaximumRequestRetries)
            {
                var acknowledgementWaiter = WaitForAcknowledgement(request.RequestId);
                Send(session, request);
                await Task.WhenAny(
                    acknowledgementWaiter,
                    Task.Delay(_configuration.RequestRetryDelay)
                );
                if (acknowledgementWaiter.IsCompleted)
                    break;
                _logger.Verbose(
                    $"Retrying request of type '{request.GetType().Name}' " +
                    "due to lack of acknowledgement " +
                    $"(RequestId={request.RequestId}, " +
                    $"RetryCount={retryCount})."
                );
            }
        }

        public async Task<TResponse> SendWithRetry<TResponse>(ISession session, IReliableRequest request)
            where TResponse : class, IReliableResponse
        {
            if (request.RequestId == 0)
                request.RequestId = request.RequestId = session.GetNextRequestId();

            var responseWaiter = _messageSource.WaitForResponse(request.RequestId);
            await Task.WhenAny(SendWithRetry(session, request), responseWaiter);
            var response = (TResponse)await responseWaiter;
            Acknowledge(request.RequestId, true);
            return response;
        }

        public void Send(ISession session, IMessage message)
        {
            var buffer = new GrowingSpanBuffer(stackalloc byte[412]);
            if (message is IEncryptedMessage)
            {
                if (session.EncryptionParameters is null)
                {
                    if (message is not AcknowledgeMessage)
                    {
                        _logger.Warning(
                            "Attempted to send an encrypted messsage before " +
                            "any encryption parameters were established " +
                            $"(EndPoint='{session.EndPoint}')."
                        );
                        return;
                    }
                    _messageWriter.WriteTo(ref buffer, message, PacketProperty);
                }
                else
                    _encryptedMessageWriter.WriteTo(
                        ref buffer, message,
                        session.EncryptionParameters.SendKey,
                        session.EncryptionParameters.SendMac,
                        PacketProperty
                    );
            }
            else
                _messageWriter.WriteTo(ref buffer, message, PacketProperty);
            OnSent?.Invoke(session, buffer.Data);
        }

        #endregion

        #region Private Methods

        private Task WaitForAcknowledgement(uint requestId)
        {
            if (!_acknowledgementWaiters.TryGetValue(requestId, out var acknowledgementWaiter))
            {
                acknowledgementWaiter = new AcknowledgementWaiter();
                _acknowledgementWaiters[requestId] = acknowledgementWaiter;
            }
            return acknowledgementWaiter.Wait();
        }

        #endregion
    }
}
