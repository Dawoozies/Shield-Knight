using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMaterialDepth : MonoBehaviour
{
    public Color albedo;
    private MeshRenderer[] meshRenderers;
    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetColor("_Color", albedo);
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
