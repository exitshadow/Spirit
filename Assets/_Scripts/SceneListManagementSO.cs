using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Global Eden/Scenes List")]
public class SceneListManagementSO : ScriptableObject
{
    [SerializeField] private List<string> _scenesList;

    public List<string> ScenesList { get { return _scenesList; } }
}
