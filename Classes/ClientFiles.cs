using System;
using System.IO;
using UnityEngine;

namespace Parrot.client.Classes
{

    public class ClientFiles
    {
        public const string RootName = "parrot.client";
        public const string LogoResource = "console.png";

        public static string RootPath { get; private set; }
        public static string ConsolePath { get; private set; }
        public static string SoundsPath { get; private set; }
        public static string LogoPath { get; private set; }

        public static void Setup()
        {
            try
            {
                RootPath = Path.Combine(GameRoot(), RootName);
                ConsolePath = Path.Combine(RootPath, "console");
                SoundsPath = Path.Combine(RootPath, "sounds");

                Directory.CreateDirectory(ConsolePath);
                Directory.CreateDirectory(SoundsPath);

                string oldConsole = Path.Combine(GameRoot(), "Console");
                if (Directory.Exists(oldConsole))
                    Directory.Delete(oldConsole, true);

                byte[] logo = EmbeddedResources.Read(LogoResource);
                if (logo != null)
                {
                    LogoPath = Path.Combine(ConsolePath, LogoResource);
                    File.WriteAllBytes(LogoPath, logo);
                }
            }
            catch (Exception exc)
            {
                Debug.LogWarning($"{PluginInfo.Name} // Could not set up the parrot.client folder: {exc.Message}");
            }
        }

        private static string GameRoot()
        {

            try { return BepInEx.Paths.GameRootPath; }
            catch { return Directory.GetCurrentDirectory(); }
        }
    }
}
