using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Configuration;
using BeatTogether.Core.Messaging.Delegates;
using BeatTogether.Core.Messaging.Messages;
using Krypton.Buffers;
using Serilog;

namespace BeatTogether.Core.Messaging.Implementations
{
    public abstract class BaseMessageSource : IMessageSource
    {
        private class MultipartMessageWaiter
        {
            private readonly BaseMessageSource _messageSource;
            private readonly ConcurrentDictionary<uint, MultipartMessage> _messages;

            private TaskCompletionSource<IMessage> _taskCompletionSource;
            private CancellationTokenSource _cancellationTokenSource;

            private uint _multipartMessageId;
            private uint _totalLength;
            private uint _receivedLength;

            public MultipartMessageWaiter(
                uint multipartMessageId,
                uint totalLength,
                BaseMessageSource messageSource)
            {
                _messageSource = messageSource;

                _multipartMessageId = multipartMessageId;
                _totalLength = totalLength;

                _messages = new ConcurrentDictionary<uint, MultipartMessage>();

                _taskCompletionSource = new TaskCompletionSource<IMessage>();
                if (_messageSource._configuration.RequestTimeout > 0)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _cancellationTokenSource.CancelAfter(_messageSource._configuration.RequestTimeout);
                    _cancellationTokenSource.Token.Register(() =>
                    {
                        if (_taskCompletionSource != null)
                            _taskCompletionSource.TrySetException(new TimeoutException());

                        if (_cancellationTokenSource != null)
                        {
                            _cancellationTokenSource.Dispose();
                            _cancellationTokenSource = null;
                        }

                        _messageSource._multipartMessageWaiters.TryRemove(_multipartMessageId, out _);
                    });
                }
            }

            public Task<IMessage> Wait() => _taskCompletionSource.Task;

            public void Complete(ISession session, IMessage message)
            {
                _messageSource._multipartMessageWaiters.TryRemove(_multipartMessageId, out _);

                if (_taskCompletionSource != null)
                    _taskCompletionSource.TrySetResult(message);

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }

                _messageSource.Signal(session, message);
            }

            public void AddMessage(ISession session, MultipartMessage message)
            {
                if (message.MultipartMessageId != _multipartMessageId)
                    return;
                if (_receivedLength >= _totalLength)
                    return;
                if (!_messages.TryAdd(message.Offset, message))
                    return;
                if (Interlocked.Add(ref _receivedLength, message.Length) >= _totalLength)
                {
                    var buffer = new GrowingSpanBuffer(stackalloc byte[(int)_totalLength]);
                    foreach (var kvp in _messages.OrderBy(kvp => kvp.Key))
                        buffer.WriteBytes(kvp.Value.Data);
                    var bufferReader = new SpanBufferReader(buffer.Data);
                    var fullMessage = _messageSource._messageReader.ReadFrom(ref bufferReader);
                    Complete(session, fullMessage);
                }
            }
        }

        private class ResponseWaiter
        {
            private readonly BaseMessageSource _messageSource;

            private TaskCompletionSource<IResponse> _taskCompletionSource;
            private CancellationTokenSource _cancellationTokenSource;

            public ResponseWaiter(
                uint requestId,
                BaseMessageSource messageSource)
            {
                _messageSource = messageSource;

                _taskCompletionSource = new TaskCompletionSource<IResponse>();
                if (_messageSource._configuration.RequestTimeout > 0)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _cancellationTokenSource.CancelAfter(_messageSource._configuration.RequestTimeout);
                    _cancellationTokenSource.Token.Register(() =>
                    {
                        if (_taskCompletionSource != null)
                            _taskCompletionSource.TrySetException(new TimeoutException());

                        if (_cancellationTokenSource != null)
                        {
                            _cancellationTokenSource.Dispose();
                            _cancellationTokenSource = null;
                        }

                        _messageSource._responseWaiters.TryRemove(requestId, out _);
                    });
                }
            }

            public Task<IResponse> Wait() => _taskCompletionSource.Task;

            public void Complete(IResponse message)
            {
                if (_taskCompletionSource != null)
                    _taskCompletionSource.TrySetResult(message);

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }

        protected abstract byte PacketProperty { get; }

        private readonly MessagingConfiguration _configuration;
        private readonly IMessageReader _messageReader;
        private readonly IEncryptedMessageReader _encryptedMessageReader;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<uint, MultipartMessageWaiter> _multipartMessageWaiters;
        private readonly ConcurrentDictionary<uint, ResponseWaiter> _responseWaiters;
        private readonly Dictionary<Type, List<MessageHandler>> _messageHandlers;

        public BaseMessageSource(
            MessagingConfiguration configuration,
            IMessageReader messageReader,
            IEncryptedMessageReader encryptedMessageReader)
        {
            _configuration = configuration;
            _messageReader = messageReader;
            _encryptedMessageReader = encryptedMessageReader;
            _logger = Log.ForContext<BaseMessageSource>();
            _multipartMessageWaiters = new ConcurrentDictionary<uint, MultipartMessageWaiter>();
            _responseWaiters = new ConcurrentDictionary<uint, ResponseWaiter>();
            _messageHandlers = new Dictionary<Type, List<MessageHandler>>();
        }

        public void Subscribe<TMessage>(MessageHandler<TMessage> messageHandler)
            where TMessage : class, IMessage
        {
            var messageType = typeof(TMessage);
            if (!_messageHandlers.TryGetValue(messageType, out var messageHandlers))
            {
                messageHandlers = new List<MessageHandler>();
                _messageHandlers[messageType] = messageHandlers;
            }
            messageHandlers.Add((endPoint, message) => messageHandler(endPoint, (TMessage)message));
        }

        public Task<IResponse> WaitForResponse(uint requestId)
        {
            if (!_responseWaiters.TryGetValue(requestId, out var responseWaiter))
            {
                responseWaiter = new ResponseWaiter(requestId, this);
                _responseWaiters[requestId] = responseWaiter;
            }
            return responseWaiter.Wait();
        }

        public void Signal(ISession session, ReadOnlySpan<byte> buffer)
        {
            var bufferReader = new SpanBufferReader(buffer);
            IMessage message;
            try
            {
                var isEncrypted = bufferReader.ReadBool();
                if (isEncrypted)
                {
                    if (session.EncryptionParameters is null)
                    {
                        _logger.Warning(
                            "Received an encrypted messsage before any " +
                            "encryption parameters were established " +
                            $"(EndPoint='{session.EndPoint}')."
                        );
                        return;
                    }
                    message = _encryptedMessageReader.ReadFrom(
                        ref bufferReader,
                        session.EncryptionParameters.ReceiveKey,
                        session.EncryptionParameters.ReceiveMac,
                        PacketProperty
                    );
                }
                else
                    message = _messageReader.ReadFrom(ref bufferReader, PacketProperty);
            }
            catch (Exception e)
            {
                _logger.Warning(e, $"Failed to read message (EndPoint='{session.EndPoint}').");
                return;
            }
            Signal(session, message);
        }

        public void Signal(ISession session, IMessage message) =>
            Task.Run(async () =>
            {
                var messageType = message.GetType();
                try
                {
                    if (message is IRequest request &&
                        !session.ShouldHandleRequest(request.RequestId))
                    {
                        _logger.Verbose(
                            "Skipping duplicate request " +
                            $"(MessageType='{messageType.Name}', " +
                            $"RequestId={request.RequestId})."
                        );
                        return;
                    }

                    if (message is MultipartMessage multipartMessage)
                    {
                        var multipartMessageWaiter = _multipartMessageWaiters.GetOrAdd(
                            multipartMessage.MultipartMessageId,
                            new MultipartMessageWaiter(
                                multipartMessage.MultipartMessageId,
                                multipartMessage.TotalLength,
                                this
                            )
                        );
                        multipartMessageWaiter.AddMessage(session, multipartMessage);
                    }

                    if (message is IResponse response && response is not AcknowledgeMessage)
                    {
                        if (_responseWaiters.TryRemove(response.ResponseId, out var responseWaiter))
                            responseWaiter.Complete(response);
                    }

                    if (_messageHandlers.TryGetValue(messageType, out var messageHandlers))
                        await Task.WhenAll(messageHandlers.Select(messageHandler => messageHandler(session, message)));
                }
                catch (Exception e)
                {
                    _logger.Error(e,
                        "An error occurred while handling message " +
                        $"(MessageType='{messageType.Name}')."
                    );
                }
            }).ConfigureAwait(false);
    }
}
