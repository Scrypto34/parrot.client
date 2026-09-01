using GorillaGameModes;
using Parrot.client.GunTools;
using Parrot.client.Notifications;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Parrot.client.Mods
{
    internal class Advantage
    {
        private const float tagCooldown = 0.25f;

        public static float tagAuraDistance = 2.5f;
        private static float lastTagAuraTime;

        private static float lastTagGunTime;
        private static float lastTagSelfTime;
        private static bool tagGunHolding;
        private static bool tagSelfHolding;

        public static void TagGun()
        {
            VRRig rig = GorillaTagger.Instance.offlineVRRig;

            Gunlib.StartBothGuns(() =>
            {
                VRRig target = Gunlib.LockedPlayer;
                if (target == null || target == rig || target.Creator == null)
                    return;

                bool firstShot = !tagGunHolding;

                rig.enabled = false;
                tagGunHolding = true;
                rig.transform.position = target.transform.position;

                if (Time.time - lastTagGunTime < tagCooldown)
                    return;
                lastTagGunTime = Time.time;

                try
                {
                    GameMode.ReportTag(target.Creator);

                    if (firstShot)
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>TAG</color><color=grey>]</color> Tagged " + target.Creator.NickName + ".");

                        if (!IsInfected(rig))
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>WARN</color><color=grey>]</color> You are not it, the game will ignore this.");
                    }
                }
                catch (Exception exc)
                {
                    if (firstShot)
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Tag failed: " + exc.Message);
                }
            }, true);

            if (Gunlib.LockedPlayer == null)
                ReleaseTagGun();
        }

        public static void ReleaseTagGun()
        {
            if (!tagGunHolding)
                return;

            GorillaTagger.Instance.offlineVRRig.enabled = true;
            tagGunHolding = false;
        }

        public static void TagAura()
        {
            if (!IsInfected(GorillaTagger.Instance.offlineVRRig))
                return;

            if (Time.time - lastTagAuraTime < tagCooldown)
                return;

            Vector3 myPos = GorillaTagger.Instance.bodyCollider.transform.position;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isOfflineVRRig || rig.isLocal || rig.Creator == null)
                    continue;

                if (IsInfected(rig))
                    continue;

                if (Vector3.Distance(myPos, rig.transform.position) > tagAuraDistance)
                    continue;

                lastTagAuraTime = Time.time;
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position;
                GameMode.ReportTag(rig.Creator);
                break;
            }
        }

        public static void TagAll()
        {
            if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
                return;

            foreach (VRRig rig in VRRigCache.m_activeRigs)
            {
                if (rig == GorillaTagger.Instance.offlineVRRig)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    continue;
                }

                if (rig.mainSkin.material.name.Contains("fected"))
                    continue;

                GorillaTagger.Instance.offlineVRRig.enabled = true;
                GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position;
                GameMode.ReportTag(rig.Creator);
            }
        }

        public static void TagSelf()
        {
            VRRig rig = GorillaTagger.Instance.offlineVRRig;

            if (IsInfected(rig))
            {
                ReleaseTagSelf();
                return;
            }

            VRRig target = null;
            float closest = float.MaxValue;
            Vector3 myPos = GorillaTagger.Instance.bodyCollider.transform.position;

            foreach (VRRig player in VRRigCache.ActiveRigs)
            {
                if (player == null || player == rig || player.isOfflineVRRig || player.isLocal)
                    continue;

                if (!IsInfected(player) || player.Creator == null)
                    continue;

                float distance = Vector3.Distance(myPos, player.transform.position);
                if (distance < closest)
                {
                    closest = distance;
                    target = player;
                }
            }

            if (target == null)
            {
                ReleaseTagSelf();
                return;
            }

            rig.enabled = false;
            tagSelfHolding = true;
            rig.transform.position = target.leftHandTransform.position;

            if (Time.time - lastTagSelfTime < tagCooldown)
                return;
            lastTagSelfTime = Time.time;

            try { GameMode.ReportTag(NetworkSystem.Instance.LocalPlayer); } catch { }
        }

        public static void ReleaseTagSelf()
        {
            if (!tagSelfHolding)
                return;

            GorillaTagger.Instance.offlineVRRig.enabled = true;
            tagSelfHolding = false;
        }

        private static bool IsInfected(VRRig rig) =>
            rig != null
            && rig.mainSkin != null
            && rig.mainSkin.material != null
            && rig.mainSkin.material.name.Contains("fected");

        public static void UntagSelf()
        {
            try
            {
                if (!(GorillaGameManager.instance is GorillaTagManager tm))
                    return;

                NetPlayer me = NetworkSystem.Instance.LocalPlayer;
                if (tm.currentInfected != null && me != null && tm.currentInfected.Contains(me))
                {
                    tm.currentInfected.Remove(me);
                    try { tm.UpdateInfectionState(); } catch { }
                }
            }
            catch { }
        }

        public static void NoTagOnJoin()
        {
            PlayerPrefs.SetString("tutorial", "nope");
            PlayerPrefs.SetString("didTutorial", "nope");
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("didTutorial", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash, null, null);
            PlayerPrefs.Save();
        }

    }
}
