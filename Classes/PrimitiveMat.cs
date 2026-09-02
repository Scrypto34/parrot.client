using System.Collections.Generic;
using UnityEngine;

namespace Parrot.client.Classes
{

    public static class PrimitiveMat
    {
        private static readonly Dictionary<Color, Material> cache = new Dictionary<Color, Material>();

        public static Material Get(Color color)
        {
            if (cache.TryGetValue(color, out Material existing) && existing != null)
                return existing;

            Shader shader = Shader.Find("GorillaTag/UberShader") ?? Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            mat.enableInstancing = true;

            cache[color] = mat;
            return mat;
        }
    }
}
