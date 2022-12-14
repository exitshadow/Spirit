using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _meshHolder;
    [SerializeField] private int _subdivisions = 1;
    [SerializeField] private int _baseResolution = 2;

    private List<GameObject> chunksList = new List<GameObject>();
    private ChunkData[] chunksBuffer;

    // todo
    // create a cube with 6 simple quads with settings to 1, 1
    // subdivisions will create more mesh chunks
    // baseResolution will increase the resolution inside that mesh


    private void Awake()
    {
        StartCoroutine(CreateFaceChunks(Vector3.forward, _subdivisions, _baseResolution));
    }

    private void OnDrawGizmos()
    {
        if (chunksBuffer == null) return;

        Gizmos.color = Color.red;
        
        for (int i = 0; i < chunksBuffer.Length; i++)
        {
            if (chunksBuffer[i] == null)
            {
                Debug.Log("Could not find chunks buffer");
                return;
            }
            
            for (int j = 0; j < chunksBuffer[i].vertices.Length; j++)
            {
                Gizmos.DrawSphere(chunksBuffer[i].vertices[j], 0.05f);
            }
        }
    }

    /// <summary>
    /// Creates a list of MeshData chunks forming a square mesh facing a specified direction.
    /// </summary>
    private IEnumerator CreateFaceChunks(Vector3 facingDirection, int subDivs, int res, float size = 1f)
    {
        int nbVerts = (res + 1) * (res + 1);    // number of vertices in a chunk
        int nbTris = res * res * 6;             // number of triangle indexes in a chunk
        float chunkSize = size / subDivs;       // the width of each individual chunk
        float chunkResStep = chunkSize / res;   // the distance between vertices in a chunk

        chunksBuffer = new ChunkData[subDivs * subDivs];
        Debug.Log(chunksBuffer.Length);

        // iterate in the number of chunks to make
        int chunksIndex = 0;
        Vector2 rootIndex = Vector2.zero;
        for (int u = 0; u < subDivs; u++)
        {
            for (int v = 0; v < subDivs; v++)
            {
                chunksBuffer[chunksIndex] = new ChunkData(new Vector3[nbVerts], new int[nbTris]);
                //Debug.Log("chunks index: " + chunksIndex);

                // generating vertices
                int vertsIndex = 0;
                for (int x = 0; x <= res; x++)
                {
                    for (int y = 0; y <= res; y++)
                    {
                        float distX = rootIndex.x + chunkResStep * x;
                        float distY = rootIndex.y + chunkResStep * y;
                        chunksBuffer[chunksIndex].vertices[vertsIndex] = new Vector3(distX, distY, size / 2);
                        //Debug.Log(chunksBuffer[chunksIndex].vertices[vertsIndex] + " " + vertsIndex);
                        vertsIndex++;
                        yield return new WaitForSeconds(0.05f);
                    }
                }

                // todo generating triangles
                for (int ti = 0, vi = 0, y = 0; y < res; y++, vi++)
                {
                    for (int x = 0; x < res; x++, ti += 6, vi++)
                    {
                        var tris = chunksBuffer[chunksIndex].triangles;

                        tris[ti] = vi;
                        tris[ti + 3] = tris[ti + 2] = vi + 1;
                        tris[ti + 4] = tris[ti + 1] = vi + res + 1;
                        tris[ti + 5] = vi + res + 2;
                    }
                }

                chunksIndex++;
                rootIndex.y += chunkSize;
                if (rootIndex.y > chunkSize * (subDivs - 1)) rootIndex.y = 0; 
            }
            rootIndex.x += chunkSize;
        }

        Debug.Log("finished creating mesh");
        for (int i = 0; i < chunksBuffer.Length; i++)
        {
            // Debug.Log("array " + i);
            // Debug.Log($"lenght of array: " + chunksBuffer[i].vertices.Length);
            // for (int j = 0; j < chunksBuffer[i].vertices.Length; j++)
            // {
            //     Debug.Log(chunksBuffer[i].vertices[j]);
            // }

            for (int j = 0; j < chunksBuffer[i].triangles.Length; j++)
            {
                Debug.Log(chunksBuffer[i].triangles[j]);
            }
        }

    }
}

public class ChunkData
{
    public Vector3[] vertices;
    public int[] triangles;

    public ChunkData(Vector3[] vertices, int[] triangles)
    {
        this.vertices = vertices;
        this.triangles = triangles;
    }
}