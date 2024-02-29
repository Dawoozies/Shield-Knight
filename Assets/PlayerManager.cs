using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Character player;
    float maxMoveSpeed;
    Vector2 moveInput;
    float jumpHeldTime;
    bool canJump;
    public MovementData gravity;
    public MovementData jumpAscent;
    void Start()
    {
        InputManager.RegisterMoveInputCallback(HandleMoveInput);
        InputManager.RegisterJumpInputCallback(HandleJumpInput);
        player.TryAddComponent(gravity);
        player.TryAddComponent(jumpAscent);

        jumpAscent.onCompletedActions.Add(() => gravity.Start(() => Time.fixedDeltaTime));
        gravity.Start(() => Time.fixedDeltaTime);
    }
    void FixedUpdate()
    {
        (bool, RaycastHit2D) groundCheckData = player.GroundedCheck();
        bool grounded = groundCheckData.Item1;
        if(jumpAscent.state != MovementData.State.InProgress && grounded && jumpHeldTime > 0)
        {
            Debug.Log("Run");
            jumpAscent.Start(() => Time.fixedDeltaTime);
        }
        jumpAscent.Update();
        gravity.Update();
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
