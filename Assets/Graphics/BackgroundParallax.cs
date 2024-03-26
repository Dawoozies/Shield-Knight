using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public Transform backgroundParent;
    [Range(0f, 100f)]
    public float parallaxDepth;
    Camera mainCamera;
    public float smoothTime;
    Vector3 velocity;
    public Vector2 axis;
    public Vector2 origin;
    private void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        Vector3 parallaxPos = Vector3.zero;
        if (parallaxDepth < 99f)
        {
            parallaxPos = (Vector3)origin - Vector3.Scale(mainCamera.transform.position, axis) * 1f/(parallaxDepth + 1);
        }
        else
        {
            parallaxPos = origin;
        }
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, parallaxPos, ref velocity, smoothTime);
    }
}
