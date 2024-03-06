using System.Collections;
using System.Collections.Generic;
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
    public LayerMask enemyLayers;
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
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag != "Enemy")
        {
            return;
        }
        //Raycast from shield parent
        //raycast out from local zero forward a distance
        Vector2 raycastOrigin = shieldParent.transform.position;
        Vector2 raycastEnd = col.transform.position + col.bounds.max;
        float dist = Vector2.Distance(raycastOrigin, raycastEnd);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, shieldParent.transform.right, dist, enemyLayers);
        Vector2 force = (shieldParent.right + shieldParent.up) * hitForce;
        if (hit.collider != null)
        {
            Debug.LogError("Hit");
            var enemy = hit.collider.GetComponent<Enemy>();
            enemy.ApplyHit(force, 3f);
        }
    }
}
