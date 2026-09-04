using BepInEx;
using Parrot.client.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Parrot.client.GunTools
{
    internal class Gunlib
    {

        public static Color pointercolor = Color.black;
        public static GradientColorKey[] pointergradient = new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.4f, 0.2f, 0.6f), 0f),
                new GradientColorKey(new Color(0.3f, 0.15f, 0.5f), 0.33f),
                new GradientColorKey(new Color(0.35f, 0.25f, 0.65f), 0.66f),
                new GradientColorKey(new Color(0.4f, 0.2f, 0.6f), 1f)
            };

        public static VRRig LockedPlayer;
        public static GameObject spherepointer;
        public static RaycastHit nray;

        public static Vector3 rayOrigin;
        public static Vector3 rayDirection = Vector3.forward;
        public static float AimAssistRadius = 1.5f;

        public static float pointerBaseScale = 0.12f;

        private static GameObject trailObj;
        private static LineRenderer trailLine;
        private const int TrailSegments = 32;
        public static float TrailWidth = 0.02f;
        public static float BendStrength = 0.1f;
        private static Vector3 bendOffset = Vector3.zero;
        private static Vector3 bendVelocity = Vector3.zero;

        public static float ElectricStrength = 0.25f;
        public static float ElectricFlickerRate = 20f;

        public static float WiggleStrength = 0.08f;
        public static float WiggleMaxAmplitude = 0.5f;
        public static float WiggleFrequency = 2.5f;
        public static float WiggleSpeed = 6f;

        public static void StartBothGuns(Action action, bool lockOn)
        {
            if (XRSettings.isDeviceActive) StartVrGun(action, lockOn);
            else StartPcGun(action, lockOn);
        }

        public static void StartVrGun(Action action, bool lockOn)
        {
            bool gripHeld = ControllerInputPoller.instance.rightGrab;
            bool triggerHeld = ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;

            if (!gripHeld)
            {
                HandleGunLogic(action, lockOn, XR: true, triggerHeld: false);
                return;
            }

            rayOrigin = GorillaTagger.Instance.rightHandTransform.position;
            rayDirection = -GorillaTagger.Instance.rightHandTransform.up;

            Physics.Raycast(rayOrigin, rayDirection, out nray, float.MaxValue);
            HandleGunLogic(action, lockOn, XR: true, triggerHeld: triggerHeld);
        }
        public static void StartPcGun(Action action, bool lockOn)
        {
            Camera cam = GameObject.Find("Shoulder Camera")?.GetComponent<Camera>();
            if (cam == null) cam = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();

            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            bool gripHeld = UnityInput.Current.GetMouseButton(1);
            bool triggerPressed = UnityInput.Current.GetMouseButton(0);

            rayOrigin = ray.origin;
            rayDirection = ray.direction;

            Physics.Raycast(rayOrigin, rayDirection, out nray, 100f);
            HandleGunLogic(action, lockOn, XR: false, triggerHeld: gripHeld && triggerPressed);
        }

        private static void HandleGunLogic(Action action, bool lockOn, bool XR, bool triggerHeld)
        {
            bool gripHeld = XR ? ControllerInputPoller.instance.rightGrab : UnityInput.Current.GetMouseButton(1);
            if (!gripHeld)
            {
                Cleanup();
                return;
            }
            if (spherepointer == null) CreatePointer();

            if (Mods.Settings.Gun.noGunLine)
            {
                if (trailObj != null) UnityEngine.Object.Destroy(trailObj);
                trailObj = null;
                trailLine = null;
            }
            else if (trailLine == null) CreateTrail();
            Vector3 handPos = GorillaTagger.Instance.rightHandTransform.position;
            Vector3 targetPos = handPos;

            bool gunLock = Mods.Settings.Gun.gunLock;
            VRRig aimedRig = gunLock ? GetAimedRig() : null;

            if (!gunLock)
                LockedPlayer = null;

            if (lockOn && gunLock)
            {
                if (aimedRig != null)
                    LockedPlayer = aimedRig;

                if (LockedPlayer != null)
                    targetPos = LockedPlayer.transform.position;
                else if (nray.collider != null)
                    targetPos = nray.point;
            }
            else if (aimedRig != null)
                targetPos = GetAimPoint(aimedRig);
            else if (nray.collider != null)
                targetPos = nray.point;

            spherepointer.transform.position = targetPos;
            spherepointer.transform.localScale = Vector3.one * pointerBaseScale;
            UpdateTrail(handPos, targetPos);
            ApplyGunColor(triggerHeld || LockedPlayer != null);

            if (triggerHeld && action != null)
                action();
        }

        private static void ApplyGunColor(bool held)
        {
            if (Mods.Settings.GunColor.index == 0)
                return;

            Color c = Mods.Settings.GunColor.Get(held);

            if (spherepointer != null)
            {
                Renderer renderer = spherepointer.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.SetColor("_BaseColor", c);
                    renderer.material.SetColor("_EmissionColor", c);
                }
            }

            if (trailLine != null && Mods.Settings.Gun.gunType != Mods.Settings.GunType.Rainbow)
            {
                Gradient grad = new Gradient();
                grad.SetColorKeys(new GradientColorKey[] { new GradientColorKey(c, 0f), new GradientColorKey(c, 1f) });
                trailLine.colorGradient = grad;
                trailLine.material.SetColor("_BaseColor", c);
                trailLine.material.SetColor("_EmissionColor", c);
            }
        }

        private static Vector3 GetAimPoint(VRRig rig) =>
            rig.headMesh != null ? rig.headMesh.transform.position : rig.transform.position;

        public static VRRig GetAimedRig()
        {
            VRRig hitRig = nray.collider != null ? nray.collider.GetComponentInParent<VRRig>() : null;
            if (hitRig != null && !hitRig.isOfflineVRRig && !hitRig.isLocal)
                return hitRig;

            VRRig closest = null;
            float closestDistance = AimAssistRadius;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isOfflineVRRig || rig.isLocal)
                    continue;

                float distance = Mathf.Min(
                    DistanceToRay(rig.transform.position),
                    DistanceToRay(GetAimPoint(rig)));

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = rig;
                }
            }

            return closest;
        }

        private static float DistanceToRay(Vector3 point)
        {
            float along = Vector3.Dot(point - rayOrigin, rayDirection);
            if (along <= 0f) return float.MaxValue;
            return Vector3.Distance(rayOrigin + rayDirection * along, point);
        }

        private static void CreatePointer()
        {
            spherepointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(spherepointer.GetComponent<Collider>());
            spherepointer.transform.localScale = Vector3.zero;

            Color pointerColor = pointercolor;
            Material mat = new Material(Shader.Find("GorillaTag/UberShader"));
            mat.SetColor("_BaseColor", pointerColor);
            mat.SetColor("_EmissionColor", pointerColor);
            mat.EnableKeyword("_EMISSION");
            spherepointer.GetComponent<Renderer>().material = mat;
        }

        private static void CreateTrail()
        {
            trailObj = new GameObject("GunTrail");
            trailObj.hideFlags = HideFlags.HideAndDontSave;

            trailLine = trailObj.AddComponent<LineRenderer>();
            trailLine.positionCount = TrailSegments;
            trailLine.useWorldSpace = true;
            trailLine.receiveShadows = false;
            trailLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            AnimationCurve widthCurve = new AnimationCurve(
                new Keyframe(0.0f, TrailWidth * 0.3f),
                new Keyframe(0.3f, TrailWidth * 1.0f),
                new Keyframe(0.7f, TrailWidth * 1.0f),
                new Keyframe(1.0f, TrailWidth * 0.3f)
            );
            trailLine.widthCurve = widthCurve;
            trailLine.widthMultiplier = 1f;

            Gradient grad = new Gradient();
            grad.SetColorKeys(pointergradient);
            trailLine.colorGradient = grad;

            Color pointerColor = pointercolor;
            Material mat = new Material(Shader.Find("GUI/Text Shader"));
            mat.SetColor("_BaseColor", pointerColor);
            mat.SetColor("_EmissionColor", pointerColor);
            mat.EnableKeyword("_EMISSION");
            trailLine.material = mat;
        }

        private static void UpdateTrail(Vector3 start, Vector3 end)
        {
            if (trailLine == null) return;

            Vector3 forward = end - start;
            float dist = forward.magnitude;

            if (dist < 0.001f)
            {
                Vector3[] collapsed = new Vector3[TrailSegments];
                for (int i = 0; i < TrailSegments; i++) collapsed[i] = start;
                trailLine.SetPositions(collapsed);
                return;
            }

            Vector3 forwardNorm = forward.normalized;

            Vector3 refUp = (Mathf.Abs(forwardNorm.y) < 0.95f) ? Vector3.up : Vector3.forward;
            Vector3 perpA = Vector3.Cross(forwardNorm, refUp).normalized;
            Vector3 perpB = Vector3.Cross(forwardNorm, perpA).normalized;

            switch (Mods.Settings.Gun.gunType)
            {
                case Mods.Settings.GunType.Normal:
                    trailLine.SetPositions(BuildStraight(start, forward));
                    return;
                case Mods.Settings.GunType.Electric:
                    trailLine.SetPositions(BuildElectric(start, forward, perpA, perpB, dist));
                    return;
                case Mods.Settings.GunType.Spiral:
                    trailLine.SetPositions(BuildSpiral(start, forward, perpA, perpB, dist));
                    return;
                case Mods.Settings.GunType.Sine:
                    trailLine.SetPositions(BuildSine(start, forward, perpA, dist));
                    return;
                case Mods.Settings.GunType.Bounce:
                    trailLine.SetPositions(BuildBounce(start, forward, perpB, dist));
                    return;
                case Mods.Settings.GunType.Rainbow:
                    ApplyRainbow();
                    trailLine.SetPositions(BuildSine(start, forward, perpA, dist));
                    return;
                case Mods.Settings.GunType.SmoothCable:
                    trailLine.SetPositions(BuildSmoothCable(start, forward, perpA, perpB, dist));
                    return;
            }

            float t = Time.time;
            float wave = WiggleFrequency * Mathf.PI * 2f;
            float amplitude = Mathf.Min(dist * WiggleStrength, WiggleMaxAmplitude);

            Vector3[] positions = new Vector3[TrailSegments];
            for (int i = 0; i < TrailSegments; i++)
            {
                float u = i / (float)(TrailSegments - 1);
                float taper = Mathf.Sin(u * Mathf.PI);
                float phase = u * wave - t * WiggleSpeed;

                positions[i] = start + forward * u
                    + perpA * (Mathf.Sin(phase) * amplitude * taper)
                    + perpB * (Mathf.Sin(phase + 1.9f) * amplitude * taper * 0.6f);
            }

            trailLine.SetPositions(positions);
        }

        private static void ApplyRainbow()
        {
            float t = Time.time * 0.5f;
            GradientColorKey[] keys = new GradientColorKey[6];
            for (int i = 0; i < 6; i++)
            {
                float hue = Mathf.Repeat(t + i / 6f, 1f);
                keys[i] = new GradientColorKey(Color.HSVToRGB(hue, 1f, 1f), i / 5f);
            }

            Gradient grad = new Gradient();
            grad.SetColorKeys(keys);

            trailLine.colorGradient = grad;
        }

        private static Vector3[] BuildStraight(Vector3 start, Vector3 forward)
        {
            Vector3[] positions = new Vector3[TrailSegments];
            for (int i = 0; i < TrailSegments; i++)
            {
                float u = i / (float)(TrailSegments - 1);
                positions[i] = start + forward * u;
            }
            return positions;
        }

        private static Vector3[] BuildSpiral(Vector3 start, Vector3 forward, Vector3 perpA, Vector3 perpB, float dist)
        {
            float t = Time.time;
            float turns = 3.5f;
            Vector3[] positions = new Vector3[TrailSegments];

            for (int i = 0; i < TrailSegments; i++)
            {
                float u = i / (float)(TrailSegments - 1);
                float taper = Mathf.Sin(u * Mathf.PI);
                float amp = Mathf.Min(dist * 0.07f, 0.4f) * taper;
                float angle = u * turns * Mathf.PI * 2f - t * 6f;

                positions[i] = start + forward * u
                    + perpA * (Mathf.Cos(angle) * amp)
                    + perpB * (Mathf.Sin(angle) * amp);
            }

            return positions;
        }

        private static Vector3[] BuildSine(Vector3 start, Vector3 forward, Vector3 perpA, float dist)
        {
            float t = Time.time;
            float waves = 3f;
            Vector3[] positions = new Vector3[TrailSegments];

            for (int i = 0; i < TrailSegments; i++)
            {
                float u = i / (float)(TrailSegments - 1);
                float taper = Mathf.Sin(u * Mathf.PI);
                float amp = Mathf.Min(dist * 0.12f, 0.5f) * taper;
                float phase = u * waves * Mathf.PI * 2f - t * 7f;

                positions[i] = start + forward * u + perpA * (Mathf.Sin(phase) * amp);
            }

            return positions;
        }

        private static Vector3[] BuildBounce(Vector3 start, Vector3 forward, Vector3 perpB, float dist)
        {
            float bounces = 3f;
            float amp = Mathf.Min(dist * 0.14f, 0.6f);
            Vector3[] positions = new Vector3[TrailSegments];

            for (int i = 0; i < TrailSegments; i++)
            {
                float u = i / (float)(TrailSegments - 1);
                float height = Mathf.Abs(Mathf.Sin(u * bounces * Mathf.PI)) * amp;

                positions[i] = start + forward * u + perpB * height;
            }

            return positions;
        }

        private static Vector3[] BuildSmoothCable(Vector3 start, Vector3 forward, Vector3 perpA, Vector3 perpB, float dist)
        {
            float t = Time.time;
            float amp = Mathf.Min(dist * 0.18f, 0.75f);
            float sag = Mathf.Min(dist * 0.12f, 0.5f);
            Vector3[] positions = new Vector3[TrailSegments];

            for (int i = 0; i < TrailSegments; i++)
            {
                float u = i / (float)(TrailSegments - 1);
                float taper = Mathf.Sin(u * Mathf.PI);

                float driftA = (Mathf.PerlinNoise(u * 1.8f + t * 0.55f, 4.2f) - 0.5f) * 2f;
                float driftB = (Mathf.PerlinNoise(u * 1.8f + t * 0.4f, 19.7f) - 0.5f) * 2f;
                float sway = Mathf.Sin(u * Mathf.PI * 1.5f - t * 1.6f);

                positions[i] = start + forward * u
                    + perpA * ((driftA + sway * 0.3f) * amp * taper)
                    + perpB * (driftB * amp * taper)
                    + Vector3.down * (Mathf.Sin(u * Mathf.PI) * sag);
            }

            return positions;
        }

        private static Vector3[] BuildElectric(Vector3 start, Vector3 forward, Vector3 perpA, Vector3 perpB, float dist)
        {
            float seed = Mathf.Floor(Time.time * ElectricFlickerRate);
            Vector3[] positions = new Vector3[TrailSegments];

            for (int i = 0; i < TrailSegments; i++)
            {
                float u = i / (float)(TrailSegments - 1);
                float taper = Mathf.Sin(u * Mathf.PI);
                float amp = ElectricStrength * dist * 0.2f * taper;

                float offsetA = (Mathf.PerlinNoise(seed * 0.37f, i * 0.9f) - 0.5f) * 2f;
                float offsetB = (Mathf.PerlinNoise(seed * 0.61f + 13.1f, i * 0.9f) - 0.5f) * 2f;
                float zigzag = ((i & 1) == 0) ? 1f : -1f;

                positions[i] = start + forward * u
                    + perpA * (offsetA + zigzag * 0.5f) * amp
                    + perpB * offsetB * amp;
            }

            return positions;
        }

        private static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float mt = 1f - t;
            float mt2 = mt * mt;
            float t2 = t * t;
            return mt2 * mt * p0
                 + 3f * mt2 * t * p1
                 + 3f * mt * t2 * p2
                 + t2 * t * p3;
        }

        private static void Cleanup()
        {
            if (spherepointer != null)
                UnityEngine.Object.Destroy(spherepointer);
            spherepointer = null;

            if (trailObj != null)
                UnityEngine.Object.Destroy(trailObj);
            trailObj = null;
            trailLine = null;

            LockedPlayer = null;
            bendOffset = Vector3.zero;
            bendVelocity = Vector3.zero;
        }

        public static void TestGunlib()
        {
            Gunlib.StartBothGuns(() => { }, false);
        }
        public static void TestGunlibLocked()
        {
            Gunlib.StartBothGuns(() => { }, true);
        }
    }
}