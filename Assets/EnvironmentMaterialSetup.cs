using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentMaterialSetup : MonoBehaviour
{
    private Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
        foreach (var material in rend.materials)
        {
            material.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.y);
        }
    }
}
