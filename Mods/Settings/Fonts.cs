using Parrot.client.Classes;
using Parrot.client.Notifications;
using System.Collections.Generic;
using UnityEngine;
using static Parrot.client.Menu.Main;
using MenuSettings = Parrot.client.Settings;

namespace Parrot.client.Mods.Settings
{
    public class Fonts
    {
        public static int fontIndex = 0;
        private static string appliedName = FontLib.DefaultName;

        public static void ChangeMenuFont()
        {

            if (FontLib.defaultFont == null)
                FontLib.defaultFont = MenuSettings.currentFont;

            List<string> names = FontLib.GetFontNames();
            if (names.Count <= 1)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> No fonts found in " + FontLib.FontFolder + ".");
                return;
            }

            fontIndex++;
            fontIndex %= names.Count;

            Font font = FontLib.Load(names[fontIndex]);
            if (font != null)
            {
                MenuSettings.currentFont = font;
                appliedName = names[fontIndex];
            }
            else
            {

                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> " + (FontLib.lastError ?? "Could not load " + names[fontIndex] + "."));
            }

            ButtonInfo button = GetIndex("Menu Font");
            if (button != null)
                button.overlapText = $"Menu Font: {appliedName}";
        }
    }
}
