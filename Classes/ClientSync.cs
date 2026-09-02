using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Parrot.client.Menu;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Parrot.client.Classes
{
    public class ClientSync : IOnEventCallback, IInRoomCallbacks
    {
        private const byte EventCode = 178;
        private const string Marker = "PARROTSYNC";

        public class PlayerState
        {
            public string name = "?";
            public string theme = "?";
            public List<string> mods = new List<string>();
            public int crownColor = 0;
            public bool menuOpen = false;
            public int pageNumber = 0;
            public Color bgColor = new Color(0.1f, 0.1f, 0.15f);
            public Color btnDisabled = new Color(0.1f, 0.1f, 0.15f);
            public Color btnEnabled = new Color(0.15f, 0.15f, 0.3f);
            public Color textDisabled = new Color(0.5f, 0.5f, 0.7f);
            public Color textEnabled = Color.white;
            public List<string> menuLabels = new List<string>();
            public List<bool> menuEnabled = new List<bool>();
        }

        public static readonly Dictionary<int, PlayerState> states = new Dictionary<int, PlayerState>();

        private static ClientSync instance;
        private static float nextBroadcast;

        public static void Register()
        {
            if (instance != null)
                return;

            instance = new ClientSync();
            PhotonNetwork.AddCallbackTarget(instance);
        }

        public static void Tick()
        {
            if (Time.time < nextBroadcast)
                return;

            nextBroadcast = Time.time + 0.5f;
            BroadcastNow();
        }

        public static void BroadcastNow()
        {
            if (!PhotonNetwork.InRoom)
                return;

            string theme = ThemeChanger.themes[ThemeChanger.currentThemeIndex].name;

            List<string> enabled = new List<string>();
            foreach (ButtonInfo[] category in Buttons.buttons)
                foreach (ButtonInfo button in category)
                    if (button.enabled)
                        enabled.Add(button.buttonText);

            bool menuOpen = Main.menu != null;
            Color bg = Settings.backgroundColor.GetCurrentColor();
            Color bD = Settings.buttonColors[0].GetCurrentColor();
            Color bE = Settings.buttonColors[1].GetCurrentColor();
            Color tD = Settings.textColors[0];
            Color tE = Settings.textColors[1];

            List<string> labels = new List<string>();
            if (menuOpen)
            {
                foreach (ButtonInfo b in Main.VisibleButtons(Main.currentCategory)
                    .Skip(Main.pageNumber * Settings.buttonsPerPage).Take(Settings.buttonsPerPage))
                {
                    string label = b.overlapText ?? b.buttonText;
                    labels.Add((b.enabled ? "E|" : "D|") + label);
                }
            }

            object[] payload = new object[]
            {
                Marker, theme, enabled.ToArray(), AdminTags.crownColorIndex,
                menuOpen, bg.r, bg.g, bg.b, bD.r, bD.g, bD.b, bE.r, bE.g, bE.b,
                tD.r, tD.g, tD.b, tE.r, tE.g, tE.b, Main.pageNumber, labels.ToArray()
            };
            PhotonNetwork.RaiseEvent(EventCode, payload,
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendUnreliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != EventCode)
                return;

            if (!(photonEvent.CustomData is object[] data) || data.Length < 3 || data[0] as string != Marker)
                return;

            PlayerState state = new PlayerState();
            state.theme = data[1] as string ?? "?";

            if (data[2] is object[] mods)
                state.mods = mods.Select(m => m as string).Where(m => m != null).ToList();

            if (data.Length >= 4)
                state.crownColor = System.Convert.ToInt32(data[3]);

            if (data.Length >= 22)
            {
                state.menuOpen = data[4] is bool b && b;
                state.bgColor = new Color((float)data[5], (float)data[6], (float)data[7]);
                state.btnDisabled = new Color((float)data[8], (float)data[9], (float)data[10]);
                state.btnEnabled = new Color((float)data[11], (float)data[12], (float)data[13]);
                state.textDisabled = new Color((float)data[14], (float)data[15], (float)data[16]);
                state.textEnabled = new Color((float)data[17], (float)data[18], (float)data[19]);
                state.pageNumber = System.Convert.ToInt32(data[20]);

                state.menuLabels = new List<string>();
                state.menuEnabled = new List<bool>();
                if (data[21] is object[] labels)
                {
                    foreach (object o in labels)
                    {
                        string s = o as string;
                        if (s == null)
                            continue;
                        state.menuEnabled.Add(s.StartsWith("E|"));
                        state.menuLabels.Add(s.Length > 2 ? s.Substring(2) : s);
                    }
                }
            }

            Player sender = PhotonNetwork.CurrentRoom?.GetPlayer(photonEvent.Sender);
            state.name = sender?.NickName ?? "?";

            states[photonEvent.Sender] = state;
        }

        public void OnPlayerEnteredRoom(Player newPlayer) => BroadcastNow();
        public void OnPlayerLeftRoom(Player otherPlayer) => states.Remove(otherPlayer.ActorNumber);
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }
        public void OnMasterClientSwitched(Player newMasterClient) { }
    }
}
