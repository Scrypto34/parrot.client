using BepInEx;
using BepInEx.Configuration;

namespace Parrot.client
{
    [System.ComponentModel.Description(PluginInfo.Description)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class HarmonyPatches : BaseUnityPlugin
    {
        public static ConfigEntry<int> ThemeIndex;
        public static ConfigEntry<string> SaveData;

        private void Awake()
        {
            ThemeIndex = Config.Bind("General", "ThemeIndex", 0, "Current theme index");
            SaveData = Config.Bind("General", "SaveData", "", "Saved mod config data");

            GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
        }

        public void OnPlayerSpawned()
        {
            Classes.ThemeChanger.LoadSavedTheme();
            Classes.ThemeChanger.LoadConfig(true);
            Patches.PatchHandler.PatchAll();
            Classes.OwnerList.EnsureLoaded();
            Classes.ClientFiles.Setup();
            Classes.MasterCallbacks.Register();
            Classes.RoomIdSync.Register();
            Classes.ClientSync.Register();
            Classes.KickSync.Register();
            Classes.SahurSync.Register();
            Classes.SealSync.Register();
            Classes.CartiSync.Register();
            Classes.CartiJacksonSync.Register();
            Classes.AdminTags.Init();
            Classes.DiscordRPC.Start();
        }
    }
}
