namespace Parrot.client.Mods.Settings
{
    public class GunSize
    {
        public static int index = 1;

        public static readonly string[] Names = { "Small", "Normal", "Big", "Bigger", "Huge", "Massive" };
        private static readonly float[] Pointer = { 0.08f, 0.12f, 0.18f, 0.24f, 0.32f, 0.6f };
        private static readonly float[] Trail   = { 0.013f, 0.02f, 0.03f, 0.04f, 0.055f, 0.12f };

        public static void Apply()
        {
            int i = index < 0 ? 0 : (index >= Names.Length ? Names.Length - 1 : index);
            GunTools.Gunlib.pointerBaseScale = Pointer[i];
            GunTools.Gunlib.TrailWidth = Trail[i];
        }

        public static void Cycle()
        {
            index = (index + 1) % Names.Length;
            Apply();
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void Back()
        {
            index = (index - 1 + Names.Length) % Names.Length;
            Apply();
            RefreshLabel();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void RefreshLabel()
        {
            var button = Menu.Main.GetIndex("Gun Size");
            if (button != null)
                button.overlapText = "Gun Size: " + Names[index < 0 ? 0 : (index >= Names.Length ? Names.Length - 1 : index)];
        }
    }
}
