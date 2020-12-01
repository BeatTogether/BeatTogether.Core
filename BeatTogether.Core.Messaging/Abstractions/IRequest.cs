namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IRequest : IMessage
    {
        uint RequestId { get; set; }
    }
}
