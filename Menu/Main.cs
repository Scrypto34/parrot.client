using BepInEx;
using GorillaLocomotion;
using HarmonyLib;
using Parrot.client.Classes;
using Parrot.client.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using static Parrot.client.Menu.Buttons;
using static Parrot.client.Settings;

namespace Parrot.client.Menu
{
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class Main : MonoBehaviour
    {

        public static float nextAutoSave;

        public static void Prefix()
        {
                if (Time.time > nextAutoSave)
                {
                    nextAutoSave = Time.time + 15f;
                    try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
                }

                if (pendingRecreate > 0f && Time.time >= pendingRecreate)
                {
                    pendingRecreate = -1f;
                    try { RecreateMenu(); } catch { }
                }

                try { Classes.ClientSync.Tick(); } catch { }
                try { Classes.RemoteMenus.Tick(); } catch { }
                try { Classes.SahurSync.Tick(); } catch { }
                try { Classes.CartiSync.Tick(); } catch { }
                try { Classes.CartiJacksonSync.Tick(); } catch { }
                try { Classes.RoomIdSync.Tick(); } catch { }

                try
                {
                    bool toOpen = (!rightHanded && ControllerInputPoller.instance.leftControllerSecondaryButton) || (rightHanded && ControllerInputPoller.instance.rightControllerSecondaryButton);
                    bool keyboardOpen = UnityInput.Current.GetKey(keyboardButton);

                    if (menu == null)
                    {
                        if (toOpen || keyboardOpen)
                        {

                            try { Mods.Settings.Audio.PlayOpenSound(); } catch { }

                            animateOpen = true;
                            CreateMenu();
                            RecenterMenu(rightHanded, keyboardOpen);
                            if (reference == null)
                                CreateReference(rightHanded);
                        }
                    }
                    else
                    {
                        if (toOpen || keyboardOpen)
                            RecenterMenu(rightHanded, keyboardOpen);
                        else
                        {
                            GameObject shoulderCam = GameObject.Find("Shoulder Camera");
                            if (shoulderCam != null)
                            {
                                Transform vcam = shoulderCam.transform.Find("CM vcam1");
                                if (vcam != null)
                                    vcam.gameObject.SetActive(true);
                            }

                            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }

                            if (Mods.Settings.OpenAnim.index != 0)
                            {
                                Classes.MenuAnimator anim = menu.AddComponent<Classes.MenuAnimator>();
                                anim.type = Mods.Settings.OpenAnim.index;
                                anim.targetScale = menu.transform.localScale;
                                anim.closing = true;
                            }
                            else
                            {
                                Rigidbody comp = menu.AddComponent(typeof(Rigidbody)) as Rigidbody;
                                comp.linearVelocity = (rightHanded ? GTPlayer.Instance.LeftHand.velocityTracker : GTPlayer.Instance.RightHand.velocityTracker).GetAverageVelocity(true, 0);
                            }

                            Destroy(menu, 2f);
                            menu = null;

                            Destroy(reference);
                            reference = null;
                        }
                    }
                }
                catch (Exception exc)
                {
                    Debug.LogError(string.Format("{0} // Error initializing at {1}: {2}", PluginInfo.Name, exc.StackTrace, exc.Message));
                }

                try
                {
                    if (GunPointer != null)
                    {
                        if (!GunPointer.activeSelf)
                            Destroy(GunPointer);
                        else
                            GunPointer.SetActive(false);
                    }

                    if (GunLine != null)
                    {
                        if (!GunLine.gameObject.activeSelf)
                        {
                            Destroy(GunLine.gameObject);
                            GunLine = null;
                        }
                        else
                            GunLine.gameObject.SetActive(false);
                    }
                } catch { }

                try
                {

                        if (fpsObject != null)
                            fpsObject.text = "Version: " + PluginInfo.Version;

                        foreach (ButtonInfo button in buttons
                            .SelectMany(list => list)
                            .Where(button => button.enabled && button.method != null))
                        {
                            try
                            {
                                button.method.Invoke();
                            }
                            catch (Exception exc)
                            {
                                Debug.LogError(string.Format("{0} // Error with mod {1} at {2}: {3}", PluginInfo.Name, button.buttonText, exc.StackTrace, exc.Message));
                            }
                        }
                } catch (Exception exc)
                {
                    Debug.LogError(string.Format("{0} // Error with executing mods at {1}: {2}", PluginInfo.Name, exc.StackTrace, exc.Message));
                }
        }

        public static void CreateMenu()
        {
            Classes.OwnerList.EnsureLoaded();

            menuJustPlaced = true;

                menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(menu.GetComponent<Rigidbody>());
                Destroy(menu.GetComponent<BoxCollider>());
                Destroy(menu.GetComponent<Renderer>());
                float menuMult = Mods.Settings.MenuScale.Multiplier;
                float wideMult = (GetIndex("Wide Menu")?.enabled ?? false) ? 1.8f : 1f;
                menuWideMult = wideMult;
                menu.transform.localScale = new Vector3(0.1f, 0.3f * menuMult, 0.3825f * menuMult);

                menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(menuBackground.GetComponent<Rigidbody>());
                Destroy(menuBackground.GetComponent<BoxCollider>());
                menuBackground.transform.parent = menu.transform;
                menuBackground.transform.rotation = Quaternion.identity;
                menuBackground.transform.localScale = new Vector3(menuSize.x, menuSize.y * wideMult, menuSize.z);
                menuBackground.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
                menuBackground.transform.position = new Vector3(0.05f, 0f, 0f);

                Decorate(menuBackground, Classes.RoundedMesh.BackgroundRadius);

                if (rainbowOutline)
                    CreateRainbowOutline();

            ColorChanger colorChanger = null;
                if (GetIndex("Bg Gradient")?.enabled ?? false)
                {
                    Color bgBase = backgroundColor.colors[0].color;
                    Color light = Color.Lerp(bgBase, Color.white, 0.3f);
                    Color dark = Color.Lerp(bgBase, Color.black, 0.6f);
                    Renderer bgr = menuBackground.GetComponent<Renderer>();
                    Shader gradShader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Texture") ?? Shader.Find("GUI/Text Shader");
                    if (gradShader != null)
                        bgr.material.shader = gradShader;
                    bgr.material.mainTexture = MakeVerticalGradient(light, dark);
                    bgr.material.color = Color.white;
                }
                else
                {
                    colorChanger = menuBackground.AddComponent<ColorChanger>();
                    colorChanger.colors = backgroundColor;
                }

                if ((GetIndex("PC UI Bg Theme")?.enabled ?? false) && !UnityEngine.XR.XRSettings.isDeviceActive)
                    menuBackground.GetComponent<Renderer>().enabled = false;

                canvasObject = new GameObject();
                canvasObject.transform.parent = menu.transform;
                Canvas canvas = canvasObject.AddComponent<Canvas>();
                CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvasScaler.dynamicPixelsPerUnit = 1000f;

                Text text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObject.transform
                    }
                }.AddComponent<Text>();
                text.font = currentFont;
                text.text = PluginInfo.Name + " <color=grey>[</color><color=white>" + (pageNumber + 1).ToString() + "</color><color=grey>]</color>";
                text.fontSize = 1;
                text.color = textColors[0];
                text.supportRichText = true;
                text.fontStyle = FontStyle.Italic;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                RectTransform component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.28f * wideMult, 0.05f);
                component.position = new Vector3(0.06f, 0f, 0.165f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                if (fpsCounter)
                {
                    fpsObject = new GameObject
                    {
                        transform =
                        {
                            parent = canvasObject.transform
                        }
                    }.AddComponent<Text>();
                    fpsObject.font = currentFont;
                    fpsObject.text = "Version: " + PluginInfo.Version;
                    fpsObject.color = textColors[0];
                    fpsObject.fontSize = 1;
                    fpsObject.supportRichText = true;
                    fpsObject.fontStyle = FontStyle.Italic;
                    fpsObject.alignment = TextAnchor.MiddleCenter;
                    fpsObject.horizontalOverflow = HorizontalWrapMode.Overflow;
                    fpsObject.resizeTextForBestFit = true;
                    fpsObject.resizeTextMinSize = 0;
                    RectTransform component2 = fpsObject.GetComponent<RectTransform>();
                    component2.localPosition = Vector3.zero;
                    component2.sizeDelta = new Vector2(0.28f, 0.02f);
                    component2.position = new Vector3(0.06f, 0f, 0.135f);
                    component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                }

                    if (disconnectButton)
                    {
                        GameObject disconnectbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (!UnityInput.Current.GetKey(keyboardButton))
                            disconnectbutton.layer = 2;
                        Destroy(disconnectbutton.GetComponent<Rigidbody>());
                        disconnectbutton.GetComponent<BoxCollider>().isTrigger = true;
                        disconnectbutton.transform.parent = menu.transform;
                        disconnectbutton.transform.rotation = Quaternion.identity;
                        disconnectbutton.transform.localScale = new Vector3(0.09f, 0.9f * wideMult, 0.08f);
                        disconnectbutton.transform.localPosition = new Vector3(0.56f, 0f, 0.6f);
                        Decorate(disconnectbutton);
                        disconnectbutton.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
                        disconnectbutton.AddComponent<Classes.Button>().relatedText = "Disconnect";

                        colorChanger = disconnectbutton.AddComponent<ColorChanger>();
                        colorChanger.colors = buttonColors[0];

                        Text discontext = new GameObject
                        {
                            transform =
                            {
                                parent = canvasObject.transform
                            }
                        }.AddComponent<Text>();
                        discontext.text = "Disconnect";
                        discontext.font = currentFont;
                        discontext.fontSize = 1;
                        discontext.color = textColors[0];
                        discontext.alignment = TextAnchor.MiddleCenter;
                        discontext.resizeTextForBestFit = true;
                        discontext.resizeTextMinSize = 0;

                        RectTransform rectt = discontext.GetComponent<RectTransform>();
                        rectt.localPosition = Vector3.zero;
                        rectt.sizeDelta = new Vector2(0.2f, 0.03f);
                        rectt.localPosition = new Vector3(0.064f, 0f, 0.23f);
                        rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                    }

                    int layout = Mods.Settings.ArrowStyle.index;
                    bool scryptoFav = layout == 2;
                    bool sideArrows = layout == 1;
                    bool homeText = scryptoFav;

                    int arrowIcon = Mods.Settings.ArrowIcon.index;
                    if (scryptoFav && arrowIcon == 0) arrowIcon = 1;
                    string leftArrowImg = arrowIcon == 1 ? "" : (arrowIcon == 2 ? "arrow2left.png" : "left3.png");
                    string rightArrowImg = arrowIcon == 1 ? "" : (arrowIcon == 2 ? "arrow2right.png" : "right3.png");

                    Vector3 prevPos, nextPos, sideScale, prevIcon, nextIcon;
                    if (sideArrows)
                    {
                        prevPos = new Vector3(0.56f, 0.68f * wideMult, 0f);
                        nextPos = new Vector3(0.56f, -0.68f * wideMult, 0f);
                        sideScale = new Vector3(0.09f, 0.18f, 1.0f);
                        prevIcon = new Vector3(0.064f, 0.204f * wideMult, 0.003f);
                        nextIcon = new Vector3(0.064f, -0.204f * wideMult, 0.003f);
                    }
                    else if (scryptoFav)
                    {
                        prevPos = new Vector3(0.56f, 0.30f, -0.60f);
                        nextPos = new Vector3(0.56f, -0.30f, -0.60f);
                        sideScale = new Vector3(0.09f, 0.28f, 0.08f);
                        prevIcon = new Vector3(0.064f, 0.09f, -0.2295f);
                        nextIcon = new Vector3(0.064f, -0.09f, -0.2295f);
                    }
                    else
                    {
                        prevPos = new Vector3(0.56f, 0.25f, -0.60f);
                        nextPos = new Vector3(0.56f, -0.25f, -0.60f);
                        sideScale = new Vector3(0.09f, 0.30f, 0.08f);
                        prevIcon = new Vector3(0.064f, 0.075f, -0.2295f);
                        nextIcon = new Vector3(0.064f, -0.075f, -0.2295f);
                    }

                    GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (!UnityInput.Current.GetKey(keyboardButton))
                        gameObject.layer = 2;
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    gameObject.transform.parent = menu.transform;
                    gameObject.transform.rotation = Quaternion.identity;
                    gameObject.transform.localScale = sideScale;
                    gameObject.transform.localPosition = prevPos;
                    Decorate(gameObject);
                    gameObject.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
                    gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";

                    colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];

                    CreatePageArrow(leftArrowImg, "<", prevIcon);

                    gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (!UnityInput.Current.GetKey(keyboardButton))
                    {
                        gameObject.layer = 2;
                    }
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    gameObject.transform.parent = menu.transform;
                    gameObject.transform.rotation = Quaternion.identity;
                    gameObject.transform.localScale = sideScale;
                    gameObject.transform.localPosition = nextPos;
                    Decorate(gameObject);
                    gameObject.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
                    gameObject.AddComponent<Classes.Button>().relatedText = "NextPage";

                    colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];

                    CreatePageArrow(rightArrowImg, ">", nextIcon);

                    gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (!UnityInput.Current.GetKey(keyboardButton))
                        gameObject.layer = 2;
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    gameObject.transform.parent = menu.transform;
                    gameObject.transform.rotation = Quaternion.identity;
                    gameObject.transform.localScale = homeText ? new Vector3(0.09f, 0.28f, 0.08f) : new Vector3(0.09f, 0.16f, 0.08f);
                    gameObject.transform.localPosition = new Vector3(0.56f, 0f, -0.60f);
                    Decorate(gameObject);
                    gameObject.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
                    gameObject.AddComponent<Classes.Button>().relatedText = "Home";

                    colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];

                    CreatePageArrow(homeText ? "" : "home-button.png", homeText ? "Home" : "H", new Vector3(0.064f, 0f, -0.2295f), textColors[0]);

                    ButtonInfo[] activeButtons = VisibleButtons(currentCategory).Skip(pageNumber * buttonsPerPage).Take(buttonsPerPage).ToArray();
                    bool plusMinus = Mods.Settings.SwitchMode.index == 1;
                    for (int i = 0; i < activeButtons.Length; i++)
                    {
                        ButtonInfo ab = activeButtons[i];
                        if (plusMinus && ab.cycleBack != null && ab.method != null)
                        {
                            CreateButton(i * 0.1f, new ButtonInfo { buttonText = "-", toolTip = ab.toolTip }, 0.2f * wideMult, 0.34f * wideMult, ab.cycleBack);
                            CreateButton(i * 0.1f, ab, 0.5f * wideMult, 0f, () => { });
                            CreateButton(i * 0.1f, new ButtonInfo { buttonText = "+", toolTip = ab.toolTip }, 0.2f * wideMult, -0.34f * wideMult, ab.method);
                        }
                        else
                        {
                            CreateButton(i * 0.1f, ab, wideMult);
                        }
                    }

                    RenderPinnedButtons();

                if (animateOpen)
                {
                    animateOpen = false;
                    if (Mods.Settings.OpenAnim.index != 0)
                    {
                        Classes.MenuAnimator anim = menu.AddComponent<Classes.MenuAnimator>();
                        anim.type = Mods.Settings.OpenAnim.index;
                        anim.targetScale = menu.transform.localScale;
                    }
                }
        }

        public static bool animateOpen;
        public static bool menuJustPlaced;
        public static float menuWideMult = 1f;

        public static ButtonInfo[] VisibleButtons(int category) =>
            buttons[category].Where(button => button.IsVisible).ToArray();

        public static readonly List<string> pinnedButtons = new List<string>();

        public static void RenderPinnedButtons()
        {
            if (pinnedButtons.Count == 0)
                return;

            float baseOffset = disconnectButton ? -0.42f : -0.32f;
            int rendered = 0;
            foreach (string text in pinnedButtons)
            {
                ButtonInfo info = GetIndex(text);
                if (info == null || !info.IsVisible)
                    continue;

                CreateButton(baseOffset - rendered * 0.1f, info, menuWideMult);
                rendered++;
            }
        }

        public static void TogglePin(string buttonText)
        {
            if (string.IsNullOrEmpty(buttonText) || buttonText == "Home" || buttonText == "NextPage" || buttonText == "PreviousPage")
                return;
            if (GetIndex(buttonText) == null)
                return;

            if (pinnedButtons.Contains(buttonText))
            {
                pinnedButtons.Remove(buttonText);
                NotifiLib.SendNotification("<color=grey>[</color><color=red>UNPIN</color><color=grey>]</color> " + buttonText);
            }
            else
            {
                pinnedButtons.Add(buttonText);
                NotifiLib.SendNotification("<color=grey>[</color><color=yellow>PIN</color><color=grey>]</color> " + buttonText);
            }

            try { Mods.Settings.Audio.PlayClickSound(); } catch { }
            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }
            RecreateMenu();
        }

        public static void CreatePageArrow(string image, string fallbackText, Vector3 localPosition, Color? tint = null)
        {
            Color color = tint ?? textColors[0];
            Texture2D arrow = Classes.ImageLib.LoadTinted(image, color);
            Quaternion rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (arrow != null)
            {
                RawImage rawImage = new GameObject
                {
                    transform =
                    {
                        parent = canvasObject.transform
                    }
                }.AddComponent<RawImage>();
                rawImage.texture = arrow;
                rawImage.color = Color.white;

                RectTransform imageRect = rawImage.GetComponent<RectTransform>();
                imageRect.localPosition = localPosition;
                imageRect.sizeDelta = new Vector2(0.03f * ((float)arrow.width / arrow.height), 0.03f);
                imageRect.rotation = rotation;
                return;
            }

            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = fallbackText;
            text.fontSize = 1;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.localPosition = localPosition;
            textRect.sizeDelta = new Vector2(0.2f, 0.03f);
            textRect.rotation = rotation;
        }

        public static void CreateButton(float offset, ButtonInfo method, float widthMul = 1f, float lateralOffset = 0f, Action directAction = null)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(keyboardButton))
                gameObject.layer = 2;

            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.9f * widthMul, 0.08f);
            gameObject.transform.localPosition = new Vector3(0.56f, lateralOffset, 0.28f - offset);
            Decorate(gameObject);
            Classes.Button btnComp = gameObject.AddComponent<Classes.Button>();
            btnComp.relatedText = method.buttonText;
            btnComp.directAction = directAction;

            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = method.enabled ? buttonColors[1] : buttonColors[0];

            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = method.buttonText;

            if (method.overlapText != null)
                text.text = method.overlapText;

            if (widthMul < 0.3f && !string.IsNullOrEmpty(method.buttonText))
                text.text = method.buttonText.Substring(0, 1);

            text.supportRichText = true;
            text.fontSize = 1;
            text.color = method.enabled ? textColors[1] : textColors[0];

            if (currentCategory == Mods.Console.Console.Category || method.buttonText == "Console")
                text.color = new Color(0.75f, 0.6f, 1f);

            if (currentCategory == 20 || method.buttonText == "Detected Mods")
                text.color = new Color(1f, 0.3f, 0.3f);
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Italic;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            bool glyph = method.buttonText == "-" || method.buttonText == "+";
            component.localPosition = Vector3.zero;
            component.sizeDelta = glyph ? new Vector2(0.12f, 0.05f) : new Vector2(.2f * widthMul, .03f);
            component.localPosition = new Vector3(.064f, lateralOffset / 4.5f, .111f - offset / 2.6f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        public static void Panic()
        {
            foreach (ButtonInfo[] category in buttons)
            {
                foreach (ButtonInfo button in category)
                {
                    if (button.isTogglable && button.enabled)
                    {
                        button.enabled = false;
                        if (button.disableMethod != null)
                            try { button.disableMethod.Invoke(); } catch { }
                    }
                }
            }

            roundedCorners = false;
            rainbowOutline = false;
            Mods.Settings.Outline.colorIndex = 0;
            Classes.AdminTags.crownColorIndex = 0;
            Classes.AdminTags.hideOwnCrown = false;

            Classes.ThemeChanger.darkMode = true;
            Classes.ThemeChanger.ApplyTheme(0);
            Classes.ThemeChanger.RefreshModeLabel();
            Mods.Settings.Outline.RefreshLabel();
            Classes.AdminTags.RefreshLabel();

            try { Classes.ThemeChanger.SaveConfigSilent(); } catch { }

            NotifiLib.SendNotification("<color=grey>[</color><color=red>PANIC</color><color=grey>]</color> All mods disabled and settings reset to default.");
        }

        public static void RecreateMenu()
        {
            if (menu != null)
            {
                Destroy(menu);
                menu = null;

                CreateMenu();
                RecenterMenu(rightHanded, UnityInput.Current.GetKey(keyboardButton));
            }
        }

        public static void UpdateButtonText()
        {
            ButtonInfo[] settingsButtons = buttons[1];
            for (int i = 0; i < settingsButtons.Length; i++)
            {
                if (settingsButtons[i].buttonText.StartsWith("Theme:"))
                {
                    settingsButtons[i].buttonText = "Theme: " + Classes.ThemeChanger.themes[Classes.ThemeChanger.currentThemeIndex].name;
                    break;
                }
            }
        }

        public static void RecenterMenu(bool isRightHanded, bool isKeyboardCondition)
        {
            if (!isKeyboardCondition)
            {
                Vector3 targetPos;
                Quaternion targetRot;
                if (!isRightHanded)
                {
                    targetPos = GorillaTagger.Instance.leftHandTransform.position;
                    targetRot = GorillaTagger.Instance.leftHandTransform.rotation;
                }
                else
                {
                    targetPos = GorillaTagger.Instance.rightHandTransform.position;
                    targetRot = Quaternion.Euler(GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles + new Vector3(0f, 0f, 180f));
                }

                float speed = Mods.Settings.Smoothing.Speed;
                if (menuJustPlaced || speed <= 0f)
                {
                    menu.transform.position = targetPos;
                    menu.transform.rotation = targetRot;
                    menuJustPlaced = false;
                }
                else
                {
                    float t = 1f - Mathf.Exp(-speed * Time.deltaTime);
                    menu.transform.position = Vector3.Lerp(menu.transform.position, targetPos, t);
                    menu.transform.rotation = Quaternion.Slerp(menu.transform.rotation, targetRot, t);
                }
            }
            else
            {
                try
                {
                    TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
                }
                catch { }

                GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(false);

                if (TPC != null)
                {
                    TPC.transform.position = new Vector3(-999f, -999f, -999f);
                    TPC.transform.rotation = Quaternion.identity;
                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bg.transform.localScale = new Vector3(10f, 10f, 0.01f);
                    bg.transform.transform.position = TPC.transform.position + TPC.transform.forward;
                    Color realcolor = backgroundColor.GetCurrentColor();
                    bg.GetComponent<Renderer>().material.color = new Color32((byte)(realcolor.r * 50), (byte)(realcolor.g * 50), (byte)(realcolor.b * 50), 255);
                    Destroy(bg, 0.05f);
                    menu.transform.parent = TPC.transform;
                    menu.transform.position = TPC.transform.position + (TPC.transform.forward * 0.5f) + (TPC.transform.up * -0.02f);
                    menu.transform.rotation = TPC.transform.rotation * Quaternion.Euler(-90f, 90f, 0f);

                    if (reference != null)
                    {
                        if (Mouse.current.leftButton.isPressed)
                        {
                            Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                            bool hitButton = Physics.Raycast(ray, out RaycastHit hit, 100);
                            if (hitButton)
                            {
                                Classes.Button collide = hit.transform.gameObject.GetComponent<Classes.Button>();
                                collide?.OnTriggerEnter(buttonCollider);
                            }
                        }
                        else
                            reference.transform.position = new Vector3(999f, -999f, -999f);
                    }
                }
            }
        }

        public static void CreateReference(bool isRightHanded)
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.parent = isRightHanded ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
            reference.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
            reference.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            buttonCollider = reference.GetComponent<SphereCollider>();

            ColorChanger colorChanger = reference.AddComponent<ColorChanger>();
            colorChanger.colors = backgroundColor;
        }

        public static void Toggle(string buttonText)
        {
            try { Mods.Settings.Audio.PlayClickSound(); } catch { }

            int lastPage = ((VisibleButtons(currentCategory).Length + buttonsPerPage - 1) / buttonsPerPage) - 1;
            if (buttonText == "Home")
            {
                currentCategory = 0;
                pageNumber = 0;
            }
            else if (buttonText == "PreviousPage")
            {
                pageNumber--;
                if (pageNumber < 0)
                    pageNumber = lastPage;
            } else
            {
                if (buttonText == "NextPage")
                {
                    pageNumber++;
                    if (pageNumber > lastPage)
                        pageNumber = 0;
                } else
                {
                    ButtonInfo target = GetIndex(buttonText);
                    if (target != null)
                    {
                        if (target.isTogglable)
                        {
                            target.enabled = !target.enabled;
                            if (target.enabled)
                            {
                                if (target.enableMethod != null)
                                    try { target.enableMethod.Invoke(); } catch { }
                            }
                            else
                            {
                                if (target.disableMethod != null)
                                    try { target.disableMethod.Invoke(); } catch { }
                            }
                        }
                        else
                        {
                            if (target.method != null)
                                try { target.method.Invoke(); } catch { }
                        }
                    }
                    else
                        Debug.LogError(buttonText + " does not exist");
                }
            }
            try { Classes.ClientSync.BroadcastNow(); } catch { }

            if (buttonAnimations)
                pendingRecreate = Time.time + 0.12f;
            else
                RecreateMenu();
        }

        public static float pendingRecreate = -1f;

        private static readonly Dictionary<string, (int Category, int Index)> cacheGetIndex = new Dictionary<string, (int Category, int Index)>();
        public static ButtonInfo GetIndex(string buttonText)
        {
            if (buttonText == null)
                return null;

            if (cacheGetIndex.ContainsKey(buttonText))
            {
                var CacheData = cacheGetIndex[buttonText];
                try
                {
                    if (buttons[CacheData.Category][CacheData.Index].buttonText == buttonText)
                        return buttons[CacheData.Category][CacheData.Index];
                }
                catch { cacheGetIndex.Remove(buttonText); }
            }

            int categoryIndex = 0;
            foreach (ButtonInfo[] buttons in buttons)
            {
                int buttonIndex = 0;
                foreach (ButtonInfo button in buttons)
                {
                    if (button.buttonText == buttonText)
                    {
                        try
                        {
                            cacheGetIndex.Add(buttonText, (categoryIndex, buttonIndex));
                        }
                        catch
                        {
                            if (cacheGetIndex.ContainsKey(buttonText))
                                cacheGetIndex.Remove(buttonText);
                        }

                        return button;
                    }
                    buttonIndex++;
                }
                categoryIndex++;
            }

            return null;
        }

        public static Vector3 RandomVector3(float range = 1f) =>
            new Vector3(UnityEngine.Random.Range(-range, range),
                        UnityEngine.Random.Range(-range, range),
                        UnityEngine.Random.Range(-range, range));

        public static Quaternion RandomQuaternion(float range = 360f) =>
            Quaternion.Euler(UnityEngine.Random.Range(0f, range),
                        UnityEngine.Random.Range(0f, range),
                        UnityEngine.Random.Range(0f, range));

        public static Color RandomColor(byte range = 255, byte alpha = 255) =>
            new Color32((byte)UnityEngine.Random.Range(0, range),
                        (byte)UnityEngine.Random.Range(0, range),
                        (byte)UnityEngine.Random.Range(0, range),
                        alpha);

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand()
        {
            Quaternion rot = GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handRotOffset;
            return (GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand()
        {
            Quaternion rot = GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handRotOffset;
            return (GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        public static void WorldScale(GameObject obj, Vector3 targetWorldScale)
        {
            Vector3 parentScale = obj.transform.parent.lossyScale;
            obj.transform.localScale = new Vector3(
                targetWorldScale.x / parentScale.x,
                targetWorldScale.y / parentScale.y,
                targetWorldScale.z / parentScale.z
            );
        }

        public static void FixStickyColliders(GameObject platform)
        {
            Vector3[] localPositions = new Vector3[]
            {
                new Vector3(0, 1f, 0),
                new Vector3(0, -1f, 0),
                new Vector3(1f, 0, 0),
                new Vector3(-1f, 0, 0),
                new Vector3(0, 0, 1f),
                new Vector3(0, 0, -1f)
            };
            Quaternion[] localRotations = new Quaternion[]
            {
                Quaternion.Euler(90, 0, 0),
                Quaternion.Euler(-90, 0, 0),
                Quaternion.Euler(0, -90, 0),
                Quaternion.Euler(0, 90, 0),
                Quaternion.identity,
                Quaternion.Euler(0, 180, 0)
            };
            for (int i = 0; i < localPositions.Length; i++)
            {
                GameObject side = GameObject.CreatePrimitive(PrimitiveType.Cube);
                try
                {
                    if (platform.GetComponent<GorillaSurfaceOverride>() != null)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = platform.GetComponent<GorillaSurfaceOverride>().overrideIndex;
                    }
                }
                catch { }
                float size = 0.025f;
                side.transform.SetParent(platform.transform);
                side.transform.localPosition = localPositions[i] * (size / 2);
                side.transform.localRotation = localRotations[i];
                WorldScale(side, new Vector3(size, size, 0.01f));
                side.GetComponent<Renderer>().enabled = false;
            }
        }

        private static int? noInvisLayerMask;
        public static int NoInvisLayerMask()
        {
            noInvisLayerMask ??= ~(
                1 << LayerMask.NameToLayer("TransparentFX") |
                1 << LayerMask.NameToLayer("Ignore Raycast") |
                1 << LayerMask.NameToLayer("Zone") |
                1 << LayerMask.NameToLayer("Gorilla Trigger") |
                1 << LayerMask.NameToLayer("Gorilla Boundary") |
                1 << LayerMask.NameToLayer("GorillaCosmetics") |
                1 << LayerMask.NameToLayer("GorillaParticle"));

            return noInvisLayerMask ?? GTPlayer.Instance.locomotionEnabledLayers;
        }

        public static bool gunLocked;
        public static VRRig lockTarget;

        public static (RaycastHit Ray, GameObject NewPointer) RenderGun(int? overrideLayerMask = null)
        {
            Transform GunTransform = GorillaTagger.Instance.rightHandTransform;

            Vector3 StartPosition = GunTransform.position;
            Vector3 Direction = GunTransform.forward;

            Physics.Raycast(StartPosition + Direction / 4f, Direction, out var Ray, 512f, overrideLayerMask ?? NoInvisLayerMask());
            Vector3 EndPosition = gunLocked ? lockTarget.transform.position : Ray.point;

            if (EndPosition == Vector3.zero)
                EndPosition = StartPosition + Direction * 512f;

            if (GunPointer == null)
                GunPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            GunPointer.SetActive(true);
            GunPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            GunPointer.transform.position = EndPosition;

            Renderer PointerRenderer = GunPointer.GetComponent<Renderer>();
            PointerRenderer.material.shader = Shader.Find("GUI/Text Shader");
            bool gunHeld = gunLocked
                || ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f
                || (ControllerInputPoller.instance != null && ControllerInputPoller.instance.rightGrab)
                || (Mouse.current != null && Mouse.current.leftButton.isPressed);
            PointerRenderer.material.color = Mods.Settings.GunColor.Get(gunHeld);

            Destroy(GunPointer.GetComponent<Collider>());

            if (GunLine == null)
            {
                GameObject line = new GameObject("iiMenu_GunLine");
                GunLine = line.AddComponent<LineRenderer>();
            }

            GunLine.gameObject.SetActive(true);
            GunLine.material.shader = Shader.Find("GUI/Text Shader");
            GunLine.startColor = backgroundColor.GetCurrentColor();
            GunLine.endColor = backgroundColor.GetCurrentColor(0.5f);
            GunLine.startWidth = 0.025f;
            GunLine.endWidth = 0.025f;
            GunLine.positionCount = 2;
            GunLine.useWorldSpace = true;

            GunLine.SetPosition(0, StartPosition);
            GunLine.SetPosition(1, EndPosition);

            return (Ray, GunPointer);
        }

        public static GameObject menu;
        public static GameObject menuBackground;
        public static GameObject reference;
        public static GameObject canvasObject;

        public static SphereCollider buttonCollider;
        public static Camera TPC;
        public static Text fpsObject;

        private static GameObject GunPointer;
        private static LineRenderer GunLine;

        public static int pageNumber = 0;
        public static int _currentCategory;
        public static int currentCategory
        {
            get => _currentCategory;
            set
            {
                _currentCategory = value;
                pageNumber = 0;
            }
        }

        private static void CreateRainbowOutline()
        {
            float radius = roundedCorners ? Classes.RoundedMesh.BackgroundRadius : 0.004f;
            float hy = menuSize.y * 0.5f;
            float hz = menuSize.z * 0.5f;
            List<Vector2> points = Classes.RoundedMesh.PerimeterPoints(hy, hz, radius);
            int n = points.Count;
            if (n < 3)
                return;

            float x = 0.05f / menu.transform.localScale.x + menuSize.x * 0.5f + 0.003f;

            GameObject go = new GameObject("Outline");
            go.transform.parent = menu.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            LineRenderer line = go.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.numCornerVertices = 4;
            line.numCapVertices = 0;
            line.widthMultiplier = 0.0035f;
            line.textureMode = LineTextureMode.Stretch;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.material = new Material(Shader.Find("GUI/Text Shader"));
            line.material.renderQueue = 3000;

            Vector3[] pos = new Vector3[n];
            float[] cum = new float[n + 1];
            for (int i = 0; i < n; i++)
                pos[i] = new Vector3(x, points[i].x, points[i].y);
            for (int i = 0; i < n; i++)
                cum[i + 1] = cum[i] + Vector3.Distance(pos[i], pos[(i + 1) % n]);
            float total = cum[n] > 0f ? cum[n] : 1f;

            line.positionCount = n;
            line.SetPositions(pos);

            Color top = Mods.Settings.Outline.TopColor;
            Color bottom = Mods.Settings.Outline.BottomColor;

            const int keys = 8;
            GradientColorKey[] colorKeys = new GradientColorKey[keys];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[keys];
            float prevTime = -1f;
            for (int k = 0; k < keys; k++)
            {
                int idx = Mathf.RoundToInt(k / (float)(keys - 1) * (n - 1));
                float time = Mathf.Clamp01(cum[idx] / total);
                if (time <= prevTime)
                    time = Mathf.Min(1f, prevTime + 0.0001f);
                prevTime = time;

                float height = Mathf.InverseLerp(-hy, hy, points[idx].x);
                colorKeys[k] = new GradientColorKey(Color.Lerp(bottom, top, height), time);
                alphaKeys[k] = new GradientAlphaKey(1f, time);
            }

            Gradient gradient = new Gradient();
            gradient.SetKeys(colorKeys, alphaKeys);
            line.colorGradient = gradient;
        }

        private static Texture2D MakeVerticalGradient(Color top, Color bottom, int height = 128)
        {
            Texture2D tex = new Texture2D(2, height);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < height; y++)
            {
                Color c = Color.Lerp(bottom, top, y / (float)(height - 1));
                tex.SetPixel(0, y, c);
                tex.SetPixel(1, y, c);
            }
            tex.Apply();
            return tex;
        }

        private static void Decorate(GameObject target, float radius = Classes.RoundedMesh.CornerRadius)
        {
            if (roundedCorners)
                Classes.RoundedMesh.Apply(target, radius);
            else if (GetIndex("Button Outline")?.enabled ?? true)
                CreateOutline(target);
        }

        private static GameObject CreateOutline(GameObject target, float thickness = 0.006f)
        {
            if (target == null)
                return null;

            GameObject outline = new GameObject("Outline");

            outline.transform.SetParent(target.transform.parent, false);
            outline.transform.localPosition = target.transform.localPosition;
            outline.transform.localRotation = target.transform.localRotation;
            outline.transform.localScale = Vector3.one;

            Vector3 size = target.transform.localScale;

            float halfY = size.y * 0.5f;
            float halfZ = size.z * 0.5f;

            CreateOutlinePart(
                outline.transform,
                new Vector3(0f, halfY + thickness * 0.5f, 0f),
                new Vector3(thickness, thickness, size.z + thickness * 2f)
            );

            CreateOutlinePart(
                outline.transform,
                new Vector3(0f, -halfY - thickness * 0.5f, 0f),
                new Vector3(thickness, thickness, size.z + thickness * 2f)
            );

            CreateOutlinePart(
                outline.transform,
                new Vector3(0f, 0f, -halfZ - thickness * 0.5f),
                new Vector3(thickness, size.y + thickness * 2f, thickness)
            );

            CreateOutlinePart(
                outline.transform,
                new Vector3(0f, 0f, halfZ + thickness * 0.5f),
                new Vector3(thickness, size.y + thickness * 2f, thickness)
            );

            return outline;
        }

        private static void CreateOutlinePart(
            Transform parent,
            Vector3 position,
            Vector3 scale)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Destroy(part.GetComponent<Rigidbody>());
            Destroy(part.GetComponent<BoxCollider>());

            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localRotation = Quaternion.identity;
            part.transform.localScale = scale;

            Renderer renderer = part.GetComponent<Renderer>();

            renderer.material.shader = Shader.Find("GUI/Text Shader");

            renderer.material.color = new Color(
                0.18f,
                0.18f,
                0.18f,
                0.55f
            );

            renderer.material.SetInt("_ZWrite", 1);
            renderer.material.SetInt(
                "_ZTest",
                (int)UnityEngine.Rendering.CompareFunction.LessEqual
            );

            renderer.material.renderQueue = 2000;
        }

    }
}
