using System;
using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Delegates;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IMessageSource
    {
        void Subscribe<TMessage>(MessageHandler<TMessage> messageHandler)
            where TMessage : class, IMessage;
        Task<IResponse> WaitForResponse(uint requestId);
        void Signal(ISession session, ReadOnlySpan<byte> buffer);
        void Signal(ISession session, IMessage message);
    }
}
