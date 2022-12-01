using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using TMPro;

public class AnimationController : MonoBehaviour
{
    [Header ("Game Manager Reference")]
    [SerializeField] private GameManager _manager;

    [Header ("Title Screen References")]
    [SerializeField] private GameObject _titleScreenHolder;
    [SerializeField] private ProceduralImage _titleScreenBackground;
    [SerializeField] private TextMeshProUGUI _titleScreenTMP;

    [Header("Background Screen Settings")]
    [SerializeField] private float _titleScreenWaitingTime;
    [SerializeField] private float _titleScreenFadeOutDuration;
    [SerializeField] private AnimationCurve _titleScreenFadeOutCurve;

    [Header("Title Screen Text Settings")]
    [SerializeField] private float _titleTextFadeInDuration = .5f;
    [SerializeField] private AnimationCurve _titleTextFadeInCurve;
    [SerializeField] private float _titleTextFadeOutDuration = .5f;
    [SerializeField] private AnimationCurve _titleTextFadeOutCurve;
    [SerializeField] private float _titleTextHoldDuration = 3f;
    [SerializeField] private float _emptyTextHoldDuration = 2f;


    [Header("Poem Snippets Settings")]
    [SerializeField] private SnippetsList _snippetsList;
    [SerializeField] private TextMeshProUGUI _TMPFieldToFill;
    [SerializeField] private float _timeToWaitBeforeStart;
    [SerializeField] private float _fadingTimeIn;
    [SerializeField] private AnimationCurve _fadingCurveIn;
    [SerializeField] private float _fadingTimeOut;
    [SerializeField] private AnimationCurve _fadingCurveOut;

    private List<PoemSnippet> _snippets;
    bool isFading = false;


    private void Awake()
    {
        _snippets = _snippetsList.OrderedSnippets;
        _TMPFieldToFill.text = "";
    }

    private void Start()
    {
        StartCoroutine(DisplayTitleScreen());
    }

    private IEnumerator DisplayTitleScreen()
    {
        // setup
        _titleScreenHolder.SetActive(true);
        _titleScreenTMP.alpha = 0;
        yield return new WaitForSeconds(_titleScreenWaitingTime);

        // fade text in
        StartCoroutine(FadeTarget(  _titleScreenTMP.gameObject,
                                    _titleTextFadeInDuration,
                                    _titleTextFadeInCurve));
        while (isFading) yield return null;

        // hold text
        yield return new WaitForSeconds(_titleTextHoldDuration);

        // fade text out
        StartCoroutine(FadeTarget(  _titleScreenTMP.gameObject,
                                    _titleTextFadeOutDuration,
                                    _titleTextFadeOutCurve));
        while (isFading) yield return null;

        // hold empty background
        yield return new WaitForSeconds(_emptyTextHoldDuration);

        // fade background out
        StartCoroutine(FadeTarget(  _titleScreenBackground.gameObject,
                                    _titleScreenFadeOutDuration,
                                    _titleScreenFadeOutCurve));
        while (isFading) yield return null;

        _titleScreenHolder.SetActive(false);

        StartCoroutine(DisplaySnippet());
    }

    private IEnumerator DisplaySnippet()
    {
        _manager.IsPoemRunning = true;

        yield return new WaitForSeconds(_timeToWaitBeforeStart);

        for (int i = 0; i < _snippets.Count; i++)
        {
            // shows first snippet
            _TMPFieldToFill.text = _snippets[i].Content;

            // fade in
            StartCoroutine(FadeTarget(  _TMPFieldToFill.gameObject,
                                        _fadingTimeIn,
                                        _fadingCurveIn));
            while (isFading) yield return null;

            // waits its display time
            yield return new WaitForSeconds(_snippets[i].DisplayTime);

            // fade out
            StartCoroutine(FadeTarget(  _TMPFieldToFill.gameObject,
                                        _fadingTimeOut,
                                        _fadingCurveOut));
            while (isFading) yield return null;

            // masks it
            _TMPFieldToFill.text = "";

            // waits its waiting time
            yield return new WaitForSeconds(_snippets[i].WaitingTime);
        }

        _manager.IsPoemRunning = false;
    }

    private IEnumerator FadeTarget(GameObject target, float duration, AnimationCurve curve)
    {
        Debug.Log($"Starting fading target: {target.name}");

        float timeStart = Time.time;
        float timeEnd = timeStart + duration;
        isFading = true;

        var tmpTarget = target.GetComponent<TextMeshProUGUI>();
        var imgTarget = target.GetComponent<ProceduralImage>();

        while (isFading)
        {
            float t = MathUtils.InverseLerp(timeStart, timeEnd, Time.time);

            if (tmpTarget != null)
                tmpTarget.alpha = curve.Evaluate(t);
            if (imgTarget != null)
                imgTarget.color = new Color(imgTarget.color.r,
                                            imgTarget.color.g,
                                            imgTarget.color.b,
                                            curve.Evaluate(t));

            if (Time.time > timeEnd) isFading = false;
            yield return null;
        }
        
        Debug.Log("End of fading reached");
    }
}
