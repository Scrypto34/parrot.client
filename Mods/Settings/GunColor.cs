using UnityEngine;

namespace Parrot.client.Mods.Settings
{
    public class GunColor
    {
        public static int index = 0;

        public static readonly string[] Names = { "Theme", "Normal", "Green", "Red", "Blue", "Purple", "White", "Rainbow" };

        public static void Cycle()
        {
            index = (index + 1) % Names.Length;
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void Back()
        {
            index = (index - 1 + Names.Length) % Names.Length;
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void RefreshLabel()
        {
            var button = Menu.Main.GetIndex("Gun Color");
            if (button != null)
                button.overlapText = "Gun Color: " + Names[Mathf.Clamp(index, 0, Names.Length - 1)];
        }

        public static Color Get(bool triggerHeld)
        {
            switch (index)
            {
                case 1: return triggerHeld ? Color.green : Color.red;
                case 2: return Color.green;
                case 3: return Color.red;
                case 4: return new Color(0.2f, 0.45f, 1f);
                case 5: return new Color(0.6f, 0.2f, 1f);
                case 6: return Color.white;
                case 7: return Color.HSVToRGB(Time.time % 1f, 1f, 1f);
                default:
                    return triggerHeld
                        ? Parrot.client.Settings.buttonColors[1].GetCurrentColor()
                        : Parrot.client.Settings.buttonColors[0].GetCurrentColor();
            }
        }
    }
}
