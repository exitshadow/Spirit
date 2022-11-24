using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _meshHolder;
    [SerializeField] private int _subdivisions = 1;
    [SerializeField] private int _baseResolution = 2;

    private List<ChunkData> chunksList = new List<ChunkData>();
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
            if (chunksBuffer[i] == null) return;
            //Debug.Log("Found chunk buffer");
            
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
        float chunkSize = size / subDivs;       // the width of each individual chunk
        float chunkResStep = chunkSize / res;   // the distance between vertices in a chunk

        chunksBuffer = new ChunkData[subDivs * subDivs];

        // iterate in the number of chunks to make
        for (int i = 0; i < chunksBuffer.Length; i++)
        {
            chunksBuffer[i] = new ChunkData(new Vector3[nbVerts], new int[nbVerts]);

            int index = 0;
            for (int x = 0; x <= res; x++)
            {
                for (int y = 0; y <= res; y++)
                {
                    float distX = i * x + chunkResStep * x;
                    float distY = i * y + chunkResStep * y;
                    chunksBuffer[i].vertices[index] = new Vector3(distX, distY, size / 2);
                    Debug.Log(chunksBuffer[i].vertices[index] + " " + index);
                    index++;

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        Debug.Log("finished creating mesh");
        for (int i = 0; i < chunksBuffer.Length; i++)
        {
            Debug.Log("array " + i);
            Debug.Log($"lenght of array: " + chunksBuffer[i].vertices.Length);
            for (int j = 0; j < chunksBuffer[i].vertices.Length; j++)
            {
                Debug.Log(chunksBuffer[i].vertices[j]);
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