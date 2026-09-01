using BepInEx;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Parrot.client.Menu;
using Parrot.client.Classes;
using Parrot.client.Notifications;
using System.Collections.Generic;
using UnityEngine;

namespace Parrot.client.Mods.Console
{

    public class BanHammer
    {
        private const string ModelFile = "ban hammer.obj";
        private const string TextureFile = "Handle1_diff.png";

        private static float sizeVal = 0.85f;
        private static Vector3 offsetVal = Vector3.zero;
        private static Vector3 eulerVal = Vector3.zero;
        private static bool gripNegX = true;

        private const float hitRange = 0.4f;

        private static GameObject hammer;
        private static GameObject model;
        private static GameObject head;
        private static float modelExtX;
        private static float modelLargest;

        private static float nextPrint;

        public static void Run()
        {
            if (!OwnerList.HasAccess())
            {
                Despawn();
                return;
            }

            Spawn();
            Adjust();
            ApplyTransform();

            if (head == null || !PhotonNetwork.InRoom)
                return;

            Vector3 headPos = head.transform.position;

            VRRig hitRig = null;
            float closest = hitRange;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isOfflineVRRig || rig.isLocal || rig.Creator == null)
                    continue;

                float distance = Mathf.Min(
                    Vector3.Distance(headPos, rig.headMesh.transform.position),
                    Vector3.Distance(headPos, rig.transform.position));

                if (distance <= closest)
                {
                    closest = distance;
                    hitRig = rig;
                }
            }

            if (hitRig != null)
            {
                Parrot.client.Mods.Overpowered.FlingUp(hitRig);
                GorillaTagger.Instance.StartVibration(false, 0.8f, 0.1f);
            }
            else
            {
                Parrot.client.Mods.Overpowered.StopFling();
            }
        }

        private static void Adjust()
        {
            float dt = Time.deltaTime;
            float rot = 60f * dt;
            float move = 0.3f * dt;
            bool changed = false;

            if (Key(KeyCode.Keypad8)) { eulerVal.x += rot; changed = true; }
            if (Key(KeyCode.Keypad2)) { eulerVal.x -= rot; changed = true; }
            if (Key(KeyCode.Keypad4)) { eulerVal.y -= rot; changed = true; }
            if (Key(KeyCode.Keypad6)) { eulerVal.y += rot; changed = true; }
            if (Key(KeyCode.Keypad7)) { eulerVal.z -= rot; changed = true; }
            if (Key(KeyCode.Keypad9)) { eulerVal.z += rot; changed = true; }

            if (Key(KeyCode.LeftArrow))  { offsetVal.x -= move; changed = true; }
            if (Key(KeyCode.RightArrow)) { offsetVal.x += move; changed = true; }
            if (Key(KeyCode.UpArrow))    { offsetVal.y += move; changed = true; }
            if (Key(KeyCode.DownArrow))  { offsetVal.y -= move; changed = true; }
            if (Key(KeyCode.Keypad1))    { offsetVal.z += move; changed = true; }
            if (Key(KeyCode.Keypad3))    { offsetVal.z -= move; changed = true; }

            if (Key(KeyCode.KeypadPlus))  { sizeVal += move; ApplySize(); changed = true; }
            if (Key(KeyCode.KeypadMinus)) { sizeVal = Mathf.Max(0.05f, sizeVal - move); ApplySize(); changed = true; }

            if (Down(KeyCode.Keypad5)) { gripNegX = !gripNegX; Despawn(); return; }

            if ((changed && Time.time > nextPrint) || Down(KeyCode.Keypad0))
            {
                nextPrint = Time.time + 0.5f;
                NotifiLib.SendNotification(
                    $"<color=grey>[HAMMER]</color> size {sizeVal:0.00}  rot ({eulerVal.x:0},{eulerVal.y:0},{eulerVal.z:0})  off ({offsetVal.x:0.00},{offsetVal.y:0.00},{offsetVal.z:0.00})  grip {(gripNegX ? "-" : "+")}");
            }
        }

        private static void ApplyTransform()
        {
            if (hammer == null)
                return;

            hammer.transform.localPosition = offsetVal;
            float baseRoll = gripNegX ? -90f : 90f;
            hammer.transform.localRotation = Quaternion.Euler(eulerVal + new Vector3(0f, 0f, baseRoll));
        }

        private static void ApplySize()
        {
            if (model == null || modelLargest <= 0f)
                return;

            float scale = sizeVal / modelLargest;
            model.transform.localScale = Vector3.one * scale;
            float half = modelExtX * scale;
            model.transform.localPosition = new Vector3(gripNegX ? half : -half, 0f, 0f);
        }

        private static void Spawn()
        {
            if (hammer != null)
                return;

            Transform hand = GorillaTagger.Instance?.rightHandTransform;
            if (hand == null)
                return;

            hammer = new GameObject("BanHammer");
            hammer.transform.SetParent(hand, false);

            model = ObjLoader.Load(ModelFile, sizeVal, new Color(0.4f, 0.2f, 0.6f), TextureFile);
            if (model != null)
            {
                model.transform.SetParent(hammer.transform, false);
                model.transform.localRotation = Quaternion.identity;

                MeshFilter mf = model.GetComponentInChildren<MeshFilter>();
                modelExtX = mf != null ? mf.sharedMesh.bounds.extents.x : 0f;
                modelLargest = model.transform.localScale.x > 0f ? sizeVal / model.transform.localScale.x : 1f;

                float half = modelExtX * model.transform.localScale.x;
                model.transform.localPosition = new Vector3(gripNegX ? half : -half, 0f, 0f);

                GameObject hit = new GameObject("HitPoint");
                hit.transform.SetParent(model.transform, false);
                hit.transform.localPosition = new Vector3(gripNegX ? modelExtX : -modelExtX, 0f, 0f);
                head = hit;
            }
            else
            {
                BuildPrimitiveHammer();
            }

            ApplyTransform();
        }

        private static void BuildPrimitiveHammer()
        {
            Color color = new Color(0.4f, 0.2f, 0.6f);

            GameObject handle = MakePart(PrimitiveType.Cylinder, new Vector3(0.03f, 0.22f, 0.03f), color);
            handle.transform.SetParent(hammer.transform, false);
            handle.transform.localPosition = Vector3.zero;

            head = MakePart(PrimitiveType.Cylinder, new Vector3(0.14f, 0.12f, 0.14f), color);
            head.transform.SetParent(hammer.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.26f, 0f);
            head.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }

        public static void Despawn()
        {
            Parrot.client.Mods.Overpowered.StopFling();

            if (hammer != null)
                UnityEngine.Object.Destroy(hammer);

            hammer = null;
            model = null;
            head = null;
        }

        private static bool Key(KeyCode key) => UnityInput.Current.GetKey(key);
        private static bool Down(KeyCode key) => UnityInput.Current.GetKeyDown(key);

        private static GameObject MakePart(PrimitiveType type, Vector3 scale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            UnityEngine.Object.Destroy(part.GetComponent<Collider>());
            part.transform.localScale = scale;

            Renderer renderer = part.GetComponent<Renderer>();
            Shader shader = Shader.Find("GorillaTag/UberShader");
            if (shader != null)
            {
                Material mat = new Material(shader);
                mat.SetColor("_BaseColor", color);
                renderer.material = mat;
            }
            else
            {
                renderer.material.color = color;
            }

            return part;
        }
    }
}
