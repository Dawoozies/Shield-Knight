using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemy : MonoBehaviour, IEnemy
{
    VelocitySystem velocitySystem;
    public VelocityData gravity, readyCharge, charge;
    public VelocityComponent gravityComponent, readyChargeComponent, chargeComponent;
    GroundCheck groundCheck;
    public bool grounded;
    IPlayerCheck playerCheckInterface;
    void Awake()
    {
        groundCheck = GetComponent<GroundCheck>();
        groundCheck.onGroundEnter.Add(OnGroundEnterHandler);
        groundCheck.onGroundExit.Add(OnGroundExitHandler);

        playerCheckInterface = GetComponent<IPlayerCheck>();
        playerCheckInterface.RegisterOnHitCallback(OnHitHandler);

        velocitySystem = GetComponent<VelocitySystem>();
        velocitySystem.SetupData(gravity, out gravityComponent);
        velocitySystem.SetupData(readyCharge, out readyChargeComponent);
        velocitySystem.SetupData(charge, out chargeComponent);

        readyChargeComponent.endTimeExceededActions.Add(
                () => 
                {
                    if(gravityComponent.isPlaying)
                    {
                        gravityComponent.Stop();
                    }
                    chargeComponent.PlayFromStart();
                }
            );

        chargeComponent.endTimeExceededActions.Add(
                () =>
                {
                    if(!gravityComponent.isPlaying)
                    {
                        gravityComponent.Play();
                    }
                }
            );
    }
    void OnGroundEnterHandler()
    {
        if(gravityComponent.isPlaying)
        {
            gravityComponent.Stop();
        }
        grounded = true;
    }
    void OnGroundExitHandler()
    {
        if(!chargeComponent.isPlaying && !gravityComponent.isPlaying)
        {
            gravityComponent.Play();
        }
        grounded = false;
    }
    void OnHitHandler(List<RaycastHit2D> results)
    {
        if(grounded && !readyChargeComponent.isPlaying && !chargeComponent.isPlaying)
        {
            foreach (var hit in results)
            {
                if(hit.collider != null)
                {
                    Vector2 chargeDirection = hit.collider.transform.position - transform.position;
                    chargeDirection.y = 0;
                    chargeDirection.Normalize();
                    readyChargeComponent.SetDirection(chargeDirection);
                    chargeComponent.SetDirection(chargeDirection);
                    break;
                }
            }
            readyChargeComponent.PlayFromStart();
        }
    }
    void Update()
    {
        if(grounded)
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }

    public void ApplyForce(Vector2 force)
    {
        Debug.LogError("Apply Force");
    }
}
