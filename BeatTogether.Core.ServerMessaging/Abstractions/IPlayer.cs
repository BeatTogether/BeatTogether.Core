using BeatTogether.Core.Enums;

namespace BeatTogether.Core.Abstractions
{
    public interface IPlayer
    {
        public string HashedUserId { get; set; }
        public string PlatformUserId { get; set; }
        public string PlayerSessionId { get; set; }
        public Platform PlayerPlatform { get; set; }
        public Version PlayerClientVersion { get; set; }
    }

}
