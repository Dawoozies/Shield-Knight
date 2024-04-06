using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingHardSurface : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public Rigidbody2D hardSurface;
    [Range(0.1f, 20f)] public float velocity;
    public bool pointToVelocity;
    Vector2 startPos => start.position;
    Vector2 endPos => end.position;
    float lerpEndTime => Vector2.Distance(startPos, endPos)/velocity;
    [Range(0f,1f)] public float startTime = 0;
    float lerpTime = 0;
    bool flipDirection = false;
    public AnimationCurve lerpCurve;
    bool shieldMovingInAir;
    private void Start()
    {
        lerpTime = lerpEndTime*startTime;
        ShieldManager.RegisterOnShieldMovingInAirCallback(() => shieldMovingInAir = true);
        ShieldManager.RegisterOnShieldMovingInAirCompleteCallback(() => shieldMovingInAir = false);
    }
    private void FixedUpdate()
    {
        if(shieldMovingInAir)
        {
            return;
        }
        if(flipDirection)
        {
            lerpTime -= Time.fixedDeltaTime;
        }
        else
        {
            lerpTime += Time.fixedDeltaTime;
        }

        Vector2 pos = Vector2.Lerp(start.position, end.position, lerpCurve.Evaluate(lerpTime/lerpEndTime));
        if(pointToVelocity)
        {
            hardSurface.transform.right = pos - hardSurface.position;
        }
        hardSurface.MovePosition(pos);

        if(flipDirection)
        {
            if(lerpTime < 0)
            {
                flipDirection = false;
            }
        }

        if(!flipDirection)
        {
            if(lerpTime > lerpEndTime)
            {
                flipDirection = true;
            }
        }
    }
}
