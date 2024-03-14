using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu]
public class MoveData : ScriptableObject
{
    public string dataName;
    public AnimationCurve curve;
    public float magnitude;
    public Vector3 direction;
    public MoveType moveType;
    public MoveEndType moveEndType;
    public bool stopOnEnd;
    [Tooltip("If moveEndType is targetReached then component ends when distance < threshold")]
    public float endDistanceThreshold;
    [Tooltip("originMin = How far from the origin point do we have to be. originMax = How close to the origin point do we have to be")]
    public Vector2 originDistanceBounds;
    [Tooltip("endMin = How far from the end point do we have to be. endMax = How close to the end point do we have to be")]
    public Vector2 endDistanceBounds;
    public MoveBoundsFlags moveBoundsFlags;
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
    protected bool stopOnEnd;

    protected Vector3 originPoint, endPoint;
    protected Vector3 previousOriginPoint, previousEndPoint;
    protected Transform originTransform, endTransform;
    protected float endDistanceThreshold;

    protected Vector2 originDistanceBounds;
    protected Vector2 endDistanceBounds;
    protected MoveBoundsFlags moveBoundsFlags;
    public MoveComponent(MoveData data)
    {
        dataName = data.dataName;
        curve = data.curve;
        magnitude = data.magnitude;
        direction = data.direction;
        moveType = data.moveType;
        moveEndType = data.moveEndType;
        endDistanceThreshold = data.endDistanceThreshold;
        stopOnEnd = data.stopOnEnd;

        originDistanceBounds = data.originDistanceBounds;
        endDistanceBounds = data.endDistanceBounds;
        moveBoundsFlags = data.moveBoundsFlags;
    }
    //Move vector either a translation or a velocity
    public virtual void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector)
    {
        if (paused)
        {
            return;
        }
        moveVector += curve.Evaluate(time) * magnitude * direction;
        time += timeDelta;
    }
    public virtual void CheckEnd(Vector3 currentVector)
    {
        switch (moveEndType)
        {
            case MoveEndType.NoEnd:
                break;
            case MoveEndType.EndTimeOrTargetReached:
                if(CheckDistanceToEnd(currentVector) || CheckTimeExceeded())
                {
                    EndComponent(moveEndType, stopOnEnd);
                }
                break;
            case MoveEndType.EndTimeExceeded:
                if (CheckTimeExceeded())
                {
                    EndComponent(moveEndType, stopOnEnd);
                }
                break;
            case MoveEndType.TargetReached:
                if (CheckDistanceToEnd(currentVector))
                {
                    EndComponent(moveEndType, stopOnEnd);
                }
                break;
        }
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
    public void SetOriginPoint(Vector3 originPoint)
    {
        this.originPoint = originPoint;
    }
    public void SetEndPoint(Vector3 endPoint)
    {
        this.endPoint = endPoint;
    }
    public void SetOriginTransform(Transform originTransform)
    {
        this.originTransform = originTransform;
    }
    public void SetEndTransform(Transform endTransform)
    {
        this.endTransform = endTransform;
    }
    public bool CheckTimeExceeded()
    {
        return time >= endTime;
    }
    public bool CheckDistanceToEnd(Vector3 currentPoint)
    {
        Vector3 finalEndPoint = endTransform != null ? endTransform.position : endPoint;
        return Vector3.Distance(currentPoint, finalEndPoint) < endDistanceThreshold;
    }
    public void EndComponent(MoveEndType endType, bool stopComponent)
    {
        foreach (var action in endActions)
        {
            action(endType);
        }
        if(stopComponent)
        {
            Stop();
        }
    }
}
//Freely moves towards direction
public class FreeMove : MoveComponent
{
    public FreeMove(MoveData data) : base(data) { }
    public override void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector)
    {
        base.Update(timeDelta, ref moveVector, currentVector);
        CheckEnd(currentVector);
    }
}
public class MoveToEnd : MoveComponent
{
    public MoveToEnd(MoveData data) : base(data)
    {
    }
    public override void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector)
    {
        if(paused)
        {
            return;
        }
        Vector3 finalEndPoint = endTransform != null ? endTransform.position : endPoint;
        Vector3 dv = Vector3.LerpUnclamped(Vector3.zero, finalEndPoint - currentVector, Mathf.Clamp(curve.Evaluate(time), -1f, 1f));
        moveVector += dv;
        time += Time.deltaTime;
        CheckEnd(currentVector);
    }
}
public class MoveBetweenOriginEnd : MoveComponent
{
    public MoveBetweenOriginEnd(MoveData data) : base(data)
    {
    }
    public override void Update(float timeDelta, ref Vector3 moveVector, Vector3 currentVector)
    {
        if(paused)
        {
            return;
        }
        float percentageAlong = Mathf.Clamp(curve.Evaluate(time), -1f, 1f);
        Vector3 finalOriginPoint = originTransform != null ? originTransform.position : originPoint;
        Vector3 finalEndPoint = endTransform != null ? endTransform.position : endPoint;

        float originToEndLength = Vector3.Distance(finalEndPoint, finalOriginPoint);
        float originMinPercentage = Mathf.InverseLerp(0, originToEndLength, originDistanceBounds.x);
        float originMaxPercentage = Mathf.InverseLerp(0, originToEndLength, originDistanceBounds.y);
        float endMinPercentage = 1 - Mathf.InverseLerp(0, originToEndLength, endDistanceBounds.x);
        float endMaxPercentage = 1 - Mathf.InverseLerp(0, originToEndLength, endDistanceBounds.y);

        //since this component doesn't use direction at all
        if (moveBoundsFlags.HasFlag(MoveBoundsFlags.OriginMax))
        {
            if(percentageAlong > originMaxPercentage)
            {
                percentageAlong = originMaxPercentage;
            }
        }
        if (moveBoundsFlags.HasFlag(MoveBoundsFlags.EndMin))
        {
            percentageAlong = Mathf.Clamp(percentageAlong, 0f, endMinPercentage);
        }
        if (moveBoundsFlags.HasFlag(MoveBoundsFlags.EndMax))
        {
            percentageAlong = Mathf.Clamp(percentageAlong, endMaxPercentage, 1f);
        }

        Vector3 targetPos = Vector3.LerpUnclamped(finalOriginPoint, finalEndPoint, percentageAlong);
        if(moveBoundsFlags.HasFlag(MoveBoundsFlags.OriginMin))
        {
            if(originToEndLength < originDistanceBounds.x)
            {
                return;
            }
        }

        moveVector += targetPos - currentVector;
        time += Time.deltaTime;
        CheckEnd(currentVector);
    }
}
public enum MoveType
{
    Free, MoveToEnd, MoveBetweenOriginEnd,
}
public enum MoveEndType
{
    NoEnd, EndTimeOrTargetReached, EndTimeExceeded, TargetReached
}
[Flags]
public enum MoveBoundsFlags
{
    None = 0,
    Everything = 1,
    OriginMin = 1 << 1,
    OriginMax = 1 << 2,
    EndMin = 1 << 3,
    EndMax = 1 << 4,
}