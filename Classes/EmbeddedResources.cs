using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Parrot.client.Classes
{

    public class EmbeddedResources
    {
        public static byte[] Read(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string suffix = "." + fileName.Replace('/', '.').Replace('\\', '.');

            foreach (string resource in assembly.GetManifestResourceNames())
            {
                if (!resource.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    continue;

                using Stream stream = assembly.GetManifestResourceStream(resource);
                if (stream == null)
                    return null;

                byte[] bytes = new byte[stream.Length];
                int read = 0;
                while (read < bytes.Length)
                {
                    int chunk = stream.Read(bytes, read, bytes.Length - read);
                    if (chunk <= 0) break;
                    read += chunk;
                }

                return bytes;
            }

            return null;
        }

        public static List<string> ListFiles(string folder, params string[] extensions)
        {
            string marker = "." + folder.Replace('/', '.').Replace('\\', '.') + ".";
            List<string> files = new List<string>();

            foreach (string resource in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                int start = resource.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (start < 0)
                    continue;

                string file = resource.Substring(start + marker.Length);

                foreach (string extension in extensions)
                {
                    if (file.EndsWith("." + extension, StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(file);
                        break;
                    }
                }
            }

            files.Sort(StringComparer.OrdinalIgnoreCase);
            return files;
        }

        public static string Unpack(string fileName, byte[] bytes = null)
        {
            bytes ??= Read(fileName);
            if (bytes == null)
                return null;

            string path = Path.Combine(Application.temporaryCachePath, "Parrot_" + fileName);

            try
            {
                File.WriteAllBytes(path, bytes);
                return path;
            }
            catch (Exception exc)
            {
                Debug.LogWarning($"{PluginInfo.Name} // Could not unpack {fileName}: {exc.Message}");
                return null;
            }
        }
    }
}
