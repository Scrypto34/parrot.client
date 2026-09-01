using UnityEngine;

namespace Parrot.client.Classes
{
    public class TwerkAnim : MonoBehaviour
    {
        public Vector3 basePosition;
        public Quaternion baseRotation;

        private const float Speed = 15f;
        private const float BobHeight = 0.5f;
        private const float PitchAngle = 20f;

        private void Update()
        {
            float t = Mathf.Sin(Time.time * Speed);
            transform.position = basePosition + Vector3.up * (t * BobHeight);
            transform.rotation = baseRotation * Quaternion.Euler(t * PitchAngle, 0f, 0f);
        }
    }
}
