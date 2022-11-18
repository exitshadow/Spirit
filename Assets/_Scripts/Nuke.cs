using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Nuke : MonoBehaviour
{
    public UnityEvent Boom;
    private bool hasLanded = false;

    private void Update()
    {
        if (transform.position.y < 10 && !hasLanded) {
            hasLanded = true;
            Debug.Log("Boom");
            Boom?.Invoke();
        }
    }
}
