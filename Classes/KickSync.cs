using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Parrot.client.Classes
{
    public class KickSync : IOnEventCallback
    {
        private const byte EventCode = 179;
        private const string Marker = "PARROTKICK";

        private static KickSync instance;

        public static void Register()
        {
            if (instance != null)
                return;

            instance = new KickSync();
            PhotonNetwork.AddCallbackTarget(instance);
        }

        public static void Kick(string userId)
        {
            if (!PhotonNetwork.InRoom || string.IsNullOrEmpty(userId))
                return;

            PhotonNetwork.RaiseEvent(
                EventCode,
                new object[] { Marker, userId },
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendReliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventCode)
                return;

            if (!(photonEvent.CustomData is object[] data) || data.Length < 2 || data[0] as string != Marker)
                return;

            string target = data[1] as string;
            if (!string.IsNullOrEmpty(target) && target == PhotonNetwork.LocalPlayer?.UserId)
            {
                if (NetworkSystem.Instance != null)
                    NetworkSystem.Instance.ReturnToSinglePlayer();
            }
        }
    }
}
