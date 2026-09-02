using Photon.Pun;
using Parrot.client.Classes;
using Parrot.client.Notifications;
using System.Collections.Generic;
using UnityEngine;

namespace Parrot.client.Mods
{
    public class Safety
    {
        private const float ScanInterval = 0.1f;
        public static float TriggerRange = 0.12f;

        private static float nextScan;
        private static float nextNotify;
        private static float nextDebug;

        private static readonly List<Vector3> myButtons = new List<Vector3>();


        private static string originalName;

        public static void HideNameOnLeaderboard()
        {
            if (string.IsNullOrEmpty(originalName) && PhotonNetwork.LocalPlayer != null && !string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName))
                originalName = PhotonNetwork.LocalPlayer.NickName;

            ChangeName(" ");
        }

        public static void RestoreName()
        {
            if (!string.IsNullOrEmpty(originalName))
                ChangeName(originalName);
        }

        public static void ChangeName(string name)
        {
            try
            {
                PhotonNetwork.LocalPlayer.NickName = name;
                PhotonNetwork.NetworkingClient.NickName = name;
                PhotonNetwork.NickName = name;
            }
            catch
            {
            }
        }

        public static void AntiReportDisconnect()
        {
            if (NetworkSystem.Instance == null || !NetworkSystem.Instance.InRoom)
                return;

            if (Time.time < nextScan)
                return;
            nextScan = Time.time + ScanInterval;

            List<GorillaPlayerScoreboardLine> lines = GorillaScoreboardTotalUpdater.allScoreboardLines;
            if (lines == null || lines.Count == 0)
                return;

            int localActor = PhotonNetwork.LocalPlayer != null ? PhotonNetwork.LocalPlayer.ActorNumber : -1;
            VRRig localRig = VRRig.LocalRig;

            myButtons.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                GorillaPlayerScoreboardLine line = lines[i];
                if (line == null || line.reportButton == null)
                    continue;

                bool mine =
                    (localRig != null && line.playerVRRig == localRig) ||
                    (line.linePlayer != null && line.linePlayer == NetworkSystem.Instance.LocalPlayer) ||
                    (localActor > 0 && line.playerActorNumber == localActor);

                if (mine)
                    myButtons.Add(line.reportButton.transform.position);
            }

            float closest = 999f;

            if (myButtons.Count > 0)
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig == null || rig.isLocal || rig.isOfflineVRRig || rig.rightHandTransform == null || rig.leftHandTransform == null)
                        continue;

                    Vector3 rHand = rig.rightHandTransform.position;
                    Vector3 lHand = rig.leftHandTransform.position;

                    for (int b = 0; b < myButtons.Count; b++)
                    {
                        float distance = Mathf.Min(Vector3.Distance(rHand, myButtons[b]), Vector3.Distance(lHand, myButtons[b]));
                        if (distance < closest)
                            closest = distance;

                        if (distance < TriggerRange)
                        {
                            NetworkSystem.Instance.ReturnToSinglePlayer();

                            if (Time.time > nextNotify)
                            {
                                nextNotify = Time.time + 1f;
                                string name = RigManager.GetPlayerFromVRRig(rig)?.NickName ?? "Someone";
                                NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + name + " tried to report you — disconnected.");
                            }

                            return;
                        }
                    }
                }
            }

            if (Time.time > nextDebug)
            {
                nextDebug = Time.time + 2f;
                Debug.Log($"{PluginInfo.Name} // ANTI-REPORT: lines={lines.Count} myButtons={myButtons.Count} closestHand={(closest > 900f ? -1f : closest):0.00}");
            }
        }
    }
}
