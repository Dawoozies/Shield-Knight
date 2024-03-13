using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu]
public class ScaleData : ScriptableObject
{
    public string dataName;
    public ApplicationType applicationType;
    public AnimationCurve curveX, curveY;
    public float magnitude;
    public float speedMultiplier;
}
public abstract class ScaleComponent
{
    protected string dataName;
    protected bool paused = true;
    public bool isPlaying => !paused;
    protected AnimationCurve curveX, curveY;
    protected float magnitude;
    protected float time;
    protected float speedMultiplier;
    protected float endTime => Mathf.Max(curveX.keys[curveX.keys.Length - 1].time, curveY.keys[curveY.keys.Length - 1].time);
    public List<Action> endTimeExceededActions = new();
    public ScaleComponent(ScaleData data)
    {
        dataName = data.dataName;
        curveX = data.curveX;
        curveY = data.curveY;
        magnitude = data.magnitude;
        speedMultiplier = data.speedMultiplier;
    }
    public virtual void Update(float timeDelta, ref Vector3 scale)
    {
        if(paused)
        {
            return;
        }
        scale += new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), 0f)*magnitude + Vector3.forward;
        time += timeDelta * speedMultiplier;
    }
    public void Play()
    {
        paused = false;
    }
    public void Pause()
    {
        paused = true;
    }
    public void Stop()
    {
        time = 0;
        paused = true;
    }
    public void PlayFromStart()
    {
        time = 0;
        paused = false;
    }
    public void SetMagnitude(float magnitude)
    {
        this.magnitude = magnitude;
    }
}
public class OneShotScaleEffect : ScaleComponent
{
    public OneShotScaleEffect(ScaleData data) : base(data) { }
    public override void Update(float timeDelta, ref Vector3 scale)
    {
        base.Update(timeDelta, ref scale);
        if(time >= endTime)
        {
            foreach (var action in endTimeExceededActions)
            {
                action();
            }
            Debug.Log("One Shot Scale Effect Stopping: " + dataName);
            Stop();
        }
    }
}
public class ContinuousScaleEffect : ScaleComponent
{
    public ContinuousScaleEffect(ScaleData data) : base(data) { }
    public override void Update(float timeDelta, ref Vector3 scale)
    {
        base.Update(timeDelta, ref scale);
    }
}