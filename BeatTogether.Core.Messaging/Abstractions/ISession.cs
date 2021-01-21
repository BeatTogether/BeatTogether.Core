using System.Net;
using BeatTogether.Core.Messaging.Models;

namespace BeatTogether.Core.Messaging.Abstractions
{
    public interface ISession
    {
        EndPoint EndPoint { get; }
        uint Epoch { get; set; }
        EncryptionParameters? EncryptionParameters { get; set; }

        uint GetNextSequenceId();
        uint GetNextRequestId();
        bool ShouldHandleRequest(uint requestId);
    }
}
