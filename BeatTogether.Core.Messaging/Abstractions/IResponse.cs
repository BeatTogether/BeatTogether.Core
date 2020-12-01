namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IResponse : IMessage
    {
        uint ResponseId { get; set; }
    }
}
