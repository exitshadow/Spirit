using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Nuke : MonoBehaviour
{
    [SerializeField] private Transform _earthTransform;
    [SerializeField] private float _earthSurfaceRadius = 6000;
    [SerializeField] private float _dropSpeed = 200f;
    [SerializeField] private float _followFactor = 1f;
    public UnityEvent Boom;

    private Vector3 launchPos;
    private Vector3 currentPos;
    private Vector3 deltaPos;

    private float startSpeed;

    private bool hasLaunched = false;
    private bool hasLanded = false;


    public void Launch(float speed)
    {
        Debug.Log("launching");
        hasLaunched = true;
        launchPos = transform.parent.position;
        transform.SetParent(null);
        this.startSpeed = speed;
    }

    private void Update()
    {
        if (hasLaunched)
        {
            currentPos = new Vector3(launchPos.x, launchPos.y, launchPos.z * startSpeed);
            deltaPos = (launchPos - currentPos) * _followFactor;

            float step = _dropSpeed * Time.fixedDeltaTime;

            transform.position = deltaPos + Vector3.MoveTowards(transform.position, _earthTransform.position, step);
        }

        float dist = Vector3.Distance(_earthTransform.position, transform.position);
        if (dist < _earthSurfaceRadius && !hasLanded)
        {
            hasLanded = true;
            Debug.Log("Boom");
            Boom?.Invoke();
        }
    }
}
