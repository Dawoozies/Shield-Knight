using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    BoxCollider2D boundingBoxCollider;
    public LayerMask groundingLayers;
    public float groundedCheckDistance;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boundingBoxCollider = GetComponent<BoxCollider2D>();
    }
    public (bool, RaycastHit2D) GroundedCheck()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boundingBoxCollider.size, 0f, Vector2.down, groundedCheckDistance);
        return (hit.collider == true, hit);
    }
}