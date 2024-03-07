using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCast : MonoBehaviour
{
    BoxCollider2D boxCollider;
    public float angle;
    public Vector2 direction;
    public float distance;
    public LayerMask layerMask;
    RaycastHit2D result;

    public Vector2 hitPoint;
    public Vector2 colliderPos;
    public Vector2 sizeClamp;
    public LineRenderer lineRenderer;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = boxCollider.size.y;
        lineRenderer.endWidth = boxCollider.size.y;
    }
    void Update()
    {
        //transform.rotation = Quaternion.identity;

        //angle = Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg;

        //transform.Rotate(Vector3.forward, angle);
        angle = Vector2.Angle(Vector2.right, transform.right);
        result = Physics2D.BoxCast(transform.position, boxCollider.size, angle, transform.right, distance, layerMask);
        if (result.collider == null)
        {
            hitPoint = transform.position;
            colliderPos = transform.position;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.right*distance);
        }
        else
        {
            hitPoint = result.point;
            colliderPos = result.collider.transform.position;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.right * (result.distance + boxCollider.size.x));
        }
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Gizmos.matrix = transform.localToWorldMatrix;

        float hitDistance = distance; 
        if (result.collider != null)
        {
            Gizmos.color = Color.green;
            hitDistance = result.distance;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        float castLength = 0;
        int loopBreaker = 0;
        while (castLength < hitDistance)
        {
            Vector2 pos = Vector2.zero + Vector2.right * castLength;
            Gizmos.DrawCube(pos, boxCollider.size);
            castLength += boxCollider.size.x;
            loopBreaker++;
            if (loopBreaker >= 1000)
                break;
        }

        Gizmos.matrix = Matrix4x4.identity;
        if (result.collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, hitPoint);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, colliderPos);
        }
    }
}
