using BepInEx;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using static Parrot.client.Settings;

namespace Parrot.client.Notifications
{
    [BepInPlugin("org.gorillatag.lars.notifications2", "NotificationLibrary", "2.0.0")]
    public class NotifiLib : BaseUnityPlugin
    {
        private static GameObject canvasObj;
        private static Transform canvasT;
        private static GameObject mainCamera;
        private static bool hasInit;

        private static Sprite roundedSprite;
        private static readonly List<NotifCard> cards = new List<NotifCard>();
        private static readonly Queue<string[]> pending = new Queue<string[]>();

        private static readonly Color PanelColor  = new Color(0.10f, 0.08f, 0.15f, 0.97f);
        public static Color AccentColor = new Color(0.62f, 0.34f, 0.98f, 1f);
        private static readonly Color HeaderColor = new Color(0.74f, 0.70f, 0.86f, 1f);
        private static readonly Color BodyColor   = Color.white;

        public static bool RoomActivity = true;
        public static bool ModActivity  = true;

        private const float Scale    = 0.0012f;
        private const float CanvasW  = 1920f;
        private const float CanvasH  = 1080f;
        private const float CardW    = 720f;
        private const float CardH    = 150f;
        private const int   MaxCards = 4;

        public const float BaseX   = -380f;
        public const float BaseY   = 150f;
        public const float Spacing = 178f;

        public static bool IsEnabled = true;
        public static string PreviousNotifi;

        private void Awake()
        {
            Logger.LogInfo("Plugin NotificationLibrary is loaded!");
        }

        private void Init()
        {
            mainCamera = GameObject.Find("Main Camera");

            canvasObj = new GameObject("NOTIFICATIONLIB_HUD_OBJ");

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = mainCamera.GetComponent<Camera>();

            RectTransform crt = canvasObj.GetComponent<RectTransform>();
            crt.sizeDelta = new Vector2(CanvasW, CanvasH);

            canvasObj.transform.SetParent(mainCamera.transform, false);
            canvasObj.transform.localPosition = new Vector3(0f, 0f, 1.6f);
            canvasObj.transform.localRotation = Quaternion.identity;
            canvasObj.transform.localScale = Vector3.one * Scale;

            canvasT = canvasObj.transform;
            roundedSprite = MakeRounded();

            while (pending.Count > 0)
            {
                string[] p = pending.Dequeue();
                Spawn(p[0], p[1]);
            }
        }

        private void FixedUpdate()
        {
            if (hasInit && (canvasObj == null || mainCamera == null))
                hasInit = false;

            if (!hasInit && GameObject.Find("Main Camera") != null)
            {
                Init();
                hasInit = true;
            }
        }

        public static void SendNotification(string message) => SendNotification("Mod activity", message);

        public static void SendNotification(string header, string message)
        {
            if (disableNotifications || !IsEnabled)
                return;

            if (header == "Room activity" && !RoomActivity)
                return;
            if (header == "Mod activity" && !ModActivity)
                return;

            string body = Clean(message);
            if (string.IsNullOrEmpty(body))
                return;

            string key = header + "|" + body;
            if (key == PreviousNotifi)
                return;
            PreviousNotifi = key;

            if (!hasInit || canvasT == null)
            {
                pending.Enqueue(new string[] { header, body });
                return;
            }

            Spawn(header, body);
        }

        public static void ClearAllNotifications()
        {
            foreach (NotifCard c in cards.ToArray())
                if (c != null)
                    Destroy(c.gameObject);
            cards.Clear();
        }

        public static int IndexOf(NotifCard c) => cards.IndexOf(c);
        public static void Remove(NotifCard c) => cards.Remove(c);

        private static void Spawn(string header, string body)
        {
            while (cards.Count >= MaxCards)
            {
                NotifCard old = cards[0];
                cards.RemoveAt(0);
                if (old != null)
                    Destroy(old.gameObject);
            }

            GameObject card = new GameObject("Notif");
            card.transform.SetParent(canvasT, false);

            RectTransform rt = card.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(CardW, CardH);
            rt.localScale = Vector3.one;

            CanvasGroup cg = card.AddComponent<CanvasGroup>();

            Image bg = card.AddComponent<Image>();
            bg.sprite = roundedSprite;
            bg.type = Image.Type.Sliced;
            bg.color = PanelColor;

            float pad = 34f;
            float textW = CardW - pad - 26f;

            NewText(card.transform, "header", header.ToUpper(), AccentColor, 26, new Vector2(pad, 86f), new Vector2(textW, 44f), TextAnchor.LowerLeft, FontStyle.Bold);
            NewText(card.transform, "body", body, BodyColor, 44, new Vector2(pad, 26f), new Vector2(textW, 56f), TextAnchor.UpperLeft, FontStyle.Normal);

            float barMaxW = CardW - 40f;
            Image bar = NewImage(card.transform, "bar", AccentColor, new Vector2(20f, 12f), new Vector2(barMaxW, 6f), true);

            NotifCard nc = card.AddComponent<NotifCard>();
            nc.group = cg;
            nc.rect = rt;
            nc.born = Time.time;
            nc.slotY = BaseY + cards.Count * Spacing;
            nc.bar = bar.rectTransform;
            nc.barMaxW = barMaxW;
            cards.Add(nc);
        }

        private static Image NewImage(Transform parent, string name, Color color, Vector2 pos, Vector2 size, bool rounded)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image img = go.AddComponent<Image>();
            img.color = color;
            if (rounded)
            {
                img.sprite = roundedSprite;
                img.type = Image.Type.Sliced;
            }
            RectTransform rt = img.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0f, 0f);
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
            return img;
        }

        private static Text NewText(Transform parent, string name, string text, Color color, int fontSize, Vector2 pos, Vector2 size, TextAnchor anchor, FontStyle style)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Text t = go.AddComponent<Text>();
            t.text = text;
            t.font = currentFont;
            t.color = color;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.alignment = anchor;
            t.supportRichText = false;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform rt = t.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0f, 0f);
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
            return t;
        }

        private static Sprite MakeRounded()
        {
            int s = 48, r = 16;
            Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            Color32[] px = new Color32[s * s];
            Color32 on = new Color32(255, 255, 255, 255);
            Color32 off = new Color32(255, 255, 255, 0);
            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    bool inside = true;
                    if (x < r && y < r) inside = (r - x) * (r - x) + (r - y) * (r - y) <= r * r;
                    else if (x >= s - r && y < r) inside = (x - (s - r - 1)) * (x - (s - r - 1)) + (r - y) * (r - y) <= r * r;
                    else if (x < r && y >= s - r) inside = (r - x) * (r - x) + (y - (s - r - 1)) * (y - (s - r - 1)) <= r * r;
                    else if (x >= s - r && y >= s - r) inside = (x - (s - r - 1)) * (x - (s - r - 1)) + (y - (s - r - 1)) * (y - (s - r - 1)) <= r * r;
                    px[y * s + x] = inside ? on : off;
                }
            }
            tex.SetPixels32(px);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(r, r, r, r));
        }

        private static string Clean(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = Regex.Replace(s, "<.*?>", "");
            s = s.Trim();
            if (s.StartsWith("["))
            {
                int i = s.IndexOf(']');
                if (i >= 0 && i <= 18)
                    s = s.Substring(i + 1).Trim();
            }
            if (s.StartsWith("Name:"))
                s = s.Substring(5).Trim();
            return s;
        }
    }

    public class NotifCard : MonoBehaviour
    {
        public CanvasGroup group;
        public RectTransform rect;
        public RectTransform bar;
        public float barMaxW;
        public float born;
        public float slotY;

        private const float InTime  = 0.34f;
        private const float Life    = 3.6f;
        private const float OutTime = 0.24f;

        private void Update()
        {
            int idx = NotifiLib.IndexOf(this);
            if (idx < 0)
                return;

            float targetY = NotifiLib.BaseY + idx * NotifiLib.Spacing;
            slotY = Mathf.Lerp(slotY, targetY, Time.deltaTime * 12f);

            float age = Time.time - born;
            float scale;
            float a;

            if (age < InTime)
            {
                float t = age / InTime;
                scale = Mathf.Lerp(0.55f, 1f, EaseOutBack(t));
                a = Mathf.Clamp01(t / 0.55f);
            }
            else if (age < InTime + Life)
            {
                scale = 1f;
                a = 1f;
            }
            else if (age < InTime + Life + OutTime)
            {
                float t = (age - InTime - Life) / OutTime;
                scale = Mathf.Lerp(1f, 0.45f, EaseInBack(t));
                a = 1f - t;
            }
            else
            {
                NotifiLib.Remove(this);
                Destroy(gameObject);
                return;
            }

            group.alpha = a;
            rect.localScale = Vector3.one * scale;
            rect.anchoredPosition = new Vector2(NotifiLib.BaseX, slotY);

            if (bar != null)
            {
                float p = Mathf.Clamp01((age - InTime) / Life);
                Vector2 sd = bar.sizeDelta;
                sd.x = barMaxW * (1f - p);
                bar.sizeDelta = sd;
            }
        }

        private static float EaseOutBack(float t)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;
            float p = t - 1f;
            return 1f + c3 * p * p * p + c1 * p * p;
        }

        private static float EaseInBack(float t)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;
            return c3 * t * t * t - c1 * t * t;
        }
    }
}
