using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parrot.client.Mods
{
    internal class Sound
    {
        public static void PlaySound(int index)
        {
            if (!ControllerInputPoller.instance.rightControllerTriggerButton) return;
            if (!PhotonNetwork.InRoom) { GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(index, false, float.MaxValue); return; }
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, index, false, float.MaxValue);
        }
        public static void SIUnlockAll() =>
            Array.ForEach(SIProgression.Instance.unlockedTechTreeData, g => Array.Fill(g, true));
        public static void MetalSpam() => PlaySound(18);
        public static void HugeCrystalSpam() => PlaySound(213);
        public static void AK47Spam() => PlaySound(203);
        public static void RandomSpam() => PlaySound(UnityEngine.Random.Range(0, 259));
        public static void JmanSpam()
        {
            PlaySound(337);
        }
    }
}
