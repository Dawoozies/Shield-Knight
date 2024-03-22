using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bashable : MonoBehaviour, IHitReceiver
{
    public float moveClamp;
    public Vector2 axisOfMove;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void ApplyForce(Vector2 force)
    {
        Vector2 projectedVector = (Vector2.Dot(force, axisOfMove)/axisOfMove.sqrMagnitude)*axisOfMove;
        Debug.Log("Box got hit");
        rb.velocity += projectedVector;
    }
}
