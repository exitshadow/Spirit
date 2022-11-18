using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private SnippetsList _snippetsList;
    [SerializeField] private TextMeshProUGUI _TMPFieldToFill;
    [SerializeField] private float _timeToWaitBeforeStart;
    [SerializeField] private float _fadingTimeIn;
    [SerializeField] private float _fadingTimeOut;
    private List<PoemSnippet> _snippets;
    private bool _hasStarted = false;

    private void Awake()
    {
        _snippets = _snippetsList.OrderedSnippets;
        _TMPFieldToFill.text = "";
    }

    private void Start()
    {
        StartCoroutine(DisplaySnippet());
    }

    IEnumerator DisplaySnippet()
    {
        // waits for time
        if (!_hasStarted)
        {
            yield return new WaitForSeconds(_timeToWaitBeforeStart);
            _hasStarted = true;
        }

        for (int i = 0; i < _snippets.Count; i++)
        {
            // shows first snippet
            _TMPFieldToFill.text = _snippets[i].Content;

            // waits its display time
            yield return new WaitForSeconds(_snippets[i].DisplayTime);

            // masks it
            _TMPFieldToFill.text = "";

            // waits its waiting time
            yield return new WaitForSeconds(_snippets[i].WaitingTime);
        }
    }
}
