using UnityEngine;

namespace Parrot.client.Mods
{
    internal class MotionTrail
    {
        private static GameObject trailObj;
        private static TrailRenderer trail;

        public static void Apply()
        {
            if (GorillaTagger.Instance == null || GorillaTagger.Instance.bodyCollider == null)
                return;

            if (trail == null)
            {
                trailObj = new GameObject("ParrotMotionTrail");
                trailObj.hideFlags = HideFlags.HideAndDontSave;

                trail = trailObj.AddComponent<TrailRenderer>();
                trail.time = 0.7f;
                trail.startWidth = 0.35f;
                trail.endWidth = 0f;
                trail.minVertexDistance = 0.05f;
                trail.numCapVertices = 4;
                trail.autodestruct = false;
                trail.receiveShadows = false;
                trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                trail.material = new Material(Shader.Find("GUI/Text Shader"));
            }

            Transform body = GorillaTagger.Instance.bodyCollider.transform;
            trailObj.transform.position = body.position;

            float scale = GorillaLocomotion.GTPlayer.Instance != null ? GorillaLocomotion.GTPlayer.Instance.scale : 1f;
            trail.startWidth = 0.35f * scale;

            Color c;
            try { c = Parrot.client.Settings.backgroundColor.GetCurrentColor(); }
            catch { c = Color.cyan; }

            trail.startColor = c;
            Color end = c;
            end.a = 0f;
            trail.endColor = end;
        }

        public static void Stop()
        {
            if (trailObj != null)
                Object.Destroy(trailObj);
            trailObj = null;
            trail = null;
        }
    }
}
