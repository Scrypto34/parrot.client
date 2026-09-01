using Parrot.client.Classes;
using UnityEngine;

namespace Parrot.client
{
    public class Settings
    {

        public static ExtGradient backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.darkGray) };
        public static ExtGradient[] buttonColors = new ExtGradient[]
        {
            new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) },
            new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.gray) }
        };
        public static Color[] textColors = new Color[]
        {
            Color.white,
            Color.white
        };

        public static Font currentFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font ?? Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf") as Font;

        public static bool fpsCounter = true;
        public static bool disconnectButton = true;
        public static bool rightHanded;
        public static bool disableNotifications;
        public static bool roundedCorners;
        public static bool rainbowOutline;
        public static bool buttonAnimations;

        public static KeyCode keyboardButton = KeyCode.Q;

        public static Vector3 menuSize = new Vector3(0.1f, 1f, 1f);
        public static int buttonsPerPage = 8;

        public static float gradientSpeed = 0.5f;
    }
}
