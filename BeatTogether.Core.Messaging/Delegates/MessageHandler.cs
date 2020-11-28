using System.Net;
using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Abstractions;

namespace BeatTogether.Core.Messaging.Delegates
{
    public delegate Task MessageHandler(ISession session, IMessage message);
    public delegate Task MessageHandler<TMessage>(ISession session, TMessage message)
        where TMessage : class, IMessage;
    public delegate Task<TResponse> MessageHandler<TRequest, TResponse>(ISession session, TRequest request)
        where TRequest : class, IMessage
        where TResponse : class, IMessage;
}
