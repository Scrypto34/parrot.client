using Parrot.client.Notifications;
using UnityEngine;

namespace Parrot.client.Mods
{
    public class Weather
    {
        public const int Category = 28;

        public static void Morning() => SetTime(0.22f);
        public static void Day() => SetTime(0.45f);
        public static void Evening() => SetTime(0.72f);
        public static void Night() => SetTime(0.95f);

        private static void SetTime(float fraction)
        {
            BetterDayNightManager mgr = BetterDayNightManager.instance;
            if (mgr == null)
                return;

            int count = mgr.dayNightLightmapNames != null && mgr.dayNightLightmapNames.Length > 0
                ? mgr.dayNightLightmapNames.Length
                : 8;

            int index = Mathf.Clamp(Mathf.RoundToInt((count - 1) * fraction), 0, count - 1);
            mgr.SetTimeOfDay(index, true);

            try { NotifiLib.SendNotification("<color=grey>[</color><color=cyan>WEATHER</color><color=grey>]</color> " + mgr.GetTimeOfDayString()); } catch { }
        }
    }
}
