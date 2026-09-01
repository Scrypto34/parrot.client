using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Parrot.client.Mods.Console;
using System.Collections.Generic;
using UnityEngine;

namespace Parrot.client.Classes
{
    public class SahurSync : IOnEventCallback
    {
        private const byte EventCode = 180;
        private const string Marker = "PARROTSAHUR";
        private const float HeartbeatRate = 2f;
        private const float Timeout = 6f;

        private static SahurSync instance;

        private static bool localActive;
        private static float nextBroadcast;

        private static readonly Dictionary<int, GameObject> remote = new Dictionary<int, GameObject>();
        private static readonly Dictionary<int, float> lastSeen = new Dictionary<int, float>();

        public static void Register()
        {
            if (instance != null)
                return;

            instance = new SahurSync();
            PhotonNetwork.AddCallbackTarget(instance);
        }

        public static void SetActive(bool active)
        {
            if (!active)
            {
                if (localActive)
                {
                    localActive = false;
                    Broadcast(false);
                }
                return;
            }

            localActive = true;
            if (PhotonNetwork.InRoom && Time.time >= nextBroadcast)
            {
                nextBroadcast = Time.time + HeartbeatRate;
                Broadcast(true);
            }
        }

        private static void Broadcast(bool active)
        {
            if (!PhotonNetwork.InRoom)
                return;

            PhotonNetwork.RaiseEvent(
                EventCode,
                new object[] { Marker, active },
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendReliable);
        }

        public static void Tick()
        {
            if (remote.Count == 0)
                return;

            List<int> drop = null;
            foreach (KeyValuePair<int, GameObject> pair in remote)
            {
                bool timedOut = !lastSeen.TryGetValue(pair.Key, out float seen) || Time.time - seen > Timeout;
                if (timedOut || pair.Value == null)
                {
                    if (pair.Value != null)
                        Object.Destroy(pair.Value);

                    (drop ??= new List<int>()).Add(pair.Key);
                }
            }

            if (drop != null)
            {
                foreach (int actor in drop)
                {
                    remote.Remove(actor);
                    lastSeen.Remove(actor);
                }
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventCode)
                return;

            if (!(photonEvent.CustomData is object[] data) || data.Length < 2 || data[0] as string != Marker)
                return;

            int actor = photonEvent.Sender;
            bool active = data[1] is bool b && b;

            if (active)
                SpawnRemote(actor);
            else
                DespawnRemote(actor);
        }

        private static void SpawnRemote(int actor)
        {
            lastSeen[actor] = Time.time;

            if (remote.TryGetValue(actor, out GameObject existing) && existing != null)
                return;

            Player owner = null;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == actor)
                {
                    owner = p;
                    break;
                }
            }

            if (owner == null)
                return;

            VRRig rig = RigManager.GetVRRigFromPlayer(owner);
            if (rig == null || rig.rightHandTransform == null)
                return;

            remote[actor] = Sahur.Attach(rig.rightHandTransform);
        }

        private static void DespawnRemote(int actor)
        {
            if (remote.TryGetValue(actor, out GameObject obj) && obj != null)
                Object.Destroy(obj);

            remote.Remove(actor);
            lastSeen.Remove(actor);
        }
    }
}
