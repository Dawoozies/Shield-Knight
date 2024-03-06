using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Character character;
    public MovementData gravity;
    bool grounded;
    float hitStunTime;
    Rigidbody2D rb;
    Vector2 force;
    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        character.TryAddComponent(gravity);
        gravity.Start(() => Time.fixedDeltaTime);
    }
    void Update()
    {
        (bool, RaycastHit2D) groundCheckData = character.GroundedCheck();
        grounded = groundCheckData.Item1;
    }
    void FixedUpdate()
    {
        gravity.Update();
    }
    public void ApplyHit(Vector2 force, float hitStunTime)
    {
        this.hitStunTime = hitStunTime;
        this.force = force;
    }
}
