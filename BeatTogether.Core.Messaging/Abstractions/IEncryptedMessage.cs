namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface IEncryptedMessage : IMessage
    {
        uint SequenceId { get; set; }
    }
}
