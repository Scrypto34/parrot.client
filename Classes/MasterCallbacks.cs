using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Parrot.client.Notifications;

namespace Parrot.client.Classes
{

    public class MasterCallbacks : IInRoomCallbacks
    {
        private static MasterCallbacks instance;

        public static void Register()
        {
            if (instance != null)
                return;

            instance = new MasterCallbacks();
            PhotonNetwork.AddCallbackTarget(instance);
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            if (newMasterClient != null && newMasterClient.IsLocal)
                NotifiLib.SendNotification("<color=grey>[</color><color=green>MASTER</color><color=grey>]</color> You are now the host - Kick Everyone will work.");
        }

        public void OnPlayerEnteredRoom(Player newPlayer) { }
        public void OnPlayerLeftRoom(Player otherPlayer) { }
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }
    }
}
