using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldThrownInteractable : MonoBehaviour, IEmbeddable
{
    public float moveClamp;
    public Vector2 axisOfMove;
    Rigidbody2D rb;
    public bool noEmbeddingForce;
    public bool noRecallForce;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void ApplyForce(Vector2 force)
    {
        Vector2 projectedVector = (Vector2.Dot(force, axisOfMove) / axisOfMove.sqrMagnitude) * axisOfMove;
        Debug.Log("Box got recalled");
        rb.velocity += projectedVector;
    }

    public void TryEmbed(ShieldThrow shieldThrow, Vector3 embeddingVelocity)
    {
        if(noEmbeddingForce)
        {
            return;
        }
        ApplyForce(embeddingVelocity);
    }
    public void TryRemoveEmbed(ShieldThrow shieldThrow, Vector3 recallVelocity)
    {
        if(noRecallForce)
        {
            return;
        }
        ApplyForce(recallVelocity);
    }
}