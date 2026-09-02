using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace Parrot.client.Mods
{
    internal class DrawMod
    {
        private static GameObject rightPointer;
        private static GameObject leftPointer;
        private static readonly List<GameObject> drawings = new List<GameObject>();

        private static Color drawColor = Color.white;
        private static int currentColor;
        private static bool colorChangerCooldown;

        public static void Draw()
        {
            if (GTPlayer.Instance == null || ControllerInputPoller.instance == null)
                return;

            if (!ControllerInputPoller.instance.rightGrab)
            {
                if (rightPointer != null)
                    Object.Destroy(rightPointer);
                rightPointer = null;

                if (leftPointer != null)
                    Object.Destroy(leftPointer);
                leftPointer = null;

                colorChangerCooldown = false;
                return;
            }

            if (rightPointer == null)
                rightPointer = MakeSphere();
            if (leftPointer == null)
                leftPointer = MakeSphere();

            rightPointer.transform.position = GTPlayer.Instance.RightHand.controllerTransform.position;
            leftPointer.transform.position = GTPlayer.Instance.LeftHand.controllerTransform.position;
            SetColor(rightPointer, drawColor);
            SetColor(leftPointer, drawColor);

            GameObject rightDot = MakeSphere();
            rightDot.transform.position = GTPlayer.Instance.RightHand.controllerTransform.position;
            SetColor(rightDot, drawColor);
            drawings.Add(rightDot);

            if (ControllerInputPoller.instance.leftGrab)
            {
                GameObject leftDot = MakeSphere();
                leftDot.transform.position = GTPlayer.Instance.LeftHand.controllerTransform.position;
                SetColor(leftDot, drawColor);
                drawings.Add(leftDot);
            }

            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                if (!colorChangerCooldown)
                {
                    currentColor = (currentColor + 1) % 13;
                    drawColor = (currentColor == 1) ? Color.blue : Color.white;
                    colorChangerCooldown = true;
                }
                return;
            }

            colorChangerCooldown = false;
        }

        public static void StopDraw()
        {
            if (rightPointer != null)
                Object.Destroy(rightPointer);
            rightPointer = null;

            if (leftPointer != null)
                Object.Destroy(leftPointer);
            leftPointer = null;

            DestroyAllDrawings();
        }

        private static GameObject MakeSphere()
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = Vector3.one * 0.1f;
            Object.Destroy(sphere.GetComponent<Rigidbody>());
            Object.Destroy(sphere.GetComponent<Collider>());

            Material mat = new Material(Shader.Find("GorillaTag/UberShader"));
            mat.SetColor("_BaseColor", drawColor);
            sphere.GetComponent<Renderer>().material = mat;
            return sphere;
        }

        private static void SetColor(GameObject obj, Color col)
        {
            Renderer r = obj.GetComponent<Renderer>();
            if (r != null)
            {
                r.material.SetColor("_BaseColor", col);
                r.material.color = col;
            }
        }

        private static void DestroyAllDrawings()
        {
            foreach (GameObject dot in drawings)
                if (dot != null)
                    Object.Destroy(dot);
            drawings.Clear();
        }
    }
}
