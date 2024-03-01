using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTrailingEffect : MonoBehaviour
{
    public int trailLength;
    public float trailTime;
    float trailTimer;
    GameObject trailObject;
    SpriteRenderer spriteRenderer;
    Transform trailParent;
    List<SpriteRenderer> trailRenderers = new();
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailObject = new GameObject("TrailObject", typeof(SpriteRenderer));
        SpriteRenderer trailObjectSpriteRenderer = trailObject.GetComponent<SpriteRenderer>();
        trailObjectSpriteRenderer.color = Color.clear;

        trailParent = new GameObject().transform;
        for (int i = 0; i < trailLength; i++)
        {
            GameObject trailObjectClone = Instantiate(trailObject, trailParent);
            SpriteRenderer trailRenderer = trailObjectClone.GetComponent<SpriteRenderer>();
            trailRenderers.Add(trailRenderer);
        }
    }
    void Update()
    {

    }
}
