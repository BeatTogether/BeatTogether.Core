using BeatTogether.Core.Models;
using BeatTogether.Core.Enums;
using System.Net;

namespace BeatTogether.Core.Abstractions
{
    public interface IServerInstance
    {
        public string ServerName { get; set; }
        public IPEndPoint InstanceEndPoint { get; set; }
        public string Secret { get; set; }
        public string Code { get; set; }
        public MultiplayerGameState GameState { get; set; }
        public BeatmapDifficultyMask BeatmapDifficultyMask { get; set; }
        public GameplayModifiersMask GameplayModifiersMask { get; set; }
        public GameplayServerConfiguration GameplayServerConfiguration { get; set; }
        public string SongPackMasks { get; set; }
        public HashSet<string> PlayerHashes { get; set; }
        public string InstanceId { get; set; }
        public string ManagerId { get; set; }
        public bool PermanentManager { get; set; }
        public long ServerStartJoinTimeout { get; set; } //Milliseconds
        public bool NeverCloseServer { get; set; }
        public long ResultScreenTime { get; set; }
        public long BeatmapStartTime { get; set; }
        public long PlayersReadyCountdownTime { get; set; }
        public bool AllowPerPlayerModifiers { get; set; }
        public bool AllowPerPlayerDifficulties { get; set; }
        public bool AllowChroma { get; set; }
        public bool AllowME { get; set; }
        public bool AllowNE { get; set; }
        public VersionRange SupportedVersionRange { get; set; }
	}

}
