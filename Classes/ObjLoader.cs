using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Parrot.client.Classes
{

    public class ObjLoader
    {
        private static readonly Dictionary<string, Mesh> cache = new Dictionary<string, Mesh>();

        public static GameObject Load(string fileName, float targetSize, Color color, string textureFile = null)
        {
            Mesh mesh = LoadMesh(fileName);
            if (mesh == null)
                return null;

            GameObject go = new GameObject(fileName);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;

            Texture2D texture = string.IsNullOrEmpty(textureFile) ? null : ImageLib.Load(textureFile);
            if (!string.IsNullOrEmpty(textureFile) && texture == null)
                Debug.LogWarning($"{PluginInfo.Name} // ObjLoader: texture {textureFile} did not load");

            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            Material mat = new Material(BestShader());

            Color tint = texture != null ? Color.white : color;
            mat.color = tint;
            mat.SetColor("_BaseColor", tint);
            mat.SetColor("_Color", tint);

            if (texture != null)
            {
                mat.mainTexture = texture;
                mat.SetTexture("_BaseMap", texture);
                mat.SetTexture("_MainTex", texture);
            }

            renderer.material = mat;

            Vector3 size = mesh.bounds.size;
            float largest = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
            if (largest > 0.0001f)
                go.transform.localScale = Vector3.one * (targetSize / largest);

            return go;
        }

        private static Shader BestShader() =>
            Shader.Find("Universal Render Pipeline/Lit")
            ?? Shader.Find("Universal Render Pipeline/Simple Lit")
            ?? Shader.Find("Universal Render Pipeline/Unlit")
            ?? Shader.Find("GorillaTag/UberShader")
            ?? Shader.Find("Standard");

        private static Mesh LoadMesh(string fileName)
        {
            if (cache.TryGetValue(fileName, out Mesh cached) && cached != null)
                return cached;

            byte[] bytes = EmbeddedResources.Read(fileName) ?? ReadFromDisk(fileName);
            if (bytes == null)
            {
                Debug.LogWarning($"{PluginInfo.Name} // ObjLoader: {fileName} not found in the DLL or the parrot.client/console folder");
                return null;
            }

            Mesh mesh = Parse(Encoding.UTF8.GetString(bytes));
            if (mesh != null)
            {
                mesh.name = fileName;
                cache[fileName] = mesh;
            }

            return mesh;
        }

        private static byte[] ReadFromDisk(string fileName)
        {
            try
            {
                string path = System.IO.Path.Combine(ClientFiles.ConsolePath ?? "", fileName);
                return System.IO.File.Exists(path) ? System.IO.File.ReadAllBytes(path) : null;
            }
            catch
            {
                return null;
            }
        }

        private static Mesh Parse(string text)
        {
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            List<Vector3> outPositions = new List<Vector3>();
            List<Vector3> outNormals = new List<Vector3>();
            List<Vector2> outUvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            Dictionary<string, int> lookup = new Dictionary<string, int>();
            bool hasNormals = false;

            foreach (string rawLine in text.Split('\n'))
            {
                string line = rawLine.Trim();
                if (line.Length == 0 || line[0] == '#')
                    continue;

                string[] parts = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    continue;

                switch (parts[0])
                {
                    case "v":
                        positions.Add(new Vector3(F(parts, 1), F(parts, 2), F(parts, 3)));
                        break;
                    case "vn":
                        normals.Add(new Vector3(F(parts, 1), F(parts, 2), F(parts, 3)));
                        break;
                    case "vt":

                        uvs.Add(new Vector2(F(parts, 1), 1f - F(parts, 2)));
                        break;
                    case "f":

                        int[] indices = new int[parts.Length - 1];
                        for (int i = 1; i < parts.Length; i++)
                            indices[i - 1] = ResolveVertex(parts[i], positions, normals, uvs,
                                outPositions, outNormals, outUvs, lookup, ref hasNormals);

                        for (int i = 1; i < indices.Length - 1; i++)
                        {
                            triangles.Add(indices[0]);
                            triangles.Add(indices[i]);
                            triangles.Add(indices[i + 1]);
                        }
                        break;
                }
            }

            if (outPositions.Count == 0 || triangles.Count == 0)
                return null;

            Mesh mesh = new Mesh
            {
                indexFormat = outPositions.Count > 65000
                    ? UnityEngine.Rendering.IndexFormat.UInt32
                    : UnityEngine.Rendering.IndexFormat.UInt16
            };

            Bounds bounds = new Bounds(outPositions[0], Vector3.zero);
            foreach (Vector3 p in outPositions)
                bounds.Encapsulate(p);
            for (int i = 0; i < outPositions.Count; i++)
                outPositions[i] -= bounds.center;

            mesh.SetVertices(outPositions);
            if (hasNormals)
                mesh.SetNormals(outNormals);
            if (outUvs.Count == outPositions.Count)
                mesh.SetUVs(0, outUvs);
            mesh.SetTriangles(triangles, 0);

            if (!hasNormals)
                mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static int ResolveVertex(string token,
            List<Vector3> positions, List<Vector3> normals, List<Vector2> uvs,
            List<Vector3> outPositions, List<Vector3> outNormals, List<Vector2> outUvs,
            Dictionary<string, int> lookup, ref bool hasNormals)
        {
            if (lookup.TryGetValue(token, out int existing))
                return existing;

            string[] refs = token.Split('/');
            int vi = Index(refs, 0, positions.Count);
            int ti = refs.Length > 1 ? Index(refs, 1, uvs.Count) : -1;
            int ni = refs.Length > 2 ? Index(refs, 2, normals.Count) : -1;

            outPositions.Add(vi >= 0 && vi < positions.Count ? positions[vi] : Vector3.zero);
            outUvs.Add(ti >= 0 && ti < uvs.Count ? uvs[ti] : Vector2.zero);

            if (ni >= 0 && ni < normals.Count)
            {
                outNormals.Add(normals[ni]);
                hasNormals = true;
            }
            else
            {
                outNormals.Add(Vector3.up);
            }

            int index = outPositions.Count - 1;
            lookup[token] = index;
            return index;
        }

        private static int Index(string[] refs, int slot, int count)
        {
            if (slot >= refs.Length || refs[slot].Length == 0)
                return -1;

            if (!int.TryParse(refs[slot], NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                return -1;

            return value > 0 ? value - 1 : count + value;
        }

        private static float F(string[] parts, int i) =>
            i < parts.Length && float.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ? v : 0f;
    }
}
