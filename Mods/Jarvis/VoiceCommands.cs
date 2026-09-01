using System.Collections.Generic;
using System.IO;
using System.Text;
using Parrot.client.Classes;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods.Jarvis
{
    public static class VoiceCommands
    {
      

        public static Dictionary<string, string> Load()
        {
            var map = new Dictionary<string, string>();
            try
            {
                string path = Path.Combine(ClientFiles.RootPath ?? "", "voice_commands.txt");
                if (!File.Exists(path))
                    return map;

                foreach (string line in File.ReadAllLines(path))
                {
                    string l = line.Trim();
                    if (l.Length == 0 || l.StartsWith("#"))
                        continue;

                    int eq = l.IndexOf('=');
                    if (eq <= 0)
                        continue;

                    string name = l.Substring(0, eq).Trim();
                    string button = l.Substring(eq + 1).Trim();
                    if (name.Length > 0 && button.Length > 0)
                        map[name.ToLowerInvariant()] = button;
                }
            }
            catch { }

            return map;
        }

        public static string PromptSection()
        {
            var sb = new StringBuilder();
            sb.Append("\n\nWhen the user asks you to turn something on/off or run a mod, put it on its own line exactly as: RUN: <exact mod name>. You may output several RUN lines. Toggling a mod that is on turns it off. These are the exact mod names you can use:\n");
            sb.Append(AllButtons());

            var map = Load();
            if (map.Count > 0)
            {
                sb.Append("\nExtra spoken aliases (left says which mod on the right): ");
                bool first = true;
                foreach (var kv in map)
                {
                    if (!first) sb.Append("; ");
                    sb.Append(kv.Key).Append(" -> ").Append(kv.Value);
                    first = false;
                }
            }
            return sb.ToString();
        }

        private static string AllButtons()
        {
            var seen = new HashSet<string>();
            var sb = new StringBuilder();
            var all = Parrot.client.Menu.Buttons.buttons;
            if (all == null)
                return "";

            foreach (var cat in all)
            {
                if (cat == null) continue;
                foreach (var b in cat)
                {
                    if (b == null || string.IsNullOrEmpty(b.buttonText)) continue;
                    if (b.buttonText.StartsWith("Return")) continue;
                    if (seen.Add(b.buttonText))
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(b.buttonText);
                    }
                }
            }
            return sb.ToString();
        }

        public static string RunFromResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return "";

            var map = Load();
            var done = new StringBuilder();

            foreach (string raw in response.Split('\n'))
            {
                string line = raw.Trim();
                int idx = line.ToUpperInvariant().IndexOf("RUN:");
                if (idx < 0)
                    continue;

                string name = line.Substring(idx + 4).Trim().TrimEnd('.', '!', ',').Trim();
                if (name.Length == 0)
                    continue;

                string button = null;
                if (map.TryGetValue(name.ToLowerInvariant(), out string mapped))
                    button = mapped;
                else
                    button = FindButton(name);

                var info = button != null ? GetIndex(button) : null;
                if (info == null)
                    continue;

                try { Toggle(button); } catch { continue; }

                string label = CleanLabel(button);
                if (info.isTogglable)
                    done.Append(info.enabled ? "Enabled " : "Disabled ").Append(label).Append(". ");
                else
                    done.Append(label).Append(". ");
            }

            return done.ToString().Trim();
        }

        private static string CleanLabel(string button)
        {
            int p = button.IndexOf('(');
            string s = p > 0 ? button.Substring(0, p) : button;
            return s.Trim();
        }

        private static string FindButton(string name)
        {
            var all = Parrot.client.Menu.Buttons.buttons;
            if (all == null)
                return null;

            foreach (var cat in all)
            {
                if (cat == null) continue;
                foreach (var b in cat)
                {
                    if (b != null && !string.IsNullOrEmpty(b.buttonText) &&
                        string.Equals(b.buttonText, name, System.StringComparison.OrdinalIgnoreCase))
                        return b.buttonText;
                }
            }
            return null;
        }
    }
}
