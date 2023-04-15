using System;
using System.Net;
using System.Threading.Tasks;
using AsyncUdp;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Messages;
using Serilog;

namespace BeatTogether.Core.Messaging.Implementations
{
    public abstract class BaseUdpClient : AsyncUdpServer
    {
        public ISession Session { get; }

        private readonly IMessageSource _messageSource;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly ILogger _logger;

        public BaseUdpClient(
            IPEndPoint endPoint,
            IMessageSource messageSource,
            IMessageDispatcher messageDispatcher)
            : base(endPoint, false, 1)
        {
            Session = GetSession(endPoint);

            _messageSource = messageSource;
            _messageDispatcher = messageDispatcher;
            _logger = Log.ForContext<BaseUdpClient>();

            // TODO: Remove byte array allocation
            _messageDispatcher.OnSent += (session, buffer) =>
            {
                var data = buffer.ToArray();
                _logger.Verbose(
                    "Handling OnSent " +
                    $"(EndPoint='{session.EndPoint}', " +
                    $"Data='{BitConverter.ToString(data)}')."
                );
                SendAsync(session.EndPoint, data);
            };
            _messageSource.Subscribe<AcknowledgeMessage>((session, message) =>
            {
                _messageDispatcher.Acknowledge(message.ResponseId, message.MessageHandled);
                return Task.CompletedTask;
            });
            _messageSource.Subscribe((session, message) =>
            {
                if (message is IReliableRequest reliableRequest)
                    _messageDispatcher.Send(session, new AcknowledgeMessage()
                    {
                        ResponseId = reliableRequest.RequestId,
                        MessageHandled = true
                    });
                return Task.CompletedTask;
            });
        }

        #region Abstract Methods

        protected abstract ISession GetSession(EndPoint endPoint);

        #endregion

        #region Protected Methods

        protected override void OnReceived(EndPoint endpoint, Memory<byte> buffer)
        {
            _logger.Verbose(
                "Handling OnReceived " +
                $"(EndPoint='{endpoint}', " +
                $"Data='{BitConverter.ToString(buffer.ToArray())}')."
);
            if (buffer.Length > 0)
                _messageSource.Signal(Session, buffer.Span);
        }

        #endregion
    }
}
