using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    public Shield shield;
    public Character player;
    public float distanceFromPlayer;
    Vector2 pointOnCircle;
    public LayerMask shieldNonIntersecting;
    void Start()
    {
        InputManager.RegisterMouseInputCallback(HandleMouseInput);
        shield.transform.position = player.transform.position;
    }
    void Update()
    {
        //Go to point on circle which intersects
        //Do raycast to push shield closer to player if pointing at walls
        shield.transform.position = player.transform.position + (Vector3)pointOnCircle;
        shield.transform.right = pointOnCircle;

        RaycastHit2D hit = Physics2D.BoxCast(player.transform.position, shield.boxCollider.size, Vector2.SignedAngle(Vector2.right, shield.transform.right), (shield.transform.position - player.transform.position).normalized, distanceFromPlayer + shield.boxCollider.size.x + 0.02f, shieldNonIntersecting);
        Vector3 shieldIntersectOffset = Vector3.zero;
        if(hit.collider != null)
        {
            shieldIntersectOffset = - hit.distance * shield.transform.right;
        }
        shield.transform.position += shieldIntersectOffset;
    }
    void HandleMouseInput(Vector2 mouseWorldPos)
    {
        pointOnCircle = ((Vector3)mouseWorldPos - player.transform.position).normalized;
    }
}
