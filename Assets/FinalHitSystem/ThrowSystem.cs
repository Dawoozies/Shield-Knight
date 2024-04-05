using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSystem : MonoBehaviour
{
    public float velocity;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    public bool fireTest;
    public List<Collider2D> hitColliders = new();
    public List<float> hitTimes = new();
    private int hitIndex;
    private float hitTimeEnd;
    private Vector3 hitPointStart;
    private Vector3 hitPointEnd;
    private float hitLerpTime;
    public float throwDistance;
    public LayerMask hitLayers;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if (fireTest)
        {
            hitColliders.Clear();
            hitTimes.Clear();
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                    rb.transform.position,
                    boxCollider.size,
                    Vector2.Angle(rb.transform.right, Vector2.right),
                    rb.transform.right,
                    throwDistance,
                    hitLayers
                );
            foreach (var hit in hits)
            {
                hitColliders.Add(hit.collider);
                hitTimes.Add(hit.distance/velocity);
            }
            if (hitTimes.Count > 0)
            {
                hitPointStart = rb.transform.position;
                hitTimeEnd = hitTimes[hits.Length-1];
                hitPointEnd = hits[hits.Length-1].centroid;
                hitLerpTime = 0;
                hitIndex = 0;
            }
            fireTest = false;
        }
    }
    private void FixedUpdate()
    {
        if (hitLerpTime < hitTimeEnd)
        {
            hitLerpTime += Time.fixedDeltaTime;
            Vector3 nextPos = Vector3.Lerp(hitPointStart, hitPointEnd, hitLerpTime/hitTimeEnd);
            rb.MovePosition(nextPos);
            if (hitTimes.Count > 0 && hitIndex < hitTimes.Count - 1)
            {
                //if there are hits at all
                //if last exceeded index
                if (hitLerpTime > hitTimes[hitIndex])
                {
                    int loopBreaker = 0;
                    while (hitLerpTime > hitTimes[hitIndex])
                    {
                        loopBreaker++;
                        //trigger event for hitColliders[hitIndex]
                        //Debug.LogError($"hitLerpTime {hitLerpTime} > hitTimes[hitIndex] {hitTimes[hitIndex]}");
                        hitIndex++;
                        if (hitLerpTime < hitTimes[hitIndex])
                        {
                            break;
                        }
                        if (loopBreaker > 100)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
