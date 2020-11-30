using System.Threading.Tasks;
using BeatTogether.Core.Messaging.Delegates;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IMessageDispatcher
    {
        event MessageDispatchHandler OnSent;
        void Acknowledge(uint requestId, bool messageHandled);
        Task SendWithRetry(ISession session, IReliableRequest request);
        Task<TResponse> SendWithRetry<TResponse>(ISession session, IReliableRequest request)
            where TResponse : class, IReliableResponse;
        void Send(ISession session, IMessage message);
    }
}
