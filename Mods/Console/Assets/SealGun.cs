using Parrot.client.Classes;
using System.Collections.Generic;
using UnityEngine;
using GL = Parrot.client.GunTools.Gunlib;

namespace Parrot.client.Mods.Console
{

    public class SealGun
    {
        private const string TextureFile = "SealCube.jpeg";
        private const float SealScale = 0.35f;
        private const float Speed = 18f;
        private const float FireRate = 0.18f;
        private const float Lifetime = 20f;
        private const int MaxLive = 25;

        private static float nextFire;
        private static readonly Queue<GameObject> live = new Queue<GameObject>();

        private static readonly Color Body = new Color(0.6f, 0.62f, 0.65f);

        public static void Run()
        {
            if (!OwnerList.HasAccess())
                return;

            GL.StartBothGuns(Fire, false);
        }

        private static void Fire()
        {
            if (Time.time < nextFire)
                return;

            nextFire = Time.time + FireRate;

            Vector3 dir = GL.rayDirection.sqrMagnitude > 0f ? GL.rayDirection.normalized : Vector3.forward;
            Vector3 origin = GL.rayOrigin + dir * 0.3f;
            Vector3 velocity = dir * Speed;

            SpawnProjectile(origin, velocity);
            SealSync.Fire(origin, velocity);

            GorillaTagger.Instance.StartVibration(false, 0.3f, 0.05f);
        }

        public static void SpawnProjectile(Vector3 position, Vector3 velocity)
        {
            GameObject seal = new GameObject("SealProjectile");
            seal.layer = LayerMask.NameToLayer("Default");
            seal.transform.position = position;
            seal.transform.rotation = velocity.sqrMagnitude > 0f ? Quaternion.LookRotation(velocity) : Quaternion.identity;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(cube.GetComponent<Collider>());
            cube.layer = seal.layer;
            cube.transform.SetParent(seal.transform, false);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localScale = Vector3.one * SealScale;

            Texture2D texture = ImageLib.Load(TextureFile);
            Color tint = texture != null ? Color.white : Body;
            Material mat = new Material(
                Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                ?? Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Standard"));
            mat.color = tint;
            mat.SetColor("_BaseColor", tint);
            mat.SetColor("_Color", tint);
            if (texture != null)
            {
                mat.mainTexture = texture;
                mat.SetTexture("_BaseMap", texture);
                mat.SetTexture("_MainTex", texture);
            }

            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material = mat;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            BoxCollider col = seal.AddComponent<BoxCollider>();
            col.size = Vector3.one * SealScale;

            Rigidbody rb = seal.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 1f;
            rb.linearVelocity = velocity;
            rb.angularVelocity = new Vector3(0f, 0f, 1f);
            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.6f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (GorillaTagger.Instance != null && GorillaTagger.Instance.bodyCollider != null)
                Physics.IgnoreCollision(col, GorillaTagger.Instance.bodyCollider);

            live.Enqueue(seal);
            while (live.Count > MaxLive)
            {
                GameObject old = live.Dequeue();
                if (old != null)
                    Object.Destroy(old);
            }

            Object.Destroy(seal, Lifetime);
        }
    }
}
