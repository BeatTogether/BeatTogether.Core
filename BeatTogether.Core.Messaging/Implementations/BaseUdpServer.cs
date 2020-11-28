using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Messages;
using Serilog;

namespace BeatTogether.Core.Messaging.Implementations
{
    public abstract class BaseUdpServer<TSession> : NetCoreServer.UdpServer
        where TSession : class, ISession, new()
    {
        private readonly IMessageSource _messageSource;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<EndPoint, ISession> _sessions;

        public BaseUdpServer(
            IPEndPoint endPoint,
            IMessageSource messageSource,
            IMessageDispatcher messageDispatcher)
            : base(endPoint)
        {
            _messageSource = messageSource;
            _messageDispatcher = messageDispatcher;
            _logger = Log.ForContext<BaseUdpServer<TSession>>();
            _sessions = new ConcurrentDictionary<EndPoint, ISession>();

            _messageDispatcher.OnSend += (session, buffer) => SendAsync(session.EndPoint, buffer);
            _messageSource.Subscribe<AcknowledgeMessage>((session, message) =>
            {
                _messageDispatcher.Acknowledge(message.ResponseId, message.MessageHandled);
                return Task.CompletedTask;
            });
        }

        #region Protected Methods

        protected override void OnStarted() => ReceiveAsync();

        protected override void OnReceived(EndPoint endPoint, ReadOnlySpan<byte> buffer)
        {
            _logger.Verbose($"Handling OnReceived (EndPoint='{endPoint}', Size={buffer.Length}).");
            if (buffer.Length > 0)
            {
                var session = GetOrAddSession(endPoint);
                _messageSource.Signal(session, buffer);
            }
            ReceiveAsync();
        }

        protected override void OnError(SocketError error) =>
            _logger.Error($"Handling OnError (Error={error}).");

        protected ISession GetOrAddSession(EndPoint endPoint) =>
            _sessions.GetOrAdd(
                endPoint,
                key =>
                {
                    _logger.Verbose($"Opening session (EndPoint='{endPoint}').");
                    return new TSession()
                    {
                        EndPoint = key
                    };
                }
            );

        #endregion
    }
}
