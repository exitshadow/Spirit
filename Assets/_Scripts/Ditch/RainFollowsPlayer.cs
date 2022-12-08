using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainFollowsPlayer : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private void Update()
    {
        transform.position = new Vector3(_player.position.x, transform.position.y, _player.position.z);
    }
}
