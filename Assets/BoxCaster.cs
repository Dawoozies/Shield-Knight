using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCaster : MonoBehaviour
{
    BoxCollider2D boxCollider;
    public float angle;
    public float boxAngle;
    public Vector2 direction;
    public float distance;
    public LayerMask layerMask;
    Collider2D result;

    public Vector2 hitPoint;
    public Vector2 colliderPos;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        //transform.rotation = Quaternion.identity;

        //angle = Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg;

        //transform.Rotate(Vector3.forward, angle);
        angle = Vector2.Angle(Vector2.right, transform.right);

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size, angle, transform.right, distance, layerMask);
        result = hit.collider;
        if(result == null)
        {
            hitPoint = transform.position;
            colliderPos = transform.position;
        }
        else
        {
            hitPoint = hit.point;
            colliderPos = hit.collider.transform.position;
        }
    }
    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            return;
        }
        Gizmos.matrix = transform.localToWorldMatrix;

        if(result != null)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        float castLength = 0;
        int loopBreaker = 0;
        while(castLength < distance)
        {
            Vector2 pos = Vector2.zero + Vector2.right * castLength;
            Gizmos.DrawCube(pos, boxCollider.size);
            castLength += boxCollider.size.x;
            loopBreaker++;
            if (loopBreaker >= 1000)
                break;
        }

        Gizmos.matrix = Matrix4x4.identity;
        if (result != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, hitPoint);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, colliderPos);
        }
    }
}
