using Parrot.client.Classes;
using UnityEngine;

namespace Parrot.client.Mods.Console
{
    public class Sans
    {
        private const string ModelFile = "Sans.obj";
        private const string TextureFile = "Sans_Tex.png";

        private static readonly float sizeVal = 0.9f;
        private static readonly Vector3 offsetVal = new Vector3(0f, 0.1f, -0.15f);
        private static readonly Vector3 eulerVal = Vector3.zero;

        private static GameObject holder;
        private static GameObject model;

        public static void Run()
        {
            if (!OwnerList.HasAccess())
            {
                Despawn();
                return;
            }

            Spawn();
        }

        private static void Spawn()
        {
            if (holder != null)
                return;

            Transform hand = GorillaTagger.Instance?.rightHandTransform;
            if (hand == null)
                return;

            holder = new GameObject("SansHold");
            holder.transform.SetParent(hand, false);
            holder.transform.localPosition = offsetVal;
            holder.transform.localRotation = Quaternion.Euler(eulerVal);

            model = ObjLoader.Load(ModelFile, sizeVal, Color.white, TextureFile);
            if (model != null)
            {
                model.transform.SetParent(holder.transform, false);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
            }
        }

        public static void Despawn()
        {
            if (holder != null)
                UnityEngine.Object.Destroy(holder);

            holder = null;
            model = null;
        }
    }
}
