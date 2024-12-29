using BeatTogether.Core.Enums;
using BeatTogether.Core.ServerMessaging.Models;
using BinaryRecords;

namespace BeatTogether.Core.Extensions
{
    public static class BinaryBufferReaderExtensions
    {
        public static Player ReadPlayer(this ref BinaryBufferReader bufferReader)
        {
            var Player = new Player();
            Player.PlayerPlatform = (Platform)bufferReader.ReadUInt8();
            Player.PlayerSessionId = bufferReader.ReadUTF16String();
            Player.PlayerClientVersion = bufferReader.ReadUTF16String();
            Player.HashedUserId = bufferReader.ReadUTF16String();
            Player.PlatformUserId = bufferReader.ReadUTF16String();
            return Player;
        }

        public static Server ReadServer(this ref BinaryBufferReader bufferReader)
        {
            var server = new Server();

            server.ServerName = bufferReader.ReadUTF16String();
            server.Secret = bufferReader.ReadUTF16String();
            server.Code = bufferReader.ReadUTF16String();
            server.InstanceId = bufferReader.ReadUTF16String();

            server.GameState = (MultiplayerGameState)bufferReader.ReadUInt8();
            server.BeatmapDifficultyMask = (BeatmapDifficultyMask)bufferReader.ReadUInt8();
            server.GameplayModifiersMask = (GameplayModifiersMask)bufferReader.ReadUInt16();
            server.GameplayServerConfiguration.MaxPlayerCount = bufferReader.ReadInt16();
            server.GameplayServerConfiguration.DiscoveryPolicy = (DiscoveryPolicy)bufferReader.ReadUInt8();
            server.GameplayServerConfiguration.InvitePolicy = (InvitePolicy)bufferReader.ReadUInt8();
            server.GameplayServerConfiguration.GameplayServerMode = (GameplayServerMode)bufferReader.ReadUInt8();
            server.GameplayServerConfiguration.SongSelectionMode = (SongSelectionMode)bufferReader.ReadUInt8();
            server.GameplayServerConfiguration.GameplayServerControlSettings = (GameplayServerControlSettings)bufferReader.ReadUInt8();
            server.SongPackMasks = bufferReader.ReadUTF16String();

            server.ManagerId = bufferReader.ReadUTF16String();
            server.PermanentManager = bufferReader.ReadBool();
            server.ServerStartJoinTimeout = bufferReader.ReadInt64();
            server.NeverCloseServer = bufferReader.ReadBool();
            server.ResultScreenTime = bufferReader.ReadInt64();
            server.BeatmapStartTime = bufferReader.ReadInt64();
            server.PlayersReadyCountdownTime = bufferReader.ReadInt64();
            server.AllowPerPlayerModifiers = bufferReader.ReadBool();
            server.AllowPerPlayerDifficulties = bufferReader.ReadBool();
            server.AllowChroma = bufferReader.ReadBool();
            server.AllowME = bufferReader.ReadBool();
            server.AllowNE = bufferReader.ReadBool();

            return server;
        }
    }
}
