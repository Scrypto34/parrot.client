using ExitGames.Client.Photon;
using GorillaNetworking;
using HarmonyLib;
using Mono.Cecil.Cil;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
namespace Parrot.client.Mods
{

    public class Rooms
    {
        public static void LobbyHop()
        {
            try
            
            {
                PhotonNetworkController pc = PhotonNetworkController.Instance;
                if (pc == null)
                    return;

                GorillaNetworkJoinTrigger trigger = pc.currentJoinTrigger;
                if (trigger == null && pc.allJoinTriggers != null && pc.allJoinTriggers.Count > 0)
                    trigger = pc.allJoinTriggers[0];

                if (trigger == null)
                    return;

                pc.AttemptToJoinPublicRoom(trigger, GorillaNetworking.JoinType.Solo, null, false);
            }
            catch { }
        }

        public static void JoinCode(string code)
        {
            if (PhotonNetworkController.Instance == null || string.IsNullOrEmpty(code))
                return;

            bool inParty = GorillaTagScripts.FriendshipGroupDetection.Instance != null
                && GorillaTagScripts.FriendshipGroupDetection.Instance.IsInParty;

            GorillaNetworking.JoinType joinType = inParty
                ? GorillaNetworking.JoinType.ForceJoinWithParty
                : GorillaNetworking.JoinType.Solo;

            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(code, joinType);
        }



        #region SetRoomID's
        public static void SetRoomIdToFemboy()
        {
            if (!NetworkSystem.Instance.InRoom) return;
            Classes.RoomIdSync.Broadcast("FEMBOY :3");
        }
        public static void SetRoomIdToUwu()
        {
            if (!NetworkSystem.Instance.InRoom) return;
            Classes.RoomIdSync.Broadcast("UWU");
        }
        public static void SetRoomIdToScryptoIsaBoyKisser()
        {
            if (!NetworkSystem.Instance.InRoom) return;
            Classes.RoomIdSync.Broadcast("SCRYPTO IS A BOYKISSER");
        }
        public static void SetRoomIdToFemboyFurry()
        {
            if (!NetworkSystem.Instance.InRoom) return;
            Classes.RoomIdSync.Broadcast("FEMBOY FURRY");
        }

        public static void SetRoomIdToParrotStinks()
        {
            if (!NetworkSystem.Instance.InRoom) return;
            Classes.RoomIdSync.Broadcast("PARROT STINKS");
        }



    }
}
#endregion SetRoomID's


