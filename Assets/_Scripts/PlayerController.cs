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
    [Range(0.001f, 2f)] [SerializeField] private float _steeringSpeed = .05f;
    [Range(0.01f, 0.05f)] [SerializeField] private float _rotationSpeed = .02f;
    [SerializeField] private AnimationCurve _steeringCurveOut;
    [SerializeField] private float _steeringBackDuration = 15f;
    [SerializeField] private float _maxLateralAngle = 35f;

    [Header("Tilting Settings")]
    [SerializeField] private AnimationCurve _tiltingCurveIn;
    [Range(0.001f, 2f)] [SerializeField] private float _tiltingSpeed = .05f;
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
//      - the tilting seems to block when the tilting back is going on

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

            // limits the steering inclination
            if (!((rotZ <= 360 - _maxLateralAngle && rotZ > 180) || (rotZ >= _maxLateralAngle && rotZ < 180)))
            {
                // evaluates players inputs and remaps according to the curve
                float t;
                if (direction.x > 0)
                    t = _steeringCurveIn.Evaluate(direction.x);
                else
                    t = -_steeringCurveIn.Evaluate(-direction.x);

                // multiplies the result by the speed and inverses controls
                steering = _steeringSpeed * t * -1;
            }
        }
        else
        {
            // sniffs the moment the player stops their input
            if (!isSteeringBack)
            {
                isSteeringBack = true;
                steerBackStartTime = Time.time;
                steerBackEndTime = steerBackStartTime + _steeringBackDuration;
            }

            // normalizes the value between the starting time and the expected back time
            float t = MathUtils.InverseLerp(steerBackStartTime, steerBackEndTime, Time.time);

            // steers back in the right direction depending on the angle the player’s in
            if (rotZ > 2 && rotZ < 180)
                steering = - _steeringCurveOut.Evaluate(t);
            else if (rotZ > 180 && rotZ < 358)
                steering = _steeringCurveOut.Evaluate(t);
            else
                isSteeringBack = false;
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
            isTiltingBack = false;
            Debug.Log("isTiltingBack: " + isTiltingBack);

            float t;
            if (direction.y > 0)
                t = _tiltingCurveIn.Evaluate(direction.y);
            else
                t = -_tiltingCurveIn.Evaluate(-direction.y);

            tilting = _tiltingSpeed * t;

            // limits the rotation between bounds
            if ((rotX <= 360 - _maxFrontalAngle && rotX > 180) || (rotX >= _maxFrontalAngle && rotX < 180))
            {
                tilting = 0;
            }

            altitudeShift = tilting * _altitudeShiftSpeed;

            Debug.Log(direction.y);
            Debug.Log(tilting);
        }
        else
        {
            if (!isTiltingBack)
            {
                isTiltingBack = true;
                tiltBackStartTime = Time.time;
                tiltBackEndTime = tiltBackStartTime + _tiltingBackDuration;
            }

            float t = MathUtils.InverseLerp(tiltBackStartTime, tiltBackEndTime, Time.time);

            if (rotX > 2 && rotX < 180)
                tilting = - _steeringCurveOut.Evaluate(t);
            else if (rotX > 180 && rotX < 358)
                tilting = _steeringCurveOut.Evaluate(t);
            else
                isTiltingBack = false;
            
            Debug.Log("isTiltingBack: " + isTiltingBack);
        }

        // tilts the aircraft frontally
        transform.Rotate(tilting, 0, 0);

        // moves up or down
        transform.position = new Vector3(transform.position.x,
                                        transform.position.y + altitudeShift * -1,
                                        transform.position.z);
    }
    
}
