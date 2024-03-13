using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    VelocitySystem velocitySystem;
    public VelocityData gravity, jumpAscent, run, airDash;
    public VelocityComponent gravityComponent, jumpAscentComponent, runComponent, airDashComponent;
    Vector2 mousePos;
    GroundCheck groundCheck;
    public bool grounded;
    void Awake()
    {
        #region Ground Action Setup
        groundCheck = GetComponent<GroundCheck>();
        groundCheck.onGroundEnter.Add(GroundEnterHandler);
        groundCheck.onGroundExit.Add(GroundExitHandler);
        #endregion

        velocitySystem = GetComponent<VelocitySystem>();
        #region Gravity Setup
        velocitySystem.SetupData(gravity, out gravityComponent);
        #endregion
        #region Jump Setup
        velocitySystem.SetupData(jumpAscent, out jumpAscentComponent);
        jumpAscentComponent.endTimeExceededActions.Add(
                () => gravityComponent.PlayFromStart()
            );
        //InputManager.RegisterJumpDownInputCallback(
        //        () => 
        //        {
        //            //If grounded we can jump
        //            if (grounded)
        //            {
        //                gravityComponent.Stop();
        //                jumpAscentComponent.PlayFromStart();a
        //            }
        //        }
        //    );

        InputManager.RegisterJumpInputCallback(
                (float heldTime) =>
                {
                    if(grounded && heldTime > 0)
                    {
                        if(!jumpAscentComponent.isPlaying)
                        {
                            if(gravityComponent.isPlaying)
                            {
                                gravityComponent.Stop();
                            }
                            jumpAscentComponent.PlayFromStart();
                        }
                    }
                }
            );

        InputManager.RegisterJumpUpInputCallback(
                () =>
                {
                    //If jump ascent is playing then we can do this when we let go of space
                    if(jumpAscentComponent.isPlaying)
                    {
                        jumpAscentComponent.Stop();
                        gravityComponent.PlayFromStart();
                    }
                }
            );
        #endregion
        #region Run Setup
        velocitySystem.SetupData(run, out runComponent);
        runComponent.Play();
        InputManager.RegisterMoveInputCallback(
                (Vector2 moveInput) =>
                {
                    runComponent.SetDirection(Vector2.right * moveInput.x);
                }
            );
        #endregion
        #region AirDash Setup
        velocitySystem.SetupData(airDash, out airDashComponent);
        airDashComponent.endTimeExceededActions.Add(
                () =>
                {
                    gravityComponent.Play();
                    runComponent.Play();
                }
            );
        InputManager.RegisterMouseInputCallback((Vector2 mouseWorldPos) => mousePos = mouseWorldPos);

        InputManager.RegisterMouseDownCallback(
                (Vector3Int mouseDownInput) =>
                {
                    if(mouseDownInput.y > 0)
                    {
                        gravityComponent.Stop();
                        jumpAscentComponent.Stop();
                        runComponent.Stop();
                        airDashComponent.SetDirection((mousePos - (Vector2)transform.position).normalized);
                        airDashComponent.PlayFromStart();
                    }
                }
            );
        #endregion

    }
    void GroundEnterHandler()
    {
        //If Enter Ground Stop Gravity
        if (gravityComponent.isPlaying)
        {
            gravityComponent.Stop();
        }
        grounded = true;
    }
    void GroundExitHandler()
    {
        if(!jumpAscentComponent.isPlaying && !airDashComponent.isPlaying && !gravityComponent.isPlaying)
        {
            gravityComponent.Play();
        }
        grounded = false;
    }
    void Update()
    {
        if(grounded)
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
