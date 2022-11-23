using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    /// <summary>
    /// For a normalized value t between two points a and b, returns the interpolation between the points.
    /// </summary>
    public static float Lerp(float a, float b, float t)
    {
        return (1.0f - t) * a + (b * t);
    }

    /// <summary>
    /// Returns a normalized value for any value between a minimum and maximum.
    /// </summary>
    public static float InverseLerp(float min, float max, float value)
    {
        return (value - min) / (max - min);
    }

    public static float Remap(  float inputMin,
                                float inputMax,
                                float outputMin,
                                float outputMax,
                                float value)
    {
        float t = InverseLerp(inputMin, inputMax, value);
        return Lerp(outputMin, outputMax, t);
    }
}
