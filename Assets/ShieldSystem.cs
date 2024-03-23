using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSystem : MonoBehaviour
{
    public Transform player;
    public float holdDistance;
    Vector2 mousePos;
    public Vector2 playerToMouse;
    void Start()
    {
        InputManager.RegisterMouseInputCallback((Vector2 mousePos) => this.mousePos = mousePos);
    }
    void Update()
    {
        if (player == null)
            return;

        playerToMouse = mousePos - (Vector2)player.transform.position;
    }
}
