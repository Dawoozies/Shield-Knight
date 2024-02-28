using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class MovementData : ScriptableObject
{
    public float output { get; set; }
    public AnimationCurve curve;
    public float curveTime => curve.keys[curve.keys.Length - 1].time;
    public float currentTime { get; set; }
    public enum State
    {
        Starting, InProgress, Completed
    }
    public State state { get; set; }
    public void Update(float timeDelta)
    {
        if(currentTime == 0)
        {
            state = State.Starting;
        }
        if(currentTime > 0)
        {
            state = State.InProgress;
        }
        if(currentTime >= 1)
        {
            state = State.Completed;
        }
        currentTime += timeDelta;
    }
}
