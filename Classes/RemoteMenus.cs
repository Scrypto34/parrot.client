using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Parrot.client.Classes
{
    public class RemoteMenus
    {
        private const float Width = 0.42f;
        private const float RowHeight = 0.05f;
        private const float TitleHeight = 0.08f;

        private static readonly Dictionary<int, GameObject> panels = new Dictionary<int, GameObject>();
        private static readonly Dictionary<int, string> signatures = new Dictionary<int, string>();

        public static void Tick()
        {
            if (!PhotonNetwork.InRoom)
            {
                ClearAll();
                return;
            }

            Transform cam = CameraTransform();
            HashSet<int> seen = new HashSet<int>();

            foreach (KeyValuePair<int, ClientSync.PlayerState> pair in ClientSync.states)
            {
                ClientSync.PlayerState state = pair.Value;
                if (!state.menuOpen)
                    continue;

                Player owner = FindPlayer(pair.Key);
                if (owner == null)
                    continue;

                VRRig rig = RigManager.GetVRRigFromPlayer(owner);
                if (rig == null || rig.rightHandTransform == null)
                    continue;

                seen.Add(pair.Key);

                string sig = Signature(state);
                if (!panels.TryGetValue(pair.Key, out GameObject panel) || panel == null || !signatures.TryGetValue(pair.Key, out string old) || old != sig)
                {
                    panel = Build(pair.Key, state);
                    signatures[pair.Key] = sig;
                }

                if (panel.transform.parent != rig.rightHandTransform)
                    panel.transform.SetParent(rig.rightHandTransform, false);
                panel.transform.localPosition = new Vector3(0f, 0.32f, 0f);
                if (cam != null)
                    panel.transform.rotation = Quaternion.LookRotation(panel.transform.position - cam.position, Vector3.up);
            }

            List<int> drop = null;
            foreach (int actor in panels.Keys)
                if (!seen.Contains(actor))
                    (drop ?? (drop = new List<int>())).Add(actor);

            if (drop != null)
                foreach (int actor in drop)
                    Remove(actor);
        }

        private static GameObject Build(int actor, ClientSync.PlayerState state)
        {
            if (panels.TryGetValue(actor, out GameObject old) && old != null)
                Object.Destroy(old);

            int count = Mathf.Min(state.menuLabels.Count, 8);
            float height = TitleHeight + count * RowHeight + 0.03f;
            float top = height * 0.5f;

            GameObject root = new GameObject("RemoteMenu");

            MakeQuad(root.transform, new Vector3(0f, 0f, 0.004f), new Vector3(Width, height, 1f), state.bgColor, 1f);

            MakeText(root.transform, new Vector3(0f, top - 0.035f, 0f), state.name + "  [" + state.theme + "]",
                0.014f, state.textEnabled, TextAnchor.MiddleCenter);

            float rowY = top - TitleHeight;
            for (int i = 0; i < count; i++)
            {
                bool on = i < state.menuEnabled.Count && state.menuEnabled[i];
                Color btn = on ? state.btnEnabled : state.btnDisabled;
                Color txt = on ? state.textEnabled : state.textDisabled;

                MakeQuad(root.transform, new Vector3(0f, rowY, 0.002f), new Vector3(Width - 0.03f, RowHeight - 0.01f, 1f), btn, 1f);
                MakeText(root.transform, new Vector3(0f, rowY, 0f), state.menuLabels[i], 0.013f, txt, TextAnchor.MiddleCenter);

                rowY -= RowHeight;
            }

            panels[actor] = root;
            return root;
        }

        private static void MakeQuad(Transform parent, Vector3 localPos, Vector3 scale, Color color, float alpha)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.Destroy(quad.GetComponent<Collider>());
            quad.transform.SetParent(parent, false);
            quad.transform.localPosition = localPos;
            quad.transform.localScale = scale;

            Renderer renderer = quad.GetComponent<Renderer>();
            Shader shader = Shader.Find("GUI/Text Shader");
            if (shader != null)
                renderer.material.shader = shader;
            renderer.material.color = new Color(color.r, color.g, color.b, alpha);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        private static void MakeText(Transform parent, Vector3 localPos, string text, float size, Color color, TextAnchor anchor)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.identity;

            TextMesh tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = size;
            tm.fontSize = 60;
            tm.anchor = anchor;
            tm.alignment = TextAlignment.Center;
            tm.color = color;
            tm.GetComponent<Renderer>().material.renderQueue = 3001;
        }

        private static string Signature(ClientSync.PlayerState state)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(state.pageNumber).Append('|').Append(state.name).Append('|').Append(state.theme).Append('|');
            sb.Append(ColorUtility.ToHtmlStringRGB(state.bgColor)).Append(ColorUtility.ToHtmlStringRGB(state.textEnabled)).Append('|');
            for (int i = 0; i < state.menuLabels.Count; i++)
            {
                sb.Append(i < state.menuEnabled.Count && state.menuEnabled[i] ? '1' : '0');
                sb.Append(state.menuLabels[i]).Append(';');
            }
            return sb.ToString();
        }

        private static Player FindPlayer(int actor)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
                if (p.ActorNumber == actor)
                    return p;
            return null;
        }

        private static Transform CameraTransform()
        {
            if (GorillaTagger.Instance != null && GorillaTagger.Instance.mainCamera != null)
                return GorillaTagger.Instance.mainCamera.transform;
            return Camera.main != null ? Camera.main.transform : null;
        }

        private static void Remove(int actor)
        {
            if (panels.TryGetValue(actor, out GameObject go) && go != null)
                Object.Destroy(go);
            panels.Remove(actor);
            signatures.Remove(actor);
        }

        private static void ClearAll()
        {
            if (panels.Count == 0)
                return;
            foreach (GameObject go in panels.Values)
                if (go != null)
                    Object.Destroy(go);
            panels.Clear();
            signatures.Clear();
        }
    }
}
