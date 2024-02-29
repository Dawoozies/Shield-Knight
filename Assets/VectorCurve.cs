using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class VectorCurve
{
    public float speedMultiplier;
    public float maxOutput;
    public AnimationCurve curveX;
    public AnimationCurve curveY;
    public Vector2 output => new Vector2(outputX, outputY)*maxOutput;
    float outputX => curveX.Evaluate(currentTime);
    float outputY => curveY.Evaluate(currentTime);
    public float curveTimeX => curveX.keys[curveX.keys.Length - 1].time;
    public float curveTimeY => curveY.keys[curveY.keys.Length - 1].time;
    public float currentTime { get; set; }
    public void Update(float timeDelta)
    {
        currentTime += timeDelta * speedMultiplier;
    }
    public void Reset()
    {
        currentTime = 0;
    }
}
