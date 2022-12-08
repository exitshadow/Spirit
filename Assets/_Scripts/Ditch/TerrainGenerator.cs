using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _previewPrefab;
    [SerializeField] private int _maxDistX;
    [SerializeField] private int _maxDistZ;
    [SerializeField] private int _maximumHeight;
    [SerializeField] private int _maximumDepth;
    [SerializeField] private int _resolution;
    [SerializeField] [Range(0, 10f)] private float _horizontalIntensity = 2f;
    [SerializeField] [Range(0, 10f)] private float _verticalIntensity = 2f;

    private Hashtable chunksContainer = new Hashtable();
    private List<Vector3> chunksPositions = new List<Vector3>();
    private float step;
    private float stepY;

    private void Start()
    {
        step = _maxDistX / _resolution;
        GeneratePositions();
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
