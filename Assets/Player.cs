using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IHitReceiver
{
    VelocitySystem velocitySystem;
    public VelocityData gravity, jumpAscent, run, airDash, aimGravity, aimMove;
    public VelocityComponent gravityComponent, jumpAscentComponent, runComponent, airDashComponent, aimGravityComponent, aimMoveComponent;
    Vector2 mousePos;
    GroundCheck groundCheck;
    public bool grounded;
    public bool aiming;
    public float runMagnitude;
    public float aimMoveMagnitude;
    float aimMoveMagnitudeLeft;
    public int damageTaken;
    [Range(0f,1f)]
    public float damageResistance;
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
        velocitySystem.SetupData(aimGravity, out aimGravityComponent);
        #endregion
        #region Jump Setup
        velocitySystem.SetupData(jumpAscent, out jumpAscentComponent);
        jumpAscentComponent.endTimeExceededActions.Add(
                () => 
                {
                    TurnOnGravity(true);
                }
            );
        InputManager.RegisterJumpInputCallback(
                (float heldTime) =>
                {
                    if(grounded && heldTime > 0)
                    {
                        if(!jumpAscentComponent.isPlaying)
                        {
                            TurnOffGravity();
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
                        TurnOnGravity(true);
                    }
                }
            );
        #endregion
        #region Run Setup
        velocitySystem.SetupData(run, out runComponent);
        runComponent.Play();
        velocitySystem.SetupData(aimMove, out aimMoveComponent);
        aimMoveComponent.Play();
        InputManager.RegisterMoveInputCallback(
                (Vector2 moveInput) =>
                {
                    if(aiming)
                    {
                        aimMoveComponent.SetMagnitude(aimMoveMagnitudeLeft);
                        aimMoveMagnitudeLeft -= Time.deltaTime * aimMoveMagnitude;
                        if(aimMoveMagnitudeLeft <= 0)
                        {
                            aimMoveMagnitudeLeft = 0;
                        }
                        runComponent.SetMagnitude(0);
                    }
                    else
                    {
                        runComponent.SetMagnitude(runMagnitude);
                        aimMoveComponent.SetMagnitude(0);
                    }

                    runComponent.SetDirection(Vector2.right * moveInput.x);
                    aimMoveComponent.SetDirection(moveInput);
                }
            );
        #endregion
        #region AirDash Setup
        velocitySystem.SetupData(airDash, out airDashComponent);
        airDashComponent.endTimeExceededActions.Add(
                () =>
                {
                    TurnOnGravity(true);
                    runComponent.Play();
                }
            );
        InputManager.RegisterMouseInputCallback((Vector2 mouseWorldPos) => mousePos = mouseWorldPos);

        InputManager.RegisterMouseDownCallback(
                (Vector3Int mouseDownInput) =>
                {
                    if(mouseDownInput.z > 0)
                    {
                        TurnOffGravity();
                        jumpAscentComponent.Stop();
                        runComponent.Stop();
                        airDashComponent.SetDirection((mousePos - (Vector2)transform.position).normalized);
                        airDashComponent.PlayFromStart();
                    }
                }
            );
        #endregion

        InputManager.RegisterMouseClickCallback(
                (Vector3 mouseClickInput) =>
                {
                    aiming = mouseClickInput.y > 0;
                }
            );
    }
    void GroundEnterHandler()
    {
        //If Enter Ground Stop Gravity
        TurnOffGravity();
        grounded = true;
        aimMoveMagnitudeLeft = aimMoveMagnitude;
    }
    void GroundExitHandler()
    {
        if(!jumpAscentComponent.isPlaying && !airDashComponent.isPlaying)
        {
            if (aiming)
            {
                if (!aimGravityComponent.isPlaying)
                {
                    aimGravityComponent.Play();
                }
            }
            else
            {
                if (!gravityComponent.isPlaying)
                {
                    gravityComponent.Play();
                }
            }
        }
        grounded = false;
    }
    void Update()
    {
        if(aiming && gravityComponent.isPlaying)
        {
            gravityComponent.Stop();
            aimGravityComponent.PlayFromStart();
        }
        if(grounded)
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }
    void TurnOnGravity(bool playFromStart)
    {
        if(aiming)
        {
            if (playFromStart)
            {
                aimGravityComponent.PlayFromStart();
            }
            else
            {
                aimGravityComponent.Play();
            }
        }
        else
        {
            if(playFromStart)
            {
                gravityComponent.PlayFromStart();
            }
            else
            {
                gravityComponent.Play();
            }
        }
    }
    void TurnOffGravity()
    {
        gravityComponent.Stop();
        aimGravityComponent.Stop();
    }

    public void ApplyForce(Vector2 force)
    {
        damageTaken += Mathf.FloorToInt(force.magnitude * (1-damageResistance));
    }
}
