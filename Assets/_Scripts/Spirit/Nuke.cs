using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Nuke : MonoBehaviour
{
    [SerializeField] private Transform _earthTransform;
    [SerializeField] private float _earthSurfaceRadius = 6000;
    [SerializeField] private float _dropSpeed = 200f;
    public UnityEvent Boom;

    private float startSpeed;
    private bool hasLaunched = false;
    private bool hasLanded = false;


    public void Launch(float speed)
    {
        if (transform.parent != null)
        {
            Debug.Log("launching");
            hasLaunched = true;
            transform.SetParent(null);
            this.startSpeed = speed;
        }
    }

    private void Update()
    {
        if (hasLanded) return;

        if (hasLaunched)
            FallTowardsEarth();

        CheckIfLanded();
    }

    private void FallTowardsEarth()
    {
        float step = _dropSpeed * Time.fixedDeltaTime;

        Vector3 stepTowardsEarth = Vector3.MoveTowards(transform.position, _earthTransform.position, step);
        transform.position = stepTowardsEarth;
        transform.RotateAround(_earthTransform.position, transform.right, startSpeed * Time.fixedDeltaTime);
    }

    private void CheckIfLanded()
    {
        float dist = Vector3.Distance(_earthTransform.position, transform.position);
        if (dist < _earthSurfaceRadius && !hasLanded)
        {
            Debug.Log("Boom");
            Boom?.Invoke();
            hasLanded = true;
        }
    }
}
