using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GroundCheck : MonoBehaviour
{
    BoxCollider2D characterBoxCollider;
    public Vector3 checkOffset;
    public Vector2 size;
    public float checkDistance;
    public LayerMask layerMask;
    RaycastHit2D hitResult;
    
    public List<Action> onGroundEnter = new();
    public List<Action> onGroundExit = new();
    public List<Action<Vector3, Collider2D>> onGroundEnterDetailed = new();
    bool lastGroundedValue;

    void Start()
    {
        //onGroundEnter.Add(() => Debug.Log("Ground Enter Event"));
        //onGroundExit.Add(() => Debug.Log("Ground Exit Event"));
        characterBoxCollider = GetComponent<BoxCollider2D>();
        checkOffset = new Vector3(0, characterBoxCollider.size.y/2f, 0);
        DoBoxCast();
        if(hitResult.collider != null)
        {
            foreach (var action in onGroundEnter)
            {
                action();
            }
            foreach (var action in onGroundEnterDetailed)
            {
                action(hitResult.centroid, hitResult.collider);
            }
            lastGroundedValue = true;
        }
        if(hitResult.collider == null)
        {
            foreach(var action in onGroundExit)
            {
                action();
            }
            lastGroundedValue = false;
        }
    }
    void Update()
    {
        DoBoxCast();
        DoGroundCheck();
    }
    void DoBoxCast()
    {
        //hitResult = Physics2D.BoxCast(transform.position + checkOffset, size, 0f, Vector2.down, checkDistance, layerMask);
        hitResult = Physics2D.BoxCast(transform.position+checkOffset, characterBoxCollider.size, 0f, Vector2.down, characterBoxCollider.size.y, layerMask);
    }
    void DoGroundCheck()
    {
        if (hitResult.collider != null)
        {
            if (!lastGroundedValue)
            {
                foreach (var action in onGroundEnter)
                {
                    action();
                }
                foreach (var action in onGroundEnterDetailed)
                {
                    action(hitResult.centroid, hitResult.collider);
                }
            }

            lastGroundedValue = true;
            return;
        }
        if (hitResult.collider == null)
        {
            if (lastGroundedValue)
            {
                foreach (var action in onGroundExit)
                {
                    action();
                }
            }

            lastGroundedValue = false;
            return;
        }
    }

    public Collider2D GetResultCollider()
    {
        return hitResult.collider;
    }
}
