using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Nuclear Device")]
    [SerializeField] private Nuke _nuke;

    [Header("Orbiting Settings")]
    [SerializeField] private Transform _rotationTarget;
    [SerializeField] private float _cruisingSpeed = .05f;

    [Header("Steering Settings")]
    [Range(0.01f, 0.1f)] [SerializeField] private float _steeringSpeed = .05f;
    [Range(0.01f, 0.05f)] [SerializeField] private float _rotationSpeed = .02f;
    [SerializeField] private float _steeringBackSpeed = 15f;
    [SerializeField] private float _maxLateralAngle = 35f;

    [Header("Tilting Settings")]
    [Range(0.001f, 0.05f)] [SerializeField] private float _tiltingSpeed = .05f;
    [Range(0.001f, 0.05f)] [SerializeField] private float _altitudeShiftSpeed = 1f;
    [SerializeField] private float _tiltingBackSpeed = 15f;
    [SerializeField] private float _maxFrontalAngle = 15f;

    private DefaultInputActions playerActions;
    private InputAction move;
    private Vector2 direction;
    private Vector2 accelerationTime = new Vector2();
    private float steering;
    private float tilting;

    public void Nuke()
    {
        if (_nuke != null)
        _nuke.Launch(_cruisingSpeed);
    }

    private void Start()
    {
        playerActions = new DefaultInputActions();
        move = playerActions.Player.Move;
        move.Enable();
    }

    private void Update()
    {
        direction = move.ReadValue<Vector2>();
        
        Orbit();
        Steer();
        Tilt();
    }

    private void Orbit()
    {
        transform.RotateAround(_rotationTarget.position, transform.right, _cruisingSpeed * Time.deltaTime);
    }

    private void Steer()
    {
        float rotZ = transform.rotation.eulerAngles.z;
        steering = 0;

        if (move.IsInProgress() && direction.x != 0)
        {
            accelerationTime.x += Time.fixedDeltaTime * .3f;
            steering = direction.x * _steeringSpeed * accelerationTime.x * -1;

            if ((rotZ <= 360 - _maxLateralAngle && rotZ > 180) || (rotZ >= _maxLateralAngle && rotZ < 180))
            {
                steering = 0;
            }
        }
        else
        {
            accelerationTime.x = 0;

            if (rotZ > 2 && rotZ < 180) steering = -Time.fixedDeltaTime * _steeringBackSpeed;
            else if (rotZ > 180 && rotZ < 358) steering = Time.fixedDeltaTime * _steeringBackSpeed;
            else steering = 0;
        }

        // tilts the aircraft laterally
        transform.Rotate(0,0, steering);
        // rotates the aircraft by direction
        transform.Rotate(0, direction.x * _rotationSpeed, 0);
    }

    private void Tilt()
    {
        float altitudeShift = 0;
        float rotX = transform.rotation.eulerAngles.x;
        tilting = 0;

        if (move.IsInProgress() && direction.y != 0)
        {
            tilting = direction.y * _tiltingSpeed * accelerationTime.y;
            accelerationTime.y += Time.fixedDeltaTime * .3f;

            if ((rotX <= 360 - _maxFrontalAngle && rotX > 180) || (rotX >= _maxFrontalAngle && rotX < 180))
            {
                tilting = 0;
            }

            altitudeShift = tilting * _altitudeShiftSpeed;
        }
        else
        {
            accelerationTime.y = 0;

            if (rotX > 2 && rotX < 180) tilting = -Time.fixedDeltaTime * _tiltingBackSpeed;
            else if (rotX > 180 && rotX < 358) tilting = Time.fixedDeltaTime * _tiltingBackSpeed;
        }

        // tilts the aircraft frontally
        transform.Rotate(tilting, 0, 0);

        // moves up or down
        transform.position = new Vector3(transform.position.x,
                                        transform.position.y + altitudeShift * -1,
                                        transform.position.z);
    }
    
}
