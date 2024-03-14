using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu]
public class MoveData : ScriptableObject
{
    public string dataName;
    public ApplicationType applicationType;
    public AnimationCurve curve;
    public float magnitude;
    public Vector3 direction;
    public MoveType moveType;
    public MoveEndType moveEndType;
    [Tooltip("If moveEndType is targetReached then component ends when distance < threshold")]
    public float targetDistanceThreshold;
}
public abstract class MoveComponent
{
    protected string dataName;
    protected bool paused = true;
    public bool isPlaying => !paused;
    protected AnimationCurve curve;
    protected float magnitude;
    protected Vector3 direction;
    protected float time;
    protected float endTime => curve.keys[curve.keys.Length - 1].time;
    public List<Action<MoveEndType>> endActions = new();
    protected MoveType moveType;
    protected MoveEndType moveEndType;
    protected Transform target;
    protected Vector3 targetPoint;
    protected float targetDistanceThreshold;
    public MoveComponent(MoveData data)
    {
        dataName = data.dataName;
        curve = data.curve;
        magnitude = data.magnitude;
        direction = data.direction;
        moveType = data.moveType;
        moveEndType = data.moveEndType;
        targetDistanceThreshold = data.targetDistanceThreshold;
    }
    //Move vector either a translation or a velocity
    public virtual void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector)
    {
        if (paused)
        {
            return;
        }
        switch (moveType)
        {
            case MoveType.Free:
                moveVector += curve.Evaluate(time) * magnitude * direction;
                time += timeDelta;
                return;
            case MoveType.TransformFollow:
                Update(timeDelta, ref moveVector, currentVector, target);
                return;
            case MoveType.TargetPoint:
                Update(timeDelta, ref moveVector, currentVector, targetPoint);
                return;
        }
    }
    public virtual void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector, Transform target)
    {
        Vector3 dv = Vector3.LerpUnclamped(Vector3.zero, target.position-currentVector, Mathf.Clamp(curve.Evaluate(time), -1f, 1f));
        moveVector += dv;
        time += timeDelta*magnitude;
    }
    public virtual void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector, Vector3 targetPoint)
    {
        SetDirection((targetPoint - currentVector).normalized);
        moveVector += curve.Evaluate(time) * magnitude * direction * timeDelta;
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
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void SetTargetPoint(Vector3 targetPoint)
    {
        this.targetPoint = targetPoint;
    }
    public bool CheckDistanceWithinThreshold(Vector3 currentVector)
    {
        switch (moveType)
        {
            case MoveType.TransformFollow:
                if(target != null)
                {
                    return Vector3.Distance(currentVector, target.position) < targetDistanceThreshold;
                }
                break;
            case MoveType.TargetPoint:
                return Vector3.Distance(currentVector, targetPoint) < targetDistanceThreshold;
        }

        return false;
    }
    public bool CheckTimeExceeded()
    {
        return time >= endTime;
    }
    public void EndComponent(MoveEndType endType)
    {
        foreach (var action in endActions)
        {
            action(endType);
        }
        Stop();
    }
    public void EndComponentNoStop(MoveEndType endType)
    {
        foreach (var action in endActions)
        {
            action(endType);
        }
    }
}
//Always ends when either end time or target reached
public class OneShotMove : MoveComponent
{
    public OneShotMove(MoveData data) : base(data) { }
    public override void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector)
    {
        base.Update(timeDelta, ref moveVector, currentVector);

        switch (moveEndType)
        {
            case MoveEndType.EndTimeOrTargetReached:
                if(CheckDistanceWithinThreshold(currentVector))
                {
                    EndComponent(MoveEndType.TargetReached);
                    break;
                }
                if(CheckTimeExceeded())
                {
                    EndComponent(MoveEndType.EndTimeExceeded);
                    break;
                }
                break;
            case MoveEndType.EndTimeExceeded:
                if (CheckTimeExceeded())
                {
                    EndComponent(MoveEndType.EndTimeExceeded);
                }
                break;
            case MoveEndType.TargetReached:
                if (CheckDistanceWithinThreshold(currentVector))
                {
                    EndComponent(MoveEndType.TargetReached);
                }
                break;
        }
    }
}
//Does not (by itself) stop, can fire off target reached action
public class ContinuousMove : MoveComponent
{
    public ContinuousMove(MoveData data) : base(data) { }
    public override void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector)
    {
        base.Update(timeDelta, ref moveVector, currentVector);
        switch (moveEndType)
        {
            case MoveEndType.NoEnd:
                return;
            case MoveEndType.TargetReached:
                if(CheckDistanceWithinThreshold(currentVector))
                {
                    EndComponentNoStop(MoveEndType.TargetReached);
                }
                return;
        }
    }
}
//Free = Move in given direction, ends when time elapses for one shot
//TransformFollow = Move towards a transforms position,
public enum MoveType
{
    Free, TransformFollow, TargetPoint
}
//Only endtime is ignored in continuous
public enum MoveEndType
{
    NoEnd, EndTimeOrTargetReached, EndTimeExceeded, TargetReached
}