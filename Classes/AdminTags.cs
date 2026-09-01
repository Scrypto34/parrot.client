using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace Parrot.client.Classes
{

    public class AdminTags : MonoBehaviour
    {
        public const string LogoFile = "console.png";
        private const float heightAboveHead = 0.6f;
        private const float size = 0.4f;

        public static bool hideOwnCrown;

        public static readonly string[] CrownColorNames = { "Yellow", "Red", "Green", "Grey", "Black", "Blue", "Brown" };
        public static readonly Color[] CrownColorValues =
        {
            new Color(1f, 0.85f, 0.1f),
            new Color(0.9f, 0.15f, 0.15f),
            new Color(0.2f, 0.8f, 0.25f),
            new Color(0.55f, 0.55f, 0.55f),
            new Color(0.06f, 0.06f, 0.06f),
            new Color(0.2f, 0.45f, 0.95f),
            new Color(0.5f, 0.32f, 0.15f)
        };
        public static int crownColorIndex;

        private static AdminTags instance;
        private static Texture2D logo;

        private readonly Dictionary<int, GameObject> tags = new Dictionary<int, GameObject>();
        private readonly List<int> stale = new List<int>();

        public static void Init()
        {
            if (instance != null)
                return;

            GameObject holder = new GameObject("ParrotAdminTags");
            DontDestroyOnLoad(holder);
            holder.hideFlags = HideFlags.HideAndDontSave;
            instance = holder.AddComponent<AdminTags>();
        }

        private void Update()
        {
            if (logo == null)
                logo = ImageLib.Load(LogoFile);

            Transform cam = CameraTransform();

            HashSet<int> seen = new HashSet<int>();

            if (logo != null && VRRigCache.Instance != null)
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig == null || rig.Creator == null || string.IsNullOrEmpty(rig.Creator.UserId))
                        continue;

                    if (!OwnerList.IsOwner(rig.Creator.UserId))
                        continue;

                    bool isSelf = rig.isLocal
                        || (GorillaTagger.Instance != null && rig == GorillaTagger.Instance.offlineVRRig)
                        || (PhotonNetwork.LocalPlayer != null && rig.Creator.UserId == PhotonNetwork.LocalPlayer.UserId);
                    if (isSelf && hideOwnCrown)
                        continue;

                    int actor = rig.Creator.ActorNumber;
                    seen.Add(actor);

                    GameObject tag = GetOrCreate(actor);
                    Vector3 headPos = HeadPosition(rig) + Vector3.up * heightAboveHead;
                    tag.transform.position = headPos;

                    if (cam != null)
                        tag.transform.rotation = Quaternion.LookRotation(cam.forward, cam.up);

                    Renderer tagRenderer = tag.GetComponent<Renderer>();
                    if (tagRenderer != null)
                        tagRenderer.material.color = ColorForActor(isSelf, actor);
                }
            }

            stale.Clear();
            foreach (int actor in tags.Keys)
                if (!seen.Contains(actor))
                    stale.Add(actor);

            foreach (int actor in stale)
            {
                Destroy(tags[actor]);
                tags.Remove(actor);
            }
        }

        private GameObject GetOrCreate(int actor)
        {
            if (tags.TryGetValue(actor, out GameObject existing) && existing != null)
                return existing;

            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(quad.GetComponent<Collider>());
            quad.transform.localScale = Vector3.one * size;

            MeshRenderer renderer = quad.GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            Material mat = new Material(TransparentShader()) { mainTexture = logo };
            mat.SetTexture("_MainTex", logo);
            mat.SetTexture("_BaseMap", logo);
            mat.SetInt("_Cull", 0);
            renderer.material = mat;

            tags[actor] = quad;
            return quad;
        }

        public static void CycleCrownColor()
        {
            crownColorIndex = (crownColorIndex + 1) % CrownColorNames.Length;
            RefreshLabel();
            try { ThemeChanger.SaveConfigSilent(); } catch { }
            try { ClientSync.BroadcastNow(); } catch { }
        }

        public static void RefreshLabel()
        {
            ButtonInfo button = Menu.Main.GetIndex("Crown Color");
            if (button != null)
                button.overlapText = $"Crown Color [{CrownColorNames[Mathf.Clamp(crownColorIndex, 0, CrownColorNames.Length - 1)]}]";
        }

        private static Color ColorForActor(bool isSelf, int actor)
        {
            int idx = 0;
            if (isSelf)
                idx = crownColorIndex;
            else if (ClientSync.states.TryGetValue(actor, out ClientSync.PlayerState state))
                idx = state.crownColor;

            return CrownColorValues[Mathf.Clamp(idx, 0, CrownColorValues.Length - 1)];
        }

        private static Vector3 HeadPosition(VRRig rig) =>
            rig.headMesh != null ? rig.headMesh.transform.position : rig.transform.position;

        private static Transform CameraTransform()
        {
            if (GorillaTagger.Instance != null && GorillaTagger.Instance.mainCamera != null)
                return GorillaTagger.Instance.mainCamera.transform;

            return Camera.main != null ? Camera.main.transform : null;
        }

        private static Shader TransparentShader() =>
            Shader.Find("Sprites/Default")
            ?? Shader.Find("Unlit/Transparent")
            ?? Shader.Find("UI/Default");
    }
}
