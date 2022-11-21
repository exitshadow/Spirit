using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


[RequireComponent(typeof(Volume))]
public class VolumesControl : MonoBehaviour
{
    [SerializeField] private float _explosionTime = 10;
    private Volume _volume;
    private VolumeProfile _profile;
    private LiftGammaGain _lgg;

    public IEnumerator FlashAndFade()
    {
        // turns the settings on
        _lgg.lift.overrideState = true;
        _lgg.gamma.overrideState = true;
        _lgg.gain.overrideState = true;

        yield return new WaitForSeconds(_explosionTime);

        // turns the settings off
        _lgg.lift.overrideState = false;
        _lgg.gamma.overrideState = false;
        _lgg.gain.overrideState = false;

    }

    void Start()
    {     
        _volume = GetComponent<Volume>();
        _profile = _volume.sharedProfile;

        // fetches the lift gamma grain or creates one if not fetched
        if (!_profile.TryGet<LiftGammaGain>(out _lgg))
        {
            _lgg = _profile.Add<LiftGammaGain>(false);
        }

        // turns off anything that is there
        _lgg.lift.overrideState = false;
        _lgg.gamma.overrideState = false;
        _lgg.gain.overrideState = false;
    }

    public void Flash()
    {
        StartCoroutine(FlashAndFade());
    }
}
