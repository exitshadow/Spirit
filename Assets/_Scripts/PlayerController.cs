using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Nuke _nuke;

    public void Nuke()
    {
        _nuke.GetComponent<Rigidbody>().useGravity = true;
    }
}
