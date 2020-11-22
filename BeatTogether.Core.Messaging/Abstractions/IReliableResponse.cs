namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IReliableResponse : IMessage
    {
        uint ResponseId { get; set; }
    }
}
