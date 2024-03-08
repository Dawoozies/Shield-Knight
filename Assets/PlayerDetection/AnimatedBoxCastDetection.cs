using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBoxCastDetection : BoxCastDetection
{
    public float nextCastTime;
    float timer;
    int castIndex;
    public List<BoxCastData> castData = new();
    protected override void Start()
    {
        base.Start();
        boxCastData = castData[0];
    }
    protected override void Update()
    {
        BoxCastData current = castData[castIndex];
        boxCastData = current;

        castTransform.right = current.castDirection;
        current.castAngle = Vector2.Angle(Vector2.right, castTransform.right);
        hits = Physics2D.BoxCastAll(transform.position, current.size, current.castAngle, castTransform.right, current.castDistance, current.castLayerMask);

        if(timer < nextCastTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            castIndex++;
            castIndex %= castData.Count;
        }
    }
}
