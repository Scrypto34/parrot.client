using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Parrot.client.Notifications;
using UnityEngine;

namespace Parrot.client.Patches
{
    [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerLeftRoom")]
    public class LeavePatch : MonoBehaviour
    {
        private static void Prefix(Player otherPlayer)
        {
            if (otherPlayer != PhotonNetwork.LocalPlayer && otherPlayer != a)
            {
                NotifiLib.SendNotification("Room activity", otherPlayer.NickName + " left.");
                a = otherPlayer;
            }
        }

        private static Player a;
    }
}