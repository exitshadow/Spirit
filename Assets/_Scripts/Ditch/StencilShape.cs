using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo
//  refactor using an Vector2[7] array and not a list to force the 4 points of control in the bezier

[CreateAssetMenu(menuName = "Global Eden/Ditch/Stencil Shape")]
public class StencilShape : ScriptableObject
{
    [Header("Fill control points in range between 0 and 1 for X value")]
    [SerializeField] List<Vector2> _controlPoints;

    public StencilShape(List<Vector2> controlPoints)
    {
        _controlPoints = controlPoints;
    }

    public List<Vector2> GetPositions()
    {
        return _controlPoints;
    }

    public void ScaleX(float factor)
    {
        for (int i = 0; i < _controlPoints.Count; i++)
        {
            _controlPoints[i] = new Vector2(_controlPoints[i].x * factor, _controlPoints[i].y);
        }
    }

}
