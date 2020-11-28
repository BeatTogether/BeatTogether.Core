using System.Collections.Generic;
using System.Net;
using System.Threading;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Models;

namespace BeatTogether.Core.Messaging.Implementations
{
    public abstract class BaseSession : ISession
    {
        public EndPoint EndPoint { get; init; }
        public uint Epoch { get; set; }
        public EncryptionParameters EncryptionParameters { get; set; }

        private uint _lastSentSequenceId = 0;
        private uint _lastSentRequestId = 0;
        private HashSet<uint> _handledRequests = new HashSet<uint>();
        private uint _lastHandledRequestId = 0;

        public uint GetNextSequenceId()
            => unchecked(Interlocked.Increment(ref _lastSentSequenceId));

        public uint GetNextRequestId()
            => (unchecked(Interlocked.Increment(ref _lastSentRequestId)) % 64) | Epoch;

        public bool ShouldHandleRequest(uint requestId)
        {
            lock (_handledRequests)
            {
                if (_handledRequests.Add(requestId))
                {
                    if (_handledRequests.Count > 64)
                        _handledRequests.Remove(_lastHandledRequestId);
                    _lastHandledRequestId = requestId;
                    return true;
                }
                return false;
            }
        }
    }
}
