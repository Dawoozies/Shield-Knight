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
    }
    void HandleMouseInput(Vector2 mouseWorldPos)
    {
        pointOnCircle = distanceFromPlayer*((Vector3)mouseWorldPos - player.transform.position).normalized;
    }
}
