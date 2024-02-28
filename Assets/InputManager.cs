using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static List<Action<Vector2>> mouseInputActions = new();
    public static List<Action<Vector2>> moveInputActions = new();
    public static List<Action<float>> mouseLeftClickActions = new();
    float leftClickHeldTime = 0f;
    public static List<Action<float>> jumpInputActions = new();
    float jumpHeldTime = 0f;
    void Update()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        foreach (Action<Vector2> action in mouseInputActions)
        {
            action(mouseWorldPos);
        }
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        foreach (Action<Vector2> action in moveInputActions)
        {
            action(moveInput);
        }
        if(Input.GetMouseButton(0))
        {
            leftClickHeldTime += Time.deltaTime;
        }
        else
        {
            leftClickHeldTime = 0f;
        }
        foreach (Action<float> action in mouseLeftClickActions)
        {
            action(leftClickHeldTime);
        }
        bool jumpInput = Input.GetButton("Jump");
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
    }
    public static void RegisterMouseInputCallback(Action<Vector2> action)
    {
        mouseInputActions.Add(action);
    }
    public static void RegisterMoveInputCallback(Action<Vector2> action)
    {
        moveInputActions.Add(action);
    }
    public static void RegisterMouseLeftClickCallback (Action<float> action)
    {
        mouseLeftClickActions.Add(action);
    }
    public static void RegisterJumpInputCallback(Action<float> action)
    {
        jumpInputActions.Add(action);
    }
}
