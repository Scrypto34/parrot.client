using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Parrot.client.Classes
{
    public class RigManager
    {
        private static VRRigCollection _cachedCollection;
        public static List<VRRig> vrrigs
        {
            get
            {
                if (_cachedCollection == null)
                    _cachedCollection = Object.FindFirstObjectByType<VRRigCollection>();
                if (_cachedCollection == null) return new List<VRRig>();
                return _cachedCollection.containedRigs.Where(rc => rc.vrrig != null).Select(rc => rc.vrrig).ToList();
            }
        }

        public static VRRig GetVRRigFromPlayer(Player p) =>
            GorillaGameManager.instance.FindPlayerVRRig(p);

        public static VRRig GetRandomVRRig(bool includeSelf)
        {
            List<VRRig> rigs = vrrigs;
            if (rigs.Count == 0) return null;
            List<VRRig> candidates = includeSelf ? rigs : rigs.Where(r => r != VRRig.LocalRig).ToList();
            if (candidates.Count == 0) return null;
            return candidates[Random.Range(0, candidates.Count)];
        }

        public static VRRig GetClosestVRRig()
        {
            float num = float.MaxValue;
            VRRig outRig = null;
            foreach (VRRig vrrig in vrrigs)
            {
                if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position) < num)
                {
                    num = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position);
                    outRig = vrrig;
                }
            }
            return outRig;
        }

        public static PhotonView GetPhotonViewFromVRRig(VRRig p) =>
            (PhotonView)Traverse.Create(p).Field("photonView").GetValue();

        public static Player GetRandomPlayer(bool includeSelf)
        {
            if (includeSelf)
                return PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length - 1)];
            else
                return PhotonNetwork.PlayerListOthers[Random.Range(0, PhotonNetwork.PlayerListOthers.Length - 1)];
        }

        public static Player GetPlayerFromVRRig(VRRig p) =>
            GetPhotonViewFromVRRig(p).Owner;

        public static Player GetPlayerFromID(string id)
        {
            Player found = null;
            foreach (Player target in PhotonNetwork.PlayerList)
            {
                if (target.UserId == id)
                {
                    found = target;
                    break;
                }
            }
            return found;
        }

        public static Color GetPlayerColor(VRRig Player)
        {
            if (Player.bodyRenderer.cosmeticBodyType == GorillaBodyType.Skeleton)
                return Color.green;

            switch (Player.setMatIndex)
            {
                case 1:
                    return Color.red;
                case 2:
                case 11:
                    return new Color32(255, 128, 0, 255);
                case 3:
                case 7:
                    return Color.blue;
                case 12:
                    return Color.green;
                default:
                    return Player.playerColor;
            }
        }
    }
}