using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class MovementData
{
    public string movementName;
    public List<string> nullifyWhileActive;
    public bool keepAtMax;
    public float maxOutput;
    public Vector2 direction;
    public float output => maxOutput * curve.Evaluate(currentTime);
    public AnimationCurve curve;
    public float curveTime => curve.keys[curve.keys.Length - 1].time;
    public float currentTime { get; set; }
    public enum State
    {
        Awake, InProgress, Completed
    }
    public State state { get; set; }
    Func<float> TimeDelta;
    public List<Action> onStartActions = new();
    public List<Action> onCompletedActions = new();
    public void Update()
    {
        if(TimeDelta == null)
            return;

        if (currentTime >= curveTime)
        {
            state = State.Completed;
            TimeDelta = () => 0;
            if(!keepAtMax)
            {
                currentTime = 0;
            }
            foreach(var action in onCompletedActions)
            {
                action();
            }
        }
        currentTime += TimeDelta();
    }
    public void Start(Func<float> TimeDeltaFunc)
    {
        state = State.InProgress;
        currentTime = 0;
        TimeDelta = TimeDeltaFunc;
        foreach (var action in onStartActions)
        {
            action();
        }
    }
    public void ForceEnd()
    {
        currentTime = curveTime;
    }
}