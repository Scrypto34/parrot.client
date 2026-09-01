using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Parrot.client.Classes
{

    public class AudioLib : MonoBehaviour
    {
        public static float volume = 1f;

        private static readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        private static readonly HashSet<string> loading = new HashSet<string>();
        private static readonly HashSet<string> warned = new HashSet<string>();

        private static AudioLib instance;
        private static AudioSource source;

        private static AudioLib Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject holder = new GameObject("ParrotAudio");
                    DontDestroyOnLoad(holder);
                    holder.hideFlags = HideFlags.HideAndDontSave;

                    instance = holder.AddComponent<AudioLib>();

                    source = holder.AddComponent<AudioSource>();
                    source.playOnAwake = false;
                    source.spatialBlend = 0f;
                    source.bypassEffects = true;
                    source.bypassReverbZones = true;
                }

                return instance;
            }
        }

        public static void Play(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            AudioClip clip = Preload(fileName);
            if (clip != null)
                source.PlayOneShot(clip, volume);
        }

        public static void PlayFile(string path)
        {
            if (string.IsNullOrEmpty(path) || Instance == null)
                return;

            if (clips.TryGetValue(path, out AudioClip cached))
            {
                if (cached != null)
                    source.PlayOneShot(cached, volume);
                return;
            }

            if (!File.Exists(path))
            {
                Warn($"sound not found: {path}");
                clips[path] = null;
                return;
            }

            if (GetExtension(path) == "wav")
            {
                AudioClip clip = null;
                try { clip = WavToClip(File.ReadAllBytes(path), Path.GetFileName(path)); }
                catch (Exception exc) { Warn($"could not read {path}: {exc.Message}"); }

                if (clip == null)
                    Warn($"{Path.GetFileName(path)} is not a wav this can read (try 16 bit PCM)");

                clips[path] = clip;
                if (clip != null)
                    source.PlayOneShot(clip, volume);
                return;
            }

            if (loading.Add(path))
                instance.StartCoroutine(LoadFileAndPlay(path));
        }

        private static IEnumerator LoadFileAndPlay(string path)
        {
            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip($"file://{path}", GetAudioType(path)))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                    clips[path] = clip;
                    if (clip != null)
                        source.PlayOneShot(clip, volume);
                }
                else
                {
                    Warn($"could not load {Path.GetFileName(path)}: {request.error}");
                }
            }

            loading.Remove(path);
        }

        public static void PlayFileMic(string path)
        {
            if (string.IsNullOrEmpty(path) || Instance == null)
                return;

            EnsureClip(path, clip =>
            {
                if (clip != null)
                    instance.StartCoroutine(PlayThroughMic(clip));
            });
        }

        private static void EnsureClip(string path, Action<AudioClip> onReady)
        {
            if (clips.TryGetValue(path, out AudioClip cached))
            {
                onReady(cached);
                return;
            }

            if (!File.Exists(path))
            {
                Warn($"sound not found: {path}");
                clips[path] = null;
                onReady(null);
                return;
            }

            if (GetExtension(path) == "wav")
            {
                AudioClip clip = null;
                try { clip = WavToClip(File.ReadAllBytes(path), Path.GetFileName(path)); }
                catch (Exception exc) { Warn($"could not read {path}: {exc.Message}"); }

                clips[path] = clip;
                onReady(clip);
                return;
            }

            instance.StartCoroutine(LoadFileClip(path, onReady));
        }

        private static IEnumerator LoadFileClip(string path, Action<AudioClip> onReady)
        {
            using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip($"file://{path}", GetAudioType(path));
            yield return request.SendWebRequest();

            AudioClip clip = request.result == UnityWebRequest.Result.Success
                ? DownloadHandlerAudioClip.GetContent(request)
                : null;

            if (clip == null)
                Warn($"could not load {Path.GetFileName(path)}: {request.error}");

            clips[path] = clip;
            onReady?.Invoke(clip);
        }

        private static IEnumerator PlayThroughMic(AudioClip clip)
        {
            Recorder recorder = GorillaTagger.Instance?.myRecorder;
            if (recorder == null)
                yield break;

            recorder.SourceType = Recorder.InputSourceType.AudioClip;
            recorder.AudioClip = clip;
            recorder.RestartRecording(true);
            recorder.DebugEchoMode = true;

            yield return new WaitForSeconds(clip.length + 0.3f);

            recorder.DebugEchoMode = false;
            recorder.SourceType = Recorder.InputSourceType.Microphone;
            recorder.AudioClip = null;
            recorder.RestartRecording(true);
        }

        public static AudioClip Preload(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            if (Instance == null)
                return null;

            if (clips.TryGetValue(fileName, out AudioClip cached))
                return cached;

            byte[] bytes = EmbeddedResources.Read(fileName);
            if (bytes == null)
            {
                Warn($"{fileName} was not found in Resources/Server/Audio");
                clips[fileName] = null;
                return null;
            }

            if (GetExtension(fileName) == "wav")
            {
                AudioClip clip = WavToClip(bytes, fileName);
                if (clip == null)
                    Warn($"{fileName} is not a wav file this can read (try 16 bit PCM)");

                clips[fileName] = clip;
                return clip;
            }

            if (loading.Add(fileName))
                instance.StartCoroutine(LoadCompressed(fileName, bytes));

            return null;
        }

        private static IEnumerator LoadCompressed(string fileName, byte[] bytes)
        {
            string path = EmbeddedResources.Unpack(fileName, bytes);

            if (path != null)
            {
                using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip($"file://{path}", GetAudioType(fileName));
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                    clips[fileName] = DownloadHandlerAudioClip.GetContent(request);
                else
                    Warn($"Could not load {fileName}: {request.error}");
            }

            loading.Remove(fileName);
        }

        private static string GetExtension(string fileName)
        {
            int dot = fileName.LastIndexOf('.');
            return dot < 0 ? "" : fileName.Substring(dot + 1).ToLowerInvariant();
        }

        private static AudioType GetAudioType(string fileName)
        {
            switch (GetExtension(fileName))
            {
                case "mp3": return AudioType.MPEG;
                case "ogg": return AudioType.OGGVORBIS;
                case "aiff":
                case "aif": return AudioType.AIFF;
                default: return AudioType.WAV;
            }
        }

        private static void Warn(string message)
        {
            if (warned.Add(message))
                Debug.LogWarning($"{PluginInfo.Name} // Audio: {message}");
        }

        private static AudioClip WavToClip(byte[] data, string name)
        {
            if (data.Length < 44 || Encoding.ASCII.GetString(data, 0, 4) != "RIFF")
                return null;

            int channels = 1;
            int sampleRate = 44100;
            int bitsPerSample = 16;
            int formatTag = 1;
            int dataOffset = -1;
            int dataLength = 0;

            int position = 12;
            while (position + 8 <= data.Length)
            {
                string chunkId = Encoding.ASCII.GetString(data, position, 4);
                int chunkSize = BitConverter.ToInt32(data, position + 4);
                int chunkStart = position + 8;

                if (chunkSize < 0)
                    break;

                if (chunkId == "fmt " && chunkStart + 16 <= data.Length)
                {
                    formatTag = BitConverter.ToInt16(data, chunkStart);
                    channels = BitConverter.ToInt16(data, chunkStart + 2);
                    sampleRate = BitConverter.ToInt32(data, chunkStart + 4);
                    bitsPerSample = BitConverter.ToInt16(data, chunkStart + 14);
                }
                else if (chunkId == "data")
                {
                    dataOffset = chunkStart;
                    dataLength = Mathf.Min(chunkSize, data.Length - chunkStart);
                }

                position = chunkStart + chunkSize + (chunkSize % 2);
            }

            if (dataOffset < 0 || dataLength <= 0 || channels <= 0 || bitsPerSample < 8)
                return null;

            int bytesPerSample = bitsPerSample / 8;
            int sampleCount = dataLength / bytesPerSample;
            if (sampleCount < channels)
                return null;

            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                int offset = dataOffset + i * bytesPerSample;

                switch (bitsPerSample)
                {
                    case 8:
                        samples[i] = (data[offset] - 128) / 128f;
                        break;
                    case 16:
                        samples[i] = BitConverter.ToInt16(data, offset) / 32768f;
                        break;
                    case 24:
                        int packed = (data[offset + 2] << 16) | (data[offset + 1] << 8) | data[offset];
                        samples[i] = ((packed << 8) >> 8) / 8388608f;
                        break;
                    case 32:
                        samples[i] = formatTag == 3
                            ? BitConverter.ToSingle(data, offset)
                            : BitConverter.ToInt32(data, offset) / 2147483648f;
                        break;
                    default:
                        return null;
                }
            }

            AudioClip clip = AudioClip.Create(name, sampleCount / channels, channels, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
