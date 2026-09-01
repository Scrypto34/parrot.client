using UnityEngine;

namespace Parrot.client.Classes
{

    public class MenuAnimator : MonoBehaviour
    {
        public int type;
        public Vector3 targetScale;
        public float duration = 0.22f;
        public bool closing;

        private float startTime;

        private void Start()
        {
            startTime = Time.unscaledTime;
            Apply(0f);
        }

        private void Update()
        {
            float t = duration <= 0f ? 1f : Mathf.Clamp01((Time.unscaledTime - startTime) / duration);
            Apply(t);

            if (t >= 1f)
            {
                if (closing)
                {
                    transform.localScale = Vector3.zero;
                    Destroy(gameObject);
                }
                else
                {
                    transform.localScale = targetScale;
                    Destroy(this);
                }
            }
        }

        private void Apply(float t)
        {
            float f = closing ? EaseOut(1f - t) : EaseOut(t);
            float b = closing ? EaseBack(1f - t) : EaseBack(t);

            switch (type)
            {
                case 1:
                    transform.localScale = targetScale * f;
                    break;
                case 2:
                    transform.localScale = new Vector3(targetScale.x, targetScale.y, targetScale.z * f);
                    break;
                case 3:
                    transform.localScale = new Vector3(targetScale.x, targetScale.y * f, targetScale.z);
                    break;
                case 4:
                    transform.localScale = targetScale * b;
                    break;
                default:
                    transform.localScale = targetScale;
                    break;
            }
        }

        private static float EaseOut(float t) => 1f - (1f - t) * (1f - t);

        private static float EaseBack(float t)
        {
            const float s = 1.70158f;
            t -= 1f;
            return 1f + (s + 1f) * t * t * t + s * t * t;
        }
    }
}
