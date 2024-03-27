using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwap : MonoBehaviour
{
    public enum SwapState
    {
        Default, Mono
    }
    public SwapState activeState;
    public Material[] materials;
    SpriteRenderer[] spriteRenderers;
    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }
    void Update()
    {
        foreach (var renderer in spriteRenderers)
        {
            renderer.material = materials[(int)activeState];
        }
    }
    public void SwapMaterial(SwapState state)
    {
        activeState = state;
    }
}
