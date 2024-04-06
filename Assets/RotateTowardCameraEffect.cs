using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardCameraEffect : MonoBehaviour
{
    public float zDepth;
    public float effectRadius;
    Quaternion defaultRotation;
    Quaternion xRotMin;
    Quaternion xRotMax;
    public Vector2 xRotBounds;
    void Start()
    {
        defaultRotation = transform.localRotation;
        xRotMin = Quaternion.Euler(new Vector3(90f,0f,xRotBounds.x));
        xRotMax = Quaternion.Euler(new Vector3(90f, 0f, xRotBounds.y));
        InputManager.RegisterMouseInputCallback(MouseInputHandler);
    }
    void MouseInputHandler(Vector2 mousePos)
    {
        Vector2 toMouse = (mousePos - (Vector2)transform.position).normalized;
        Quaternion finalRot = defaultRotation;
        if (toMouse.magnitude < effectRadius)
        {
            float xLerpParameter = Mathf.InverseLerp(-effectRadius, effectRadius, toMouse.x);
            finalRot = Quaternion.Slerp(xRotMin, xRotMax, xLerpParameter);
        }
        transform.localRotation = finalRot;
    }
}
