using UnityEngine;

namespace Parrot.client.Mods.Settings
{
    public class Notifs
    {
        public static int colorIndex = 0;

        public static readonly string[] Names = { "Purple", "Blue", "Green", "Red", "Pink", "Cyan", "Orange", "White" };

        private static readonly Color[] Colors =
        {
            new Color(0.62f, 0.34f, 0.98f, 1f),
            new Color(0.35f, 0.55f, 1f, 1f),
            new Color(0.30f, 0.85f, 0.45f, 1f),
            new Color(1f, 0.35f, 0.35f, 1f),
            new Color(1f, 0.45f, 0.75f, 1f),
            new Color(0.25f, 0.85f, 0.95f, 1f),
            new Color(1f, 0.6f, 0.2f, 1f),
            Color.white
        };

        public static void Apply()
        {
            Parrot.client.Notifications.NotifiLib.AccentColor = Colors[Mathf.Clamp(colorIndex, 0, Colors.Length - 1)];
        }

        public static void CycleColor()
        {
            colorIndex = (colorIndex + 1) % Names.Length;
            Apply();
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void BackColor()
        {
            colorIndex = (colorIndex - 1 + Names.Length) % Names.Length;
            Apply();
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void RefreshLabel()
        {
            var button = Menu.Main.GetIndex("Notification Color");
            if (button != null)
                button.overlapText = "Notification Color: " + Names[Mathf.Clamp(colorIndex, 0, Names.Length - 1)];
        }
    }
}
