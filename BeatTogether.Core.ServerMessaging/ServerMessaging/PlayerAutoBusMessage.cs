using BeatTogether.Core.Abstractions;
using BeatTogether.Core.Enums;

namespace BeatTogether.Core.ServerMessaging.Models
{
    public class Player
    {
        public Player(IPlayer player)
        {
            HashedUserId = player.HashedUserId;
            PlatformUserId = player.PlatformUserId;
            PlayerSessionId = player.PlayerSessionId;
            PlayerPlatform = player.PlayerPlatform;
            PlayerClientVersion = player.PlayerClientVersion.ToString();
        }
        public Player() { }

        public string HashedUserId { get; set; } = string.Empty;
        public string PlatformUserId { get; set; } = string.Empty;
        public string PlayerSessionId { get; set; } = string.Empty;
        public Platform PlayerPlatform { get; set; }
        public string PlayerClientVersion { get; set; } = string.Empty;
    }
}
