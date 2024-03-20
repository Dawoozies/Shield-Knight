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
    void Update()
    {
        Vector3 cameraTarget = player.transform.position + cameraOffset;
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
}
