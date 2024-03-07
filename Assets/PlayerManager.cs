using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Character player;
    public Camera mainCamera;
    public float cameraSmoothTime;
    public Vector2 cameraOffset;
    Vector3 cameraVelocity;
    float maxMoveSpeed;
    Vector2 moveInput;
    float jumpHeldTime;
    bool grounded;
    float leftClickHeldTime, rightClickHeldTime, middleClickHeldTime;
    Vector2 mouseWorldPos;
    public MovementData gravity;
    public MovementData jumpAscent;
    public MovementData run;
    public MovementData airDash;
    public int airDashesLeft;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        InputManager.RegisterMoveInputCallback(HandleMoveInput);
        InputManager.RegisterJumpInputCallback(HandleJumpInput);
        InputManager.RegisterMouseClickCallback(HandleMouseClick);
        InputManager.RegisterMouseInputCallback(HandleMouseInput);
        InputManager.RegisterMouseDownCallback(HandleMouseDown);
        player.TryAddComponent(gravity);
        player.TryAddComponent(jumpAscent);
        player.TryAddComponent(run);
        player.TryAddComponent(airDash);
        jumpAscent.onCompletedActions.Add(() => gravity.Start(() => Time.fixedDeltaTime));
        gravity.Start(() => Time.fixedDeltaTime);
        run.Start(() => Time.fixedDeltaTime);
    }
    void Update()
    {
        (bool, RaycastHit2D) groundCheckData = player.GroundedCheck();
        grounded = groundCheckData.Item1;

        if(grounded && airDashesLeft != 1)
        {
            airDashesLeft = 1;
        }
    }
    void FixedUpdate()
    {
        Vector3 cameraTarget = player.transform.position + (Vector3)cameraOffset;
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

        if(airDash.state == MovementData.State.InProgress)
        {
            airDash.Update();
        }
        else
        {
            airDash.direction = Vector2.zero;
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
    void HandleMouseClick(Vector3 mouseHeldTimes)
    {
        leftClickHeldTime = mouseHeldTimes.x;
        rightClickHeldTime = mouseHeldTimes.y;
        middleClickHeldTime = mouseHeldTimes.z;
    }
    void HandleMouseInput(Vector2 mouseWorldPos)
    {
        this.mouseWorldPos = mouseWorldPos;
    }
    void HandleMouseDown(Vector3Int mouseDownInput)
    {
        bool leftClickDown = mouseDownInput.x > 0;
        bool rightClickDown = mouseDownInput.y > 0;
        bool middleClickDown = mouseDownInput.z > 0;

        if(rightClickDown && airDashesLeft > 0)
        {
            airDash.direction = ((Vector3)mouseWorldPos - player.transform.position).normalized;
            airDash.Start(() => Time.fixedDeltaTime);
            airDashesLeft--;
        }
    }
}
