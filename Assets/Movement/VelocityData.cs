using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class VelocityData : ScriptableObject
{
    public string dataName;
    [Tooltip("Used for directing how to construct the VelocityComponent from this data")]
    public ApplicationType applicationType;
    public AnimationCurve curve;
    public float magnitude;
    public Vector2 direction;
    public List<string> ignoreList = new();
}
//Jump ascent - One shot
//Gravity - continuous
//Run - continuous
public abstract class VelocityComponent
{
    protected string dataName;
    protected bool paused = true;
    public bool isPlaying => !paused;
    protected AnimationCurve curve;
    protected float magnitude;
    protected Vector2 direction;
    public readonly List<string> ignoreList = new();
    protected float time;
    protected float endTime => curve.keys[curve.keys.Length - 1].time;
    public List<Action> endTimeExceededActions = new();
    public VelocityComponent(VelocityData data)
    {
        dataName = data.dataName;
        curve = data.curve;
        magnitude = data.magnitude;
        direction = data.direction;
        ignoreList = data.ignoreList;
    }
    public virtual void Update(float timeDelta, ref Vector2 velocity)
    {
        if(paused)
        {
            return;
        }
        velocity += curve.Evaluate(time) * magnitude * direction;
        time += timeDelta;
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
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }
    public Vector3 GetDirection()
    {
        return direction;
    }
    public float GetMagnitude()
    {
        return magnitude;
    }
    public Vector3 GetVelocity()
    {
        return direction * magnitude;
    }
}
public class OneShotVelocity : VelocityComponent
{
    public OneShotVelocity(VelocityData data) : base(data)
    {
    }

    public override void Update(float timeDelta, ref Vector2 velocity)
    {
        base.Update(timeDelta, ref velocity);
        if (time >= endTime)
        {
            foreach (var action in endTimeExceededActions)
            {
                action();
            }
            Stop();
        }
    }
}
public class ContinuousVelocity : VelocityComponent
{
    public ContinuousVelocity(VelocityData data) : base(data)
    {
    }

    public override void Update(float timeDelta, ref Vector2 velocity)
    {
        base.Update(timeDelta, ref velocity);
    }
}
//OneShot has completion and start
//Continuous does not complete but can start max time here does not make sense
public enum ApplicationType
{
    OneShot,
    Continuous,
}