using Parrot.client.Classes;
using UnityEngine;

namespace Parrot.client.Mods.Console
{

    public class Sahur
    {
        private const string ModelFile = "Tung Tung Tung Sahur.obj";
        private const string TextureFile = "TungTexture.png";
        private const float ModelSize = 1.6f;

        private static GameObject local;

        private static readonly Color Wood = new Color(0.45f, 0.28f, 0.13f);
        private static readonly Color DarkWood = new Color(0.30f, 0.18f, 0.08f);
        private static readonly Color Skin = new Color(0.80f, 0.62f, 0.42f);

        public static void Run()
        {
            if (!OwnerList.HasAccess())
            {
                Despawn();
                return;
            }

            if (local == null)
            {
                Transform hand = GorillaTagger.Instance?.rightHandTransform;
                if (hand == null)
                    return;

                local = Attach(hand);
            }

            SahurSync.SetActive(true);
        }

        public static void Despawn()
        {
            SahurSync.SetActive(false);

            if (local != null)
                Object.Destroy(local);

            local = null;
        }

        public static GameObject Attach(Transform parent)
        {
            GameObject root = new GameObject("TungTungSahur");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            root.transform.localRotation = Quaternion.identity;

            GameObject model = ObjLoader.Load(ModelFile, ModelSize, Wood, TextureFile);
            if (model == null)
                model = Build();

            model.transform.SetParent(root.transform, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            return root;
        }

        private static GameObject Build()
        {
            GameObject figure = new GameObject("SahurFigure");
            figure.transform.localScale = Vector3.one * ModelSize;

            MakePart(figure.transform, PrimitiveType.Cylinder, new Vector3(0f, 0.7f, 0f), new Vector3(0.5f, 0.7f, 0.5f), Vector3.zero, Wood);
            MakePart(figure.transform, PrimitiveType.Cylinder, new Vector3(0f, 1.35f, 0f), new Vector3(0.42f, 0.12f, 0.42f), Vector3.zero, DarkWood);
            MakePart(figure.transform, PrimitiveType.Cylinder, new Vector3(0f, 0.02f, 0f), new Vector3(0.52f, 0.1f, 0.52f), Vector3.zero, DarkWood);

            MakePart(figure.transform, PrimitiveType.Sphere, new Vector3(0f, 1.05f, 0.4f), new Vector3(0.6f, 0.62f, 0.18f), Vector3.zero, Skin);

            MakePart(figure.transform, PrimitiveType.Sphere, new Vector3(-0.18f, 1.16f, 0.5f), new Vector3(0.2f, 0.24f, 0.12f), Vector3.zero, Color.white);
            MakePart(figure.transform, PrimitiveType.Sphere, new Vector3(0.18f, 1.16f, 0.5f), new Vector3(0.2f, 0.24f, 0.12f), Vector3.zero, Color.white);
            MakePart(figure.transform, PrimitiveType.Sphere, new Vector3(-0.18f, 1.14f, 0.56f), new Vector3(0.09f, 0.09f, 0.06f), Vector3.zero, Color.black);
            MakePart(figure.transform, PrimitiveType.Sphere, new Vector3(0.18f, 1.14f, 0.56f), new Vector3(0.09f, 0.09f, 0.06f), Vector3.zero, Color.black);

            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(-0.18f, 1.33f, 0.5f), new Vector3(0.24f, 0.05f, 0.06f), new Vector3(0f, 0f, 18f), DarkWood);
            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(0.18f, 1.33f, 0.5f), new Vector3(0.24f, 0.05f, 0.06f), new Vector3(0f, 0f, -18f), DarkWood);

            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(0f, 0.86f, 0.48f), new Vector3(0.36f, 0.18f, 0.1f), Vector3.zero, Color.black);
            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(0f, 0.93f, 0.52f), new Vector3(0.32f, 0.05f, 0.05f), Vector3.zero, Color.white);

            MakePart(figure.transform, PrimitiveType.Cylinder, new Vector3(-0.42f, 0.95f, 0f), new Vector3(0.08f, 0.34f, 0.08f), new Vector3(0f, 0f, 65f), Wood);
            MakePart(figure.transform, PrimitiveType.Cylinder, new Vector3(0.42f, 0.95f, 0f), new Vector3(0.08f, 0.34f, 0.08f), new Vector3(0f, 0f, -65f), Wood);

            MakePart(figure.transform, PrimitiveType.Cylinder, new Vector3(-0.16f, -0.1f, 0f), new Vector3(0.09f, 0.22f, 0.09f), Vector3.zero, Wood);
            MakePart(figure.transform, PrimitiveType.Cylinder, new Vector3(0.16f, -0.1f, 0f), new Vector3(0.09f, 0.22f, 0.09f), Vector3.zero, Wood);
            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(-0.16f, -0.32f, 0.06f), new Vector3(0.12f, 0.06f, 0.24f), Vector3.zero, DarkWood);
            MakePart(figure.transform, PrimitiveType.Cube, new Vector3(0.16f, -0.32f, 0.06f), new Vector3(0.12f, 0.06f, 0.24f), Vector3.zero, DarkWood);

            return figure;
        }

        private static void MakePart(Transform parent, PrimitiveType type, Vector3 localPos, Vector3 localScale, Vector3 euler, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            Object.Destroy(part.GetComponent<Collider>());

            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPos;
            part.transform.localRotation = Quaternion.Euler(euler);
            part.transform.localScale = localScale;

            Renderer renderer = part.GetComponent<Renderer>();
            renderer.sharedMaterial = PrimitiveMat.Get(color);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
    }
}
