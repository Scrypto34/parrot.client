using BepInEx;
using GorillaLocomotion;
using Parrot.client.GunTools;
using Parrot.client.Classes;
using Parrot.client.Menu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods
{
    public class Movement
    {
        public static void Fly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * Settings.Movement.flySpeed;
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }

        public static float fastFlySpeed = 200f;

        public static void FastFly()
        {
            if (ControllerInputPoller.instance == null || GTPlayer.Instance == null || GorillaTagger.Instance == null)
                return;

            if (!ControllerInputPoller.instance.rightControllerPrimaryButton)
                return;

            GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * (Time.deltaTime * fastFlySpeed);
            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
        }
        
        
        public static float FlySpeed;

        public static float pullSpeed = 30f;

        public static float grappleSpeed = 16f;

        public static float flyTowardSpeed = 15f;
        
        
        public static void FlyTowardGun()
        {
            Parrot.client.GunTools.Gunlib.StartBothGuns(() =>
            {
                if (GorillaTagger.Instance == null)
                    return;

                VRRig target = Parrot.client.GunTools.Gunlib.LockedPlayer;
                Vector3 targetPos;

                if (target != null && !target.isLocal && !target.isOfflineVRRig)
                    targetPos = target.transform.position;
                else if (Parrot.client.GunTools.Gunlib.nray.collider != null)
                    targetPos = Parrot.client.GunTools.Gunlib.nray.point;
                else
                    return;

                Vector3 from = GorillaTagger.Instance.bodyCollider.transform.position;
                Vector3 dir = targetPos - from;
                if (dir.magnitude > 0.6f)
                    GorillaTagger.Instance.rigidbody.linearVelocity = dir.normalized * flyTowardSpeed;
            }, true);
        }

        public static void GrappleGun()
        {
            Parrot.client.GunTools.Gunlib.StartBothGuns(() =>
            {
                if (GorillaTagger.Instance == null)
                    return;

                if (Physics.Raycast(Parrot.client.GunTools.Gunlib.rayOrigin, Parrot.client.GunTools.Gunlib.rayDirection, out RaycastHit hit, 300f))
                {
                    Vector3 from = GorillaTagger.Instance.bodyCollider.transform.position;
                    Vector3 dir = hit.point - from;
                    if (dir.magnitude > 0.4f)
                        GorillaTagger.Instance.rigidbody.linearVelocity = dir.normalized * grappleSpeed;
                }
            }, false);
        }


        
        public static void Pull()
        {
            if (ControllerInputPoller.instance == null || GorillaTagger.Instance == null || GTPlayer.Instance == null)
                return;

            if (!ControllerInputPoller.instance.rightGrab)
                return;

            Rigidbody rb = GorillaTagger.Instance.rigidbody;

            Vector3 dir = GorillaTagger.Instance.headCollider.transform.forward.normalized;
            rb.linearVelocity = dir * (pullSpeed * 0.2f);
        }

        private static float origVelocityLimit = -1f;
        private static float origSlideVelocityLimit = -1f;

        public static void NoSpeedLimit()
        {
            if (GTPlayer.Instance == null)
                return;

            if (origVelocityLimit < 0f)
            {
                origVelocityLimit = GTPlayer.Instance.velocityLimit;
                origSlideVelocityLimit = GTPlayer.Instance.slideVelocityLimit;
            }

            GTPlayer.Instance.velocityLimit = 9999f;
            GTPlayer.Instance.slideVelocityLimit = 9999f;
        }

        public static void RestoreSpeedLimit()
        {
            if (GTPlayer.Instance == null || origVelocityLimit < 0f)
                return;

            GTPlayer.Instance.velocityLimit = origVelocityLimit;
            GTPlayer.Instance.slideVelocityLimit = origSlideVelocityLimit;
        }

        public static void AutoElevatorClimb()
        {
            if (GTPlayer.Instance == null || ControllerInputPoller.instance == null)
                return;

            if (!ControllerInputPoller.instance.rightGrab)
                return;

            float d = Mathf.Sin((float)Time.frameCount / 2.5f) * 0.6f;
            Transform transform = GTPlayer.Instance.bodyCollider.transform;
            GTPlayer.Instance.RightHand.controllerTransform.position = transform.position + transform.right * (0.31f + Mathf.Cos((float)Time.frameCount / 2.5f) * 0.3f) + transform.up * d + transform.forward * 0.65f;
        }



        public static void SlingshotFly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
                GorillaTagger.Instance.rigidbody.linearVelocity += GTPlayer.Instance.headCollider.transform.forward * (Time.deltaTime * (FlySpeed * 2));
        }

        public static void NoClipFly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                Noclip(true);
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * (Time.deltaTime * FlySpeed);
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
            Noclip(false);
        }

        public static void Noclip(bool b)
        {
            foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
            {
                if (b)
                {
                    collider.enabled = false;
                }
                else
                {
                    collider.enabled = true;
                }
            }
        }

        public static float startX = -1;
        public static float startY = -1;
        public static float subThingy;
        public static float subThingyZ;
        public static Vector3 lastPosition = Vector3.zero;

        public static void WASDFly()
        {

            bool stationary = !Main.GetIndex("Disable Stationary WASD Fly").enabled;

            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if (kb == null) return;

            bool W = kb.wKey.isPressed;
            bool A = kb.aKey.isPressed;
            bool S = kb.sKey.isPressed;
            bool D = kb.dKey.isPressed;
            bool Space = kb.spaceKey.isPressed;
            bool Ctrl = kb.leftCtrlKey.isPressed;
            bool Shift = kb.leftShiftKey.isPressed;
            bool Alt = kb.leftAltKey.isPressed;

            bool LeftArrow = kb.leftArrowKey.isPressed;
            bool RightArrow = kb.rightArrowKey.isPressed;
            bool UpArrow = kb.upArrowKey.isPressed;
            bool DownArrow = kb.downArrowKey.isPressed;

            if (stationary || W || A || S || D || Space || Ctrl)
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;

            Transform parentTransform;
            if (menu != null)
                parentTransform = GTPlayer.Instance.GetControllerTransform(false).parent;
            else
                parentTransform = GorillaTagger.Instance.headCollider.transform;

            float turnSpeed = 250f;

            bool canRotate = menu == null;

            if (canRotate && LeftArrow)
                parentTransform.eulerAngles += new Vector3(0, -turnSpeed, 0) * Time.deltaTime;
            if (canRotate && RightArrow)
                parentTransform.eulerAngles += new Vector3(0, turnSpeed, 0) * Time.deltaTime;
            if (canRotate && UpArrow)
                parentTransform.eulerAngles += new Vector3(-turnSpeed, 0, 0) * Time.deltaTime;
            if (canRotate && DownArrow)
                parentTransform.eulerAngles += new Vector3(turnSpeed, 0, 0) * Time.deltaTime;

            if (canRotate && mouse != null && mouse.rightButton.isPressed)
            {
                Quaternion currentRotation = parentTransform.rotation;
                Vector3 euler = currentRotation.eulerAngles;

                if (startX < 0)
                {
                    startX = euler.y;
                    subThingy = mouse.position.value.x / Screen.width;
                }

                if (startY < 0)
                {
                    startY = euler.x;
                    subThingyZ = mouse.position.value.y / Screen.height;
                }

                float newX = startY - (mouse.position.value.y / Screen.height - subThingyZ) * 360 * 1.33f;
                float newY = startX + (mouse.position.value.x / Screen.width - subThingy) * 360 * 1.33f;

                newX = newX > 180f ? newX - 360f : newX;
                newX = Mathf.Clamp(newX, -90f, 90f);

                parentTransform.rotation = Quaternion.Euler(newX, newY, euler.z);
            }
            else
            {
                startX = -1;
                startY = -1;
            }

            float speed = Settings.Movement.flySpeed * 0.5f;
            if (Shift)
                speed *= 2f;
            else if (Alt)
                speed /= 2;

            if (W)
                GorillaTagger.Instance.rigidbody.transform.position += parentTransform.forward * (Time.deltaTime * speed);
            if (S)
                GorillaTagger.Instance.rigidbody.transform.position += parentTransform.forward * (Time.deltaTime * -speed);
            if (A)
                GorillaTagger.Instance.rigidbody.transform.position += parentTransform.right * (Time.deltaTime * -speed);
            if (D)
                GorillaTagger.Instance.rigidbody.transform.position += parentTransform.right * (Time.deltaTime * speed);
            if (Space)
                GorillaTagger.Instance.rigidbody.transform.position += new Vector3(0f, Time.deltaTime * speed, 0f);
            if (Ctrl)
                GorillaTagger.Instance.rigidbody.transform.position += new Vector3(0f, Time.deltaTime * -speed, 0f);

            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

            if (!W && !A && !S && !D && !Space && !Ctrl && lastPosition != Vector3.zero && stationary)
                GorillaTagger.Instance.rigidbody.transform.position = lastPosition;
            else
                lastPosition = GorillaTagger.Instance.rigidbody.transform.position;

            GorillaTagger.Instance.rigidbody.useGravity = !stationary;
        }

        public static GameObject platl;
        public static GameObject platr;
        public static Rigidbody platlRb;
        public static Rigidbody platrRb;

        public static void Platforms()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                if (platl == null)
                {
                    platl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platl.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platlRb = platl.AddComponent<Rigidbody>();
                    platlRb.isKinematic = true;

                    FixStickyColliders(platl);

                    ColorChanger colorChanger = platl.AddComponent<ColorChanger>();
                    colorChanger.colors = Parrot.client.Settings.backgroundColor;
                }
                else
                {
                    platlRb.MovePosition(TrueLeftHand().position);
                    platlRb.MoveRotation(TrueLeftHand().rotation);
                }
            }
            else
            {
                if (platl != null)
                {
                    Object.Destroy(platl);
                    platl = null;
                    platlRb = null;
                }
            }

            if (ControllerInputPoller.instance.rightGrab)
            {
                if (platr == null)
                {
                    platr = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platr.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platrRb = platr.AddComponent<Rigidbody>();
                    platrRb.isKinematic = true;

                    FixStickyColliders(platr);

                    ColorChanger colorChanger = platr.AddComponent<ColorChanger>();
                    colorChanger.colors = Parrot.client.Settings.backgroundColor;
                }
                else
                {
                    platrRb.MovePosition(TrueRightHand().position);
                    platrRb.MoveRotation(TrueRightHand().rotation);
                }
            }
            else
            {
                if (platr != null)
                {
                    Object.Destroy(platr);
                    platr = null;
                    platrRb = null;
                }
            }
        }

        private static bool teleportGunPressed;

        public static void TeleportGun()
        {
            bool wasTriggered = false;

            GunTools.Gunlib.StartBothGuns(() =>
            {
                wasTriggered = true;

                if (!teleportGunPressed && GunTools.Gunlib.spherepointer != null)
                {
                    Vector3 targetPos = GunTools.Gunlib.spherepointer.transform.position;

                    GorillaLocomotion.GTPlayer.Instance.transform.position = targetPos;
                    GorillaTagger.Instance.transform.position = targetPos;

                    Rigidbody rb = GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.linearVelocity = Vector3.zero;

                    teleportGunPressed = true;
                }
            }, false);

            if (!wasTriggered)
                teleportGunPressed = false;
        }

        private static bool doubleJumpUsed;
        private static bool doubleJumpAPrev;

        public static void DoubleJump()
        {
            if (GTPlayer.Instance == null || GorillaTagger.Instance == null || ControllerInputPoller.instance == null)
                return;

            float scale = GTPlayer.Instance.scale;
            Vector3 origin = GTPlayer.Instance.bodyCollider.transform.position;

            bool grounded = Physics.Raycast(origin, Vector3.down, 1.3f * scale, ~0, QueryTriggerInteraction.Ignore);
            if (grounded)
                doubleJumpUsed = false;

            bool aPressed = ControllerInputPoller.instance.rightControllerPrimaryButton;

            if (aPressed && !doubleJumpAPrev && !grounded && !doubleJumpUsed)
            {
                Rigidbody rb = GorillaTagger.Instance.rigidbody;
                Vector3 v = rb.linearVelocity;
                v.y = 6.8f * scale;
                rb.linearVelocity = v;
                doubleJumpUsed = true;
            }

            doubleJumpAPrev = aPressed;
        }

        public static void SpeedBoost()
        {
            GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed = 8f;
        }

        public static void IshowSpeedJR()
        {
            GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed = 42f;
        }



        public static void WalkOnWater()
        {
            GameObject gameObject = GameObject.Find("Beach/B_WaterVolumes");
            Transform transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject gameObject2 = transform.GetChild(i).gameObject;
                gameObject2.layer = LayerMask.NameToLayer("Default");
            }
        }

        public static float wallAssistAmount = 2.5f;

        public static void Bouncy()
        {
            ((Collider)GorillaTagger.Instance.bodyCollider).material.bounciness = 1f;
            ((Collider)GorillaTagger.Instance.bodyCollider).material.bounceCombine = (PhysicsMaterialCombine)3;
            ((Collider)GorillaTagger.Instance.bodyCollider).material.dynamicFriction = 0f;
        }

        public static void ResetBouncy()
        {
            ((Collider)GorillaTagger.Instance.bodyCollider).material.bounciness = 0f;
            ((Collider)GorillaTagger.Instance.bodyCollider).material.bounceCombine = (PhysicsMaterialCombine)0;
            ((Collider)GorillaTagger.Instance.bodyCollider).material.dynamicFriction = 0f;
        }

        public static void JoystickFly()
        {
            Transform transform = ((Component)GTPlayer.Instance.bodyCollider).transform;
            Physics.gravity = Vector3.zero;
            Vector2 leftControllerPrimary2DAxis = ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerPrimary2DAxis;
            Vector2 rightControllerPrimary2DAxis = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimary2DAxis;
            Vector3 val = transform.forward * leftControllerPrimary2DAxis.y;
            Vector3 val2 = transform.right * leftControllerPrimary2DAxis.x;
            Vector3 val3 = transform.up * rightControllerPrimary2DAxis.y;
            Vector3 val4 = val + val2 + val3;
            Vector3 normalized = val4.normalized;
            Transform transform2 = ((Component)GTPlayer.Instance).transform;
            transform2.position += normalized * 9f * Time.deltaTime;
            ((Component)GTPlayer.Instance).GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
        }

        public static void Mosaboost()
        {
            GTPlayer.Instance.maxJumpSpeed = 8.5f;
            GTPlayer.Instance.jumpMultiplier = 6.25f;
        }

        public static void NoTagFreeze()
        {
            GTPlayer.Instance.disableMovement = false;
        }

        public static void TagFreeze()
        {
            GTPlayer.Instance.disableMovement = true;
        }

        public static GameObject airSwimPart;
        public static void AirSwim()
        {
            if ((Object)(object)airSwimPart == (Object)null)
            {
                airSwimPart = Object.Instantiate<GameObject>(GameObject.Find("Environment Objects/LocalObjects_Prefab/ForestToBeach/ForestToBeach_Prefab_V4/CaveWaterVolume"));
                airSwimPart.transform.localScale = new Vector3(5f, 5f, 5f);
                airSwimPart.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                GTPlayer.Instance.audioManager.UnsetMixerSnapshot(0.1f);
                airSwimPart.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 2.5f, 0f);
            }
        }

        public static void DisableAirSwim()
        {
            if ((Object)(object)airSwimPart != (Object)null)
            {
                Object.Destroy((Object)(object)airSwimPart);
                airSwimPart = null;
            }
        }

        public static void FastSwim()
        {
            if (GTPlayer.Instance.InWater)
            {
                Rigidbody component = ((Component)GTPlayer.Instance).gameObject.GetComponent<Rigidbody>();
                component.linearVelocity *= 1.069f;
            }
        }

        
        
        public static void CarMonkey()
        {

            Vector3 forward = ((Component)GTPlayer.Instance.headCollider).transform.forward;
            forward.y = 0f;
            forward.Normalize();
            if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat > 0.2f || UnityInput.Current.GetKey((KeyCode)116))
            {
                Transform transform = ((Component)GTPlayer.Instance).transform;
                transform.position += forward * Time.deltaTime * 25f;
                ((Component)GTPlayer.Instance).GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
            if (((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat > 0.2f || UnityInput.Current.GetKey((KeyCode)121))
            {
                Transform transform2 = ((Component)GTPlayer.Instance).transform;
                transform2.position -= forward * Time.deltaTime * 25f;
                ((Component)GTPlayer.Instance).GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
        }

        
        public static void TpToStump()
        {
            if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(1))
            {
                Noclip();
                ((Component)GTPlayer.Instance).transform.position = new Vector3(-63.8717f, 12.1881f, -83.0144f);
            }
            else
            {
                Yesclip();
            }
        }

        public static void TpToCity()
        {
            if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(1))
            {
                Noclip();
                ((Component)GTPlayer.Instance).transform.position = new Vector3(-66.9824f, 14.0115f, -97.0772f);
            }
            else
            {
                Yesclip();
            }
        }

        public static void TpToTut()
        {
            if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(1))
            {
                Noclip();
                ((Component)GTPlayer.Instance).transform.position = new Vector3(-86.6707f, 36.4451f, -65.8458f);
            }
            else
            {
                Yesclip();
            }
        }

        public static void Noclip()
        {
            MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
            foreach (MeshCollider val in array)
            {
                ((Collider)val).enabled = false;
            }
        }

        public static void Yesclip()
        {
            MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
            foreach (MeshCollider val in array)
            {
                ((Collider)val).enabled = true;
            }
        }

        public static void IronMonke()
        {
            if (((ControllerInputPoller)ControllerInputPoller.instance).rightGrab)
            {
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(115, false, 0.1f);
                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 10f, GorillaTagger.Instance.tapHapticDuration);
                ((Component)GTPlayer.Instance).GetComponent<Rigidbody>().AddForce(new Vector3(5f * GTPlayer.Instance.RightHand.controllerTransform.right.x, 5f * GTPlayer.Instance.RightHand.controllerTransform.right.y, 5f * GTPlayer.Instance.RightHand.controllerTransform.right.z), (ForceMode)2);
                GameObject val = GameObject.CreatePrimitive((PrimitiveType)2);
                Object.Destroy((Object)(object)val.GetComponent<Collider>());
                val.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                val.transform.localPosition = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, -0.05f, 0f);
                val.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                val.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                val.GetComponent<Renderer>().material.color = new Color32(200, 200, 200, 80);
                Object.Destroy((Object)(object)val, 0.3f);
            }
            if (((ControllerInputPoller)ControllerInputPoller.instance).leftGrab)
            {
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(115, true, 0.1f);
                GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 10f, GorillaTagger.Instance.tapHapticDuration);
                ((Component)GTPlayer.Instance).GetComponent<Rigidbody>().AddForce(new Vector3(5f * GTPlayer.Instance.LeftHand.controllerTransform.right.x * -1f, 5f * GTPlayer.Instance.LeftHand.controllerTransform.right.y * -1f, 5f * GTPlayer.Instance.LeftHand.controllerTransform.right.z * -1f), (ForceMode)2);
                GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)2);
                Object.Destroy((Object)(object)val2.GetComponent<Collider>());
                val2.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                val2.transform.localPosition = GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, -0.05f, 0f);
                val2.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                val2.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                val2.GetComponent<Renderer>().material.color = new Color32(200, 200, 200, 80);
                Object.Destroy((Object)(object)val2, 0.3f);
            }
        }

    }
}
