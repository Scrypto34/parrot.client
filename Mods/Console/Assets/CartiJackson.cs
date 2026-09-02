using Parrot.client.Classes;
using UnityEngine;

namespace Parrot.client.Mods.Console
{
    public class CartiJackson
    {
        private const float Scale = 5f;

        public static readonly Vector3 Position = new Vector3(-76f, 1.7f, -80f);
        public static readonly Quaternion Rotation = Quaternion.Euler(0f, 40f, 0f);

        private static readonly Color Dark = new Color(0.12f, 0.12f, 0.15f);
        private static readonly Color Skin = new Color(0.55f, 0.42f, 0.32f);

        private static GameObject local;

        public static void Run()
        {
            if (!OwnerList.HasAccess())
            {
                Despawn();
                return;
            }

            if (local == null)
                local = Spawn();

            CartiJacksonSync.SetActive(true);
        }

        public static void Despawn()
        {
            CartiJacksonSync.SetActive(false);

            if (local != null)
                Object.Destroy(local);

            local = null;
        }

        public static GameObject Spawn()
        {
            GameObject figure = BuildPrimitive();

            GameObject root = new GameObject("CartiJackson");
            figure.transform.SetParent(root.transform, false);
            root.transform.position = Position;
            root.transform.rotation = Rotation;
            root.transform.localScale = Vector3.one * Scale;

            TwerkAnim anim = root.AddComponent<TwerkAnim>();
            anim.basePosition = Position;
            anim.baseRotation = Rotation;
            return root;
        }

        private static GameObject BuildPrimitive()
        {
            GameObject figure = new GameObject("CartiFigure");

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
