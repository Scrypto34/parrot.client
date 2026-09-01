using UnityEngine;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public class MenuScale
    {
        public static int index = 1;

        public static readonly string[] Names = { "Small", "Normal", "Big", "Bigger", "Huge" };
        private static readonly float[] Mult = { 0.65f, 1f, 1.4f, 1.85f, 2.4f };

        public static float Multiplier => Mult[Mathf.Clamp(index, 0, Mult.Length - 1)];

        public static void Cycle()
        {
            index = (index + 1) % Names.Length;
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
            RecreateMenu();
        }

        public static void RefreshLabel()
        {
            var button = GetIndex("Menu Size");
            if (button != null)
                button.overlapText = "Menu Size: " + Names[Mathf.Clamp(index, 0, Names.Length - 1)];
        }
    }
}
