using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemy : MonoBehaviour, IEnemy
{
    VelocitySystem velocitySystem;
    public VelocityData gravity, readyCharge, charge, knockback, hitStun;
    public VelocityComponent gravityComponent, readyChargeComponent, chargeComponent, knockbackComponent, hitStunComponent;
    GroundCheck groundCheck;
    public bool grounded;
    IPlayerCheck playerCheckInterface;
    Vector3 spawn;
    public float damagePercentage;
    BoxCollider2D boxCollider2D;
    List<Action<IEnemy>> onDeathActions = new();
    List<Action<IEnemy>> onRespawnActions = new();
    public LayerMask canBounceOff;
    void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        groundCheck = GetComponent<GroundCheck>();
        groundCheck.onGroundEnter.Add(OnGroundEnterHandler);
        groundCheck.onGroundExit.Add(OnGroundExitHandler);

        playerCheckInterface = GetComponent<IPlayerCheck>();
        playerCheckInterface.RegisterOnHitCallback(OnHitHandler);

        velocitySystem = GetComponent<VelocitySystem>();
        velocitySystem.SetupData(gravity, out gravityComponent);
        velocitySystem.SetupData(readyCharge, out readyChargeComponent);
        velocitySystem.SetupData(charge, out chargeComponent);
        velocitySystem.SetupData(knockback, out knockbackComponent);
        velocitySystem.SetupData(hitStun, out hitStunComponent);

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
        hitStunComponent.endTimeExceededActions.Add(
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
        if (gravityComponent.isPlaying)
        {
            gravityComponent.Stop();
        }
        grounded = true;
    }
    void OnGroundExitHandler()
    {
        if (!chargeComponent.isPlaying && !gravityComponent.isPlaying && !hitStunComponent.isPlaying)
        {
            gravityComponent.Play();
        }
        grounded = false;
    }
    void OnHitHandler(List<RaycastHit2D> results)
    {
        if(grounded && !readyChargeComponent.isPlaying && !chargeComponent.isPlaying && !knockbackComponent.isPlaying && !hitStunComponent.isPlaying)
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


    public void SetSpawn(Vector3 spawn)
    {
        this.spawn = spawn;
    }
    public void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.tag == "HardSurface")
        {
            if(knockbackComponent.isPlaying)
            {
                if(!hitStunComponent.isPlaying)
                {
                    Vector2 posToCol = col.transform.position - transform.position;
                    //If force is going into a collider
                    if(Vector2.Dot(velocitySystem.finalVelocity, posToCol) > 0)
                    {
                        Debug.LogError($"Hit hard surface with velocity {velocitySystem.finalVelocity} + damage percentage {damagePercentage}%");
                        damagePercentage += velocitySystem.finalVelocity.magnitude;
                        gravityComponent.Stop();
                        readyChargeComponent.Stop();
                        chargeComponent.Stop();
                        knockbackComponent.Stop();

                        if(damagePercentage >= 100)
                        {
                            foreach (var action in onDeathActions)
                            {
                                action(this);
                            }
                            return;
                        }

                        hitStunComponent.PlayFromStart();
                    }
                }
            }
        }
    }
    public void Kill()
    {
        gameObject.SetActive(false);
    }
    public void RegisterEnemyDeathCallback(Action<IEnemy> action)
    {
        onDeathActions.Add(action);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        transform.position = spawn;
        foreach (var action in onRespawnActions)
        {
            action(this);
        }
    }

    public void RegisterEnemyRespawnCallback(Action<IEnemy> action)
    {
        onRespawnActions.Add(action);
    }
    public void ApplyDamage(Vector2 force)
    {
        Vector2 originalForce = force;
        Vector2 projectedSum = Vector2.zero;
        Collider2D[] nearbyCol = Physics2D.OverlapBoxAll(transform.position, boxCollider2D.size + Vector2.one * 0.02f, 0f, canBounceOff);
        foreach (var col in nearbyCol)
        {
            Vector2 normal = ((Vector2)transform.position - col.ClosestPoint(transform.position)).normalized;
            Vector2 projectedForce = (Vector2.Dot(force, normal) / normal.magnitude)*normal;
            projectedSum += projectedForce;
        }
        Debug.LogError($"[ Original {originalForce} norm = {originalForce.magnitude} ] [ ProjectedSum = {projectedSum} norm = {projectedSum.magnitude} ]");

        if (!knockbackComponent.isPlaying)
        {
            readyChargeComponent.Stop();
            chargeComponent.Stop();
            knockbackComponent.SetDirection(force.normalized);
            knockbackComponent.Play();
        }
    }
}
