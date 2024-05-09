using BeatTogether.Core.ServerMessaging.Models;
using BinaryRecords;

namespace BeatTogether.Core.Extensions
{
    public static class BinaryBufferWriterExtensions
    {
        public static void WritePlayer(this ref BinaryBufferWriter bufferWriter, Player player)
        {
            bufferWriter.WriteUInt8((byte)player.PlayerPlatform);
            bufferWriter.WriteUTF16String(player.PlayerSessionId);
            bufferWriter.WriteUTF16String(player.PlayerClientVersion);
            bufferWriter.WriteUTF16String(player.HashedUserId);
            bufferWriter.WriteUTF16String(player.PlatformUserId);
        }


        public static void WriteServer(this ref BinaryBufferWriter bufferWriter, Server server)
        {
            bufferWriter.WriteUTF16String(server.ServerName);
            bufferWriter.WriteUTF16String(server.Secret);
            bufferWriter.WriteUTF16String(server.Code);
            bufferWriter.WriteUTF16String(server.InstanceId);

            bufferWriter.WriteUInt8((byte)server.GameState);
            bufferWriter.WriteUInt8((byte)server.BeatmapDifficultyMask);
            bufferWriter.WriteUInt16((ushort)server.GameplayModifiersMask);

            bufferWriter.WriteUInt16((ushort)server.GameplayServerConfiguration.MaxPlayerCount);
            bufferWriter.WriteUInt8((byte)server.GameplayServerConfiguration.DiscoveryPolicy);
            bufferWriter.WriteUInt8((byte)server.GameplayServerConfiguration.InvitePolicy);
            bufferWriter.WriteUInt8((byte)server.GameplayServerConfiguration.GameplayServerMode);
            bufferWriter.WriteUInt8((byte)server.GameplayServerConfiguration.SongSelectionMode);
            bufferWriter.WriteUInt8((byte)server.GameplayServerConfiguration.GameplayServerControlSettings);

            bufferWriter.WriteUTF16String(server.SongPackMasks);

            bufferWriter.WriteUTF16String(server.ManagerId);
            bufferWriter.WriteBool(server.PermanentManager);
            bufferWriter.WriteInt64(server.ServerStartJoinTimeout);
            bufferWriter.WriteBool(server.NeverCloseServer);
            bufferWriter.WriteInt64(server.ResultScreenTime);
            bufferWriter.WriteInt64(server.BeatmapStartTime);
            bufferWriter.WriteInt64(server.PlayersReadyCountdownTime);

            bufferWriter.WriteBool(server.AllowPerPlayerModifiers);
            bufferWriter.WriteBool(server.AllowPerPlayerDifficulties);
            bufferWriter.WriteBool(server.AllowChroma);
            bufferWriter.WriteBool(server.AllowME);
            bufferWriter.WriteBool(server.AllowNE);
        }
    }
}
