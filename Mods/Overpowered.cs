using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GrabPatches;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Parrot.client.Menu;
using Parrot.client.GunTools;
using Parrot.client.Mods;
using Parrot.client.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Parrot.client.Mods
{
    internal class Overpowered
    {

        public static void STumpkickall()
        {
            GorillaComputer.instance.OnGroupJoinButtonPress(0, GorillaComputer.instance.friendJoinCollider);
        }

        private static float grabCooldown;

        private static bool HasGrabbableHand(VRRig rig)
        {
            if (rig == null)
                return false;

            return rig.leftHandLink.CanBeGrabbed() || rig.rightHandLink.CanBeGrabbed();
        }

        private static void SetGrabPatch(bool state)
        {
            GrabPatches.GrabPatches.GrabPatch.enabled = state;

            if (!state)
                VRRig.LocalRig.enabled = true;
        }

        private static void GrabPlayer(VRRig rig, Vector3 position)
        {
            if (rig == null || rig.isLocal)
                return;

            if (!HasGrabbableHand(rig))
            {
                SetGrabPatch(false);
                VRRig.LocalRig.BreakHandLinks();
                return;
            }
            
            SetGrabPatch(true);

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = position;

            bool useLeftHand = rig.leftHandLink.CanBeGrabbed();

            var targetHand = useLeftHand ? rig.leftHandLink : rig.rightHandLink;
            var localHand = useLeftHand ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;

            if (targetHand.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
                return;

            if (grabCooldown <= Time.time)
            {
                VRRig.LocalRig.transform.position = rig.syncPos;
                localHand.TentacleTryCreateLink(targetHand);
            }

            grabCooldown = Time.time + 0.2f;
        }

        public static void GrabFlingGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.LockedPlayer == null)
                    return;

                Vector3 oldPos = GTPlayer.Instance.transform.position;

                Vector3 flingPosition = new Vector3(
                    UnityEngine.Random.Range(-250000f, 250000f),
                    250000f,
                    UnityEngine.Random.Range(-250000f, 250000f)
                );

                for (int i = 0; i < 5; i++)
                    GrabPlayer(Gunlib.LockedPlayer, flingPosition);

                GTPlayer.Instance.transform.position = oldPos;
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }, true);

            bool isHoldingInput =
                ControllerInputPoller.instance.rightControllerGripFloat > 0.5f ||
                ControllerInputPoller.instance.leftControllerGripFloat > 0.5f ||
                ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f ||
                ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

            if (!isHoldingInput && GrabPatches.GrabPatches.GrabPatch.enabled)
            {
                VRRig.LocalRig.BreakHandLinks();
                SetGrabPatch(false);
            }
        }

        public static void FlingUp(VRRig target)
        {
            if (target == null || target.isLocal || target.isOfflineVRRig)
                return;

            Vector3 oldPos = GTPlayer.Instance.transform.position;
            Vector3 flingPosition = target.transform.position + new Vector3(0f, 250000f, 0f);

            for (int i = 0; i < 5; i++)
                GrabPlayer(target, flingPosition);

            GTPlayer.Instance.transform.position = oldPos;
            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
        }

        public static void StopFling()
        {
            if (GrabPatches.GrabPatches.GrabPatch.enabled)
            {
                VRRig.LocalRig.BreakHandLinks();
                SetGrabPatch(false);
            }
        }

        public static void FlingGun()
        {
            Gunlib.StartBothGuns(() => FlingUp(Gunlib.LockedPlayer), true);

            bool isHoldingInput =
                ControllerInputPoller.instance.rightControllerGripFloat > 0.5f ||
                ControllerInputPoller.instance.leftControllerGripFloat > 0.5f ||
                ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f ||
                ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

            if (!isHoldingInput)
                StopFling();
        }

        private static float nextBarrel;
        private static bool braceletSpamState;

        public static void SpamBracelet()
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (GorillaTagger.Instance == null || GorillaTagger.Instance.myVRRig == null)
                return;

            braceletSpamState = !braceletSpamState;
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, braceletSpamState, false);
        }

        public static void BarrelFlingGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                VRRig target = Gunlib.LockedPlayer;
                if (target == null || target.isLocal || target.isOfflineVRRig)
                    return;

                if (PhotonNetwork.InRoom && Time.time >= nextBarrel)
                {
                    nextBarrel = Time.time + 0.08f;
                    RoomSystem.SendLaunchProjectile(
                        target.transform.position + Vector3.down * 0.25f,
                        Vector3.up * 45f,
                        RoomSystem.ProjectileSource.RightHand,
                        4, true, 0, 0, 0, 255);
                }

                FlingUp(target);
            }, true);

            bool isHoldingInput =
                ControllerInputPoller.instance.rightControllerGripFloat > 0.5f ||
                ControllerInputPoller.instance.leftControllerGripFloat > 0.5f ||
                ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f ||
                ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

            if (!isHoldingInput)
                StopFling();
        }

        public static void GrabFlingAll()
        {
            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isLocal || rig.isOfflineVRRig || !HasGrabbableHand(rig))
                    continue;

                Vector3 flingPosition = new Vector3(
                    UnityEngine.Random.value < 0.5f ? -95000f : 95000f,
                    95000f,
                    UnityEngine.Random.value < 0.5f ? -95000f : 95000f
                );

                GrabPlayer(rig, flingPosition);
            }

            bool isHoldingInput =
                ControllerInputPoller.instance.rightControllerGripFloat > 0.5f ||
                ControllerInputPoller.instance.leftControllerGripFloat > 0.5f ||
                ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f ||
                ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

            if (isHoldingInput || !GrabPatches.GrabPatches.GrabPatch.enabled)
                return;

            VRRig.LocalRig.BreakHandLinks();
            SetGrabPatch(false);
        }

        public static float hoverboarddelay = 0f;

        public static void HoverboardMinigun()
        {
            if (hoverboarddelay >= Time.time)
                return;

            if (ControllerInputPoller.instance.rightGrab)
            {
                FreeHoverboardManager.instance.SendDropBoardRPC(
                    GorillaTagger.Instance.rightHandTransform.position,
                    GorillaTagger.Instance.rightHandTransform.rotation,
                    GorillaTagger.Instance.rightHandTransform.forward * 30f,
                    Vector3.zero,
                    new Color(0, 0, 0));

                hoverboarddelay = Time.time + 0.5f;
            }

            if (ControllerInputPoller.instance.leftGrab)
            {
                FreeHoverboardManager.instance.SendDropBoardRPC(
                    GorillaTagger.Instance.leftHandTransform.position,
                    GorillaTagger.Instance.leftHandTransform.rotation,
                    GorillaTagger.Instance.leftHandTransform.forward * 30f,
                    Vector3.zero,
                    new Color(0, 0, 0));

                hoverboarddelay = Time.time + 0.5f;
            }
        }

        private static float waterDelay;

        public static void Watersplash()
        {
            if (Time.time < waterDelay)
                return;

            waterDelay = Time.time + 0.1f;

            if (!PhotonNetwork.InRoom)
                return;

            if (GorillaTagger.Instance == null ||
                GorillaTagger.Instance.myVRRig == null)
                return;

            bool rightGrip = ControllerInputPoller.instance != null &&
                             ControllerInputPoller.instance.rightGrab;

            bool leftGrip = ControllerInputPoller.instance != null &&
                            ControllerInputPoller.instance.leftGrab;

            if (rightGrip)
            {
                GorillaTagger.Instance.myVRRig.SendRPC(
                    "RPC_PlaySplashEffect",
                    RpcTarget.All,
                    GorillaTagger.Instance.rightHandTransform.position,
                    GorillaTagger.Instance.rightHandTransform.rotation,
                    100f,
                    100f,
                    true,
                    false);

                Safetyy.RPCProtection();
            }

            if (leftGrip)
            {
                GorillaTagger.Instance.myVRRig.SendRPC(
                    "RPC_PlaySplashEffect",
                    RpcTarget.All,
                    GorillaTagger.Instance.leftHandTransform.position,
                    GorillaTagger.Instance.leftHandTransform.rotation,
                    100f,
                    100f,
                    true,
                    false);

                Safetyy.RPCProtection();
            }
        }

        public static float waterdelay;

        public static void Watergun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (!PhotonNetwork.InRoom || Gunlib.LockedPlayer == null)
                    return;

                VRRig target = Gunlib.LockedPlayer;

                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position =
                    target.transform.position - new Vector3(0f, 1.9f, 0f);

                if (Time.time > waterdelay)
                {
                    waterdelay = Time.time + 0.3f;

                    GorillaTagger.Instance.myVRRig.SendRPC(
                        "RPC_PlaySplashEffect",
                        RpcTarget.All,
                        target.transform.position,
                        target.transform.rotation,
                        100f,
                        100f,
                        true,
                        false
                    );

                    Safetyy.RPCProtection();
                }
            }, true);

            VRRig.LocalRig.enabled = Gunlib.LockedPlayer == null;
        }

        public static void ElevatorKickGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.LockedPlayer == null ||
                    GRElevatorManager._instance == null)
                    return;

                PhotonView photonView = GRElevatorManager._instance.photonView;

                if (photonView == null)
                    return;

                photonView.RPC(
                    "RemoteActivateTeleport",
                    RpcTarget.All,
                    new object[]
                    {
                        GRElevatorManager._instance.currentLocation,
                        GRElevatorManager.ElevatorLocation.GhostReactor,
                        GRElevatorManager.LowestActorNumberInElevator()
                        });
            }, true);
        }

        public static void ElevatorKickAll()
        {
            if (GRElevatorManager._instance == null)
                return;

            PhotonView photonView = GRElevatorManager._instance.photonView;

            if (photonView == null)
                return;

            photonView.RPC(
                "RemoteActivateTeleport",
                RpcTarget.Others,
                new object[]
                {
            GRElevatorManager._instance.currentLocation,
            GRElevatorManager.ElevatorLocation.GhostReactor,
            GRElevatorManager.LowestActorNumberInElevator()
                });
        }
        
   
        

        private static float LagDelay;

        public static void GuardianAll()
        {
            if (NetworkSystem.Instance.IsMasterClient)
            {
                int i = 0;
                foreach (var gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers.Where(gorillaGuardianZoneManager => gorillaGuardianZoneManager.enabled && gorillaGuardianZoneManager.IsZoneValid()))
                {
                    gorillaGuardianZoneManager.SetGuardian(PhotonNetwork.PlayerList[i]);
                    i++;
                }
            }
        }

        public static void GhostMonkey()
        {
            Fun.GhostMonkey();
        }


 //  public static void GetFP()
 //  {
 //      try
 //      {
 //          CosmeticsController cc = CosmeticsController.instance;
 //          if (cc == null || cc.v2_allCosmetics == null)
 //          {
 //              NotifiLib.SendNotification("<color=grey>[</color><color=red>FP</color><color=grey>]</color> Cosmetics not loaded yet, try again in a moment.");
 //              return;
 //          }

 //          string playFabID = null;
 //          string displayName = null;
 //          foreach (var info in cc.v2_allCosmetics)
 //          {
 //              if (string.IsNullOrEmpty(info.displayName) || string.IsNullOrEmpty(info.playFabID))
 //                  continue;
 //              if (info.displayName.ToLowerInvariant().Contains("finger paint"))
 //              {
 //                  playFabID = info.playFabID;
 //                  displayName = info.displayName;
 //                  break;
 //              }
 //          }

 //          if (playFabID == null)
 //          {
 //              NotifiLib.SendNotification("<color=grey>[</color><color=red>FP</color><color=grey>]</color> Finger Painter badge not found.");
 //              return;
 //          }

 //          cc.AddTempUnlockToWardrobe(playFabID);
 //          try { cc.UpdateWardrobeModelsAndButtons(); } catch { }
 //          NotifiLib.SendNotification("<color=grey>[</color><color=green>FP</color><color=grey>]</color> " + displayName + " added to your wardrobe - go wear it like a normal badge.");
 //      }
 //      catch (Exception exc)
 //      {
 //          NotifiLib.SendNotification("<color=grey>[</color><color=red>FP</color><color=grey>]</color> Failed: " + exc.Message);
 //      }
 //  }

        private static float guardianDelay;

        public static void GuardianGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                VRRig target = Gunlib.LockedPlayer;
                if (target == null || target.isLocal || target.Creator == null)
                    return;

                if (Time.time <= guardianDelay)
                    return;
                guardianDelay = Time.time + 0.1f;

                try
                {

                    GorillaGuardianZoneManager zone = UnityEngine.Object.FindObjectOfType<GorillaGuardianZoneManager>();
                    if (zone != null)
                        zone.SetGuardian(target.Creator);
                }
                catch { }
            }, true);
        }

        public static void UnguardianGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                VRRig target = Gunlib.LockedPlayer;
                if (target == null || target.isLocal || target.Creator == null)
                    return;

                if (Time.time <= guardianDelay)
                    return;
                guardianDelay = Time.time + 0.1f;

                try
                {

                    foreach (var zone in GorillaGuardianZoneManager.zoneManagers
                        .Where(z => z.enabled && z.IsZoneValid() && z.CurrentGuardian == target.Creator))
                        zone.SetGuardian(null);
                }
                catch { }
            }, true);
        }

        private static float kickDelay;
        private static string lastKickTarget;

        public static void KickGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                VRRig target = Gunlib.LockedPlayer;

                if (!PhotonNetwork.InRoom || target == null || target.Creator == null
                    || target.isLocal || target.isOfflineVRRig
                    || target.Creator.UserId == PhotonNetwork.LocalPlayer.UserId)
                    return;

                if (Time.time < kickDelay)
                    return;
                kickDelay = Time.time + 1.5f;

                Parrot.client.Classes.KickSync.Kick(target.Creator.UserId);

                Player player = PhotonNetwork.CurrentRoom?.GetPlayer(target.Creator.ActorNumber);
                if (PhotonNetwork.IsMasterClient)
                {
                    if (player != null)
                        PhotonNetwork.CloseConnection(player);
                }
                else
                {
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                }

                if (lastKickTarget != target.Creator.UserId)
                {
                    lastKickTarget = target.Creator.UserId;
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>KICK</color><color=grey>]</color> Kicked " + target.Creator.NickName + ".");
                }
            }, true);
        }

        public static void FlickTagGun()
        {

            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.nray.collider == null)
                    return;

                Transform hand = GTPlayer.Instance.GetControllerTransform(false);
                Vector3 body = GorillaTagger.Instance.bodyCollider.transform.position;
                Vector3 target = Gunlib.nray.point;

                if (Vector3.Distance(target, body) > 4f)
                    target = body + (target - body).normalized * 4f;

                hand.position = target;
            }, false);
        }

        public static void LagAll()
        {
            if (Time.time > LagDelay)
            {
                for (int i = 0; i < 900; i++)
                {
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(3, new Hashtable() { }, new RaiseEventOptions() { Receivers = ReceiverGroup.Others }, SendOptions.SendUnreliable);
                }
                Safetyy.RPCProtection();
                LagDelay = Time.time + 2.2f;
            }
        }



        public static void LagOnTouch()
        {
            if (Time.time > LagDelay)
            {
                foreach (VRRig vrrig in VRRigCache.ActiveRigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig &&
                        (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, vrrig.headMesh.transform.position) < 0.25f ||
                         Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, vrrig.headMesh.transform.position) < 0.25f ||
                         Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, vrrig.bodyTransform.position) < 0.25f ||
                         Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, vrrig.bodyTransform.position) < 0.25f))
                    {
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(
                            3,
                            new Hashtable(),
                            new RaiseEventOptions()
                            {
                                TargetActors = new int[] { vrrig.Creator.ActorNumber }
                            },
                            SendOptions.SendUnreliable
                        );

                        Safetyy.RPCProtection();
                    }
                }

                LagDelay = Time.time + 2.2f;
            }
        }

    }
}
