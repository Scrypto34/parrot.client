using GorillaLocomotion;
using HarmonyLib;

namespace GrabPatches
{

    public class GrabPatches
    {
        [HarmonyPatch(typeof(GTPlayer), nameof(GTPlayer.TakeMyHand_ProcessMovement))]
        public class GrabPatch
        {
            public static bool enabled;

            public static bool Prefix(GTPlayer __instance) => !enabled;
        }
    }

}