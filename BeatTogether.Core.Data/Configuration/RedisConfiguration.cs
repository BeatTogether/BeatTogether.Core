namespace BeatTogether.Core.Data.Configuration
{
    public class RedisConfiguration
    {
        public bool Enabled { get; set; } = false;
        public string Endpoint { get; set; } = "127.0.0.1:6379";
        public int ConnectionPoolSize { get; set; } = 10;
        public int SyncTimeout { get; set; } = 50000;
        public int KeepAlive { get; set; } = 60;
    }
}
