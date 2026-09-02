using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Parrot.client.Notifications;
using UnityEngine;

namespace Parrot.client.Patches
{
    [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerEnteredRoom")]
    public class JoinPatch : MonoBehaviour
    {
        private static void Prefix(Player newPlayer)
        {
            if (newPlayer != oldnewplayer)
            {
                NotifiLib.SendNotification("Room activity", newPlayer.NickName + " joined.");
                oldnewplayer = newPlayer;
            }
        }

        private static Player oldnewplayer;
    }
}