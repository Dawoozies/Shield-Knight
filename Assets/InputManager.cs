using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static List<Action<Vector2>> mouseInputActions = new();
    public static List<Action<Vector2>> moveInputActions = new();
    public static List<Action<Vector3>> mouseClickActions = new();
    Vector3 mouseClickHeldTime = Vector3.zero;
    public static List<Action<float>> jumpInputActions = new();
    float jumpHeldTime = 0f;
    public static List<Action<Vector3Int>> mouseDownActions = new();
    public static List<Action> jumpDownActions = new();
    public static List<Action> jumpUpActions = new();
    void Update()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        foreach (Action<Vector2> action in mouseInputActions)
        {
            action(mouseWorldPos);
        }
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        foreach (Action<Vector2> action in moveInputActions)
        {
            action(moveInput);
        }
        if(Input.GetMouseButton(0))
        {
            mouseClickHeldTime.x += Time.deltaTime;
        }
        else
        {
            mouseClickHeldTime.x = 0f;
        }
        if(Input.GetMouseButton(1))
        {
            mouseClickHeldTime.y += Time.deltaTime;
        }
        else
        {
            mouseClickHeldTime.y = 0f;
        }
        if(Input.GetMouseButton(2))
        {
            mouseClickHeldTime.z += Time.deltaTime;
        }
        else
        {
            mouseClickHeldTime.z = 0f;
        }
        foreach (Action<Vector3> action in mouseClickActions)
        {
            action(mouseClickHeldTime);
        }
        Vector3Int mouseDownInput = Vector3Int.zero;
        if(Input.GetMouseButtonDown(0))
        {
            mouseDownInput.x = 1;
        }
        else
        {
            mouseDownInput.x = 0;
        }
        if (Input.GetMouseButtonDown(1))
        {
            mouseDownInput.y = 1;
        }
        else
        {
            mouseDownInput.y = 0;
        }
        if (Input.GetMouseButtonDown(2))
        {
            mouseDownInput.z = 1;
        }
        else
        {
            mouseDownInput.z = 0;
        }
        foreach (Action<Vector3Int> action in mouseDownActions)
        {
            action(mouseDownInput);
        }
        bool jumpInput = Input.GetButton("Jump");
        bool jumpDownInput = Input.GetButtonDown("Jump");
        bool jumpUpInput = Input.GetButtonUp("Jump");
        if (jumpInput)
        {
            jumpHeldTime += Time.deltaTime;
        }
        else
        {
            jumpHeldTime = 0f;
        }
        foreach (Action<float> action in jumpInputActions)
        {
            action(jumpHeldTime);
        }
        if(jumpDownInput)
        {
            foreach (Action action in jumpDownActions)
            {
                action();
            }
        }
        if(jumpUpInput)
        {
            foreach (Action action in jumpUpActions)
            {
                action();
            }
        }
    }
    public static void RegisterMouseInputCallback(Action<Vector2> action)
    {
        mouseInputActions.Add(action);
    }
    public static void RegisterMoveInputCallback(Action<Vector2> action)
    {
        moveInputActions.Add(action);
    }
    public static void RegisterMouseClickCallback (Action<Vector3> action)
    {
        mouseClickActions.Add(action);
    }
    public static void RegisterJumpInputCallback(Action<float> action)
    {
        jumpInputActions.Add(action);
    }
    public static void RegisterMouseDownCallback(Action<Vector3Int> action)
    {
        mouseDownActions.Add(action);
    }
    public static void RegisterJumpDownInputCallback(Action action)
    {
        jumpDownActions.Add(action);
    }
    public static void RegisterJumpUpInputCallback(Action action)
    {
        jumpUpActions.Add(action);
    }
}
