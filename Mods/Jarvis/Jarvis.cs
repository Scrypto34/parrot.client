using System.IO;
using BepInEx;
using UnityEngine;
using Parrot.client.Classes;
using Parrot.client.Notifications;

namespace Parrot.client.Mods.Jarvis
{
    public class Jarvis
    {
        public static KeyCode PushToTalk = KeyCode.X;
        public static bool Speak = true;
        public static string Voice = "Charon";

        public const string SystemPrompt =
            "You are Jarvis, a friendly voice assistant living inside the VR game Gorilla Tag, part of the parrot.client mod menu. " +
            "The user talks to you with their microphone. Talk with them like a normal person - answer any question, hold a conversation, " +
            "crack a joke, whatever they want. You can ALSO turn mods on and off in the menu when they ask you to. " +
            "Keep replies to one or two short spoken sentences. No markdown, no emojis, no lists.";

        private const int MaxSeconds = 15;

        private static bool recording;
        private static AudioClip micClip;
        private static string micDevice;

        public static void Tick()
        {
            JarvisRunner.Ensure();

            bool held = UnityInput.Current.GetKey(PushToTalk) ||
                (ControllerInputPoller.instance != null && ControllerInputPoller.instance.leftControllerPrimaryButton);

            if (held && !recording)
                StartRecording();
            else if (!held && recording)
                StopAndSend();
        }

        public static void Stop()
        {
            if (recording && micDevice != null)
                Microphone.End(micDevice);
            recording = false;
            JarvisRunner.ClearHistory();
        }

        private static void StartRecording()
        {
            if (Microphone.devices == null || Microphone.devices.Length == 0)
            {
                NotifiLib.SendNotification("Jarvis", "No microphone found.");
                return;
            }

            micDevice = Microphone.devices[0];
            micClip = Microphone.Start(micDevice, false, MaxSeconds, 16000);
            recording = true;
        }

        private static void StopAndSend()
        {
            recording = false;
            int pos = Microphone.GetPosition(micDevice);
            Microphone.End(micDevice);

            if (micClip == null || pos <= 0)
                return;

            float[] data = new float[pos * micClip.channels];
            micClip.GetData(data, 0);

            AudioClip trimmed = AudioClip.Create("JarvisMic", pos, micClip.channels, micClip.frequency, false);
            trimmed.SetData(data, 0);

            byte[] wav = WavUtil.ClipToWav(trimmed);
            if (JarvisRunner.Instance != null)
                JarvisRunner.Instance.Process(wav);
        }

        public static string ApiKey()
        {
            try
            {
                string path = Path.Combine(ClientFiles.RootPath ?? "", "gemini.txt");
                return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
            }
            catch { return null; }
        }
    }
}
