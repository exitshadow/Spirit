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
    [SerializeField] private AnimationCurve _steeringCurveIn;
    [Range(0.01f, 0.1f)] [SerializeField] private float _steeringSpeed = .05f;
    [Range(0.01f, 0.05f)] [SerializeField] private float _rotationSpeed = .02f;
    [SerializeField] private AnimationCurve _steeringCurveOut;
    [SerializeField] private float _steeringBackDuration = 15f;
    [SerializeField] private float _maxLateralAngle = 35f;

    [Header("Tilting Settings")]
    [SerializeField] private AnimationCurve _tiltingCurveIn;
    [Range(0.001f, 0.05f)] [SerializeField] private float _tiltingSpeed = .05f;
    [Range(0.001f, 0.05f)] [SerializeField] private float _altitudeShiftSpeed = 1f;
    [SerializeField] private AnimationCurve _tiltingCurveOut;
    [SerializeField] private float _tiltingBackDuration = 15f;
    [SerializeField] private float _maxFrontalAngle = 15f;

    // inputs
    private DefaultInputActions playerActions;
    private InputAction move;
    private Vector2 direction;

    // steering & tilting state variables
    private Vector2 accelerationTime = new Vector2();
    private float steering;                 // amount of steering
    private float tilting;                  // amount of tilting
    private float steerBackStartTime;
    private float steerBackEndTime;
    private float tiltBackStartTime;
    private float tiltBackEndTime;
    private bool isSteeringBack;
    private bool isTiltingBack;

// todo
//  replace simple multipliers with animation curves
//  bugs:
//      - the curves are useless because it’s just adding to the steering
//      without being really modulated (just the beginnings of each curve)

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

        // if player is giving X axis input
        if (move.IsInProgress() && direction.x != 0)
        {
            isSteeringBack = false;
            accelerationTime.x += Time.fixedDeltaTime * .3f;
            float t;
            t = _steeringCurveIn.Evaluate(accelerationTime.x);

            
            Debug.Log(t);

            steering = direction.x * _steeringSpeed * t * -1;

            if ((rotZ <= 360 - _maxLateralAngle && rotZ > 180) || (rotZ >= _maxLateralAngle && rotZ < 180))
            {
                steering = 0;
            }
        }
        // if player isn’t giving X axis input
        else
        {
            accelerationTime.x = 0;

            if (!isSteeringBack)
            {
                isSteeringBack = true;
                steerBackStartTime = Time.time;
                steerBackEndTime = steerBackStartTime + _steeringBackDuration;
            }

            float t = MathUtils.InverseLerp(steerBackStartTime, steerBackEndTime, Time.time);

            if (rotZ > 2 && rotZ < 180)
            {
                steering = - _steeringCurveOut.Evaluate(Time.fixedDeltaTime * _steeringBackDuration);
            }
            else if (rotZ > 180 && rotZ < 358)
            {
                steering = _steeringCurveOut.Evaluate(Time.fixedDeltaTime * _steeringBackDuration);
            }
            else
            {
                isSteeringBack = false;
                steering = 0;
            }
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
            float t;
            accelerationTime.y += Time.fixedDeltaTime * .3f;

            t = _tiltingCurveIn.Evaluate(accelerationTime.y);

            tilting = direction.y * _tiltingSpeed * t;

            if ((rotX <= 360 - _maxFrontalAngle && rotX > 180) || (rotX >= _maxFrontalAngle && rotX < 180))
            {
                tilting = 0;
            }

            altitudeShift = tilting * _altitudeShiftSpeed;
        }
        else
        {
            accelerationTime.y = 0;

            if (rotX > 2 && rotX < 180) tilting = -Time.fixedDeltaTime * _tiltingBackDuration;
            else if (rotX > 180 && rotX < 358) tilting = Time.fixedDeltaTime * _tiltingBackDuration;
        }

        // tilts the aircraft frontally
        transform.Rotate(tilting, 0, 0);

        // moves up or down
        transform.position = new Vector3(transform.position.x,
                                        transform.position.y + altitudeShift * -1,
                                        transform.position.z);
    }
    
}
