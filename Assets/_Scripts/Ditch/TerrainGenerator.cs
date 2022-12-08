using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _previewPrefab;
    [SerializeField] private Mesh _sliceMesh;
    [SerializeField] private int _maxDistX;
    [SerializeField] private int _maxDistZ;
    [SerializeField] private int _resolution;
    [SerializeField] [Range(0, 10f)] private float _horizontalIntensity = 2f;
    [SerializeField] [Range(0, 10f)] private float _verticalIntensity = 2f;
    private float step;

    private Hashtable chunksContainer = new Hashtable();
    private List<Vector3> chunksPositions = new List<Vector3>();

    private void Start()
    {
        step = _maxDistX / _resolution;

        if (_sliceMesh == null)
        {
            GeneratePositions();
        }
        else
        {
            CastVertsPositions(_sliceMesh);
        }
        
    }

    private void CastVertsPositions(Mesh mesh)
    {
        Vector3[] verts = new Vector3[mesh.vertices.Length];

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(mesh.vertices[i]);

            float posX = worldPos.x
                         + (float)NoiseS3D.Noise(worldPos.x, worldPos.z)
                         * _horizontalIntensity;
            float posZ = worldPos.z
                         + (float)NoiseS3D.Noise(worldPos.x, worldPos.z)
                         * _horizontalIntensity;
            float posY = (float)NoiseS3D.Noise(posX, posZ) * _verticalIntensity;

            worldPos = new Vector3(posX, posY, posZ);

            Vector3 newLocalPos = transform.InverseTransformPoint(worldPos);

            verts[i] = newLocalPos;
        }

        var meshFilter = GetComponent<MeshFilter>();
        Mesh newMesh = new Mesh();
        newMesh.name = "Modified Terrain";
        newMesh.vertices = verts;
        newMesh.triangles = mesh.triangles;
        newMesh.RecalculateNormals();

        meshFilter.sharedMesh = newMesh;
    }

    private void GeneratePositions()
    {
        for (float x = - _maxDistX; x < _maxDistX; x += step)
        {
            for (float z = -_maxDistZ; z < _maxDistZ; z += step)
            {
                float posX = x + (float)NoiseS3D.Noise(x, z) * _horizontalIntensity;
                float posZ = z + (float)NoiseS3D.Noise(x, z) * _horizontalIntensity;
                float posY = (float)NoiseS3D.Noise(posX, posZ) * _verticalIntensity;

                Vector3 position = new Vector3(posX, posY, posZ);

                GameObject dummy = Instantiate(_previewPrefab, position, Quaternion.identity);
            }
        }
    }

}
