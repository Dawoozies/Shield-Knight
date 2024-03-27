using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentSystem : MonoBehaviour
{
    private GroundCheck groundCheck;
    private VelocitySystem velocitySystem;
    private Transform followTransform;
    void Start()
    {
        followTransform = new GameObject("parentSystemTransform").transform;
        groundCheck = GetComponent<GroundCheck>();
        velocitySystem = GetComponent<VelocitySystem>();
        groundCheck.onGroundEnter.Add(OnGroundEnterHandler);
        groundCheck.onGroundExit.Add(OnGroundExitHandler);
    }
    void Update()
    {
        
    }

    void OnGroundEnterHandler()
    {
        
    }
    void OnGroundExitHandler()
    {
        
    }
}
