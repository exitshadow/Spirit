using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static float Lerp(float a, float b, float t)
    {
        return (1.0f - t) * a + (b * t);
    }
    public static float InverseLerp(float a, float b, float value)
    {
        return (value - a) / (b - a);
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
