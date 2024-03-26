using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour, Manager
{
    Player player;
    public Camera mainCamera;
    public float cameraSmoothTime;
    public Vector3 cameraOffset;
    public Vector2 xAxisClamp;
    public Vector2 yAxisClamp;
    Vector3 cameraVelocity;
    static BoxCollider2D activeZone;
    static CameraZone.ZoneType activeZoneType;
    static Vector2 activeZoneOffset;
    Vector3 cameraTarget;
    public void ManagedUpdate()
    {
        if(activeZone != null)
        {
            switch (activeZoneType)
            {
                case CameraZone.ZoneType.Default:
                    Vector2 zonePos = activeZone.transform.position;
                    cameraTarget = zonePos;
                    break;
                case CameraZone.ZoneType.Offset:
                    Vector2 zoneOffset = activeZoneOffset;
                    cameraTarget = player.transform.position + (Vector3)zoneOffset;
                    break;
            }
        }
        else
        {
            cameraTarget = player.transform.position + cameraOffset;
        }
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraTarget.z = cameraPosition.z;
        Vector3 newCameraPosition = Vector3.SmoothDamp(
                cameraPosition,
                cameraTarget,
                ref cameraVelocity,
                cameraSmoothTime
            );
        if(xAxisClamp.x != xAxisClamp.y)
        {
            newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, xAxisClamp.x, xAxisClamp.y);
        }
        if(yAxisClamp.x != yAxisClamp.y)
        {
            newCameraPosition.y = Mathf.Clamp(newCameraPosition.y, yAxisClamp.x, yAxisClamp.y);
        }
        mainCamera.transform.position = newCameraPosition;
    }
    public void ManagedStart()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    public void RegisterPlayer(Player player)
    {
        this.player = player;
    }
    public void PlayerDied()
    {
    }
    public static void CameraEnterLockedZone(BoxCollider2D zoneCollider, CameraZone.ZoneType zoneType, Vector2 offset)
    {
        activeZone = zoneCollider;
        activeZoneType = zoneType;
        activeZoneOffset = offset;
    }
    public static void CameraExitLockedZone(BoxCollider2D zoneCollider)
    {
        if(activeZone == zoneCollider)
        {
            activeZone = null;
        }
    }
}
