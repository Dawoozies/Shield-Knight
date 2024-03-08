using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityAnimation : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new();
    Rigidbody2D rb;
    int sprite;
    float time;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();
    }
    void Update()
    {
        if(rb.velocity.magnitude > 0)
        {
            time += Time.deltaTime*rb.velocity.magnitude;
            if(time > 1)
            {
                time = 0;
                sprite++;
                sprite %= sprites.Count;
            }
        }
        spriteRenderer.sprite = sprites[sprite];
    }
}
