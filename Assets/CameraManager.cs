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
    Vector3 cameraVelocity;
    static BoxCollider2D activeZone;
    Vector3 cameraTarget;
    public void ManagedUpdate()
    {
        if(activeZone != null)
        {
            //Do this and return
            Vector2 zonePos = activeZone.transform.position;
            cameraTarget = zonePos;
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
    public static void CameraEnterLockedZone(BoxCollider2D zoneCollider)
    {
        activeZone = zoneCollider;
    }
    public static void CameraExitLockedZone(BoxCollider2D zoneCollider)
    {
        if(activeZone == zoneCollider)
        {
            activeZone = null;
        }
    }
}
