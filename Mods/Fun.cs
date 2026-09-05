using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
using GorillaTagScripts.ScavengerHunt;
using Photon.Pun;
using Parrot.client.Menu;
using Parrot.client.GunTools;
using Parrot.client.Menu;
using Parrot.client.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Technie.PhysicsCreator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Parrot.client.Mods
{
    internal class Fun
    {
        private static float ropeDelay;

        public static void FlingRopeGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.nray.collider == null)
                    return;

                GorillaLocomotion.Gameplay.GorillaRopeSwing rope = Gunlib.nray.collider.GetComponentInParent<GorillaLocomotion.Gameplay.GorillaRopeSwing>();
                if (rope == null || Time.time < ropeDelay)
                    return;

                ropeDelay = Time.time + 0.125f;
                rope.SetVelocity(rope.ropeLength, RandomVector3(100f), true, default);
            }, false);
        }
        
        public static void SnowballFlingGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                VRRig target = Gunlib.LockedPlayer;
                if (target == null || target.isLocal || target.isOfflineVRRig)
                    return;

                Overpowered.FlingUp(target);
            }, true);

            bool isHoldingInput = ControllerInputPoller.instance != null &&
                (ControllerInputPoller.instance.rightControllerGripFloat > 0.5f ||
                 ControllerInputPoller.instance.leftControllerGripFloat > 0.5f ||
                 ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f ||
                 ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f);

            if (!isHoldingInput && GrabPatches.GrabPatches.GrabPatch.enabled)
            {
                VRRig.LocalRig.BreakHandLinks();
                Overpowered.StopFling();
            }
        }

        private static bool ghostMonkeyHeld;
        private static bool invisibleMonkeyHeld;
        private static bool holdSkinActive;
        private static Color holdSkinOldColor = Color.white;
        private static readonly Color ghostMonkeyColor = new Color(1f, 1f, 1f, 0.5f);
        private static readonly Color invisibleMonkeyColor = new Color(1f, 1f, 1f, 0.06f);
        private static GameObject holdDecoy;

        private static bool RightPrimaryHeld()
        {
            return ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightControllerPrimaryButton;
        }

        private static VRRig HoldSkinRig()
        {
            if (GorillaTagger.Instance == null)
                return null;

            VRRig rig = GorillaTagger.Instance.offlineVRRig;
            if (rig == null || rig.mainSkin == null || rig.mainSkin.material == null)
                return null;

            return rig;
        }

        public static void GhostMonkey()
        {
            if (HoldSkinRig() == null)
                return;

            bool holding = RightPrimaryHeld();
            if (holding == ghostMonkeyHeld)
                return;

            ghostMonkeyHeld = holding;
            RefreshHoldSkin();
        }

        public static void GhostMonkeyReset()
        {
            if (!ghostMonkeyHeld)
                return;

            ghostMonkeyHeld = false;
            RefreshHoldSkin();
        }

        public static void InvisibleMonkey()
        {
            if (HoldSkinRig() == null)
                return;

            bool holding = RightPrimaryHeld();
            if (holding == invisibleMonkeyHeld)
                return;

            invisibleMonkeyHeld = holding;
            RefreshHoldSkin();
        }

        public static void InvisibleMonkeyReset()
        {
            if (!invisibleMonkeyHeld)
                return;

            invisibleMonkeyHeld = false;
            RefreshHoldSkin();
        }

        private static void RefreshHoldSkin()
        {
            VRRig rig = HoldSkinRig();
            if (rig == null)
                return;

            Material mat = rig.mainSkin.material;

            if (ghostMonkeyHeld || invisibleMonkeyHeld)
            {
                if (!holdSkinActive)
                {
                    holdSkinOldColor = mat.color;
                    SpawnHoldDecoy(rig);
                    SetMaterialTransparent(mat, true);
                    holdSkinActive = true;
                }

                Color c = mat.color;
                c.a = 0f;
                mat.color = c;
            }
            else if (holdSkinActive)
            {
                RemoveHoldDecoy();
                mat.color = holdSkinOldColor;
                SetMaterialTransparent(mat, false);
                holdSkinActive = false;
            }
        }

      
        
        private static void SpawnHoldDecoy(VRRig rig)
        {
            if (holdDecoy != null || rig.mainSkin == null)
                return;

            SkinnedMeshRenderer smr = rig.mainSkin;

            Mesh baked = new Mesh();
            smr.BakeMesh(baked);

            holdDecoy = new GameObject("HoldDecoy");
            holdDecoy.transform.position = smr.transform.position;
            holdDecoy.transform.rotation = smr.transform.rotation;
            holdDecoy.transform.localScale = smr.transform.lossyScale;

            holdDecoy.AddComponent<MeshFilter>().mesh = baked;

            MeshRenderer mr = holdDecoy.AddComponent<MeshRenderer>();
            Material copy = new Material(smr.material);
            copy.color = holdSkinOldColor;
            mr.material = copy;
        }

        private static void RemoveHoldDecoy()
        {
            if (holdDecoy != null)
                UnityEngine.Object.Destroy(holdDecoy);
            holdDecoy = null;
        }

        private static void SetMaterialTransparent(Material mat, bool transparent)
        {
            if (transparent)
            {
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = -1;
            }
        }

        private static Vector3 ServerPos;
        private static float getOwnershipDelay;
        private static Coroutine BugCoroutine;

        private static ThrowableBug GetBugObject(string name)
        {
            if (GorillaTagger.Instance == null)
                return null;

            ThrowableBug[] bugs = UnityEngine.Object.FindObjectsOfType<ThrowableBug>();
            ThrowableBug best = null;
            float bestDist = float.MaxValue;
            Vector3 me = GorillaTagger.Instance.bodyCollider.transform.position;

            foreach (ThrowableBug b in bugs)
            {
                if (b == null)
                    continue;
                if (!string.IsNullOrEmpty(name) && !b.gameObject.name.Contains(name))
                    continue;

                float d = Vector3.SqrMagnitude(b.transform.position - me);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = b;
                }
            }

            return best;
        }

        private static System.Collections.IEnumerator ReturnRig()
        {
            yield return null;
            yield return null;
            if (VRRig.LocalRig != null)
                VRRig.LocalRig.enabled = true;
        }

        public static ThrowableBug GetBug(string name)
        {
            ThrowableBug bug = GetBugObject(name);

            if (bug == null)
                return null;

            GameObject bugObject = bug.gameObject;

            if (!NetworkSystem.Instance.InRoom)
                return bug;

            RequestableOwnershipGuard guard = bug.worldShareableInstance.guard;
            if (guard == null)
                return null;

            if (!bug.IsMyItem())
            {
                if (bug.currentState != TransferrableObject.PositionState.Dropped && bug.currentState != TransferrableObject.PositionState.None)
                    return null;

                VRRig.LocalRig.enabled = true;
                if (Vector3.SqrMagnitude(bugObject.transform.position - GorillaTagger.Instance.bodyCollider.transform.position) > 15f)
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = bugObject.transform.position;

                    if (BugCoroutine != null)
                        GorillaTagger.Instance.StopCoroutine(BugCoroutine);

                    BugCoroutine = GorillaTagger.Instance.StartCoroutine(ReturnRig());
                }

                if (Vector3.SqrMagnitude(bugObject.transform.position - ServerPos) > 15f)
                    return null;

                if (Time.time < getOwnershipDelay)
                    return null;

                getOwnershipDelay = Time.time + 0.5f;

                NetworkingState guardState = guard.currentState;
                Action failureAction = null;
                if ((int)guardState < 3)
                    failureAction = () => guard.currentState = guardState;

                switch (guard.currentState)
                {
                    case NetworkingState.IsOwner:
                        return null;
                    case NetworkingState.IsBlindClient:
                        guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, failureAction);
                        guard.currentState = NetworkingState.RequestingOwnershipWaitingForSight;
                        return null;
                    case NetworkingState.IsClient:
                        guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, failureAction);
                        guard.ownershipRequestNonce = Guid.NewGuid().ToString();
                        guard.currentState = NetworkingState.RequestingOwnership;
                        guard.netView.SendRPC("OwnershipRequested", guard.actualOwner, guard.ownershipRequestNonce);
                        return null;
                    case NetworkingState.ForcefullyTakingOver:
                    case NetworkingState.RequestingOwnership:
                    case NetworkingState.RequestingOwnershipWaitingForSight:
                    case NetworkingState.ForcefullyTakingOverWaitingForSight:
                        guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, failureAction);
                        return null;
                    default:
                        return null;
                }
            }

            if (BugCoroutine != null)
            {
                GorillaTagger.Instance.StopCoroutine(BugCoroutine);
                VRRig.LocalRig.enabled = true;
            }

            bug.worldShareableInstance.transferableObjectState = TransferrableObject.PositionState.Dropped;
            return bug;
        }

        public static void BugGun()
        {
            if (GorillaTagger.Instance == null || ControllerInputPoller.instance == null)
                return;

            if (!ControllerInputPoller.instance.rightGrab)
                return;

            ServerPos = GorillaTagger.Instance.bodyCollider.transform.position;

            ThrowableBug bug = GetBug(null);
            if (bug == null)
                return;

            bug.transform.position = GorillaTagger.Instance.rightHandTransform.position;
        }

        public static void Get_Bracelet(bool Enable, bool isleft)
        {
            if (Enable)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, true, isleft);
                Safetyy.RPCProtection();
            }
            else
            {
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, false, isleft);
            }
        }

        public static void UnlockLemming()
        {
            foreach (ScavengerTarget scavengerManager in UnityEngine.Object.FindObjectsOfType(typeof(ScavengerTarget)))
            {
                if (scavengerManager.TargetName.Contains("Lemming"))
                {
                    UnityEngine.Object.FindObjectOfType<ScavengerManager>().Collect(scavengerManager);
                }
            }

        

            var cosmeticItem = new CosmeticsController.CosmeticItem { itemName = "LMAWS." };

            CosmeticsController.instance.itemToBuy = cosmeticItem;
            CosmeticsController.instance.PurchaseItem();
        }

        public static void RGBMonke()
        {
            float time = Time.time * 1.8f;
            var R = Mathf.Sin(time) * 0.5f + 0.5f;
            var G = Mathf.Sin(time + 2f * Mathf.PI / 3f) * 0.5f + 0.5f;
            var B = Mathf.Sin(time + 4f * Mathf.PI / 3f) * 0.5f + 0.5f;
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { R, G, B });
        }

        private static GameObject drawRightPointer;
        private static GameObject drawLeftPointer;
        private static readonly List<GameObject> drawings = new List<GameObject>();
        private static Color drawColor = Color.white;
        private static int drawCurrentColor;
        private static bool drawColorCooldown;

        public static void Draw()
        {
            if (GTPlayer.Instance == null || ControllerInputPoller.instance == null)
                return;

            if (!ControllerInputPoller.instance.rightGrab)
            {
                if (drawRightPointer != null) UnityEngine.Object.Destroy(drawRightPointer);
                drawRightPointer = null;
                if (drawLeftPointer != null) UnityEngine.Object.Destroy(drawLeftPointer);
                drawLeftPointer = null;
                drawColorCooldown = false;
                return;
            }

            if (drawRightPointer == null) drawRightPointer = MakeOrb();
            if (drawLeftPointer == null) drawLeftPointer = MakeOrb();

            drawRightPointer.transform.position = GTPlayer.Instance.RightHand.controllerTransform.position;
            drawLeftPointer.transform.position = GTPlayer.Instance.LeftHand.controllerTransform.position;
            SetOrbColor(drawRightPointer, drawColor);
            SetOrbColor(drawLeftPointer, drawColor);

            GameObject rightDot = MakeOrb();
            rightDot.transform.position = GTPlayer.Instance.RightHand.controllerTransform.position;
            SetOrbColor(rightDot, drawColor);
            drawings.Add(rightDot);

            if (ControllerInputPoller.instance.leftGrab)
            {
                GameObject leftDot = MakeOrb();
                leftDot.transform.position = GTPlayer.Instance.LeftHand.controllerTransform.position;
                SetOrbColor(leftDot, drawColor);
                drawings.Add(leftDot);
            }

            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                if (!drawColorCooldown)
                {
                    drawCurrentColor = (drawCurrentColor + 1) % 13;
                    drawColor = (drawCurrentColor == 1) ? Color.blue : Color.white;
                    drawColorCooldown = true;
                }
                return;
            }

            drawColorCooldown = false;
        }

        public static void StopDraw()
        {
            if (drawRightPointer != null) UnityEngine.Object.Destroy(drawRightPointer);
            drawRightPointer = null;
            if (drawLeftPointer != null) UnityEngine.Object.Destroy(drawLeftPointer);
            drawLeftPointer = null;
            foreach (GameObject dot in drawings) if (dot != null) UnityEngine.Object.Destroy(dot);
            drawings.Clear();
        }

        private static GameObject motionTrailObj;
        private static TrailRenderer motionTrail;

        public static void MotionTrail()
        {
            if (GorillaTagger.Instance == null || GorillaTagger.Instance.bodyCollider == null)
                return;

            if (motionTrail == null)
            {
                motionTrailObj = new GameObject("ParrotMotionTrail");
                motionTrailObj.hideFlags = HideFlags.HideAndDontSave;
                motionTrail = motionTrailObj.AddComponent<TrailRenderer>();
                motionTrail.time = 0.7f;
                motionTrail.startWidth = 0.35f;
                motionTrail.endWidth = 0f;
                motionTrail.minVertexDistance = 0.05f;
                motionTrail.numCapVertices = 4;
                motionTrail.autodestruct = false;
                motionTrail.receiveShadows = false;
                motionTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                motionTrail.material = new Material(Shader.Find("GUI/Text Shader"));
            }

            motionTrailObj.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
            float mScale = GTPlayer.Instance != null ? GTPlayer.Instance.scale : 1f;
            motionTrail.startWidth = 0.35f * mScale;

            Color mc;
            try { mc = Parrot.client.Settings.backgroundColor.GetCurrentColor(); }
            catch { mc = Color.cyan; }
            motionTrail.startColor = mc;
            Color mend = mc; mend.a = 0f;
            motionTrail.endColor = mend;
        }

        public static void MotionTrailStop()
        {
            if (motionTrailObj != null) UnityEngine.Object.Destroy(motionTrailObj);
            motionTrailObj = null;
            motionTrail = null;
        }

        private const int OrbCount = 5;
        private static GameObject[] orbs;

        public static void OrbitBalls()
        {
            if (GorillaTagger.Instance == null || GorillaTagger.Instance.bodyCollider == null)
                return;

            if (orbs == null)
            {
                orbs = new GameObject[OrbCount];
                for (int i = 0; i < OrbCount; i++) orbs[i] = MakeOrb();
            }

            Vector3 center = GorillaTagger.Instance.bodyCollider.transform.position;
            float scale = GTPlayer.Instance != null ? GTPlayer.Instance.scale : 1f;
            float radius = 0.9f * scale;
            float t = Time.time * 2f;

            Color c;
            try { c = Parrot.client.Settings.backgroundColor.GetCurrentColor(); }
            catch { c = Color.cyan; }

            for (int i = 0; i < OrbCount; i++)
            {
                if (orbs[i] == null) orbs[i] = MakeOrb();
                float a = t + i * (Mathf.PI * 2f / OrbCount);
                float y = Mathf.Sin(t * 1.5f + i) * 0.3f * scale;
                orbs[i].transform.position = center + new Vector3(Mathf.Cos(a) * radius, y, Mathf.Sin(a) * radius);
                orbs[i].transform.localScale = Vector3.one * 0.1f * scale;
                SetOrbColor(orbs[i], c);
            }
        }

        public static void OrbitBallsStop()
        {
            if (orbs != null)
                for (int i = 0; i < orbs.Length; i++)
                    if (orbs[i] != null) UnityEngine.Object.Destroy(orbs[i]);
            orbs = null;
        }

        private static GameObject MakeOrb()
        {
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.transform.localScale = Vector3.one * 0.1f;
            UnityEngine.Object.Destroy(orb.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(orb.GetComponent<Collider>());
            Material mat = new Material(Shader.Find("GorillaTag/UberShader"));
            mat.EnableKeyword("_EMISSION");
            orb.GetComponent<Renderer>().material = mat;
            return orb;
        }

        private static void SetOrbColor(GameObject obj, Color col)
        {
            Renderer r = obj.GetComponent<Renderer>();
            if (r == null) return;
            r.material.color = col;
            if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", col);
            if (r.material.HasProperty("_EmissionColor")) r.material.SetColor("_EmissionColor", col);
        }


        static Dictionary<string, string> modsForModCheck = new Dictionary<string, string> {

            { "genesis", "Genesis" },
            { "HP_Left", "Holdable Pad" },
            { "GrateVersion", "Grate" },
            { "void", "Void" },
            { "BANANAOS", "Banana OS" },
            { "GC", "Gorilla Craft" },
            { "CarName", "Gorilla Vehicles" },
            { "6p72ly3j85pau2g9mda6ib8px", "CCM V2" },
            { "FPS-Nametags for Zlothy", "FPS Tags" },
            { "ORBIT", "Orbit" },
            { "Violet On Top", "Violet" },
            { "MP25", "Monke Phone" },
            { "GorillaWatch", "Gorilla Watch" },
            { "InfoWatch", "Gorilla Info Watch" },
            { "BananaPhone", "Banana Phone" },
            { "Vivid", "Vivid" },
            { "RGBA", "Custom Cosmetics" },
            { "cheese is gouda", "Whos Icheating" },
            { "shirtversion", "Gorilla Shirts" },
            { "gpronouns", "Gorilla Pronouns" },
            { "gfaces", "Gorilla Faces" },
            { "monkephone", "Monke Phone" },
            { "pmversion", "Player Models" },
            { "gtrials", "Gorilla Trials" },
            { "msp", "Monke Smartphone" },
            { "gorillastats", "Gorilla Stats" },
            { "MediaPad", "Media Pad" },
            { "using gorilladrift", "Gorilla Drift" },
            { "monkehavocversion", "Monke Havoc" },
            { "tictactoe", "Tic Tac Toe" },
            { "ccolor", "Index" },
            { "imposter", "Gorilla Among Us" },
            { "spectapeversion", "Spec Tape" },
            { "cats", "Cats" },
            { "made by biotest05 :3", "Dogs" },
            { "fys cool magic mod", "Fys Magic Mod" },
            { "colour", "Custom Cosmetics" },
            { "chainedtogether", "Chained Together" },
            { "goofywalkversion", "Goofy Walk" },
            { "void_menu_open", "Void" },
            { "violetpaiduser", "Violet Paid" },
            { "violetfree", "Violet Free" },
            { "obsidianmc", "Obsidian.Lol" },
            { "dark", "Shiba GT Dark" },
            { "hidden menu", "Hidden" },
            { "oblivionuser", "Oblivion" },
            { "hgrehngio889584739_hugb\n", "Resurgence" },
            { "eyerock reborn", "Eye Rock" },
            { "asteroidlite", "Asteroid Lite" },
            { "elux", "Elux" },
            { "cokecosmetics", "Coke Cosmetx" },
            { "GFaces", "G Faces" },
            { "github.com/maroon-shadow/SimpleBoards", "Simple Boards" },
            { "ObsidianMC", "Obsidian" },
            { "hgrehngio889584739_hugb", "Resurgence" },
            { "GTrials", "G Trials" },
            { "github.com/ZlothY29IQ/GorillaMediaDisplay", "Gorilla Media Display" },
            { "github.com/ZlothY29IQ/TooMuchInfo", "Too Much Info" },
            { "github.com/ZlothY29IQ/RoomUtils-IW", "Room Utils IW" },
            { "github.com/ZlothY29IQ/MonkeClick", "Monke Click" },
            { "github.com/ZlothY29IQ/MonkeClick-CI", "Monke Click CI" },
            { "github.com/ZlothY29IQ/MonkeRealism", "Monke Realism" },
            { "GorillaCinema", "Gorilla Cinema" },
            { "ChainedTogetherActive", "Chained Together" },
            { "GPronouns", "G Pronouns" },
            { "CSVersion", "Custom Skin" },
            { "github.com/ZlothY29IQ/Zloth-RecRoomRig", "Zloth Rec Room Rig" },
            { "ShirtProperties", "Shirts Old" },
            { "GorillaShirts", "Shirts" },
            { "GS", "Old Shirts" },
            { "6XpyykmrCthKhFeUfkYGxv7xnXpoe2", "CCM V2" },
            { "Body Tracking", "Body Track Old" },
            { "Body Estimation", "Han Body Est" },
            { "Gorilla Track", "Body Track" },
            { "CustomMaterial", "Custom Cosmetics" },
            { "I like cheese", "Rec Room Rig" },
            { "silliness", "Silliness" },
            { "EmoteWheel", "Fortnite Emote Wheel" },
            { "untitled", "Untitled" },
            { "BoyDoILoveInformation Public", "BoyDoILoveInformation" },
            { "DTAOI", "DTAOI" },
            { "GorillaShop", "GorillaShop" },
            { "Fusioned", "Fusioned" },
            { "y u lookin in here weirdo", "Malachi Menu Reborn" },
            { "ØƦƁƖƬ", "Orbit" },
            { "Atlas", "Atlas" }
        };

        public static void BreakModCheckers()
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            foreach (string mod in modsForModCheck.Keys)
            {
                hash[mod] = true;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public static void GrabRig()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = VRRig.LocalRig.rightHandTransform.position;
            }
            else if (!ControllerInputPoller.instance.rightGrab)
            {
                VRRig.LocalRig.enabled = true;
            }
        }

        public static void MoveRigGun()
        {
            GorillaTagger.Instance.offlineVRRig.enabled = true;

            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.spherepointer != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position =
                        Gunlib.spherepointer.transform.position + Vector3.up * 1f;
                }
            }, false);
        }

        public static GameObject UCam;

        public static void SpectateGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.LockedPlayer == null)
                    return;

                if (UCam == null)
                {
                    UCam = new GameObject("Freecam boiiiiiiiiiiiiiiiiii");

                    var c = UCam.AddComponent<Camera>();
                    c.fieldOfView = 120;
                    c.depth = 4;
                    c.nearClipPlane = 0.1f;
                    c.cameraType = CameraType.Game;

                    UCam.transform.position = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position;
                    UCam.transform.rotation = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.rotation;

                    UnityEngine.Object.DontDestroyOnLoad(UCam);
                }

                float lerpSpeed = 12f;

                UCam.transform.position = Vector3.Lerp(
                    UCam.transform.position,
                    Gunlib.LockedPlayer.head.rigTarget.position,
                    lerpSpeed * Time.deltaTime);

                UCam.transform.rotation = Quaternion.Slerp(
                    UCam.transform.rotation,
                    Gunlib.LockedPlayer.head.rigTarget.rotation,
                    lerpSpeed * Time.deltaTime);

            }, true);

            if (Gunlib.LockedPlayer == null && UCam != null)
            {
                UnityEngine.Object.Destroy(UCam);
                UCam = null;
            }
        }

        public static void LoudHandTaps()
        {
            EffectDataPatch.enabled = true;
            EffectDataPatch.tapsEnabled = true;
            EffectDataPatch.doOverride = true;
            EffectDataPatch.overrideVolume = 99999f;
            EffectDataPatch.tapMultiplier = 10;
            GorillaTagger.Instance.handTapVolume = 99999f;
        }

        public static void SilentHandTaps()
        {
            EffectDataPatch.enabled = true;
            EffectDataPatch.tapsEnabled = false;
            EffectDataPatch.doOverride = false;
            EffectDataPatch.overrideVolume = 0f;
            EffectDataPatch.tapMultiplier = 0;
            GorillaTagger.Instance.handTapVolume = 0f;
        }

        private static float instantPartyDelay;
        public static void InstantParty()
        {
            if (Time.time > instantPartyDelay)
            {
                instantPartyDelay = Time.time + 0.1f;

                FriendshipGroupDetection.Instance.suppressPartyCreationUntilTimestamp = 0f;
                FriendshipGroupDetection.Instance.groupCreateAfterTimestamp = 0f;

                List<int> provisionalMembers = FriendshipGroupDetection.Instance.playersInProvisionalGroup;

                if (provisionalMembers.Count > 0)
                {
                    Color targetColor = GTColor.RandomHSV(FriendshipGroupDetection.Instance.braceletRandomColorHSVRanges);
                    FriendshipGroupDetection.Instance.myBraceletColor = targetColor;

                    List<int> members = new List<int> { PhotonNetwork.LocalPlayer.ActorNumber };
                    members.AddRange(from player in PhotonNetwork.PlayerListOthers where FriendshipGroupDetection.Instance.IsInMyGroup(player.UserId) || provisionalMembers.Contains(player.ActorNumber) select player.ActorNumber);
                    FriendshipGroupDetection.Instance.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(targetColor), members.ToArray(), false);
                    Safetyy.RPCProtection();
                }
            }
        }

        public static float splashDel;
        public static void GiveWaterSplashHandsGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (PhotonNetwork.InRoom && Gunlib.LockedPlayer != null)
                {
                    VRRig target = Gunlib.LockedPlayer;

                    if (target.rightMiddle.calcT > 0.5f || target.leftMiddle.calcT > 0.5f)
                    {
                        if (Time.time > splashDel)
                        {
                            Vector3 splashPosition = target.rightMiddle.calcT > 0.5f
                                ? target.rightHandTransform.position
                                : target.leftHandTransform.position;

                            Quaternion splashRotation = target.rightMiddle.calcT > 0.5f
                                ? target.rightHandTransform.rotation
                                : target.leftHandTransform.rotation;

                            GorillaTagger.Instance.myVRRig.SendRPC(
                                "RPC_PlaySplashEffect",
                                RpcTarget.All,
                                splashPosition,
                                splashRotation,
                                100f,
                                100f,
                                true,
                                false
                            );

                            splashDel = Time.time + 0.1f;
                        }
                    }
                }
            }, true);
        }

        public static Quaternion RandomQuaternion()
        {
            return UnityEngine.Random.rotation;
        }

        public static Vector3 RandomVector3(float range)
        {
            return new Vector3(
                UnityEngine.Random.Range(-range, range),
                UnityEngine.Random.Range(-range, range),
                UnityEngine.Random.Range(-range, range)
            );
        }

        public static void Predictions()
        {
            VRRig rig = VRRig.LocalRig;
            if (rig == null)
                return;

            float amount = Parrot.client.Mods.Settings.Movement.predictionAmount;

            if (rig.leftHand != null)
                rig.leftHand.trackingPositionOffset = RandomVector3(amount);

            if (rig.rightHand != null)
                rig.rightHand.trackingPositionOffset = RandomVector3(amount);
        }

        private static int sizeIndex = 2;

        public static void SizeChanger()
        {
            string[] names = new string[] { "Tiny", "Small", "Normal", "Big", "Giant" };
            float[] mult = new float[] { 0.35f, 0.65f, 1f, 1.6f, 2.4f };

            sizeIndex = (sizeIndex + 1) % names.Length;
            if (GTPlayer.Instance != null)
                GTPlayer.Instance.SetScaleMultiplier(mult[sizeIndex]);

            var button = Parrot.client.Menu.Main.GetIndex("Size Changer");
            if (button != null)
                button.overlapText = "Size Changer: " + names[sizeIndex];
        }

        public static void StopPredictions()
        {
            VRRig rig = VRRig.LocalRig;
            if (rig == null)
                return;

            if (rig.leftHand != null)
                rig.leftHand.trackingPositionOffset = Vector3.zero;

            if (rig.rightHand != null)
                rig.rightHand.trackingPositionOffset = Vector3.zero;
        }

        public static void WaterSplashAura() =>
        GorillaTagger.Instance.myVRRig.SendRPC(
        "RPC_PlaySplashEffect",
        RpcTarget.All,
        VRRig.LocalRig.transform.position + RandomVector3(2f),
        RandomQuaternion(),
        100f,
        100f,
        true,
        false
        );

        public static void OrbitWaterSplash() =>
            GorillaTagger.Instance.myVRRig.SendRPC(
                "RPC_PlaySplashEffect",
                RpcTarget.All,
                GorillaTagger.Instance.headCollider.transform.position +
                new Vector3(
                    Mathf.Cos((float)Time.frameCount / 30f),
                    1f,
                    Mathf.Sin((float)Time.frameCount / 30f)
                ),
                RandomQuaternion(),
                100f,
                100f,
                true,
                false
            );

        public static void PrioritizeVoiceGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.LockedPlayer != null)
                {
                    foreach (VRRig rig in VRRigCache.ActiveRigs)
                        rig.voiceAudio.volume = rig == Gunlib.LockedPlayer ? 2f : 0.1f;
                }
            }, true);

            if (Gunlib.LockedPlayer == null)
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                    rig.voiceAudio.volume = 1f;
            }
        }

        private static float muteDelay;

        public static void MuteGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                VRRig target = Gunlib.LockedPlayer;

                if (target != null && target.creator != null && Time.time > muteDelay)
                {
                    foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                    {
                        if (line.linePlayer == target.creator)
                        {
                            muteDelay = Time.time + 0.5f;

                            line.muteButton.isOn = !line.muteButton.isOn;
                            line.PressButton(
                                line.muteButton.isOn,
                                GorillaPlayerLineButton.ButtonType.Mute
                            );

                            break;
                        }
                    }
                }
            }, true);
        }

        public static void GliderGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.spherepointer == null)
                    return;

                GliderHoldable[] allType = Resources.FindObjectsOfTypeAll<GliderHoldable>();

                foreach (GliderHoldable glider in allType)
                {
                    if (glider == null)
                        continue;

                    PhotonView view = glider.GetComponent<PhotonView>();

                    if (view != null && view.Owner == PhotonNetwork.LocalPlayer)
                    {
                        glider.transform.position =
                            Gunlib.spherepointer.transform.position + Vector3.up;
                    }
                    else
                    {
                        NetworkHoldableObject holdable =
                            glider.GetComponent<NetworkHoldableObject>();

                        if (holdable != null)
                            holdable.OnHover(null, null);
                    }
                }
            }, false);
        }

        public static void OrbitGliders()
        {
            GliderHoldable[] allType =
                Resources.FindObjectsOfTypeAll<GliderHoldable>();

            int index = 0;

            foreach (GliderHoldable glider in allType)
            {
                if (glider == null)
                    continue;

                PhotonView view = glider.GetComponent<PhotonView>();

                if (view != null && view.Owner == PhotonNetwork.LocalPlayer)
                {
                    float angle = 360f / Mathf.Max(1, allType.Length) * index;
                    float rotation = angle + Time.frameCount / 30f;

                    glider.transform.position =
                        GorillaTagger.Instance.headCollider.transform.position +
                        new Vector3(
                            Mathf.Cos(rotation) * 5f,
                            2f,
                            Mathf.Sin(rotation) * 5f
                        );

                    index++;
                }
                else
                {
                    NetworkHoldableObject holdable =
                        glider.GetComponent<NetworkHoldableObject>();

                    if (holdable != null)
                        holdable.OnHover(null, null);
                }
            }
        }
        public static void GliderOrbitPlayerGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.LockedPlayer == null)
                    return;

                GliderHoldable[] gliders =
                    Resources.FindObjectsOfTypeAll<GliderHoldable>();

                int index = 0;
                int ownedCount = 0;

                foreach (GliderHoldable glider in gliders)
                {
                    if (glider == null)
                        continue;

                    PhotonView view = glider.GetComponent<PhotonView>();

                    if (view != null && view.Owner == PhotonNetwork.LocalPlayer)
                        ownedCount++;
                }

                foreach (GliderHoldable glider in gliders)
                {
                    if (glider == null)
                        continue;

                    PhotonView view = glider.GetComponent<PhotonView>();

                    if (view != null && view.Owner == PhotonNetwork.LocalPlayer)
                    {
                        float angle = (360f / Mathf.Max(1, ownedCount)) * index;
                        float rotation = angle + Time.frameCount / 30f;

                        glider.transform.position =
                            Gunlib.LockedPlayer.transform.position +
                            new Vector3(
                                Mathf.Cos(rotation) * 5f,
                                2f,
                                Mathf.Sin(rotation) * 5f
                            );

                        index++;
                    }
                    else
                    {
                        NetworkHoldableObject holdable =
                            glider.GetComponent<NetworkHoldableObject>();

                        if (holdable != null)
                            holdable.OnHover(null, null);
                    }
                }
            }, true);
        }

        public static void GliderBlindGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.LockedPlayer == null)
                    return;

                GliderHoldable[] gliders =
                    Resources.FindObjectsOfTypeAll<GliderHoldable>();

                foreach (GliderHoldable glider in gliders)
                {
                    if (glider == null)
                        continue;

                    PhotonView view = glider.GetComponent<PhotonView>();

                    if (view != null && view.Owner == PhotonNetwork.LocalPlayer)
                    {
                        glider.transform.position =
                            Gunlib.LockedPlayer.headMesh.transform.position;

                        glider.transform.rotation = UnityEngine.Random.rotation;
                    }
                    else
                    {
                        NetworkHoldableObject holdable =
                            glider.GetComponent<NetworkHoldableObject>();

                        if (holdable != null)
                            holdable.OnHover(null, null);
                    }
                }
            }, true);
        }

        public static float hoverboardDelay;

        public static void OrbitHoverboards()
        {
            if (Time.time < hoverboardDelay)
                return;

            hoverboardDelay = Time.time + 0.25f;

            Vector3 center = GorillaTagger.Instance.headCollider.transform.position;
            float time = Time.frameCount / 30f;

            for (int i = 0; i < 2; i++)
            {
                float angle = i * 180f;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle + time) * 2f,
                    1f,
                    Mathf.Sin(angle + time) * 2f
                );

                Vector3 secondOffset = new Vector3(
                    Mathf.Cos(angle - 25f + time) * 2f,
                    1f,
                    Mathf.Sin(angle - 25f + time) * 2f
                );

                Vector3 direction = (secondOffset - offset).normalized;

                Quaternion rotation = Quaternion.LookRotation(direction);

                FreeHoverboardManager.instance.SendDropBoardRPC(
                    center + offset,
                    rotation,
                    direction * 6.5f,
                    new Vector3(0f, 360f, 0f),
                    UnityEngine.Random.ColorHSV()
                );

                Safetyy.RPCProtection();
            }

        }

        public static void ShootHoverboards()
        {
            if (ControllerInputPoller.instance.rightGrab &&
                Time.time >= hoverboardDelay)
            {
                hoverboardDelay = Time.time + 0.5f;

                Transform hand = GorillaTagger.Instance.rightHandTransform;

                FreeHoverboardManager.instance.SendDropBoardRPC(
                    hand.position,
                    hand.rotation,
                    hand.forward * 10f,
                    Vector3.zero,
                    UnityEngine.Random.ColorHSV()
                );

                Safetyy.RPCProtection();
            }
        }

        public static void HoverboardGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.spherepointer == null)
                    return;

                if (Time.time < hoverboardDelay)
                    return;

                hoverboardDelay = Time.time + 0.5f;

                Vector3 position = Gunlib.spherepointer.transform.position;

                FreeHoverboardManager.instance.SendDropBoardRPC(
                    position,
                    Gunlib.spherepointer.transform.rotation,
                    Vector3.zero,
                    Vector3.zero,
                    UnityEngine.Random.ColorHSV()
                );

                Safetyy.RPCProtection();
            }, false);
        }

        private static float splashAnnoyDelay;

        public static void SplashAnnoyAll()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat <= 0.5f)
                return;

            if (!PhotonNetwork.InRoom || Time.time < splashAnnoyDelay)
                return;

            splashAnnoyDelay = Time.time + 0.5f;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig == VRRig.LocalRig)
                    continue;

                GorillaTagger.Instance.myVRRig.SendRPC(
                    "RPC_PlaySplashEffect",
                    RpcTarget.All,
                    rig.headMesh.transform.position +
                        rig.headMesh.transform.forward * 0.2f,
                    rig.headMesh.transform.rotation,
                    999f,
                    999f,
                    true,
                    true
                );
            }

            Safetyy.RPCProtection();
        }

        public static float waterdelay;

        public static void Watergun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.LockedPlayer == null)
                    return;

                if (!PhotonNetwork.InRoom || Time.time < waterdelay)
                    return;

                waterdelay = Time.time + 0.3f;

                Vector3 position =
                    Gunlib.LockedPlayer.transform.position;

                Quaternion rotation =
                    Gunlib.LockedPlayer.transform.rotation;

                GorillaTagger.Instance.myVRRig.SendRPC(
                    "RPC_PlaySplashEffect",
                    RpcTarget.All,
                    position,
                    rotation,
                    100f,
                    100f,
                    true,
                    false
                );

                Safetyy.RPCProtection();
            }, true);
        }

        public static void FreezeRig()
        {
            if (!ControllerInputPoller.instance.rightControllerTriggerButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                return;
            }
            GorillaTagger.Instance.offlineVRRig.transform.position = GTPlayer.Instance.bodyCollider.transform.position;
            GorillaTagger.Instance.offlineVRRig.enabled = false;
        }

        public static void AutoFunnyRun()
        {
            if (GTPlayer.Instance == null || ControllerInputPoller.instance == null)
                return;

            if (!ControllerInputPoller.instance.rightGrab)
                return;

            float num = Mathf.Sin(-Time.time * 40f) * -0.3f;
            float num2 = Mathf.Cos(-Time.time * 40f) * -0.3f;
            Transform transform = GTPlayer.Instance.bodyCollider.transform;
            GTPlayer.Instance.RightHand.controllerTransform.position = transform.position + transform.forward * num2 + transform.up * num;
            GTPlayer.Instance.LeftHand.controllerTransform.position = transform.position + transform.forward * -num2 + transform.up * -num;
        }

        public static void CopyGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = Gunlib.LockedPlayer.transform.position;
                VRRig.LocalRig.transform.rotation = Gunlib.LockedPlayer.transform.rotation;
                VRRig.LocalRig.leftHand.rigTarget.transform.position = Gunlib.LockedPlayer.leftHandTransform.position;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = Gunlib.LockedPlayer.rightHandTransform.position;
                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Gunlib.LockedPlayer.leftHandTransform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Gunlib.LockedPlayer.rightHandTransform.rotation;
                VRRig.LocalRig.head.rigTarget.transform.rotation = Gunlib.LockedPlayer.headMesh.transform.rotation;
            }, true);
        }

        private static void FireParticle(Vector3 position)
        {
            GameObject fireEffect = new GameObject("FireEffect");
            fireEffect.transform.position = position;
            ParticleSystem fireParticles = fireEffect.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = fireParticles.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(Color.red, Color.black);
            mainModule.startSize = 0.05f;
            mainModule.startSpeed = 0.25f;
            mainModule.startLifetime = 1.5f;
            mainModule.loop = true;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
            mainModule.maxParticles = 30;
            ParticleSystemRenderer particleRenderer = fireParticles.GetComponent<ParticleSystemRenderer>();
            particleRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            particleRenderer.material.SetColor("_Color", Color.red);
            ParticleSystem.EmissionModule emission = fireParticles.emission;
            emission.rateOverTime = 5f;
            ParticleSystem.ShapeModule shape = fireParticles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 20f;
            shape.radius = 0.1f;
            UnityEngine.Object.Destroy(fireEffect, 0.5f);
        }
        public static void DemonicHands()
        {
            FireParticle(GorillaTagger.Instance.leftHandTransform.position);
            FireParticle(GorillaTagger.Instance.rightHandTransform.position);
        }

        public static void ParticleGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                FireParticle(Gunlib.spherepointer.transform.position);
            }, true);
        }

        public static void CopyMovementGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                VRRig target = Gunlib.LockedPlayer;

                if (target == null || target == VRRig.LocalRig)
                    return;

                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = target.transform.position;
                VRRig.LocalRig.transform.rotation = target.transform.rotation;

                VRRig.LocalRig.head.rigTarget.transform.position =
                    target.head.rigTarget.transform.position;

                VRRig.LocalRig.head.rigTarget.transform.rotation =
                    target.head.rigTarget.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position =
                    target.leftHandTransform.position;

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation =
                    target.leftHandTransform.rotation;

                VRRig.LocalRig.rightHand.rigTarget.transform.position =
                    target.rightHandTransform.position;

                VRRig.LocalRig.rightHand.rigTarget.transform.rotation =
                    target.rightHandTransform.rotation;

            }, true);

            if (Gunlib.LockedPlayer == null)
            {
                VRRig.LocalRig.enabled = true;
            }
        }

        public static void GrabBeachBall()
        {
            if (ControllerInputPoller.instance.rightGrab || Mouse.current.leftButton.isPressed)
            {
                GameObject.Find("BeachBall").transform.position = VRRig.LocalRig.rightHandTransform.position;
                GameObject.Find("Ball").transform.position = VRRig.LocalRig.rightHandTransform.position;
            }
        }

        public static void BeachBallGun()
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Gunlib.nray.collider == null)
                    return;

                Vector3 position = Gunlib.nray.point;

                GameObject beachBall = GameObject.Find("BeachBall");
                GameObject ball = GameObject.Find("Ball");

                if (beachBall != null)
                    beachBall.transform.position = position;

                if (ball != null)
                    ball.transform.position = position;

            }, false);
        }

    }
}
