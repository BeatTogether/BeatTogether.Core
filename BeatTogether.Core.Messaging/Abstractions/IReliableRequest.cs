namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IReliableRequest : IMessage
    {
        uint RequestId { get; set; }
    }
}
