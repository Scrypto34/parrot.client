using System.Collections.Generic;
using UnityEngine;

namespace Parrot.client.Mods
{
    internal class BoardMod
    {
        private static readonly Dictionary<Renderer, Color> originals = new Dictionary<Renderer, Color>();
        private static readonly List<Renderer> boards = new List<Renderer>();
        private static float nextScan;

        public static void Apply()
        {
            if (Time.time >= nextScan)
            {
                nextScan = Time.time + 2f;
                Rescan();
            }

            Color theme = Parrot.client.Settings.backgroundColor.GetCurrentColor();

            for (int i = 0; i < boards.Count; i++)
            {
                Renderer r = boards[i];
                if (r == null || r.material == null)
                    continue;

                r.material.color = theme;
                if (r.material.HasProperty("_BaseColor"))
                    r.material.SetColor("_BaseColor", theme);
            }
        }

        private static void Rescan()
        {
            boards.Clear();

            Renderer[] all = Object.FindObjectsOfType<Renderer>();
            foreach (Renderer r in all)
            {
                if (r == null)
                    continue;

                string n = r.gameObject.name.ToLower();
                if (!n.Contains("board") && !n.Contains("scoreboard") && !n.Contains("leaderboard"))
                    continue;

                boards.Add(r);

                if (!originals.ContainsKey(r) && r.material != null)
                    originals[r] = r.material.color;
            }
        }

        public static void Restore()
        {
            foreach (KeyValuePair<Renderer, Color> kv in originals)
            {
                if (kv.Key == null || kv.Key.material == null)
                    continue;

                kv.Key.material.color = kv.Value;
                if (kv.Key.material.HasProperty("_BaseColor"))
                    kv.Key.material.SetColor("_BaseColor", kv.Value);
            }

            originals.Clear();
            boards.Clear();
            nextScan = 0f;
        }
    }
}
