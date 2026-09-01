using Parrot.client.Classes;
using Parrot.client.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods
{

    public class Soundboard
    {
        public const int Category = 17;

        private static readonly string[] Extensions = { ".wav", ".mp3", ".ogg", ".aiff", ".aif", ".mp4" };

        public static ButtonInfo[] BuildButtons()
        {
            List<ButtonInfo> page = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Refresh Sounds", method =() => Refresh(), isTogglable = false, toolTip = "Rescans the parrot.client/sounds folder for new files."},
            };

            foreach (string file in FindSounds())
            {
                string path = file;
                page.Add(new ButtonInfo
                {
                    buttonText = Path.GetFileNameWithoutExtension(path),
                    method = () => AudioLib.PlayFileMic(path),
                    isTogglable = false,
                    toolTip = "Plays " + Path.GetFileName(path) + " out of your mic."
                });
            }

            return page.ToArray();
        }

        public static void Refresh()
        {
            Parrot.client.Menu.Buttons.buttons[Category] = BuildButtons();
            RecreateMenu();
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SOUNDBOARD</color><color=grey>]</color> Sounds refreshed.");
        }

        private static List<string> FindSounds()
        {
            List<string> sounds = new List<string>();

            try
            {
                string folder = ClientFiles.SoundsPath;
                if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                    return sounds;

                foreach (string file in Directory.GetFiles(folder))
                {
                    string ext = Path.GetExtension(file).ToLowerInvariant();
                    if (Array.IndexOf(Extensions, ext) >= 0)
                        sounds.Add(file);
                }
            }
            catch (Exception exc)
            {
                Debug.LogWarning($"{PluginInfo.Name} // Soundboard could not read the sounds folder: {exc.Message}");
            }

            sounds.Sort(StringComparer.OrdinalIgnoreCase);
            return sounds;
        }
    }
}
