using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public abstract class ShieldSystem : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected BoxCollider2D col;
    protected List<IOnHit> onHitInterfaces = new();
    public List<Collider2D> hitColliders = new();
    List<RaycastHit2D> validHits = new();
    protected List<float> hitTimes = new();
    protected int hitIndex;
    protected float hitTimeEnd;
    protected Vector3 hitPointStart;
    protected Vector3 hitPointEnd;
    protected float hitLerpTime;
    public ShieldSystemType shieldSystemType;
    public float systemVelocity;
    public float systemCastDistance;
    public LayerMask validCastLayers;
    public List<string> earlyStopColliderTags = new();

    public Action<Vector3, Collider2D> earlyStopCallback;
    [Serializable]
    public class DebugHit
    {
        public Vector2 playerPos;
        public Vector2 playerDown;
        public Vector2 playerRight;
        public Collider2D hitCollider;
        public Vector2 hitPoint;
        public Vector2 hitCentroid;
        public Vector2 hitNormal;
        public Vector2 shieldPos;
        public Vector2 shieldRight;

        public BoxCollider2D playerBox;
        public Vector2 BLCorner => playerPos + Vector2.right*(-playerBox.size.x/2f) + Vector2.up*(-playerBox.size.y/2f);
        public Vector2 BRCorner => playerPos + Vector2.right * (playerBox.size.x / 2f) + Vector2.up * (-playerBox.size.y / 2f);
        public Vector2 TLCorner => playerPos + Vector2.right * (-playerBox.size.x / 2f) + Vector2.up * (playerBox.size.y/2f);
        public Vector2 TRCorner => playerPos + Vector2.right * (playerBox.size.x / 2f) + Vector2.up * (playerBox.size.y / 2f);

        public DebugHit(RaycastHit2D hit, Rigidbody2D shield, Player player)
        {
            this.playerPos = player.transform.position;
            playerDown = -player.transform.up;
            playerRight = player.transform.right;
            hitCollider = hit.collider;
            hitPoint = hit.point;
            hitCentroid = hit.centroid;
            hitNormal = hit.normal;
            shieldPos = shield.position;
            shieldRight = shield.transform.right;

            playerBox = player.GetComponent<BoxCollider2D>();
        }
    }
    public class CastInfoDebug
    {
        public Vector2 origin;
        public Vector2 size;
        public float angle;
        public Vector2 direction;
        public float distance = Mathf.Infinity;
        public CastInfoDebug(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
        {
            this.origin = origin;
            this.size = size;
            this.angle = angle;
            this.direction = direction;
            this.distance = distance;
        }
    }
    public CastInfoDebug castInfo;
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        if(castInfo != null)
        {
            BoxCast2DGizmo.BoxCast(castInfo);
        }
        if (hitResultsDebug.Count > 0)
        {
            DebugHit debugHit = hitResultsDebug[0];
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(debugHit.hitPoint, 0.05f);
            Gizmos.color = Color.yellow;
            Vector3[] playerBoundingBox = new Vector3[]{
            debugHit.BLCorner, debugHit.BRCorner,
            debugHit.BRCorner, debugHit.TRCorner,
            debugHit.TRCorner, debugHit.TLCorner,
            debugHit.TLCorner, debugHit.BLCorner
            };
            Gizmos.DrawLineList(playerBoundingBox);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(debugHit.hitCentroid, 0.05f);
        }

    }
    public List<DebugHit> hitResultsDebug = new();
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }
    //Cast velocity is how "fast" the shield would be moving through the cast zone
    public virtual void SystemCast()
    {
        if(systemVelocity <= 0)
        {
            Debug.Log("systemVelocity <= 0");
            return;
        }
        onHitInterfaces.Clear();
        hitTimes.Clear();
        hitColliders.Clear();
        validHits.Clear();
        Vector2 origin = rb.transform.position;
        Vector2 size = col.size;
        float angle = Vector2.Angle(Vector2.right, rb.transform.position);
        Vector2 direction = rb.transform.right;
        float distance = systemCastDistance;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin,size,angle,direction,distance,validCastLayers);
        castInfo = new CastInfoDebug(origin, size, angle, direction, distance);
        hitResultsDebug.Clear();
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.fraction > 0)
            {
                validHits.Add(hit);
            }
            else
            {
                hitResultsDebug.Add(new(hit, rb, GameManager.GetActivePlayer()));
                Debug.LogError("Adding hit debug results");
            }
        }
        foreach (RaycastHit2D validHit in validHits)
        {
            onHitInterfaces.Add(validHit.collider.GetComponent<IOnHit>());
            hitTimes.Add(validHit.distance / systemVelocity);
            hitColliders.Add(validHit.collider);
        }
        if(validHits.Count > 0)
        {
            hitPointStart = rb.transform.position;
            hitPointEnd = validHits[validHits.Count - 1].centroid;
            hitTimeEnd = hitTimes[validHits.Count - 1];
            hitLerpTime = 0;
            hitIndex = 0;
        }
    }
    protected virtual void FixedUpdate() 
    {
        if(hitLerpTime < hitTimeEnd)
        {
            hitLerpTime += Time.fixedUnscaledDeltaTime;
            Vector3 nextPos = Vector3.Lerp(hitPointStart, hitPointEnd, hitLerpTime / hitTimeEnd);
            rb.MovePosition(nextPos);
            if (hitTimes.Count > 0 && hitIndex < hitTimes.Count - 1)
            {
                if(hitLerpTime > hitTimes[hitIndex])
                {
                    int loopGuard = 0;
                    while( hitLerpTime > hitTimes[hitIndex] )
                    {
                        loopGuard++;

                        IOnHit onHit = onHitInterfaces[hitIndex];
                        onHit?.Hit(shieldSystemType, rb.transform.right, systemVelocity);

                        Collider2D hitCollider = hitColliders[hitIndex];
                        if (earlyStopColliderTags.Contains(hitCollider.tag))
                        {
                            Vector3 centroid = Vector3.Lerp(hitPointStart, hitPointEnd, hitTimes[hitIndex]/hitTimeEnd);
                            earlyStopCallback?.Invoke(centroid, hitCollider);

                            hitLerpTime = hitTimeEnd;
                            return;
                        }

                        hitIndex++;
                        if(hitIndex >= hitTimes.Count || hitLerpTime < hitTimes[hitIndex] || loopGuard > 100)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
    public virtual bool SystemComplete()
    {
        if(hitLerpTime >= hitTimeEnd)
        {
            return true;
        }
        return false;
    }

}
public enum ShieldSystemType
{
    Held,
    Throw,
    Recall
}