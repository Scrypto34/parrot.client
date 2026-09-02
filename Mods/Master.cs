using ExitGames.Client.Photon;
using GorillaLocomotion;
using Parrot.client.GunTools;
using Parrot.client.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parrot.client.Mods
{
    internal class Master
    {
        public static void BecomeMaster()
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=green>MASTER</color><color=grey>]</color> You are already the host.");
                return;
            }

            bool sent = PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            NotifiLib.SendNotification(sent
                ? "<color=grey>[</color><color=yellow>MASTER</color><color=grey>]</color> Requesting host... wait for the confirmation."
                : "<color=grey>[</color><color=red>MASTER</color><color=grey>]</color> Could not request host (not connected).");
        }

        public static void KickEveryone()
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>MASTER</color><color=grey>]</color> Become host first.");
                return;
            }

            int kicked = 0;
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                PhotonNetwork.CloseConnection(player);
                kicked++;
            }

            NotifiLib.SendNotification("<color=grey>[</color><color=red>MASTER</color><color=grey>]</color> Kicked everyone (" + kicked + ").");
        }

        public static void GreyScreen()
        {
            if (GreyZoneManager.Instance == null) return;

            if (!PhotonNetwork.IsMasterClient) return;

            GreyZoneManager.Instance.ActivateGreyZoneAuthority();

        }

        public static void DisableGreyScreen()
        {
            if (GreyZoneManager.Instance == null) return;

            if (!PhotonNetwork.IsMasterClient) return;

            GTPlayer.Instance?.UnsetGravityOverride(GreyZoneManager.Instance);

            GreyZoneManager.Instance.DeactivateGreyZoneAuthority();
        }

        public static void ViberateGun()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.LockedPlayer == null)
                    return;

                PhotonNetwork.RaiseEvent(3,
                    new object[]
                    {
                    PhotonNetwork.ServerTimestamp,
                    (byte)2,
                    new object[] { 1 }
                    },
                    new RaiseEventOptions
                    {
                        TargetActors = new int[]
                    {
                        Gunlib.LockedPlayer.Creator.ActorNumber
                    }
                    },
                SendOptions.SendUnreliable);
            }, true);
        }

        public static void ViberateAll()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            PhotonNetwork.RaiseEvent(3,
                new object[]
                {
                PhotonNetwork.ServerTimestamp,
                (byte)2,
                new object[] { 1 }
                },
                new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All
                },
                SendOptions.SendUnreliable);
        }

        public static void UntagSelf()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
                gorillaTagManager.currentInfected.Remove(PhotonNetwork.LocalPlayer);
            }
        }
        public static void UntagAll()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
                    gorillaTagManager.currentInfected.Remove(player);
                }
            }
        }
        public static void ForceTagLag()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
                gorillaTagManager.tagCoolDown = 200000;
            }
        }
        public static void BreakElevator()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.RemoveInstantiatedGO(GRElevatorManager._instance.gameObject, false);
            }
        }
    }
}
