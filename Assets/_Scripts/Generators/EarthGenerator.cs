using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGenerator : MonoBehaviour
{
    [SerializeField] private int _subdivisions = 1;
    [SerializeField] private int _baseResolution = 2;

    private List<MeshData> chunksList = new List<MeshData>();

    // todo
    // create a cube with 6 simple quads with settings to 1, 1
    // subdivisions will create more mesh chunks
    // baseResolution will increase the resolution inside that mesh

    /// <summary>
    /// Creates a list of MeshData chunks forming a square mesh facing a specified direction.
    /// </summary>
    private MeshData[] CreateFaceChunks(Vector3 facingDirection, int subDivs, int res, float size = 1f)
    {
        int nbVerts = (res + 1) * (res + 1);    // number of vertices in a chunk
        float chunkSize = size / subDivs;       // the width of each individual chunk
        float chunkResStep = chunkSize / res;   // the distance between vertices in a chunk

        MeshData[] chunks = new MeshData[subDivs * subDivs];

        // iterate in the number of chunks to make
        for (int i = 0; i < subDivs; i++)
        {
            int index = 0;
            chunks[i].vertices = new Vector3[nbVerts];
            chunks[i].triangles = new int[nbVerts];

            for (int x = 0; x < res; x++)
            {
                for (int y = 0; y < res; y++)
                {
                    float distX = i * x + chunkResStep * x;
                    float distY = i * y + chunkResStep * y;
                    chunks[i].vertices[index] = new Vector3(distX, distY, size / 2);
                }
            }
        }

        return chunks;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;

    public MeshData(Vector3[] vertices, int[] triangles)
    {
        this.vertices = vertices;
        this.triangles = triangles;
    }
}