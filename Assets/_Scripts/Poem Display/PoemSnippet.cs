using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Global Eden - Spirit/Poem Snippet")]
public class PoemSnippet : ScriptableObject
{
    [TextArea]
    [SerializeField] private string _content;
    [SerializeField] private float _displayTime = 2f;
    [SerializeField] private float _waitingTime = 1f;

    public string Content { get { return _content; } }
    public float DisplayTime { get { return _displayTime; } }
    public float WaitingTime { get { return _waitingTime; } }
}
