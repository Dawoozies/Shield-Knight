using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    VelocitySystem velocitySystem;
    public VelocityData gravity;
    public VelocityData jumpAscent;
    public VelocityData run;
    public VelocityData airDash;
    Vector2 mousePos;
    void Start()
    {
        velocitySystem = GetComponent<VelocitySystem>();
        VelocityComponent gravityComponent;
        velocitySystem.SetupData(gravity, out gravityComponent);
        gravityComponent.Play();

        VelocityComponent jumpAscentComponent;
        velocitySystem.SetupData(jumpAscent, out jumpAscentComponent);
        jumpAscentComponent.endTimeExceededActions.Add(
                () => gravityComponent.PlayFromStart()
            );
        InputManager.RegisterJumpDownInputCallback(
                () => 
                {
                    gravityComponent.Stop();
                    jumpAscentComponent.PlayFromStart();
                }
            );
        InputManager.RegisterJumpUpInputCallback(
                () =>
                {
                    if(jumpAscentComponent.isPlaying)
                    {
                        jumpAscentComponent.Stop();
                        gravityComponent.PlayFromStart();
                    }
                }
            );

        VelocityComponent runComponent;
        velocitySystem.SetupData(run, out runComponent);
        runComponent.Play();
        InputManager.RegisterMoveInputCallback(
                (Vector2 moveInput) =>
                {
                    runComponent.SetDirection(Vector2.right * moveInput.x);
                }
            );

        VelocityComponent airDashComponent;
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
    }
}
