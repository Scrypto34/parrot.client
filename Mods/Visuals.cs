using GorillaLocomotion;
using System.Collections.Generic;
using UnityEngine;
using static Parrot.client.Menu.Main;
using static Parrot.client.Settings;

namespace Parrot.client.Mods
{
    public class Visuals
    {
        private static readonly List<LineRenderer> pool = new List<LineRenderer>();
        private static int used;

        public static void CasualTracers()
        {
            used = 0;

            if (GorillaGameManager.instance == null || GorillaTagger.Instance == null)
            {
                HideTracers();
                return;
            }

            bool followMenuTheme = GetIndex("Follow Menu Theme")?.enabled ?? false;
            bool transparentTheme = GetIndex("Transparent Theme")?.enabled ?? false;
            bool thin = GetIndex("Thin Tracers")?.enabled ?? false;
            bool scaleWithPlayer = GetIndex("Scale With Player")?.enabled ?? false;

            float lineWidth = (thin ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
            Color menuColor = backgroundColor.GetCurrentColor();
            Vector3 handPos = GorillaTagger.Instance.rightHandTransform.position;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isLocal || rig.isOfflineVRRig)
                    continue;

                Color color = followMenuTheme ? menuColor : rig.playerColor;
                if (transparentTheme)
                    color.a = 0.5f;

                LineRenderer line = GetLine();
                line.startColor = color;
                line.endColor = color;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.SetPosition(0, handPos);
                line.SetPosition(1, rig.transform.position);
            }

            for (int i = used; i < pool.Count; i++)
                if (pool[i] != null)
                    pool[i].gameObject.SetActive(false);
        }

        public static void HideTracers()
        {
            for (int i = 0; i < pool.Count; i++)
                if (pool[i] != null)
                    pool[i].gameObject.SetActive(false);
        }

        private static readonly List<LineRenderer> espPool = new List<LineRenderer>();
        private static int espUsed;

        public static void PlayerESP()
        {
            espUsed = 0;

            if (GorillaTagger.Instance == null)
            {
                HideESP();
                return;
            }

            Camera cam = Camera.main;
            if (cam == null && GorillaTagger.Instance.mainCamera != null)
                cam = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
            if (cam == null)
            {
                HideESP();
                return;
            }

            bool menuColorOn = GetIndex("ESP Menu Color")?.enabled ?? false;
            Color menuColor = backgroundColor.GetCurrentColor();

            Vector3 right = cam.transform.right;
            Vector3 up = cam.transform.up;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isLocal || rig.isOfflineVRRig)
                    continue;

                Color color = menuColorOn ? menuColor : rig.playerColor;

                Vector3 center = rig.transform.position + Vector3.up * 0.9f;
                float hw = 0.45f;
                float hh = 0.95f;

                Vector3 c0 = center - right * hw + up * hh;
                Vector3 c1 = center + right * hw + up * hh;
                Vector3 c2 = center + right * hw - up * hh;
                Vector3 c3 = center - right * hw - up * hh;

                LineRenderer line = GetESPLine();
                line.startColor = color;
                line.endColor = color;
                line.startWidth = 0.02f;
                line.endWidth = 0.02f;
                line.positionCount = 5;
                line.SetPosition(0, c0);
                line.SetPosition(1, c1);
                line.SetPosition(2, c2);
                line.SetPosition(3, c3);
                line.SetPosition(4, c0);
            }

            for (int i = espUsed; i < espPool.Count; i++)
                if (espPool[i] != null)
                    espPool[i].gameObject.SetActive(false);
        }

        public static void HideESP()
        {
            for (int i = 0; i < espPool.Count; i++)
                if (espPool[i] != null)
                    espPool[i].gameObject.SetActive(false);
        }

        private static LineRenderer GetESPLine()
        {
            LineRenderer line;
            if (espUsed < espPool.Count && espPool[espUsed] != null)
            {
                line = espPool[espUsed];
            }
            else
            {
                GameObject go = new GameObject("ParrotESP");
                line = go.AddComponent<LineRenderer>();
                line.positionCount = 5;
                line.useWorldSpace = true;
                line.loop = false;
                line.material = new Material(Shader.Find("GUI/Text Shader"));
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.receiveShadows = false;

                if (espUsed < espPool.Count)
                    espPool[espUsed] = line;
                else
                    espPool.Add(line);
            }

            line.gameObject.SetActive(true);
            espUsed++;
            return line;
        }

        private static LineRenderer GetLine()
        {
            LineRenderer line;
            if (used < pool.Count && pool[used] != null)
            {
                line = pool[used];
            }
            else
            {
                GameObject go = new GameObject("ParrotTracer");
                line = go.AddComponent<LineRenderer>();
                line.positionCount = 2;
                line.useWorldSpace = true;
                line.material = new Material(Shader.Find("GUI/Text Shader"));
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.receiveShadows = false;

                if (used < pool.Count)
                    pool[used] = line;
                else
                    pool.Add(line);
            }

            line.gameObject.SetActive(true);
            used++;
            return line;
        }
    }
}
