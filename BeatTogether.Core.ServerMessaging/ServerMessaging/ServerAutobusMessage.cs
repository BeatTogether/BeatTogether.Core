using BeatTogether.Core.Abstractions;
using BeatTogether.Core.Enums;
using BeatTogether.Core.Models;

namespace BeatTogether.Core.ServerMessaging.Models
{
    public class Server
    {
        public Server(IServerInstance instance)
        {
            ServerName = instance.ServerName;
            Secret = instance.Secret;
            Code = instance.Code;
            InstanceId = instance.InstanceId;

            GameState = instance.GameState;
            BeatmapDifficultyMask = instance.BeatmapDifficultyMask;
            GameplayModifiersMask = instance.GameplayModifiersMask;
            GameplayServerConfiguration = instance.GameplayServerConfiguration;
            SongPackMasks = instance.SongPackMasks;

            ManagerId = instance.ManagerId;
            PermanentManager = instance.PermanentManager;
            ServerStartJoinTimeout = instance.ServerStartJoinTimeout;
            NeverCloseServer = instance.NeverCloseServer;
            ResultScreenTime = instance.ResultScreenTime;
            BeatmapStartTime = instance.BeatmapStartTime;
            PlayersReadyCountdownTime = instance.PlayersReadyCountdownTime;

            AllowPerPlayerDifficulties = instance.AllowPerPlayerDifficulties;
            AllowPerPlayerModifiers = instance.AllowPerPlayerModifiers;
            AllowChroma = instance.AllowChroma;
            AllowME = instance.AllowME;
            AllowNE = instance.AllowNE;

            SupportedVersionRange = instance.SuportedVersionRange;
		}
		public Server() { }

        public string ServerName { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string InstanceId { get; set; } = string.Empty;

        public MultiplayerGameState GameState { get; set; }
        public BeatmapDifficultyMask BeatmapDifficultyMask { get; set; }
        public GameplayModifiersMask GameplayModifiersMask { get; set; }
        public GameplayServerConfiguration GameplayServerConfiguration { get; set; } = new();
        public string SongPackMasks { get; set; } = string.Empty;

        public string ManagerId { get; set; } = string.Empty;
        public bool PermanentManager { get; set; }
        public long ServerStartJoinTimeout { get; set; }
        public bool NeverCloseServer { get; set; }
        public long ResultScreenTime { get; set; }
        public long BeatmapStartTime { get; set; }
        public long PlayersReadyCountdownTime { get; set; }
        public bool AllowPerPlayerModifiers { get; set; }
        public bool AllowPerPlayerDifficulties { get; set; }
        public bool AllowChroma { get; set; }
        public bool AllowME { get ; set ; }
        public bool AllowNE { get; set; }
        public VersionRange SupportedVersionRange { get; set; } = new();
	}
}
