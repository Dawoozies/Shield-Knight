using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ShieldAttackData
{
    public string name;
    public float cooldown;
    public float radiusMultiplier;
    public AnimationCurve radiusCurve;
    public float circleRotationMultiplier;
    public AnimationCurve circleRotationCurve;
    public float shieldRotationMultiplier;
    public AnimationCurve shieldRotationCurve;
    public Vector3 output =>
        new Vector3(
                radiusMultiplier * radiusCurve.Evaluate(currentTime),
                circleRotationMultiplier * circleRotationCurve.Evaluate(currentTime),
                shieldRotationMultiplier * shieldRotationCurve.Evaluate(currentTime)
            );
    public float currentTime { get; set; }
    public float maxCurveTime =>
        Mathf.Max(
            radiusCurve.keys[radiusCurve.keys.Length - 1].time,
            Mathf.Max(circleRotationCurve.keys[circleRotationCurve.keys.Length - 1].time,
                shieldRotationCurve.keys[shieldRotationCurve.keys.Length - 1].time)
            );
    Func<float> TimeDelta;
    public List<Action> onStartActions = new();
    public List<Action> onCompletedActions = new();
    float cooldownTimer;
    public enum State
    {
        Ready, Firing, Completed
    }
    public State state { get; set; }
    public void Update()
    {
        if (TimeDelta == null)
            return;
        if(state == State.Completed)
        {
            if(cooldownTimer > 0)
            {
                cooldownTimer -= TimeDelta();
            }
            else
            {
                cooldownTimer = 0;
                state = State.Ready;
            }
        }
        if(state != State.Firing)
            return;

        if (currentTime >= maxCurveTime)
        {
            foreach (var action in onCompletedActions)
            {
                action();
            }
            state = State.Completed;
        }
        currentTime += TimeDelta();
    }
    public void Start(Func<float> TimeDeltaFunc)
    {
        if (state != State.Ready)
            return;

        currentTime = 0;
        TimeDelta = TimeDeltaFunc;
        foreach (var action in onStartActions)
        {
            action();
        }
        cooldownTimer = cooldown;
        state = State.Firing;
    }
    public void ForceEnd()
    {
        currentTime = maxCurveTime;
    }
}
