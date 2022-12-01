using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Nuke : MonoBehaviour
{
    [SerializeField] private GameManager _manager;
    [SerializeField] private Transform _earthTransform;
    [SerializeField] private float _earthSurfaceRadius = 6000;
    [SerializeField] private float _dropSpeed = 200f;
    [SerializeField] private AudioSource _explosionSound;
    [SerializeField] private float _timeBeforeSound = 3f;
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
            StartCoroutine(WaitAndBlast(_timeBeforeSound));
            hasLanded = true;
        }
    }

    private IEnumerator WaitAndBlast(float waitingTime)
    {
        Debug.Log("entering bomb sound delay coroutine");
        yield return new WaitForSeconds(waitingTime);
        if (_explosionSound != null)
        {
            _explosionSound.Play();
            Debug.Log("playing detonation sound");

            while (_explosionSound.isPlaying)
            {
                yield return null;
            }

            _manager.Next();
        }
    }
}
