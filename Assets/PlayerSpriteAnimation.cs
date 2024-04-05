using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] run;
    public float runAnimSpeed;
    private VelocitySystem velocitySystem;
    private float frame;
    private GroundCheck groundCheck;
    private bool grounded;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = run[0];
        velocitySystem = GetComponentInParent<VelocitySystem>();
        groundCheck = GetComponentInParent<GroundCheck>();
        groundCheck.onGroundEnter.Add(() => grounded = true);
        groundCheck.onGroundExit.Add(() => grounded = false);
    }
    void Update()
    {
        float xVelocity = velocitySystem.finalVelocity.x;
        float absXVelocity = Mathf.Abs(xVelocity);
        if (spriteRenderer.flipX)
        {
            if (xVelocity > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            if (xVelocity < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
        if (absXVelocity > 0 && grounded)
        {
            frame += Time.deltaTime * runAnimSpeed * Mathf.Clamp(absXVelocity, 0, runAnimSpeed);
        }
        frame %= run.Length;
        spriteRenderer.sprite = run[Mathf.FloorToInt(frame)];
    }
}
