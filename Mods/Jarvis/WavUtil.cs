using System;
using System.IO;
using UnityEngine;

namespace Parrot.client.Mods.Jarvis
{
    public static class WavUtil
    {
        public static byte[] ClipToWav(AudioClip clip)
        {
            int sampleCount = clip.samples * clip.channels;
            float[] data = new float[sampleCount];
            clip.GetData(data, 0);

            short[] pcm = new short[sampleCount];
            for (int i = 0; i < sampleCount; i++)
                pcm[i] = (short)Mathf.Clamp(data[i] * 32767f, -32768f, 32767f);

            byte[] pcmBytes = new byte[pcm.Length * 2];
            Buffer.BlockCopy(pcm, 0, pcmBytes, 0, pcmBytes.Length);

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter w = new BinaryWriter(ms))
            {
                int sampleRate = clip.frequency;
                int channels = clip.channels;
                int byteRate = sampleRate * channels * 2;

                w.Write(new char[] { 'R', 'I', 'F', 'F' });
                w.Write(36 + pcmBytes.Length);
                w.Write(new char[] { 'W', 'A', 'V', 'E' });
                w.Write(new char[] { 'f', 'm', 't', ' ' });
                w.Write(16);
                w.Write((short)1);
                w.Write((short)channels);
                w.Write(sampleRate);
                w.Write(byteRate);
                w.Write((short)(channels * 2));
                w.Write((short)16);
                w.Write(new char[] { 'd', 'a', 't', 'a' });
                w.Write(pcmBytes.Length);
                w.Write(pcmBytes);
                return ms.ToArray();
            }
        }

        public static AudioClip PcmToClip(byte[] pcm, int sampleRate, int channels)
        {
            int sampleCount = pcm.Length / 2;
            if (sampleCount <= 0)
                return null;

            float[] data = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                short s = (short)(pcm[i * 2] | (pcm[i * 2 + 1] << 8));
                data[i] = s / 32768f;
            }

            AudioClip clip = AudioClip.Create("JarvisTTS", sampleCount / channels, channels, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
