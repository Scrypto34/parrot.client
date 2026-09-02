using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Parrot.client.Classes
{

    public class FontLib
    {
        public const string FontFolder = "Resources/Server/Fonts";
        public const string DefaultName = "Default";

        private const int FontSize = 24;
        private const uint FR_PRIVATE = 0x10;
        private const uint WM_FONTCHANGE = 0x001D;

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern int AddFontResourceEx(string path, uint flags, IntPtr reserved);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private static readonly Dictionary<string, Font> fonts = new Dictionary<string, Font>();
        private static readonly HashSet<string> registered = new HashSet<string>();
        private static List<string> fileNames;

        public static Font defaultFont;
        public static string lastError;

        public static List<string> GetFontNames()
        {
            fileNames ??= EmbeddedResources.ListFiles(FontFolder, "ttf", "otf");

            List<string> names = new List<string> { DefaultName };
            foreach (string file in fileNames)
                names.Add(StripExtension(file));

            return names;
        }

        public static Font Load(string name)
        {
            if (string.IsNullOrEmpty(name) || name == DefaultName)
                return defaultFont;

            if (fonts.TryGetValue(name, out Font cached))
                return cached;

            Font bundleFont = LoadFromBundle(name);
            if (bundleFont != null)
            {
                lastError = null;
                fonts[name] = bundleFont;
                return bundleFont;
            }

            Font font = null;
            string file = FindFile(name);
            string realFamily = null;

            if (file != null)
            {
                byte[] bytes = EmbeddedResources.Read(file);
                realFamily = GetFamilyName(bytes);
                string path = EmbeddedResources.Unpack(file, bytes);

                if (path != null && registered.Add(path))
                {
                    try
                    {
                        AddFontResourceEx(path, FR_PRIVATE, IntPtr.Zero);
                        SendMessage(new IntPtr(0xffff), WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);
                    }
                    catch { }
                }
            }

            if (!string.IsNullOrEmpty(realFamily))
                font = Verify(Font.CreateDynamicFontFromOSFont(realFamily, FontSize));

            if (font == null)
                foreach (string family in FamilyGuesses(name))
                {
                    font = Verify(Font.CreateDynamicFontFromOSFont(family, FontSize));
                    if (font != null)
                        break;
                }

            if (font == null)
                Fail(name, $"{name} could not be loaded (family '{realFamily ?? name}')");
            else
                lastError = null;

            fonts[name] = font;
            return font;
        }

        private static string GetFamilyName(byte[] data)
        {
            if (data == null || data.Length < 12)
                return null;

            try
            {
                int numTables = U16(data, 4);
                int nameOffset = -1;

                for (int i = 0; i < numTables; i++)
                {
                    int rec = 12 + i * 16;
                    if (data[rec] == 'n' && data[rec + 1] == 'a' && data[rec + 2] == 'm' && data[rec + 3] == 'e')
                    {
                        nameOffset = (int)U32(data, rec + 8);
                        break;
                    }
                }

                if (nameOffset < 0)
                    return null;

                int count = U16(data, nameOffset + 2);
                int storage = nameOffset + U16(data, nameOffset + 4);
                string family = null, full = null;

                for (int i = 0; i < count; i++)
                {
                    int r = nameOffset + 6 + i * 12;
                    int platformID = U16(data, r);
                    int nameID = U16(data, r + 6);
                    int len = U16(data, r + 8);
                    int off = U16(data, r + 10);
                    string s = DecodeName(data, storage + off, len, platformID);

                    if (nameID == 1 && family == null) family = s;
                    if (nameID == 4 && full == null) full = s;
                }

                return family ?? full;
            }
            catch { return null; }
        }

        private static string DecodeName(byte[] data, int offset, int len, int platformID)
        {
            if (offset < 0 || offset + len > data.Length)
                return null;

            if (platformID == 3 || platformID == 0)
                return System.Text.Encoding.BigEndianUnicode.GetString(data, offset, len);

            return System.Text.Encoding.ASCII.GetString(data, offset, len);
        }

        private static int U16(byte[] d, int i) => (d[i] << 8) | d[i + 1];
        private static uint U32(byte[] d, int i) => (uint)((d[i] << 24) | (d[i + 1] << 16) | (d[i + 2] << 8) | d[i + 3]);

        private static IEnumerable<string> FamilyGuesses(string name)
        {
            yield return name;
            yield return name.Replace("_", " ");

            if (name.EndsWith(" Regular"))
                yield return name.Substring(0, name.Length - " Regular".Length);

            int space = name.IndexOf(' ');
            if (space > 0)
                yield return name.Substring(0, space);

            int underscore = name.IndexOf('_');
            if (underscore > 0)
                yield return name.Substring(0, underscore);
        }

        private static void Fail(string name, string reason)
        {
            lastError = reason;
            Debug.LogWarning($"{PluginInfo.Name} // Could not load font {name}: {reason}");
        }

        private static Font Verify(Font font)
        {
            // Dynamic OS fonts don't reliably report HasCharacter (glyphs load on demand),
            // and CreateDynamicFontFromOSFont already falls back to a working font, so trust it.
            return font;
        }

        private static readonly Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();

        private static Font LoadFromBundle(string name)
        {
            try
            {
                string path = System.IO.Path.Combine(ClientFiles.ConsolePath ?? "", name + ".bundle");
                if (!System.IO.File.Exists(path))
                    return null;

                if (!loadedBundles.TryGetValue(path, out AssetBundle bundle) || bundle == null)
                {
                    bundle = AssetBundle.LoadFromFile(path);
                    loadedBundles[path] = bundle;
                }

                if (bundle == null)
                    return null;

                Font[] found = bundle.LoadAllAssets<Font>();
                if (found.Length == 0)
                {
                    Debug.LogWarning($"{PluginInfo.Name} // FontLib: bundle '{name}' had no Font asset");
                    return null;
                }

                Font f = found[0];
                try
                {
                    Shader shader = Shader.Find("GUI/Text Shader") ?? Shader.Find("UI/Default");
                    if (shader != null && f.material != null)
                        f.material.shader = shader;
                }
                catch { }

                Debug.Log($"{PluginInfo.Name} // FontLib: loaded '{name}' from bundle (material={(f.material != null)}, tex={(f.material != null && f.material.mainTexture != null)})");
                return f;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{PluginInfo.Name} // FontLib: bundle load failed for '{name}': {e.Message}");
                return null;
            }
        }

        private static string FindFile(string name)
        {
            fileNames ??= EmbeddedResources.ListFiles(FontFolder, "ttf", "otf");

            foreach (string file in fileNames)
            {
                if (StripExtension(file) == name)
                    return file;
            }

            return null;
        }

        private static string StripExtension(string file)
        {
            int dot = file.LastIndexOf('.');
            return dot < 0 ? file : file.Substring(0, dot);
        }
    }
}
