using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Character player;
    float maxMoveSpeed;
    Vector2 moveInput;
    float jumpHeldTime;
    void Start()
    {
        InputManager.RegisterMoveInputCallback(HandleMoveInput);
        InputManager.RegisterJumpInputCallback(HandleJumpInput);
    }
    void FixedUpdate()
    {
        (bool, RaycastHit2D) groundCheckData = player.GroundedCheck();
        if(groundCheckData.Item1)
        {

        }
    }
    void HandleMoveInput(Vector2 moveInput)
    {
        this.moveInput = moveInput;
    }
    void HandleJumpInput(float jumpHeldTime)
    {
        this.jumpHeldTime = jumpHeldTime;
    }
}
