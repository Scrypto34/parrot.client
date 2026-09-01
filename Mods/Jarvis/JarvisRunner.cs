using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;
using Parrot.client.Notifications;

namespace Parrot.client.Mods.Jarvis
{
    public class JarvisRunner : MonoBehaviour
    {
        public static JarvisRunner Instance;
        private AudioSource audioSource;

        private const string ChatModel = "gemini-3.6-flash";
        private const string TtsModel = "gemini-3.1-flash-tts-preview";

        private static readonly List<object> History = new List<object>();
        private const int MaxTurns = 6;

        public static void ClearHistory()
        {
            History.Clear();
        }

        private static void Trim()
        {
            while (History.Count > MaxTurns)
                History.RemoveAt(0);
        }

        public static void Ensure()
        {
            if (Instance != null)
                return;

            GameObject go = new GameObject("ParrotJarvis");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<JarvisRunner>();

            AudioSource src = go.AddComponent<AudioSource>();
            src.spatialBlend = 0f;
            src.volume = 1f;
            src.priority = 0;
            src.bypassEffects = true;
            src.bypassListenerEffects = true;
            src.ignoreListenerPause = true;
            src.ignoreListenerVolume = true;
            Instance.audioSource = src;
        }

        public void Process(byte[] wav)
        {
            StartCoroutine(ChatCoroutine(wav));
        }

        private IEnumerator ChatCoroutine(byte[] wav)
        {
            string key = Jarvis.ApiKey();
            if (string.IsNullOrEmpty(key))
            {
                NotifiLib.SendNotification("Jarvis", "No Gemini key - put it in parrot.client/gemini.txt");
                yield break;
            }

            string prompt = Jarvis.SystemPrompt + VoiceCommands.PromptSection();
            string b64 = Convert.ToBase64String(wav);

            object userTurn = new
            {
                role = "user",
                parts = new object[]
                {
                    new { inline_data = new { mime_type = "audio/wav", data = b64 } }
                }
            };

            var contents = new List<object>(History);
            contents.Add(userTurn);

            var body = new
            {
                systemInstruction = new { parts = new[] { new { text = prompt } } },
                contents = contents.ToArray()
            };

            string url = "https://generativelanguage.googleapis.com/v1beta/models/" + ChatModel + ":generateContent?key=" + key;
            string reply = null;

            using (UnityWebRequest req = Post(url, JsonConvert.SerializeObject(body)))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    NotifiLib.SendNotification("Jarvis", FormatError(req));
                    yield break;
                }

                reply = ExtractText(req.downloadHandler.text);
            }

            if (string.IsNullOrEmpty(reply))
            {
                NotifiLib.SendNotification("Jarvis", "No response.");
                yield break;
            }

            History.Add(userTurn);
            History.Add(new { role = "model", parts = new object[] { new { text = reply } } });
            Trim();

            string actions = VoiceCommands.RunFromResponse(reply);
            string chat = StripRunLines(reply);

            string spoken;
            if (!string.IsNullOrEmpty(actions))
                spoken = string.IsNullOrEmpty(chat) ? actions : actions + " " + chat;
            else
                spoken = chat;

            if (string.IsNullOrEmpty(spoken))
                yield break;

            if (Jarvis.Speak)
                yield return TtsCoroutine(spoken, key);
            else
                NotifiLib.SendNotification("Jarvis", spoken);
        }

        private IEnumerator TtsCoroutine(string text, string key)
        {
            var body = new
            {
                contents = new[] { new { parts = new[] { new { text = text } } } },
                generationConfig = new
                {
                    responseModalities = new[] { "AUDIO" },
                    speechConfig = new { voiceConfig = new { prebuiltVoiceConfig = new { voiceName = Jarvis.Voice } } }
                }
            };

            string url = "https://generativelanguage.googleapis.com/v1beta/models/" + TtsModel + ":generateContent?key=" + key;

            using (UnityWebRequest req = Post(url, JsonConvert.SerializeObject(body)))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                {
                    NotifiLib.SendNotification("Jarvis", text);
                    yield break;
                }

                string b64 = ExtractAudio(req.downloadHandler.text);
                if (string.IsNullOrEmpty(b64))
                {
                    NotifiLib.SendNotification("Jarvis", text);
                    yield break;
                }

                byte[] pcm;
                try { pcm = Convert.FromBase64String(b64); }
                catch { NotifiLib.SendNotification("Jarvis", text); yield break; }

                AudioClip clip = WavUtil.PcmToClip(pcm, 24000, 1);
                if (clip == null)
                {
                    NotifiLib.SendNotification("Jarvis", text);
                    yield break;
                }

                NotifiLib.SendNotification("Jarvis", text);
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        private static UnityWebRequest Post(string url, string json)
        {
            UnityWebRequest req = new UnityWebRequest(url, "POST");
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }

        private static string FormatError(UnityWebRequest req)
        {
            long code = req.responseCode;

            string detail = null;
            try { detail = (string)JObject.Parse(req.downloadHandler.text)["error"]?["message"]; }
            catch { }

            if (code == 429)
                return "Too many requests - slow down and try again in a bit.";

            if (!string.IsNullOrEmpty(detail))
            {
                string low = detail.ToLowerInvariant();
                if (low.Contains("api key") || low.Contains("api_key") || low.Contains("permission") || low.Contains("not valid"))
                    return "Invalid Gemini key - get a real one (AIza...) at aistudio.google.com.";
                return "Gemini (" + code + "): " + detail;
            }

            if (code == 403)
                return "Access denied - enable the Generative Language API for your key.";
            if (code == 503)
                return "Gemini is overloaded right now. Try again in a moment.";

            return "Request failed (" + code + "): " + req.error;
        }

        private static string ExtractText(string resp)
        {
            try
            {
                JArray parts = JObject.Parse(resp)["candidates"]?[0]?["content"]?["parts"] as JArray;
                if (parts == null) return null;
                foreach (JToken p in parts)
                {
                    string t = (string)p["text"];
                    if (!string.IsNullOrEmpty(t)) return t;
                }
            }
            catch { }
            return null;
        }

        private static string ExtractAudio(string resp)
        {
            try
            {
                JArray parts = JObject.Parse(resp)["candidates"]?[0]?["content"]?["parts"] as JArray;
                if (parts == null) return null;
                foreach (JToken p in parts)
                {
                    string d = (string)p["inlineData"]?["data"];
                    if (!string.IsNullOrEmpty(d)) return d;
                }
            }
            catch { }
            return null;
        }

        private static string StripRunLines(string reply)
        {
            var sb = new StringBuilder();
            foreach (string raw in reply.Split('\n'))
            {
                if (raw.ToUpperInvariant().Contains("RUN:"))
                    continue;
                sb.Append(raw.Trim());
                sb.Append(' ');
            }
            return sb.ToString().Trim();
        }
    }
}
