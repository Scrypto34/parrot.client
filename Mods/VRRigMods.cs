using System.Linq;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;
using static Parrot.client.Menu.Main;

namespace Parrot.client.Mods
{
    public class VRRigMods
    {
        private static Vector3? oldLocalPosition;

        public static void PCButtonClick()
        {
            if (Mouse.current == null)
                return;

            if (Mouse.current.leftButton.isPressed)
            {
                int layerMask = NoInvisLayerMask();
                if (TPC != null)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(ray, out RaycastHit hit, 512f, layerMask))
                    {
                        oldLocalPosition ??= GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition;
                        var follow = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent("TransformFollow");
                        if (follow != null)
                            ((MonoBehaviour)follow).enabled = false;
                        GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                    }
                }
            }
            else
            
            
            {
                if (oldLocalPosition != null)
                {
                    GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition = oldLocalPosition.Value;
                    oldLocalPosition = null;
                }
                var follow = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent("TransformFollow");
                if (follow != null)
                    ((MonoBehaviour)follow).enabled = true;
            }
        }
        public static void StareAtClosestPlayer()
        {
            VRRig closest = GetClosestPlayer();
            if (closest != null)
                VRRig.LocalRig.headConstraint.LookAt(closest.transform.position + new Vector3(0f, 0.4f, 0f));
        }

        public static void FixHead()
        {
            VRRig.LocalRig.head.trackingRotationOffset = Vector3.zero;
            VRRig.LocalRig.headConstraint.rotation = GTPlayer.Instance.headCollider.transform.rotation;
        }

        public static VRRig GetClosestPlayer()
        {
            VRRig closest = null;
            float closestDistance = float.MaxValue;
            Vector3 myPosition = VRRig.LocalRig.transform.position;

            foreach (VRRig rig in VRRigCache.ActiveRigs.Where(r => r != VRRig.LocalRig))
            {
                if (rig == null || rig.isOfflineVRRig)
                    continue;

                float distance = Vector3.Distance(myPosition, rig.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = rig;
                }
            }

            return closest;
        }
    }
}
