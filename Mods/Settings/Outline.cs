using UnityEngine;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public class Outline
    {
        public static int colorIndex = 0;

        public static readonly string[] Names = { "White", "Pink", "Blue", "Black", "Cyan", "Brown", "Red", "Purple", "Green", "Orange", "Yellow" };

        public static readonly Color[] Top =
        {
            new Color(1f, 1f, 1f),
            new Color(1f, 0.7f, 0.85f),
            new Color(0.6f, 0.75f, 1f),
            new Color(0.55f, 0.55f, 0.55f),
            new Color(0.7f, 1f, 1f),
            new Color(0.8f, 0.6f, 0.38f),
            new Color(1f, 0.5f, 0.5f),
            new Color(0.85f, 0.6f, 1f),
            new Color(0.6f, 1f, 0.6f),
            new Color(1f, 0.75f, 0.4f),
            new Color(1f, 1f, 0.55f)
        };

        public static readonly Color[] Bottom =
        {
            new Color(0.1f, 0.1f, 0.1f),
            new Color(0.45f, 0.06f, 0.25f),
            new Color(0.04f, 0.1f, 0.45f),
            new Color(0.02f, 0.02f, 0.02f),
            new Color(0f, 0.32f, 0.32f),
            new Color(0.18f, 0.1f, 0.04f),
            new Color(0.4f, 0.03f, 0.03f),
            new Color(0.28f, 0.05f, 0.45f),
            new Color(0.05f, 0.35f, 0.08f),
            new Color(0.45f, 0.2f, 0.02f),
            new Color(0.4f, 0.35f, 0f)
        };

        public static Color TopColor => Top[Mathf.Clamp(colorIndex, 0, Top.Length - 1)];
        public static Color BottomColor => Bottom[Mathf.Clamp(colorIndex, 0, Bottom.Length - 1)];

        public static void Cycle()
        {
            colorIndex = (colorIndex + 1) % Names.Length;
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
            RecreateMenu();
        }

        public static void RefreshLabel()
        {
            var button = GetIndex("Outline Color");
            if (button != null)
                button.overlapText = $"Outline Color [{Names[Mathf.Clamp(colorIndex, 0, Names.Length - 1)]}]";
        }
    }
}
