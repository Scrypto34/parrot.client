using Parrot.client.Classes;
using UnityEngine;

namespace Parrot.client.Mods.Console
{
    public class TravisScott
    {
        private const string BundleFile = "travis scott.bundle";
        private const float Scale = 5f;

        public static readonly Vector3 Position = new Vector3(-76f, 1.7f, -80f);
        public static readonly Quaternion Rotation = Quaternion.Euler(0f, 40f, 0f);

        private static readonly Color Dark = new Color(0.12f, 0.12f, 0.15f);
        private static readonly Color Skin = new Color(0.55f, 0.42f, 0.32f);

        private static AssetBundle bundle;
        private static GameObject prefab;
        private static AnimationClip[] clips;

        private static GameObject local;

        private static void Log(string message) =>
            Debug.Log($"{PluginInfo.Name} // TravisScott: {message}");

        public static void Run()
        {
            if (!OwnerList.HasAccess())
            {
                Despawn();
                return;
            }

            if (local == null)
                local = Spawn();

            CartiSync.SetActive(true);
        }

        public static void Despawn()
        {
            CartiSync.SetActive(false);

            if (local != null)
                Object.Destroy(local);

            local = null;
        }

        public static GameObject Spawn()
        {
            GameObject root = new GameObject("TravisScott");
            root.transform.position = Position;
            root.transform.rotation = Rotation;
            root.transform.localScale = Vector3.one * Scale;

            GameObject model = LoadFromBundle();
            if (model != null)
            {
                model.transform.SetParent(root.transform, false);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
            }
            else
            {
                GameObject placeholder = BuildPrimitive();
                placeholder.transform.SetParent(root.transform, false);

                TwerkAnim anim = root.AddComponent<TwerkAnim>();
                anim.basePosition = Position;
                anim.baseRotation = Rotation;
            }

            return root;
        }

        private static GameObject LoadFromBundle()
        {
            try
            {
                string path = System.IO.Path.Combine(ClientFiles.ConsolePath ?? "", BundleFile);
                if (!System.IO.File.Exists(path))
                {
                    Log("bundle file NOT found at " + path);
                    return null;
                }

                if (bundle == null)
                {
                    bundle = AssetBundle.LoadFromFile(path);
                    if (bundle == null)
                    {
                        Log("LoadFromFile returned null - bundle likely built for a different Unity version");
                        return null;
                    }
                    Log("bundle loaded ok");
                }

                if (prefab == null)
                {
                    GameObject[] gameObjects = bundle.LoadAllAssets<GameObject>();
                    Log("GameObjects in bundle = " + gameObjects.Length);

                    foreach (GameObject go in gameObjects)
                    {
                        if (go.GetComponentInChildren<Renderer>() != null)
                        {
                            prefab = go;
                            break;
                        }
                    }
                    if (prefab == null && gameObjects.Length > 0)
                        prefab = gameObjects[0];

                    clips = bundle.LoadAllAssets<AnimationClip>();
                    Log("AnimationClips = " + (clips != null ? clips.Length : 0) + ", prefab = " + (prefab != null ? prefab.name : "null"));
                }

                if (prefab == null)
                {
                    Log("no usable prefab in bundle");
                    return null;
                }

                GameObject instance = Object.Instantiate(prefab);
                PlayAnimation(instance);
                return instance;
            }
            catch (System.Exception e)
            {
                Log("exception: " + e.Message);
                return null;
            }
        }

        private static void PlayAnimation(GameObject instance)
        {
            try
            {
                Animator animator = instance.GetComponentInChildren<Animator>();
                if (animator != null && animator.runtimeAnimatorController != null)
                {
                    animator.enabled = true;
                    animator.Play(0, 0, 0f);
                    Log("playing via Animator");
                    return;
                }

                Animation legacy = instance.GetComponentInChildren<Animation>();
                if (legacy != null && legacy.GetClipCount() > 0)
                {
                    legacy.wrapMode = WrapMode.Loop;
                    foreach (AnimationState state in legacy)
                        state.wrapMode = WrapMode.Loop;
                    legacy.Play();
                    Log("playing via existing Animation");
                    return;
                }

                if (clips != null && clips.Length > 0)
                {
                    AnimationClip clip = clips[clips.Length - 1];
                    try { clip.legacy = true; } catch { }

                    Animation anim = instance.GetComponent<Animation>();
                    if (anim == null)
                        anim = instance.AddComponent<Animation>();

                    anim.AddClip(clip, clip.name);
                    anim.clip = clip;
                    anim.wrapMode = WrapMode.Loop;
                    anim.playAutomatically = true;
                    anim.Play(clip.name);
                    Log("attached clip '" + clip.name + "' and playing");
                    return;
                }

                Log("no animation to play");
            }
            catch (System.Exception e)
            {
                Log("PlayAnimation exception: " + e.Message);
            }
        }

        private static GameObject BuildPrimitive()
        {
            GameObject figure = new GameObject("TravisPlaceholder");

            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(0f, 1.0f, 0f), new Vector3(0.55f, 0.75f, 0.32f), Dark);
            MakePart(figure.transform, PrimitiveType.Sphere, new Vector3(0f, 1.6f, 0f), new Vector3(0.42f, 0.45f, 0.42f), Skin);
            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(-0.18f, 0.35f, 0f), new Vector3(0.2f, 0.75f, 0.22f), Dark);
            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(0.18f, 0.35f, 0f), new Vector3(0.2f, 0.75f, 0.22f), Dark);
            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(-0.4f, 1.05f, 0f), new Vector3(0.16f, 0.6f, 0.16f), Dark);
            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(0.4f, 1.05f, 0f), new Vector3(0.16f, 0.6f, 0.16f), Dark);

            return figure;
        }

        private static void MakePart(Transform parent, PrimitiveType type, Vector3 localPos, Vector3 localScale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            Object.Destroy(part.GetComponent<Collider>());

            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPos;
            part.transform.localScale = localScale;

            Renderer renderer = part.GetComponent<Renderer>();
            renderer.sharedMaterial = PrimitiveMat.Get(color);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
    }
}
