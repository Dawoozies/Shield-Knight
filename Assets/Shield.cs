using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public Transform shieldParent;
    public BoxCollider2D boxCollider { get; set; }
    public SpriteRenderer shieldRenderer { get; set; }
    public float hitForceMultiplier;
    public float hitForce;
    public Color highForceColor;
    public Color defaultColor;
    public LayerMask layerMask;
    public Vector3 hitPoint;
    public Vector3 colliderPoint;

    float angle;
    float distance;
    RaycastHit2D[] hitResults;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        shieldRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    void Update()
    {
        float lerpValue = Mathf.InverseLerp(0f, hitForceMultiplier, hitForce);
        shieldRenderer.color = Color.Lerp(defaultColor, highForceColor, lerpValue);
    }
}
