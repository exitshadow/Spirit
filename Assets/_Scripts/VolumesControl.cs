using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


[RequireComponent(typeof(Volume))]
public class VolumesControl : MonoBehaviour
{
    [SerializeField] private AnimationCurve _flashCurve;
    [SerializeField] private float _explosionTime = 10;
    [Range(0.5f, 3f)] [SerializeField] private float _intensityMultiplier = 1.5f;
    private Volume volume;
    private VolumeProfile profile;
    private LiftGammaGain lgg;
    private Vector4 liftVal;
    private Vector4 gammaVal;
    private Vector4 gainVal;

    private float startTime;
    private float endTime;

    private bool hasExploded = false;

// todo
// bugfix : if the nuke never falls, we loose the base profile!

    void Start()
    {     
        volume = GetComponent<Volume>();
        profile = volume.sharedProfile;

        // fetches the lift gamma grain or creates one if not fetched
        if (!profile.TryGet<LiftGammaGain>(out lgg))
        {
            lgg = profile.Add<LiftGammaGain>(false);
        }

        // turns the settings off
        lgg.lift.overrideState = false;
        lgg.gamma.overrideState = false;
        lgg.gain.overrideState = false;

        // fetches values stored in the volume settings
        liftVal =   lgg.lift.value;
        gammaVal =  lgg.gamma.value;
        gainVal =   lgg.gain.value;

        // sets values to 0 before turning settings back on
        lgg.lift.value = new Vector4(0,0,0,0);
        lgg.gamma.value = new Vector4(0,0,0,0);
        lgg.gain.value = new Vector4(0,0,0,0);

        // turns the settings on
        lgg.lift.overrideState = true;
        lgg.gamma.overrideState = true;
        lgg.gain.overrideState = true;

    }

    private void Update()
    {
        if(hasExploded) Flash();
    }

    public void Trigger()
    {
        if (!hasExploded) startTime = Time.time;
        endTime = startTime + _explosionTime;
        Debug.Log(startTime);
        Debug.Log(endTime);
        hasExploded = true;
    }

    private void Flash()
    {
        float t = MathUtils.InverseLerp(startTime, endTime, Time.time);
        float intensity = _flashCurve.Evaluate(t);

        // retrieves initial settings and interpolates along animation curve
        lgg.lift.value = liftVal * intensity * _intensityMultiplier;
        lgg.gamma.value = gammaVal * intensity * _intensityMultiplier;
        lgg.gain.value = gainVal * intensity * _intensityMultiplier;

        if (intensity <= 0)
        {
            lgg.lift.value = liftVal;
            lgg.gamma.value = gammaVal;
            lgg.gain.value = gainVal;

            // turns the settings on
            lgg.lift.overrideState = false;
            lgg.gamma.overrideState = false;
            lgg.gain.overrideState = false;
        }
    }

    IEnumerator FlashAndFade()
    {
        // turns the settings on
        lgg.lift.overrideState = true;
        lgg.gamma.overrideState = true;
        lgg.gain.overrideState = true;

        yield return new WaitForSeconds(_explosionTime);

        // turns the settings off
        lgg.lift.overrideState = false;
        lgg.gamma.overrideState = false;
        lgg.gain.overrideState = false;

    }
}
