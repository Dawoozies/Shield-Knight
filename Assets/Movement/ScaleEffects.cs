using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffects : MonoBehaviour
{
    ScaleEffectSystem scaleEffectSystem;
    public ScaleData land, jump, moveOnGround;
    public ScaleComponent landComponent, jumpComponent, moveOnGroundComponent;
    GroundCheck groundCheck;
    bool grounded;
    VelocitySystem velocitySystem;
    void Start()
    {
        scaleEffectSystem = GetComponent<ScaleEffectSystem>();
        scaleEffectSystem.SetupData(land, out landComponent);
        scaleEffectSystem.SetupData(jump, out jumpComponent);
        scaleEffectSystem.SetupData(moveOnGround, out moveOnGroundComponent);

        groundCheck = GetComponent<GroundCheck>();
        groundCheck.onGroundEnter.Add(GroundEnterCallback);
        groundCheck.onGroundExit.Add(GroundExitCallback);

        velocitySystem = GetComponent<VelocitySystem>();
    }
    void GroundEnterCallback()
    {
        if(!landComponent.isPlaying && !velocitySystem.CheckParent())
        {
            landComponent.PlayFromStart();
        }
        grounded = true;
    }
    void GroundExitCallback()
    {
        if(!jumpComponent.isPlaying && !velocitySystem.CheckParent())
        {
            jumpComponent.PlayFromStart();
        }
        grounded = false;
    }
    void Update()
    {
        if(grounded)
        {
            if(Mathf.Abs(velocitySystem.finalVelocity.x) > 0)
            {
                if(!moveOnGroundComponent.isPlaying)
                {
                    moveOnGroundComponent.Play();
                }
            }
            else
            {
                if(moveOnGroundComponent.isPlaying)
                {
                    moveOnGroundComponent.Pause();
                }
            }
        }
    }
}
