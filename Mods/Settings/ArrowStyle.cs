using UnityEngine;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public class ArrowStyle
    {
        public static int index = 0;

        public static readonly string[] Names = { "Default", "Side Arrows", "Scrypto's Favorite" };

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
            var button = GetIndex("Menu Layout");
            if (button != null)
                button.overlapText = "Menu Layout: " + Names[Mathf.Clamp(index, 0, Names.Length - 1)];
        }
    }
}
