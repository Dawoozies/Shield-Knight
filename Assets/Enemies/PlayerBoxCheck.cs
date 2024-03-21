using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerBoxCheck : MonoBehaviour, IPlayerCheck
{
    public Vector2 size;
    public float checkDistance;
    public LayerMask layerMask;
    const string playerTag = "Player";
    List<RaycastHit2D> hitResults = new();
    List<Action<List<RaycastHit2D>>> onHitActions = new();
    public void RegisterOnHitCallback(Action<List<RaycastHit2D>> action)
    {
        onHitActions.Add(action);
    }
    void Update()
    {
        DoBoxCasts();
    }
    void DoBoxCasts()
    {
        hitResults.Clear();
        RaycastHit2D leftHitResult = Physics2D.BoxCast(transform.position, size, 0f, Vector2.left, checkDistance, layerMask);
        RaycastHit2D rightHitResult = Physics2D.BoxCast(transform.position, size, 0f, Vector2.right, checkDistance, layerMask);
        if(leftHitResult.collider != null && leftHitResult.collider.tag == playerTag)
        {
            hitResults.Add(leftHitResult);
        }
        if(rightHitResult.collider != null && rightHitResult.collider.tag == playerTag)
        {
            hitResults.Add(rightHitResult);
        }
        foreach (var hit in hitResults)
        {
            if(hit.collider != null)
            {
                foreach (var action in onHitActions)
                {
                    action(hitResults);
                }
                break;
            }
        }
    }
}
