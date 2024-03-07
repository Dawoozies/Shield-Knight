using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererClearOnStart : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.clear;
    }
}
