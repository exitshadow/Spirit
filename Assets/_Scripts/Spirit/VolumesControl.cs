using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


[RequireComponent(typeof(Volume))]
public class VolumesControl : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private Light _nukeLight;
    [SerializeField] private Light _sunLight;
    [SerializeField] private AnimationCurve _flashCurve;
    [SerializeField] private float _explosionTime = 10;
    [Range(0.5f, 3f)] [SerializeField] private float _intensityMultiplier = 1.5f;

    private VolumeProfile profile;
    private LiftGammaGain lgg;
    private Vector4 liftVal;
    private Vector4 gammaVal;
    private Vector4 gainVal;

    private float nukeLightMaxIntensity;
    private float sunLightMaxIntensity;

    private float startTime;
    private float endTime;

    private bool hasExploded = false;
    private bool isOver = false;

// todo
// bugfix : if the nuke never falls, we loose the base profile!
// refactor with coroutines so the process doesn’t keep checking in the update forever

#region public methods
    public void Trigger()
    {
        if (!hasExploded) startTime = Time.time;
        endTime = startTime + _explosionTime;
        Debug.Log(startTime);
        Debug.Log(endTime);
        hasExploded = true;
        EnableLGG(true);
        _nukeLight.enabled = true;
    }
#endregion

#region unity messages
    private void Start()
    {
        _volume = GetComponent<Volume>();
        profile = _volume.sharedProfile;

        LGGSetup();

        sunLightMaxIntensity = _sunLight.intensity;
        nukeLightMaxIntensity = _nukeLight.intensity;
        Debug.Log("sun light max intensity: " + sunLightMaxIntensity);
        Debug.Log("nuke light max intensity: " + nukeLightMaxIntensity);
        _nukeLight.intensity = 0;

    }

    private void Update()
    {
        if(hasExploded && !isOver) Flash();
    }

    private void OnDisable()
    {
        ResetLGGValues();
        EnableLGG(false);
    }

#endregion

#region private methods
    private void Flash()
    {
        float t = MathUtils.InverseLerp(startTime, endTime, Time.time);
        float intensity = _flashCurve.Evaluate(t);
        //Debug.Log("time on curve: " + t);
        //Debug.Log("intensity: " + intensity);


        // retrieves initial settings and interpolates along animation curve
        lgg.lift.value = liftVal * intensity * _intensityMultiplier;
        lgg.gamma.value = gammaVal * intensity * _intensityMultiplier;
        lgg.gain.value = gainVal * intensity * _intensityMultiplier;

        _nukeLight.intensity = nukeLightMaxIntensity * intensity;
        _sunLight.intensity = sunLightMaxIntensity * (1 - intensity);
        Debug.Log(_nukeLight.intensity);

        if (t > 1)
        {
            ResetLGGValues();
            EnableLGG(false);
            _nukeLight.intensity = 0;
            _sunLight.intensity = sunLightMaxIntensity;
            isOver = true;
        }
    }

    private void LGGSetup()
    {
        // fetches the lift gamma grain or creates one if not fetched
        if (!profile.TryGet<LiftGammaGain>(out lgg))
        {
            lgg = profile.Add<LiftGammaGain>(false);
        }

        // fetches values stored in the volume settings
        liftVal = lgg.lift.value;
        gammaVal = lgg.gamma.value;
        gainVal = lgg.gain.value;

        EnableLGG(false);
    }

    private void EnableLGG(bool value)
    {
        lgg.lift.overrideState = value;
        lgg.gamma.overrideState = value;
        lgg.gain.overrideState = value;

        Debug.Log($"LGG values are {value}");
    }

    private void ResetLGGValues()
    {
        lgg.lift.value = liftVal;
        lgg.gamma.value = gammaVal;
        lgg.gain.value = gainVal;

        Debug.Log("LGG values have been reset.");
    }


#endregion

}
