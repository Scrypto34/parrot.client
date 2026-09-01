using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Parrot.client.Mods.Console;
using UnityEngine;

namespace Parrot.client.Classes
{
    public class SealSync : IOnEventCallback
    {
        private const byte EventCode = 181;
        private const string Marker = "PARROTSEAL";

        private static SealSync instance;

        public static void Register()
        {
            if (instance != null)
                return;

            instance = new SealSync();
            PhotonNetwork.AddCallbackTarget(instance);
        }

        public static void Fire(Vector3 position, Vector3 velocity)
        {
            if (!PhotonNetwork.InRoom)
                return;

            PhotonNetwork.RaiseEvent(
                EventCode,
                new object[] { Marker, position.x, position.y, position.z, velocity.x, velocity.y, velocity.z },
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendUnreliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventCode)
                return;

            if (!(photonEvent.CustomData is object[] data) || data.Length < 7 || data[0] as string != Marker)
                return;

            Vector3 position = new Vector3((float)data[1], (float)data[2], (float)data[3]);
            Vector3 velocity = new Vector3((float)data[4], (float)data[5], (float)data[6]);

            SealGun.SpawnProjectile(position, velocity);
        }
    }
}
