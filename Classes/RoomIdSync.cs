using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
using UnityEngine;

namespace Parrot.client.Classes
{

    public class RoomIdSync : IOnEventCallback
    {
        private const byte EventCode = 177;
        private const string Marker = "SCRY_ROOMID";

        private static readonly FieldInfo NameField =
            typeof(RoomInfo).GetField("name", BindingFlags.NonPublic | BindingFlags.Instance);

        private static RoomIdSync instance;

        private static string desired;
        private static float nextRebroadcast;

        public static void Register()
        {
            if (instance != null)
                return;

            instance = new RoomIdSync();
            PhotonNetwork.AddCallbackTarget(instance);
        }

        public static void Broadcast(string roomId)
        {
            desired = roomId;
            ApplyLocal(roomId);
            SendToOthers(roomId);
        }

        public static void Tick()
        {
            if (string.IsNullOrEmpty(desired) || !PhotonNetwork.InRoom)
                return;

            ApplyLocal(desired);

            if (Time.time >= nextRebroadcast)
            {
                nextRebroadcast = Time.time + 3f;
                SendToOthers(desired);
            }
        }

        private static void SendToOthers(string roomId)
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.RaiseEvent(
                    EventCode,
                    new object[] { Marker, roomId },
                    new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                    SendOptions.SendReliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventCode)
                return;

            if (!OwnerList.IsOwnerActor(photonEvent.Sender))
                return;

            if (photonEvent.CustomData is object[] data && data.Length >= 2
                && data[0] as string == Marker && data[1] is string roomId)
                ApplyLocal(roomId);
        }

        private static void ApplyLocal(string roomId)
        {
            if (NameField != null && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
                NameField.SetValue(PhotonNetwork.CurrentRoom, roomId);
        }
    }
}
