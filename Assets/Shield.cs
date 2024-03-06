using System.Collections;
using System.Collections.Generic;
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
    public LayerMask enemyLayers;
    public Vector3 hitPoint;
    public Vector3 colliderPoint;
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
        RaycastHit2D hit = OldRaycast(col);
        if(hit.collider != null)
        {
            hitPoint = hit.point;
            colliderPoint = hit.collider.transform.position;
            //Vector2 force = (dir + Vector2.Perpendicular(dir)*0.5f)*hitForce;
            var enemy = col.GetComponent<Enemy>();
            //enemy.ApplyHit(force);
        }
    }
    RaycastHit2D OldRaycast(Collider2D col)
    {
        Vector2 dir = col.ClosestPoint((Vector2)shieldParent.position) - (Vector2)shieldParent.position;
        return Physics2D.BoxCast(shieldParent.position, boxCollider.size, Vector2.SignedAngle(Vector2.right, transform.right), dir, 100f, enemyLayers);
    }
    RaycastHit2D NewRaycast(Collider2D col)
    {
        return OldRaycast(col);
    }
    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(shieldParent.position, hitPoint);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(shieldParent.position, colliderPoint);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up);

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, boxCollider.size);
    }
}
