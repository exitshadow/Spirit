using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// loosely based on Sebastian Lague’s planet generator
public class EarthGenerator : MonoBehaviour
{
    MeshData CreateFace(Vector3 direction, int resolution)
    {
        // axis A shifts everyone right hand side
        Vector3 axisA = new Vector3(direction.y, direction.z, direction.x);
        Vector3 axisB = Vector3.Cross(direction, axisA);

        Vector3[] verts = new Vector3[resolution * resolution];
        int[] tris = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int vertIndex = x + y * resolution;
                Vector2 t = new Vector2(x, y) / (resolution - 1f);
                Vector3 point = direction + axisA * (2 * t.x - 1) + axisB * (2 * t.y -1);
                verts[vertIndex] = point;

                if (x != resolution - 1 && y != resolution -1)
                {
                    tris[triIndex + 0] = vertIndex;
                    tris[triIndex + 1] = vertIndex + resolution + 1;
                    tris[triIndex + 2] = vertIndex + resolution;
                    tris[triIndex + 3] = vertIndex;
                    tris[triIndex + 4] = vertIndex + 1;
                    tris[triIndex + 5] = vertIndex + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        return new MeshData(verts, tris);
    }

    private class MeshData
    {
        Vector3[] vertices;
        int[] triangles;

        public MeshData(Vector3[] vertices, int[] triangles)
        {
            this.vertices = vertices;
            this.triangles = triangles;
        }
    }
}
