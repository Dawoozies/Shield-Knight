using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Character character;
    public MovementData gravity;
    public MovementData hitStun;
    bool grounded;
    Rigidbody2D rb;
    Vector2 force;
    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        character.TryAddComponent(gravity);
        character.TryAddComponent(hitStun);
        gravity.Start(() => Time.fixedDeltaTime);
    }
    void Update()
    {
        (bool, RaycastHit2D) groundCheckData = character.GroundedCheck();
        grounded = groundCheckData.Item1;
    }
    void FixedUpdate()
    {
        hitStun.Update();
        gravity.Update();
    }
    public void ApplyHit(Vector2 force)
    {
        this.force = force;
        hitStun.maxOutput = force.magnitude;
        hitStun.direction = force.normalized;
        hitStun.Start(() => Time.fixedDeltaTime);
    }
}
