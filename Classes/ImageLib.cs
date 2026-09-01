using System.Collections.Generic;
using UnityEngine;

namespace Parrot.client.Classes
{

    public class ImageLib
    {
        public const string ImageFolder = "Resources/Client";

        private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, Texture2D> tinted = new Dictionary<string, Texture2D>();
        private static readonly HashSet<string> failed = new HashSet<string>();

        public static Texture2D LoadTinted(string fileName, Color color)
        {
            string key = fileName + "|" + ColorUtility.ToHtmlStringRGBA(color);
            if (tinted.TryGetValue(key, out Texture2D cached) && cached != null)
                return cached;

            Texture2D source = Load(fileName);
            if (source == null)
                return null;

            Color32[] pixels = source.GetPixels32();
            Color32 c = color;
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color32(c.r, c.g, c.b, (byte)(pixels[i].a * c.a / 255));

            Texture2D result = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            result.SetPixels32(pixels);
            result.Apply();
            result.wrapMode = TextureWrapMode.Clamp;

            tinted[key] = result;
            return result;
        }

        public static Texture2D Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || failed.Contains(fileName))
                return null;

            if (textures.TryGetValue(fileName, out Texture2D cached) && cached != null)
                return cached;

            byte[] bytes = EmbeddedResources.Read(fileName) ?? ReadFromDisk(fileName);
            if (bytes == null)
            {
                Fail(fileName, $"not found in {ImageFolder} or the parrot.client/console folder");
                return null;
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes))
            {
                Object.Destroy(texture);
                Fail(fileName, "could not be read as an image");
                return null;
            }

            texture.wrapMode = TextureWrapMode.Clamp;
            textures[fileName] = texture;
            return texture;
        }

        private static byte[] ReadFromDisk(string fileName)
        {
            try
            {
                string path = System.IO.Path.Combine(ClientFiles.ConsolePath ?? "", fileName);
                return System.IO.File.Exists(path) ? System.IO.File.ReadAllBytes(path) : null;
            }
            catch
            {
                return null;
            }
        }

        private static void Fail(string fileName, string reason)
        {
            failed.Add(fileName);
            Debug.LogWarning($"{PluginInfo.Name} // Image {fileName} {reason}");
        }
    }
}
