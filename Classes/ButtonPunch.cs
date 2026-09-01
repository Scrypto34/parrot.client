using UnityEngine;

namespace Parrot.client.Classes
{
    public class ButtonPunch : MonoBehaviour
    {
        private const float Duration = 0.12f;
        private const float Strength = 0.2f;

        private Vector3 baseScale;
        private float startTime;

        private void Awake()
        {
            baseScale = transform.localScale;
            startTime = Time.time;
        }

        private void Update()
        {
            float t = (Time.time - startTime) / Duration;
            if (t >= 1f)
            {
                transform.localScale = baseScale;
                Destroy(this);
                return;
            }

            float punch = Mathf.Sin(t * Mathf.PI) * Strength;
            transform.localScale = baseScale * (1f + punch);
        }
    }
}
