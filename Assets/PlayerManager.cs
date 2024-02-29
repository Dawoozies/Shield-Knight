using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Character player;
    public Camera mainCamera;
    public float cameraSmoothTime;
    Vector3 cameraVelocity;
    float maxMoveSpeed;
    Vector2 moveInput;
    float jumpHeldTime;
    bool grounded;
    public MovementData gravity;
    public MovementData jumpAscent;
    public MovementData run;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        InputManager.RegisterMoveInputCallback(HandleMoveInput);
        InputManager.RegisterJumpInputCallback(HandleJumpInput);
        player.TryAddComponent(gravity);
        player.TryAddComponent(jumpAscent);
        player.TryAddComponent(run);
        jumpAscent.onCompletedActions.Add(() => gravity.Start(() => Time.fixedDeltaTime));
        gravity.Start(() => Time.fixedDeltaTime);
        run.Start(() => Time.fixedDeltaTime);
    }
    void Update()
    {
        (bool, RaycastHit2D) groundCheckData = player.GroundedCheck();
        grounded = groundCheckData.Item1;
    }
    void FixedUpdate()
    {
        Vector3 cameraTarget = player.transform.position;
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraTarget.z = cameraPosition.z;
        Vector3 newCameraPosition = Vector3.SmoothDamp(
                cameraPosition,
                cameraTarget,
                ref cameraVelocity,
                cameraSmoothTime
            );
        mainCamera.transform.position = newCameraPosition;
        if (jumpAscent.state != MovementData.State.InProgress && grounded && jumpHeldTime > 0)
        {
            jumpAscent.Start(() => Time.fixedDeltaTime);
        }
        if(jumpAscent.state == MovementData.State.InProgress && jumpHeldTime == 0)
        {
            jumpAscent.ForceEnd();
        }

        jumpAscent.Update();
        gravity.Update();

        run.direction.x = moveInput.x;
        run.Update();
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
