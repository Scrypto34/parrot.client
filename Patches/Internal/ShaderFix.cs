using HarmonyLib;
using UnityEngine;

namespace Parrot.client.Patches.Internal
{
    [HarmonyPatch(typeof(GameObject), "CreatePrimitive")]
    public class ShaderFix : MonoBehaviour
    {
        private static void Postfix(GameObject __result)
        {
            Renderer renderer = __result.GetComponent<Renderer>();
            if (renderer == null) return;
            Shader uberShader = Shader.Find("GorillaTag/UberShader");
            if (uberShader == null) return;
            renderer.material.shader = uberShader;
            renderer.material.color = Color.black;
        }
    }
}