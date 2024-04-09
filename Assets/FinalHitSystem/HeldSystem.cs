using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HeldSystem : ShieldSystem
{
    public bool attacking;
    public bool charging;
    float charge;
    public Vector2 attackingStoppedPosition;
    float defaultSystemCastDistance;
    float defaultSystemVelocity;
    protected override void Start()
    {
        base.Start();
        defaultSystemCastDistance = systemCastDistance;
        defaultSystemVelocity = systemVelocity;
    }
    public void Charge(float charge)
    {
        charging = true;
        this.charge = charge;
    }
    public void StopCharge()
    {
        charging = false;
        charge = 0;
    }
    public void InitiateAttack()
    {
        charging = false;
        attacking = true;
        systemCastDistance = defaultSystemCastDistance * Mathf.Clamp01(charge);
        base.SystemCast();
        if(validHits.Count == 0)
        {
            Debug.LogError("NO HITS");
            hitPointStart = rb.transform.position;
            hitPointEnd = rb.transform.position + rb.transform.right*systemCastDistance;
            hitTimeEnd = systemCastDistance / systemVelocity;
            hitLerpTime = 0;
        }
        if(validHits.Count > 0)
        {
            Vector2 defaultEndPoint = rb.transform.position + rb.transform.right * systemCastDistance;
            float defaultTimeEnd = systemCastDistance / systemVelocity;
            bool earlyHitStop = false;
            foreach (RaycastHit2D hit in validHits)
            {
                if(earlyStopColliderTags.Contains(hit.collider.tag))
                {
                    earlyHitStop = true;
                }
            }
            if(!earlyHitStop)
            {
                hitPointEnd = defaultEndPoint;
                hitTimeEnd = defaultTimeEnd;
            }
        }
    }
    protected override void LateSystemUpdate()
    {
        if (hitLerpTime >= hitTimeEnd)
        {
            attackingStoppedPosition = hitPointEnd;
            foreach (var onHit in onHitInterfaces)
            {
                onHit?.Hit(shieldSystemType, rb.transform.right, systemVelocity);
            }
            hitEndCallback?.Invoke(hitEndNormal, hitPointEnd, Vector2.Distance(hitPointStart, hitPointEnd)/defaultSystemCastDistance);
            Debug.Log("_heldSystem attack just completed without stopping early");
        }
    }
}
public enum FinalHeldHit
{
    NoHit, HardSurface, SoftSurface
}