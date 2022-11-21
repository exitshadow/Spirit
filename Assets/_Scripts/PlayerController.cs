using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Nuke _nuke;
    [SerializeField] private Transform _rotationTarget;
    [SerializeField] private float _speed = 20f;

    public void Nuke()
    {
        _nuke.GetComponent<Rigidbody>().useGravity = true;
    }

    private void Update()
    {
        transform.RotateAround(_rotationTarget.position, transform.right, _speed * Time.deltaTime);
    }
}
