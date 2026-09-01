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
        Rainbow
    }

    public class Gun
    {
        public static int gunTypeIndex = 0;
        public static GunType gunType = GunType.Normal;

        public static bool gunLock = true;

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

        public static void ChangeGunType()
        {
            string[] typeNames = new string[] { "Normal", "Electric", "Wiggly", "Spiral", "Sine", "Bounce", "Rainbow" };
            GunType[] typeValues = new GunType[] { GunType.Normal, GunType.Electric, GunType.Wiggly, GunType.Spiral, GunType.Sine, GunType.Bounce, GunType.Rainbow };

            gunTypeIndex++;
            gunTypeIndex %= typeNames.Length;
            gunType = typeValues[gunTypeIndex];

            GetIndex("Change Gun Type").overlapText = $"Change Gun Type [{typeNames[gunTypeIndex]}]";
        }
    }
}
