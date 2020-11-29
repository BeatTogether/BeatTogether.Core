using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Abstractions;

namespace BeatTogether.Core.Messaging.Delegates
{
    public delegate Task ServiceMessageHandler<TService, TMessage>(TService service, ISession session, TMessage message)
        where TMessage : class, IMessage;
    public delegate Task<TResponse> ServiceMessageHandler<TService, TRequest, TResponse>(TService service, ISession session, TRequest request)
        where TRequest : class, IReliableRequest
        where TResponse : class, IReliableResponse;
}
