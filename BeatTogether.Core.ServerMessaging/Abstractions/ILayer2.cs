using BeatTogether.Core.Enums;
using BeatTogether.Core.Models;

namespace BeatTogether.Core.Abstractions
{
    //we will have 3 parts.
    //Api is a stage1, it handles only gamelift requests.
    //node service is a stage1 and a stage2, it handles the master servers list of servers, extra api requests, and all the node related things
    //Dedi server is a stage2, it handles all instance logic

    public interface ILayer2 //Layer 1 calls these to control layer 2
    {
        //Response from these should be awaited when using node server, instant on single exe server
        public Task<bool> CreateInstance(IServerInstance serverInstance);
        public Task CloseInstance(string InstanceSecret);
        public Task<bool> SetPlayerSessionData(string InstanceSecret, IPlayer playerSessionData);
        public Task DisconnectPlayer(string InstanceSecret, string PlayerUserId);

        public Task<IServerInstance?> GetServer(string secret);
        public Task<IServerInstance?> GetServerByCode(string code);
        public Task<IServerInstance?> GetAvailablePublicServer(
            InvitePolicy invitePolicy,
            GameplayServerMode serverMode,
            SongSelectionMode songMode,
            GameplayServerControlSettings serverControlSettings,
            BeatmapDifficultyMask difficultyMask,
            GameplayModifiersMask modifiersMask,
            string songPackMasks,
            VersionRange versionRange);
    }
}
