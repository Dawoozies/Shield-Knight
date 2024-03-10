using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Enemy : MonoBehaviour
{
    Character character;
    public MovementData gravity;
    public MovementData hitStun;
    public MovementData death;
    public MovementData mainAttack;
    public MovementData knockback;
    [Tooltip("Defines X basis axis for attack directions to be transformed into, ignored if 0")]
    public Vector2 attackBasisX;
    [Tooltip("Defines Y basis axis for attack directions to be transformed into, ignored if 0")]
    public Vector2 attackBasisY;
    public float attackCooldown;
    float cooldownTimer;
    bool grounded;
    Rigidbody2D rb;
    Vector2 force;
    public int health;
    BoxCollider2D boxCollider;
    public List<Action<Enemy, Vector2>> onDeathActions = new();
    public Transform spawn;

    public Character player;
    IDetect detectionInterface;
    void Start()
    {
        detectionInterface = GetComponent<IDetect>();
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        character.TryAddComponent(gravity);
        character.TryAddComponent(hitStun);
        character.TryAddComponent(death);
        character.TryAddComponent(mainAttack);
        gravity.Start(() => Time.fixedDeltaTime);
    }
    void Update()
    {
        (bool, RaycastHit2D) groundCheckData = character.GroundedCheck();
        grounded = groundCheckData.Item1;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            (bool, Vector2) firstResult = detectionInterface.FirstResultPosition("Player");
            if(firstResult.Item1)
            {
                if(mainAttack.state != MovementData.State.InProgress)
                {
                    Vector2 finalDirection = ((Vector3)firstResult.Item2 - transform.position).normalized;
                    if (attackBasisX != Vector2.zero || attackBasisY != Vector2.zero)
                    {
                        float xOriginal = finalDirection.x;
                        float yOriginal = finalDirection.y;
                        finalDirection.x = attackBasisX.x * xOriginal + attackBasisY.x * yOriginal;
                        finalDirection.y = attackBasisX.y * xOriginal + attackBasisY.y * yOriginal;
                    }
                    mainAttack.direction = finalDirection;
                    mainAttack.maxOutput = 30f;
                    mainAttack.Start(() => Time.fixedDeltaTime);
                    cooldownTimer = attackCooldown;
                }
            }
        }
    }
    void FixedUpdate()
    {
        if(health > 0)
        {
            hitStun.Update();
            gravity.Update();
            mainAttack.Update();

            boxCollider.isTrigger = false;
        }
        else
        {
            if(death.state == MovementData.State.Completed)
            {
                death.direction.y -= 10f * Time.fixedDeltaTime;
                death.maxOutput += 10f * Time.fixedDeltaTime;
            }
            death.Update();

            boxCollider.isTrigger = true;

            transform.Rotate(30f*Vector3.forward);
        }
    }
    public void ApplyHit(Vector2 force)
    {
        this.force = force;
        hitStun.maxOutput = force.magnitude;
        hitStun.direction = force.normalized;
        hitStun.Start(() => Time.fixedDeltaTime);

        health--;
        if(health <= 0)
        {
            foreach (var action in onDeathActions)
            {
                action(this, force);
            }
            gravity.ForceEnd();
            hitStun.ForceEnd();
            death.maxOutput = force.magnitude;
            death.direction = (force + 4*Vector2.up).normalized;
            death.keepAtMax = true;
            death.Start(() => Time.fixedDeltaTime);
        }
    }
    public void Reset()
    {
        gravity.ForceEnd();
        hitStun.ForceEnd();
        death.ForceEnd();
        mainAttack.ForceEnd();
        death.maxOutput = 0;
        death.keepAtMax = false;
        health = 1;
        boxCollider.isTrigger = false;
        Revive();
    }
    public void Revive()
    {
        transform.position = spawn.position;
        gravity.Start(() => Time.fixedDeltaTime);
    }
}
