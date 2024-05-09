
using BeatTogether.Core.Abstractions;

namespace BeatTogether.Core.ServerMessaging
{
    //we will have 3 parts.
    //Api is a stage1, it handles only gamelift requests.
    //node service is a stage1 and a stage2, it handles the master servers list of servers, extra api requests, and all the node related things
    //Dedi server is a stage2, it handles all instance logic

    public interface ILayer1 //Layer 2 calls these to inform layer 1. This is contained in layer 1. If layer2 can find this on layer1, then attatch then to events.
    {
        public void PlayerJoined(IServerInstance instance, IPlayer player);
        public void PlayerLeft(IServerInstance instance, IPlayer player);
        public void InstanceClosed(IServerInstance instance);
        public void InstanceCreated(IServerInstance instance);
        public void InstanceConfigChanged(IServerInstance instance);
        public void InstanceStateChanged(IServerInstance instance);
        public void InstancePlayersChanged(IServerInstance instance);
    }
}
