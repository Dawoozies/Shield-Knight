using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererClearOnStart : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    MeshRenderer meshRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.color = Color.clear;
        }
        meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }
}
