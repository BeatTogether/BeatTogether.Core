using System;
using BeatTogether.Core.Messaging.Abstractions;

namespace BeatTogether.Core.Messaging.Delegates
{
    public delegate void MessageDispatchHandler(ISession session, ReadOnlySpan<byte> buffer);
}
