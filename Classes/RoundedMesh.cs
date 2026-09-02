using System.Collections.Generic;
using UnityEngine;

namespace Parrot.client.Classes
{

    public static class RoundedMesh
    {
        private const int CornerSegments = 8;
        public const float CornerRadius = 0.025f;
        public const float BackgroundRadius = 0.06f;

        private static readonly Dictionary<Vector4, Mesh> cache = new Dictionary<Vector4, Mesh>();

        public static void Apply(GameObject cube, float radius = CornerRadius)
        {
            if (cube == null)
                return;

            MeshFilter filter = cube.GetComponent<MeshFilter>();
            if (filter == null)
                return;

            Vector3 size = cube.transform.localScale;
            filter.sharedMesh = Build(size, radius);
            cube.transform.localScale = Vector3.one;

            BoxCollider box = cube.GetComponent<BoxCollider>();
            if (box != null)
            {
                box.center = Vector3.zero;
                box.size = size;
            }
        }

        private static Mesh Build(Vector3 size, float cornerRadius)
        {
            Vector4 key = new Vector4(size.x, size.y, size.z, cornerRadius);
            if (cache.TryGetValue(key, out Mesh cached) && cached != null)
                return cached;

            float halfX = size.x * 0.5f;
            float hy = size.y * 0.5f;
            float hz = size.z * 0.5f;
            float radius = Mathf.Min(cornerRadius, Mathf.Min(hy, hz) * 0.95f);

            List<Vector2> face = Perimeter(hy, hz, radius);
            int n = face.Count;

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            int frontStart = verts.Count;
            for (int i = 0; i < n; i++)
                verts.Add(new Vector3(halfX, face[i].x, face[i].y));
            int frontCenter = verts.Count;
            verts.Add(new Vector3(halfX, 0f, 0f));

            int backStart = verts.Count;
            for (int i = 0; i < n; i++)
                verts.Add(new Vector3(-halfX, face[i].x, face[i].y));
            int backCenter = verts.Count;
            verts.Add(new Vector3(-halfX, 0f, 0f));

            for (int i = 0; i < n; i++)
            {
                int next = (i + 1) % n;
                tris.Add(frontCenter); tris.Add(frontStart + i); tris.Add(frontStart + next);
                tris.Add(backCenter); tris.Add(backStart + next); tris.Add(backStart + i);
            }

            for (int i = 0; i < n; i++)
            {
                int next = (i + 1) % n;
                int f0 = frontStart + i, f1 = frontStart + next;
                int b0 = backStart + i, b1 = backStart + next;
                tris.Add(f0); tris.Add(b0); tris.Add(f1);
                tris.Add(f1); tris.Add(b0); tris.Add(b1);
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            cache[key] = mesh;
            return mesh;
        }

        public static List<Vector2> PerimeterPoints(float hy, float hz, float radius) =>
            Perimeter(hy, hz, radius);

        private static List<Vector2> Perimeter(float hy, float hz, float r)
        {
            r = Mathf.Min(r, Mathf.Min(hy, hz));

            Vector2[] centers =
            {
                new Vector2(hy - r, hz - r),
                new Vector2(-(hy - r), hz - r),
                new Vector2(-(hy - r), -(hz - r)),
                new Vector2(hy - r, -(hz - r))
            };
            float[] startAngles = { 0f, 90f, 180f, 270f };

            List<Vector2> points = new List<Vector2>();
            for (int c = 0; c < 4; c++)
            {
                for (int s = 0; s <= CornerSegments; s++)
                {
                    float angle = Mathf.Deg2Rad * (startAngles[c] + 90f * s / CornerSegments);
                    points.Add(centers[c] + new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r));
                }
            }

            return points;
        }
    }
}
