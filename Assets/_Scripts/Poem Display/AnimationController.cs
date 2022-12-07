using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI.ProceduralImage;
using TMPro;

    // todo
    // change _manager.IsPoemRunning, and add states for each type of animations
    // refactor to include the conditions on the title screen / poem runner

    // todo
    // consistent nomenclature with all the fading variables

public class AnimationController : MonoBehaviour
{
    [Header ("Game Manager Reference")]
    [SerializeField] private GameManager _manager;

    [Header ("Sound Control")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AnimationCurve _audioFadeInCurve;
    [SerializeField] private AnimationCurve _audioFadeOutCurve;


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
    [SerializeField] private bool _debugSkipTitleScreen = false;


    [Header("Poem Snippets Settings")]
    [SerializeField] private SnippetsList _snippetsList;
    [SerializeField] private TextMeshProUGUI _TMPFieldToFill;
    [SerializeField] private float _timeToWaitBeforeStart;
    [SerializeField] private float _fadingTimeIn;
    [SerializeField] private AnimationCurve _fadingCurveIn;
    [SerializeField] private float _fadingTimeOut;
    [SerializeField] private AnimationCurve _fadingCurveOut;
    [SerializeField] private bool _debugSkipPoem = false;


    [Header("Trigger Text Settings")]
    [SerializeField] private TextMeshProUGUI _TMPTriggerText;
    [SerializeField] float _triggerTextDuration;
    [SerializeField] float _triggerTextFadingTimeIn;
    [SerializeField] AnimationCurve _triggerTextFadingCurveIn;
    [SerializeField] float _triggerTextFadingTimeOut;
    [SerializeField] AnimationCurve _triggerTextFadingCurveOut;
    [SerializeField] private bool _debugSkipTriggerText = false;


    [Header("SceneFadeOutSettings")]
    [SerializeField] private float _sceneHoldingTimeBefore = 10f;
    [SerializeField] private float _sceneHoldingTimeAfter = 2f;
    [SerializeField] private float _sceneFadeOutTime = 15f;
    [SerializeField] private AnimationCurve _sceneFadeOutCurve;

    private List<PoemSnippet> _snippets;

    public void StartEndFade()
    {
        StartCoroutine(FadeAndChangeScene());
    }

    private void Awake()
    {
        _snippets = _snippetsList.OrderedSnippets;
        _TMPFieldToFill.text = "";
        _TMPTriggerText.gameObject.SetActive(false);
        _manager.IsPoemRunning = true;
    }

    private void Start()
    {
        StartCoroutine(StartDisplaying());
    }

    private IEnumerator StartDisplaying()
    {
        if (!_debugSkipTitleScreen)
        {
            // setup
            _titleScreenHolder.SetActive(true);
            _titleScreenTMP.alpha = 0;
            _audioSource.volume = 0;
            yield return new WaitForSeconds(_titleScreenWaitingTime);

            // fade text in
            yield return StartCoroutine(FadeTarget( _titleScreenTMP.gameObject,
                                                    _titleTextFadeInDuration,
                                                    _titleTextFadeInCurve));

            // hold text
            yield return new WaitForSeconds(_titleTextHoldDuration);

            // sync sound in and text out
            // fade text out
            StartCoroutine(FadeTarget( _titleScreenTMP.gameObject,
                                        _titleTextFadeOutDuration,
                                        _titleTextFadeOutCurve));
            // fade sound in
            yield return StartCoroutine(FadeTarget( _audioSource.gameObject,
                                                    _titleTextFadeOutDuration, // this has to be in sync
                                                    _audioFadeInCurve));

            // hold empty background
            yield return new WaitForSeconds(_emptyTextHoldDuration);

            // fade background out
            yield return StartCoroutine(FadeTarget( _titleScreenBackground.gameObject,
                                                    _titleScreenFadeOutDuration,
                                                    _titleScreenFadeOutCurve));

            _titleScreenHolder.SetActive(false);
        }

        StartCoroutine(DisplaySnippets());
    }

    private IEnumerator DisplaySnippets()
    {
        if (!_debugSkipPoem)
        {
            yield return new WaitForSeconds(_timeToWaitBeforeStart);

            for (int i = 0; i < _snippets.Count; i++)
            {
                // shows first snippet
                _TMPFieldToFill.text = _snippets[i].Content;

                // fade in
                yield return StartCoroutine(FadeTarget(  _TMPFieldToFill.gameObject,
                                            _fadingTimeIn,
                                            _fadingCurveIn));

                // waits its display time
                yield return new WaitForSeconds(_snippets[i].DisplayTime);

                // fade out
                yield return StartCoroutine(FadeTarget(  _TMPFieldToFill.gameObject,
                                            _fadingTimeOut,
                                            _fadingCurveOut));

                // masks it
                _TMPFieldToFill.text = "";

                // waits its waiting time
                yield return new WaitForSeconds(_snippets[i].WaitingTime);
            }
        }

        _manager.IsPoemRunning = false;

        StartCoroutine(ShowTriggerText());
    }

    private IEnumerator ShowTriggerText()
    {
        if (!_debugSkipTriggerText)
        {
            // setup
            _TMPTriggerText.gameObject.SetActive(true);
            _TMPTriggerText.alpha = 0;

            // fade in
            yield return StartCoroutine(FadeTarget(  _TMPTriggerText.gameObject,
                                        _triggerTextFadingTimeIn,
                                        _triggerTextFadingCurveIn));

            // hold
            yield return new WaitForSeconds(_triggerTextDuration);

            // fade out
            yield return StartCoroutine(FadeTarget(  _TMPTriggerText.gameObject,
                                        _triggerTextFadingTimeOut,
                                        _triggerTextFadingCurveOut));

            // disables
            _TMPTriggerText.gameObject.SetActive(false);
        }
    }
    
    private IEnumerator FadeAndChangeScene()
    {        
        yield return new WaitForSeconds(_sceneHoldingTimeBefore);

        // setup
        _titleScreenHolder.gameObject.SetActive(true);
        _titleScreenTMP.gameObject.SetActive(false);
        _titleScreenBackground.gameObject.SetActive(true);

        // fades scene and audio out
        StartCoroutine(FadeTarget(  _titleScreenBackground.gameObject,
                                    _sceneFadeOutTime,
                                    _sceneFadeOutCurve));
        yield return StartCoroutine(FadeTarget( _audioSource.gameObject,
                                                _sceneFadeOutTime,      // has to be in sync!
                                                _audioFadeOutCurve));

        yield return new WaitForSeconds(_sceneHoldingTimeAfter);

        _manager.LoadNextScene();
    }

    public static IEnumerator FadeTarget(GameObject target, float duration, AnimationCurve curve)
    {
        Debug.Log($"Starting fading target: {target.name}");

        float timeStart = Time.time;
        float timeEnd = timeStart + duration;
        bool isFadingHere = true;

        var tmpTarget = target.GetComponent<TextMeshProUGUI>();
        var imgTarget = target.GetComponent<ProceduralImage>();
        var audioTarget = target.GetComponent<AudioSource>();

        while (isFadingHere)
        {
            float t = MathUtils.InverseLerp(timeStart, timeEnd, Time.time);

            if (tmpTarget != null)
                tmpTarget.alpha = curve.Evaluate(t);
            if (imgTarget != null)
                imgTarget.color = new Color(imgTarget.color.r,
                                            imgTarget.color.g,
                                            imgTarget.color.b,
                                            curve.Evaluate(t));
            if (audioTarget != null)
                audioTarget.volume = curve.Evaluate(t);

            if (Time.time > timeEnd) isFadingHere = false;
            yield return null;
        }
        
        Debug.Log("End of fading reached");
    }
}
