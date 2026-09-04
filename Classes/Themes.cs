using Parrot.client.Menu;
using Parrot.client.Notifications;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.Newtonsoft.Json;
using static Parrot.client.Settings;
using Parrot.client.GunTools;

namespace Parrot.client.Classes
{
    public class Theme
    {
        public string name;
        public ExtGradient background;
        public ExtGradient[] buttons;
        public Color[] text;

        public Theme(string name, Color bgColor, Color btnDisabled, Color btnEnabled, Color textDisabled, Color textEnabled)
        {
            this.name = name;
            background = new ExtGradient { colors = ExtGradient.GetSolidGradient(bgColor) };
            buttons = new ExtGradient[]
            {
                new ExtGradient { colors = ExtGradient.GetSolidGradient(btnDisabled) },
                new ExtGradient { colors = ExtGradient.GetSolidGradient(btnEnabled) }
            };
            text = new Color[] { textDisabled, textEnabled };
        }

        public Theme(string name, ExtGradient bg, ExtGradient[] btns, Color[] txt)
        {
            this.name = name;
            background = bg;
            buttons = btns;
            text = txt;
        }
    }

    public class ThemeChanger
    {
        public static Theme[] themes = new Theme[]
        {
            new Theme("Default", new Color(0.2f, 0.22f, 0.38f), new Color(0.11f, 0.12f, 0.24f), new Color(0.45f, 0.5f, 0.9f), new Color(0.82f, 0.84f, 0.98f), Color.white),
            new Theme("Ocean", new Color(0.1f, 0.32f, 0.52f), new Color(0.05f, 0.16f, 0.3f), new Color(0.2f, 0.62f, 0.95f), new Color(0.82f, 0.94f, 1f), Color.white),
            new Theme("Forest", new Color(0.12f, 0.36f, 0.2f), new Color(0.06f, 0.19f, 0.1f), new Color(0.28f, 0.78f, 0.4f), new Color(0.86f, 1f, 0.9f), Color.white),
            new Theme("Lava", new Color(0.48f, 0.16f, 0.06f), new Color(0.26f, 0.08f, 0.03f), new Color(1f, 0.42f, 0.12f), new Color(1f, 0.85f, 0.7f), Color.white),
            new Theme("Cotton Candy", new Color(0.95f, 0.7f, 0.85f), new Color(0.85f, 0.55f, 0.72f), new Color(1f, 0.55f, 0.8f), new Color(0.4f, 0.15f, 0.3f), new Color(0.6f, 0.1f, 0.4f)),
            new Theme("Halloween", new Color(0.4f, 0.14f, 0.03f), new Color(0.22f, 0.08f, 0.02f), new Color(1f, 0.5f, 0.05f), new Color(1f, 0.82f, 0.6f), Color.white),
            new Theme("Matrix", new Color(0.08f, 0.28f, 0.08f), new Color(0.04f, 0.15f, 0.04f), new Color(0.25f, 0.95f, 0.25f), new Color(0.75f, 1f, 0.75f), Color.white),
            new Theme("Synthwave", new Color(0.26f, 0.08f, 0.4f), new Color(0.14f, 0.04f, 0.22f), new Color(1f, 0.25f, 0.65f), new Color(0.95f, 0.78f, 1f), Color.white),
            new Theme("Arctic", new Color(0.88f, 0.94f, 0.99f), new Color(0.72f, 0.82f, 0.92f), new Color(0.45f, 0.68f, 0.96f), new Color(0.25f, 0.35f, 0.55f), new Color(0.05f, 0.2f, 0.55f)),
            new Theme("Sunset", new Color(0.7f, 0.3f, 0.4f), new Color(0.4f, 0.15f, 0.22f), new Color(1f, 0.55f, 0.25f), new Color(1f, 0.85f, 0.7f), Color.white),
            new Theme("Cherry Blossom", new Color(0.3f, 0.12f, 0.2f), new Color(0.17f, 0.06f, 0.11f), new Color(1f, 0.5f, 0.68f), new Color(1f, 0.88f, 0.92f), Color.white),
            new Theme("Toxic", new Color(0.12f, 0.25f, 0.02f), new Color(0.06f, 0.14f, 0.01f), new Color(0.6f, 1f, 0.1f), new Color(0.8f, 1f, 0.5f), Color.white),
            new Theme("Blood Moon", new Color(0.32f, 0.05f, 0.05f), new Color(0.18f, 0.02f, 0.02f), new Color(0.9f, 0.15f, 0.15f), new Color(1f, 0.7f, 0.7f), Color.white),
            new Theme("Royal", new Color(0.24f, 0.08f, 0.4f), new Color(0.13f, 0.04f, 0.22f), new Color(0.65f, 0.2f, 0.9f), new Color(1f, 0.9f, 0.6f), new Color(1f, 0.9f, 0.4f)),
            new Theme("Cyberpunk", new Color(0.1f, 0.12f, 0.24f), new Color(0.05f, 0.06f, 0.14f), new Color(0.1f, 1f, 1f), new Color(1f, 0.4f, 1f), new Color(0.2f, 1f, 1f)),
            new Theme("Peach", new Color(1f, 0.78f, 0.65f), new Color(0.9f, 0.62f, 0.5f), new Color(1f, 0.68f, 0.5f), new Color(0.45f, 0.25f, 0.15f), new Color(0.7f, 0.35f, 0.2f)),
            new Theme("Void", new Color(0.12f, 0.12f, 0.18f), new Color(0.07f, 0.07f, 0.11f), new Color(0.35f, 0.35f, 0.5f), new Color(0.6f, 0.6f, 0.72f), Color.white),
            new Theme("Crimson", new Color(0.42f, 0.06f, 0.1f), new Color(0.24f, 0.03f, 0.06f), new Color(1f, 0.2f, 0.25f), new Color(1f, 0.8f, 0.82f), Color.white),
            new Theme("Aurora", new Color(0.06f, 0.22f, 0.25f), new Color(0.03f, 0.12f, 0.14f), new Color(0.15f, 0.85f, 0.62f), new Color(0.6f, 1f, 0.82f), Color.white),
            new Theme("Strawberry", new Color(0.6f, 0.14f, 0.2f), new Color(0.34f, 0.07f, 0.11f), new Color(1f, 0.3f, 0.42f), new Color(1f, 0.85f, 0.88f), Color.white),
            new Theme("Stealth", new Color(0.18f, 0.18f, 0.2f), new Color(0.1f, 0.1f, 0.11f), new Color(0.4f, 0.4f, 0.44f), new Color(0.7f, 0.7f, 0.74f), Color.white),
            new Theme("Candy Corn", new Color(0.35f, 0.2f, 0.02f), new Color(0.2f, 0.11f, 0.01f), new Color(1f, 0.72f, 0.1f), new Color(1f, 0.95f, 0.7f), Color.white),
            new Theme("Ice Cream", new Color(0.98f, 0.9f, 0.94f), new Color(0.85f, 0.68f, 0.78f), new Color(0.9f, 0.55f, 0.72f), new Color(0.45f, 0.2f, 0.32f), new Color(0.7f, 0.3f, 0.5f)),
            new Theme("Galaxy", new Color(0.14f, 0.06f, 0.32f), new Color(0.08f, 0.03f, 0.18f), new Color(0.5f, 0.28f, 0.9f), new Color(0.75f, 0.6f, 1f), Color.white),
            new Theme("Lemonade", new Color(0.98f, 0.94f, 0.5f), new Color(0.85f, 0.8f, 0.35f), new Color(1f, 0.95f, 0.2f), new Color(0.4f, 0.36f, 0.05f), new Color(0.55f, 0.5f, 0f)),
            new Theme("Volcano", new Color(0.4f, 0.12f, 0.03f), new Color(0.22f, 0.06f, 0.02f), new Color(1f, 0.35f, 0.05f), new Color(1f, 0.8f, 0.4f), Color.white),
            new Theme("Coral Reef", new Color(0.05f, 0.28f, 0.34f), new Color(0.03f, 0.15f, 0.18f), new Color(0.1f, 0.72f, 0.82f), new Color(0.9f, 0.6f, 0.5f), new Color(0.2f, 0.9f, 1f)),
            new Theme("Sakura", new Color(0.35f, 0.12f, 0.2f), new Color(0.2f, 0.07f, 0.11f), new Color(1f, 0.55f, 0.7f), new Color(1f, 0.85f, 0.9f), Color.white),
            new Theme("Nebula", new Color(0.16f, 0.04f, 0.26f), new Color(0.09f, 0.02f, 0.15f), new Color(0.55f, 0.15f, 0.95f), new Color(0.6f, 0.7f, 1f), Color.white),
            new Theme("Mossy Stone", new Color(0.3f, 0.33f, 0.26f), new Color(0.17f, 0.19f, 0.14f), new Color(0.5f, 0.56f, 0.38f), new Color(0.8f, 0.85f, 0.7f), Color.white),
            new Theme("Ember", new Color(0.35f, 0.1f, 0.03f), new Color(0.2f, 0.05f, 0.01f), new Color(1f, 0.35f, 0.08f), new Color(1f, 0.7f, 0.4f), Color.white),
            new Theme("Frost", new Color(0.78f, 0.9f, 0.98f), new Color(0.6f, 0.78f, 0.9f), new Color(0.35f, 0.65f, 0.9f), new Color(0.15f, 0.25f, 0.4f), new Color(0f, 0.35f, 0.65f)),
            new Theme("Camo", new Color(0.24f, 0.28f, 0.16f), new Color(0.13f, 0.16f, 0.09f), new Color(0.42f, 0.48f, 0.28f), new Color(0.75f, 0.8f, 0.6f), Color.white),
            new Theme("Scrypto", new Color(0.2f, 0.2f, 0.2f), new Color(0.11f, 0.11f, 0.11f), new Color(0.4f, 0.4f, 0.4f), new Color(0.7f, 0.7f, 0.7f), Color.white),
            new Theme("Red", new Color(0.42f, 0.1f, 0.1f), new Color(0.22f, 0.05f, 0.05f), new Color(1f, 0.35f, 0.35f), new Color(1f, 0.88f, 0.88f), Color.white),
            new Theme("Pink", new Color(0.5f, 0.22f, 0.36f), new Color(0.28f, 0.1f, 0.2f), new Color(1f, 0.55f, 0.78f), new Color(1f, 0.92f, 0.96f), Color.white),
            new Theme("Blue", new Color(0.14f, 0.24f, 0.5f), new Color(0.06f, 0.11f, 0.26f), new Color(0.45f, 0.66f, 1f), new Color(0.9f, 0.94f, 1f), Color.white),
            new Theme("Purple", new Color(0.3f, 0.16f, 0.5f), new Color(0.15f, 0.07f, 0.26f), new Color(0.76f, 0.48f, 1f), new Color(0.93f, 0.87f, 1f), Color.white),
            new Theme("Black", Color.black, Color.black, new Color(0.2f, 0.2f, 0.2f), new Color(0.65f, 0.65f, 0.65f), Color.white),
            new Theme("Rainbow", Color.black, Color.black, Color.white, Color.white, Color.white),
        };

        public static int currentThemeIndex = 0;
        public static bool darkMode = true;

        public static void ApplyTheme(int index)
        {
            if (index < 0 || index >= themes.Length)
                return;

            currentThemeIndex = index;
            Theme theme = themes[index];

            if (theme.name == "Rainbow")
            {
                backgroundColor = new ExtGradient { rainbow = true };
                buttonColors = new ExtGradient[]
                {
                    new ExtGradient { rainbow = true },
                    new ExtGradient { rainbow = true }
                };
                textColors = new Color[] { Color.white, Color.white };

                Parrot.client.GunTools.Gunlib.pointercolor = Color.white;
                Parrot.client.GunTools.Gunlib.pointergradient = new GradientColorKey[]
                {
                    new GradientColorKey(Color.red, 0f),
                    new GradientColorKey(Color.green, 0.5f),
                    new GradientColorKey(Color.blue, 1f)
                };

                if (HarmonyPatches.ThemeIndex != null)
                    HarmonyPatches.ThemeIndex.Value = index;

                UpdateButtonText();
                Menu.Main.RecreateMenu();
                return;
            }

            Color bg = theme.background.GetCurrentColor();
            Color btnDisabled = theme.buttons[0].GetCurrentColor();
            Color btnEnabled = theme.buttons[1].GetCurrentColor();
            Color textDisabled = theme.text[0];
            Color textEnabled = theme.text[1];

            if (darkMode)
            {
                bg = Color.Lerp(bg, Color.black, 0.72f);
                btnDisabled = Color.Lerp(btnDisabled, Color.black, 0.55f);
                textDisabled = Color.Lerp(btnEnabled, Color.white, 0.45f);
                textEnabled = Color.white;
            }

            backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(bg) };
            buttonColors = new ExtGradient[]
            {
                new ExtGradient { colors = ExtGradient.GetSolidGradient(btnDisabled) },
                new ExtGradient { colors = ExtGradient.GetSolidGradient(btnEnabled) }
            };
            textColors = new Color[] { textDisabled, textEnabled };

            Parrot.client.GunTools.Gunlib.pointercolor = btnEnabled;

            Color gunColor = btnEnabled;

            Parrot.client.GunTools.Gunlib.pointergradient = new GradientColorKey[]
            {
                new GradientColorKey(gunColor * 0.7f, 0f),
                new GradientColorKey(gunColor * 0.85f, 0.33f),
                new GradientColorKey(gunColor, 0.66f),
                new GradientColorKey(gunColor * 0.85f, 1f)
            };

            if (HarmonyPatches.ThemeIndex != null)
                HarmonyPatches.ThemeIndex.Value = index;

            UpdateButtonText();
            Menu.Main.RecreateMenu();
        }

        public static void NextTheme()
        {
            ApplyTheme((currentThemeIndex + 1) % themes.Length);
        }

        public static void ToggleThemeMode()
        {
            darkMode = !darkMode;
            RefreshModeLabel();
            ApplyTheme(currentThemeIndex);
            SaveConfigSilent();
        }

        public static void RefreshModeLabel()
        {
            ButtonInfo button = Menu.Main.GetIndex("Theme Mode");
            if (button != null)
                button.overlapText = "Theme Mode: " + (darkMode ? "Dark" : "Light");
        }

        public static void LoadSavedTheme()
        {
            if (HarmonyPatches.ThemeIndex != null)
                ApplyTheme(HarmonyPatches.ThemeIndex.Value);
        }

        private static void UpdateButtonText()
        {
            foreach (ButtonInfo[] category in Buttons.buttons)
            {
                for (int i = 0; i < category.Length; i++)
                {
                    if (category[i].buttonText.StartsWith("Theme:"))
                    {
                        category[i].buttonText = "Theme: " + themes[currentThemeIndex].name;
                        return;
                    }
                }
            }
        }

        public static void SaveConfig()
        {
            WriteConfig();
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SAVED</color><color=grey>]</color> Config saved.");
        }

        public static void SaveConfigSilent() => WriteConfig();

        private static void WriteConfig()
        {
            var data = new Dictionary<string, object>();
            data["themeIndex"] = currentThemeIndex;
            data["darkMode"] = darkMode;
            data["openSound"] = Mods.Settings.Audio.openSoundIndex;
            data["clickSound"] = Mods.Settings.Audio.clickSoundIndex;
            data["crownColor"] = AdminTags.crownColorIndex;
            data["roundedCorners"] = Settings.roundedCorners;
            data["rainbowOutline"] = Settings.rainbowOutline;
            data["outlineColor"] = Mods.Settings.Outline.colorIndex;
            data["smoothing"] = Mods.Settings.Smoothing.index;
            data["arrowStyle"] = Mods.Settings.ArrowStyle.index;
            data["menuSize"] = Mods.Settings.MenuScale.index;
            data["buttonAnimations"] = Settings.buttonAnimations;
            data["gunColor"] = Mods.Settings.GunColor.index;
            data["gunSize"] = Mods.Settings.GunSize.index;
            data["gunLock"] = Mods.Settings.Gun.gunLock;
            data["noGunLine"] = Mods.Settings.Gun.noGunLine;
            data["notifColor"] = Mods.Settings.Notifs.colorIndex;
            data["bgColor"] = Mods.Settings.MenuColors.bgIndex;
            data["buttonColor"] = Mods.Settings.MenuColors.buttonIndex;
            data["textColor"] = Mods.Settings.MenuColors.textIndex;
            data["notifRoom"] = Notifications.NotifiLib.RoomActivity;
            data["notifMod"] = Notifications.NotifiLib.ModActivity;
            data["pinned"] = string.Join("|", Menu.Main.pinnedButtons);

            var mods = new Dictionary<string, bool>();
            foreach (ButtonInfo[] category in Buttons.buttons)
            {
                foreach (ButtonInfo button in category)
                {
                    mods[button.buttonText] = button.enabled;
                }
            }
            data["mods"] = mods;

            HarmonyPatches.SaveData.Value = JsonConvert.SerializeObject(data);
        }

        public static void LoadConfig() => LoadConfig(false);

        public static void LoadConfig(bool silent)
        {
            string json = HarmonyPatches.SaveData.Value;
            if (string.IsNullOrEmpty(json))
            {
                if (!silent)
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> No saved config found.");
                return;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                if (data.ContainsKey("darkMode"))
                    darkMode = Convert.ToBoolean(data["darkMode"]);

                if (data.ContainsKey("themeIndex"))
                {
                    int themeIdx = Convert.ToInt32(data["themeIndex"]);
                    ApplyTheme(themeIdx);
                }

                RefreshModeLabel();

                if (data.ContainsKey("openSound"))
                    Mods.Settings.Audio.openSoundIndex = Convert.ToInt32(data["openSound"]);

                if (data.ContainsKey("clickSound"))
                    Mods.Settings.Audio.clickSoundIndex = Convert.ToInt32(data["clickSound"]);

                if (data.ContainsKey("crownColor"))
                    AdminTags.crownColorIndex = Convert.ToInt32(data["crownColor"]);

                if (data.ContainsKey("roundedCorners"))
                    Settings.roundedCorners = Convert.ToBoolean(data["roundedCorners"]);

                if (data.ContainsKey("rainbowOutline"))
                    Settings.rainbowOutline = Convert.ToBoolean(data["rainbowOutline"]);

                if (data.ContainsKey("outlineColor"))
                    Mods.Settings.Outline.colorIndex = Convert.ToInt32(data["outlineColor"]);

                if (data.ContainsKey("smoothing"))
                    Mods.Settings.Smoothing.index = Convert.ToInt32(data["smoothing"]);

                if (data.ContainsKey("arrowStyle"))
                    Mods.Settings.ArrowStyle.index = Convert.ToInt32(data["arrowStyle"]);

                if (data.ContainsKey("menuSize"))
                    Mods.Settings.MenuScale.index = Convert.ToInt32(data["menuSize"]);


                if (data.ContainsKey("buttonAnimations"))
                    Settings.buttonAnimations = Convert.ToBoolean(data["buttonAnimations"]);

                if (data.ContainsKey("gunColor"))
                    Mods.Settings.GunColor.index = Convert.ToInt32(data["gunColor"]);

                if (data.ContainsKey("gunSize"))
                    Mods.Settings.GunSize.index = Convert.ToInt32(data["gunSize"]);

                if (data.ContainsKey("gunLock"))
                    Mods.Settings.Gun.gunLock = Convert.ToBoolean(data["gunLock"]);
                if (data.ContainsKey("noGunLine"))
                    Mods.Settings.Gun.noGunLine = Convert.ToBoolean(data["noGunLine"]);
                Mods.Settings.GunSize.Apply();

                if (data.ContainsKey("notifColor"))
                    Mods.Settings.Notifs.colorIndex = Convert.ToInt32(data["notifColor"]);
                if (data.ContainsKey("notifRoom"))
                    Notifications.NotifiLib.RoomActivity = Convert.ToBoolean(data["notifRoom"]);
                if (data.ContainsKey("notifMod"))
                    Notifications.NotifiLib.ModActivity = Convert.ToBoolean(data["notifMod"]);
                Mods.Settings.Notifs.Apply();

                if (data.ContainsKey("bgColor"))
                    Mods.Settings.MenuColors.bgIndex = Convert.ToInt32(data["bgColor"]);
                if (data.ContainsKey("buttonColor"))
                    Mods.Settings.MenuColors.buttonIndex = Convert.ToInt32(data["buttonColor"]);
                if (data.ContainsKey("textColor"))
                    Mods.Settings.MenuColors.textIndex = Convert.ToInt32(data["textColor"]);

                if (data.ContainsKey("pinned"))
                {
                    Menu.Main.pinnedButtons.Clear();
                    foreach (string text in Convert.ToString(data["pinned"]).Split('|'))
                        if (!string.IsNullOrEmpty(text) && Menu.Main.GetIndex(text) != null && !Menu.Main.pinnedButtons.Contains(text))
                            Menu.Main.pinnedButtons.Add(text);
                }

                Mods.Settings.Audio.RefreshLabels();
                AdminTags.RefreshLabel();
                Mods.Settings.Outline.RefreshLabel();
                Mods.Settings.Smoothing.RefreshLabel();
                Mods.Settings.ArrowStyle.RefreshLabel();
                Mods.Settings.MenuScale.RefreshLabel();
                Mods.Settings.GunColor.RefreshLabel();
                Mods.Settings.GunSize.RefreshLabel();
                Mods.Settings.Gun.RefreshGunLock();
                Mods.Settings.Gun.RefreshNoGunLine();
                Mods.Settings.Notifs.RefreshLabel();
                Mods.Settings.MenuColors.RefreshBgLabel();
                Mods.Settings.MenuColors.RefreshButtonLabel();
                Mods.Settings.MenuColors.RefreshTextLabel();
                Mods.Settings.MenuColors.ReApply();

                if (data.ContainsKey("mods"))
                {
                    var mods = JsonConvert.DeserializeObject<Dictionary<string, bool>>(data["mods"].ToString());
                    foreach (ButtonInfo[] category in Buttons.buttons)
                    {
                        foreach (ButtonInfo button in category)
                        {
                            if (mods.ContainsKey(button.buttonText))
                                button.enabled = mods[button.buttonText];
                        }
                    }
                }

                try { Menu.Main.RecreateMenu(); } catch { }

                if (!silent)
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>LOADED</color><color=grey>]</color> Config loaded.");
            }
            catch
            {
                if (!silent)
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to load config.");
            }
        }
    }
}
