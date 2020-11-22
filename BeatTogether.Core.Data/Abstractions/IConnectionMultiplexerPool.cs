using StackExchange.Redis;

namespace BeatTogether.Core.Data.Abstractions
{
    public interface IConnectionMultiplexerPool
    {
        IConnectionMultiplexer GetConnection();
    }
}
