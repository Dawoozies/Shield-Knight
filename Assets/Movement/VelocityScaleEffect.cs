using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityScaleEffect : MonoBehaviour
{
    VelocitySystem velocitySystem;
    GroundCheck groundCheck;
    SpriteRenderer spriteRenderer;
    Transform graphic;
    void Start()
    {
        velocitySystem = GetComponent<VelocitySystem>();
        groundCheck = GetComponent<GroundCheck>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        graphic = spriteRenderer.transform;
    }
}
