using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HeldSystem : ShieldSystem
{
    public FinalHeldHit finalHeldHit;
    float chargeAmount;
    public AnimationCurve noHitCurve;
    public Vector2 size => col.size;
    public Vector2 pos => rb.position;
    public Vector2 right => rb.transform.right;
    public Vector2 up => rb.transform.up;
    public override void SystemCast()
    {
        onHitInterfaces.Clear();
        hitTimes.Clear();
        hitColliders.Clear();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            rb.transform.position,
            col.size,
            Vector2.Angle(rb.transform.right, Vector2.right),
            rb.transform.right,
            systemCastDistance,
            validCastLayers
        );
        Vector3 startPoint = rb.transform.position;
        Vector3 endPoint = rb.transform.position + rb.transform.right * (systemCastDistance*chargeAmount);
        finalHeldHit = FinalHeldHit.NoHit;
        foreach (var hit in hits)
        {
            finalHeldHit = FinalHeldHit.SoftSurface;
            onHitInterfaces.Add(hit.collider.GetComponent<IOnHit>());
            hitTimes.Add(hit.distance/systemVelocity);
            hitColliders.Add(hit.collider);
            endPoint = hit.centroid;
            if (earlyStopColliderTags.Contains(hit.collider.tag))
            {
                if(hit.distance < 0.5f)
                {
                    continue;
                }
                finalHeldHit = FinalHeldHit.HardSurface;
                break;
            }
        }
        hitPointStart = startPoint;
        hitPointEnd = endPoint;
        hitTimeEnd = Vector2.Distance(startPoint, endPoint) / systemVelocity;
        hitLerpTime = 0;
        hitIndex = 0;
    }
    protected override void FixedUpdate()
    {
        if(hitLerpTime < hitTimeEnd)
        {
            hitLerpTime += Time.fixedUnscaledDeltaTime;
            Vector3 nextPos = Vector3.Lerp(hitPointStart, hitPointEnd, hitLerpTime/hitTimeEnd);
            rb.MovePosition(nextPos);
            if (hitTimes.Count > 0 && hitIndex < hitTimes.Count - 1)
            {
                if (hitLerpTime > hitTimes[hitIndex])
                {
                    int loopGuard = 0;
                    while (hitLerpTime > hitTimes[hitIndex])
                    {
                        loopGuard++;

                        IOnHit onHit = onHitInterfaces[hitIndex];
                        onHit?.Hit(shieldSystemType, rb.transform.right, systemVelocity);

                        Collider2D hitCollider = hitColliders[hitIndex];
                        if (earlyStopColliderTags.Contains(hitCollider.tag))
                        {
                            Vector3 centroid = Vector3.Lerp(hitPointStart, hitPointEnd, hitTimes[hitIndex] / hitTimeEnd);
                            earlyStopCallback?.Invoke(centroid, hitCollider);

                            hitLerpTime = hitTimeEnd;
                            return;
                        }

                        hitIndex++;
                        if (hitIndex >= hitTimes.Count || hitLerpTime < hitTimes[hitIndex] || loopGuard > 100)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
    public FinalHeldHit HeldAttack(float chargeAmount)
    {
        this.chargeAmount = chargeAmount;
        SystemCast();
        return finalHeldHit;
    }
}
//Soft surface = penetrable
public enum FinalHeldHit
{
    NoHit, HardSurface, SoftSurface
}