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
    [Range(0.01f, 0.2f)] [SerializeField] private float _steeringFactor = .05f;
    [Range(0.01f, 1.0f)] [SerializeField] private float _rotationFactor = .2f;
    [SerializeField] private float _steerBackFactor = 100f;

    private DefaultInputActions _playerActions;
    private InputAction _move;
    private Vector2 _direction;
    private Vector2 _accelerationTime = new Vector2();
    private float steering = 0;

    public void Nuke()
    {
        _nuke.GetComponent<Rigidbody>().useGravity = true;
    }

    private void Start()
    {
        _playerActions = new DefaultInputActions();
        _move = _playerActions.Player.Move;
        _move.Enable();

    }

    private void Update()
    {
        _direction = _move.ReadValue<Vector2>();
        
        Orbit();
        Turn();
        //ControlAltitude();

    }

    private void Orbit()
    {
        transform.RotateAround(_rotationTarget.position, transform.right, _speed * Time.deltaTime);
    }

    private void Turn()
    {
        Debug.Log(transform.rotation.eulerAngles.z);
        float steering = 0;
        if (_move.IsInProgress() && _direction.x != 0)
        {
            _accelerationTime.x += Time.fixedDeltaTime * .3f;
            steering = _direction.x * _steeringFactor * _accelerationTime.x * -1;

        }
        else
        {
            _accelerationTime.x = 0;
        
            float rotZ = transform.rotation.eulerAngles.z;
            if (rotZ > 2) steering = -Time.fixedDeltaTime * _steerBackFactor;
            else if (rotZ < - 2) steering = Time.fixedDeltaTime * _steerBackFactor;
            else steering = 0;

            Debug.Log("steering X: " + steering);
        }
        
        // tilts the aircraft laterally
        transform.Rotate(0,0, steering);

        // rotates the aircraft by direction
        transform.Rotate(0, _direction.x * _rotationFactor, 0);
    }

    private void ControlAltitude()
    {
        float steeringY = 0;
        if (_move.IsInProgress() && _direction.y != 0)
        {
            steeringY = _direction.y * _steeringFactor * _accelerationTime.y;
            _accelerationTime.y += Time.fixedDeltaTime * .3f;

        }
        else if (_accelerationTime.y > 0)
        {
            _accelerationTime.y -= Time.fixedDeltaTime * .3f;
            steeringY = - 1 * _steeringFactor * _accelerationTime.y;
        }

        // tilts the aircraft frontally
        transform.Rotate(steeringY, 0, 0);

        // moves up or down
        transform.position = new Vector3(transform.position.x,
                                        transform.position.y + steeringY * -1,
                                        transform.position.z);
    }
    
}
