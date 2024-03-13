using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Player player;
    public Camera mainCamera;
    public float cameraSmoothTime;
    public Vector3 cameraOffset;
    Vector3 cameraVelocity;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (player == null)
            player = FindAnyObjectByType<Player>();
    }
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
}
