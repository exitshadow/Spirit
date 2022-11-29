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
    [SerializeField] private AnimationCurve _fadingCurveIn;
    [SerializeField] private float _fadingTimeOut;
    [SerializeField] private AnimationCurve _fadingCurveOut;

    private List<PoemSnippet> _snippets;

    private float fadeInStart;
    private float fadeInEnd;
    private float fadeOutStart;
    private float fadeOutEnd;

    private bool hasStarted = false;
    private bool isFadingOut = false;
    private bool isFadingIn = false;

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
        if (!hasStarted)
        {
            yield return new WaitForSeconds(_timeToWaitBeforeStart);
            hasStarted = true;
        }

        for (int i = 0; i < _snippets.Count; i++)
        {
            // shows first snippet
            _TMPFieldToFill.text = _snippets[i].Content;

            // fade in
            if (!isFadingIn)
            {
                fadeInStart = Time.time;
                fadeInEnd = fadeInStart + _fadingTimeIn;
                isFadingIn = true;
            }

            while (isFadingIn)
            {
                float t = MathUtils.InverseLerp(fadeInStart, fadeInEnd, Time.time);
                _TMPFieldToFill.alpha = _fadingCurveIn.Evaluate(t);
                if (Time.time > fadeInEnd) isFadingIn = false;
                yield return null;
            }


            // waits its display time
            yield return new WaitForSeconds(_snippets[i].DisplayTime);


            // fade out
            if (!isFadingOut)
            {
                fadeOutStart = Time.time;
                fadeOutEnd = fadeOutStart + _fadingTimeOut;
                isFadingOut = true;
            }

            while (isFadingOut)
            {
                float t = MathUtils.InverseLerp(fadeOutStart, fadeOutEnd, Time.time);
                _TMPFieldToFill.alpha = _fadingCurveOut.Evaluate(t);
                if (Time.time > fadeOutEnd) isFadingOut = false;
                yield return null;
            }

            // masks it
            _TMPFieldToFill.text = "";

            // waits its waiting time
            yield return new WaitForSeconds(_snippets[i].WaitingTime);
        }
    }
}
