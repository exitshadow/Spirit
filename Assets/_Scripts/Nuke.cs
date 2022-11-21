using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Nuke : MonoBehaviour
{
    public UnityEvent Boom;
    [SerializeField] private float _earthSurfaceRadius = 6000;
    private bool hasLanded = false;

    private void Update()
    {
        if (transform.position.y < _earthSurfaceRadius && !hasLanded) {
            hasLanded = true;
            Debug.Log("Boom");
            Boom?.Invoke();
        }
    }
}
