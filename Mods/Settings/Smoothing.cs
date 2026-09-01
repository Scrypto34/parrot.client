using UnityEngine;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public class Smoothing
    {
        public static int index = 0;

        public static readonly string[] Names = { "Off", "Low", "Medium", "High" };
        private static readonly float[] Speeds = { 0f, 20f, 12f, 6f };

        public static float Speed => Speeds[Mathf.Clamp(index, 0, Speeds.Length - 1)];

        public static void Cycle()
        {
            index = (index + 1) % Names.Length;
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void RefreshLabel()
        {
            var button = GetIndex("Menu Smoothing");
            if (button != null)
                button.overlapText = "Menu Smoothing: " + Names[Mathf.Clamp(index, 0, Names.Length - 1)];
        }
    }
}
