using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Messages;
using Serilog;

namespace BeatTogether.Core.Messaging.Implementations
{
    using UdpClient = NetCoreServer.UdpClient;

    public abstract class BaseUdpClient : UdpClient
    {
        public ISession Session { get; }

        private readonly IMessageSource _messageSource;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly ILogger _logger;

        public BaseUdpClient(
            IPEndPoint endPoint,
            IMessageSource messageSource,
            IMessageDispatcher messageDispatcher)
            : base(endPoint)
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

        protected override void OnConnected() => ReceiveAsync();

        protected override void OnReceived(EndPoint endPoint, ReadOnlySpan<byte> buffer)
        {
            _logger.Verbose(
                "Handling OnReceived " +
                $"(EndPoint='{endPoint}', " +
                $"Data='{BitConverter.ToString(buffer.ToArray())}')."
            );
            if (buffer.Length > 0)
                _messageSource.Signal(Session, buffer);
            ReceiveAsync();
        }

        protected override void OnError(SocketError error) =>
            _logger.Error($"Handling OnError (Error={error}).");

        #endregion
    }
}
