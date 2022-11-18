using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Global Eden - Spirit/Snippets List")]
public class SnippetsList : ScriptableObject
{
    [SerializeField] private List<PoemSnippet> _orderedSnippets;
    public List<PoemSnippet> OrderedSnippets { get { return _orderedSnippets; } }
}
