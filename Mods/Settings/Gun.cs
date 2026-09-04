using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Settings
{
    public enum GunType
    {
        Normal,
        Electric,
        Wiggly,
        Spiral,
        Sine,
        Bounce,
        Rainbow,
        SmoothCable
    }

    public class Gun
    {
        public static int gunTypeIndex = 0;
        public static GunType gunType = GunType.Normal;

        public static bool gunLock = true;

        public static bool noGunLine = false;

        public static void ToggleNoGunLine()
        {
            noGunLine = !noGunLine;
            RefreshNoGunLine();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void RefreshNoGunLine()
        {
            var button = GetIndex("No Gun Line");
            if (button != null)
                button.overlapText = "No Gun Line (" + (noGunLine ? "On" : "Off") + ")";
        }

        public static void ToggleGunLock()
        {
            gunLock = !gunLock;
            RefreshGunLock();
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
        }

        public static void RefreshGunLock()
        {
            var button = GetIndex("Gun Lock");
            if (button != null)
                button.overlapText = "Gun Lock (" + (gunLock ? "On" : "Off") + ")";
        }

        public static void ChangeGunType(int dir = 1)
        {
            string[] typeNames = new string[] { "Normal", "Electric", "Wiggly", "Spiral", "Sine", "Bounce", "Rainbow", "Smooth Cable" };
            GunType[] typeValues = new GunType[] { GunType.Normal, GunType.Electric, GunType.Wiggly, GunType.Spiral, GunType.Sine, GunType.Bounce, GunType.Rainbow, GunType.SmoothCable };

            gunTypeIndex = (gunTypeIndex + dir + typeNames.Length) % typeNames.Length;
            gunType = typeValues[gunTypeIndex];

            var button = GetIndex("Change Gun Type");
            if (button != null)
                button.overlapText = $"Change Gun Type [{typeNames[gunTypeIndex]}]";
        }
    }
}
