using UnityEngine;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public class ArrowStyle
    {
        public static int index = 0;

        public static readonly string[] Names = { "Default", "Side", "Chevron" };

        public static void Cycle()
        {
            index = (index + 1) % Names.Length;
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
            RecreateMenu();
        }

        public static void Back()
        {
            index = (index - 1 + Names.Length) % Names.Length;
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
            RecreateMenu();
        }

        public static void RefreshLabel()
        {
            var button = GetIndex("Arrow Style");
            if (button != null)
                button.overlapText = "Arrow Style: " + Names[Mathf.Clamp(index, 0, Names.Length - 1)];
        }
    }
}
