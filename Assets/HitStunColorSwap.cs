using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStunColorSwap : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color defaultColor;
    float t;
    public Color swapColor;
    public float returnToNormalSpeed;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }
    void Update()
    {
        spriteRenderer.color = Color.Lerp(defaultColor, swapColor, t);
        if(t > 0)
        {
            t -= Time.unscaledDeltaTime * returnToNormalSpeed;
        }
    }
    public void DoSwap()
    {
        t = 1;
    }
}
