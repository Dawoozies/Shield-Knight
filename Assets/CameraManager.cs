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
    private static float activeZoneFieldOfView;
    Vector3 cameraTarget;
    public float depth;
    private Vector3 screenCornerA;
    private Vector3 screenCornerB;
    public float worldSpaceWidth;
    public float worldSpaceHeight;
    public Transform xAxisClampMin, xAxisClampMax;
    private float fieldOfViewTarget;
    private float defaultFieldOfView;
    public float fieldOfViewSmoothTime;
    private float fieldOfViewVelocity;
    public void ManagedUpdate()
    {
        screenCornerA.z = depth;
        screenCornerB.z = depth;
        Vector2 cornerA = mainCamera.ScreenToWorldPoint(screenCornerA);
        Vector2 cornerB = mainCamera.ScreenToWorldPoint(screenCornerB);
        worldSpaceWidth = Mathf.Abs(cornerA.x - cornerB.x);
        worldSpaceHeight = Mathf.Abs(cornerA.y - cornerB.y);

        if(activeZone != null)
        {
            switch (activeZoneType)
            {
                case CameraZone.ZoneType.Default:
                    Vector2 zonePos = activeZone.transform.position;
                    cameraTarget = zonePos;
                    break;
                case CameraZone.ZoneType.Offset:
                    cameraTarget = player.transform.position + (Vector3)activeZoneOffset;
                    break;
                case CameraZone.ZoneType.OffsetAndFOV:
                    cameraTarget = player.transform.position + (Vector3)activeZoneOffset;
                    fieldOfViewTarget = activeZoneFieldOfView;
                    break;
            }
        }
        else
        {
            cameraTarget = player.transform.position;
            fieldOfViewTarget = defaultFieldOfView;
        }
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraTarget.z = cameraPosition.z;
        Vector3 newCameraPosition = Vector3.SmoothDamp(
                cameraPosition,
                cameraTarget,
                ref cameraVelocity,
                cameraSmoothTime
            );
        float cameraFieldOfView = mainCamera.fieldOfView;
        float newFieldOfView =
            Mathf.SmoothDamp(cameraFieldOfView, fieldOfViewTarget, ref fieldOfViewVelocity, fieldOfViewSmoothTime);
        if(xAxisClamp.x != xAxisClamp.y)
        {
            newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, xAxisClamp.x + worldSpaceWidth/2f, xAxisClamp.y - worldSpaceWidth/2f);
            if(xAxisClampMin != null)
            {
                xAxisClampMin.position = new Vector2(xAxisClamp.x - 0.5f, 480);
                xAxisClampMax.position = new Vector2(xAxisClamp.y + 0.5f, 480f);
            }
        }
        if(yAxisClamp.x != yAxisClamp.y)
        {
            newCameraPosition.y = Mathf.Clamp(newCameraPosition.y, yAxisClamp.x + worldSpaceHeight/2f, yAxisClamp.y - worldSpaceHeight/2f);
        }
        mainCamera.transform.position = newCameraPosition + cameraOffset;
        mainCamera.fieldOfView = newFieldOfView;
    }
    public void ManagedStart()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            screenCornerA = new Vector3(0f,0f,depth);
            screenCornerB = new Vector3(mainCamera.pixelWidth, mainCamera.pixelHeight, depth);
            defaultFieldOfView = mainCamera.fieldOfView;
        }
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

    public static void CameraEnterOffsetAndFOVZone(BoxCollider2D zoneCollider, CameraZone.ZoneType zoneType,
        Vector2 offset, float fieldOfView)
    {
        CameraEnterLockedZone(zoneCollider,zoneType,offset);
        activeZoneFieldOfView = fieldOfView;
    }
}
