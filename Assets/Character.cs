using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    BoxCollider2D boundingBoxCollider;
    public LayerMask groundingLayers;
    public float groundedCheckDistance;
    public Dictionary<string, MovementComponent> movementComponents = new();
    public SpriteRenderer graphic;
    public Vector2 originalScale;
    public VectorCurve groundMoveScale;
    public VectorCurve landScale;
    Vector2 lastVelocity;
    Vector2 currentVelocity;
    Vector2 velocityDelta;
    bool lastGroundedValue;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boundingBoxCollider = GetComponent<BoxCollider2D>();
        landScale.currentTime = 1;
    }
    public (bool, RaycastHit2D) GroundedCheck()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boundingBoxCollider.size, 0f, Vector2.down, groundedCheckDistance, groundingLayers);
        return (hit.collider == true, hit);
    }
    void Update()
    {
        (bool, RaycastHit2D) groundCheckData = GroundedCheck();
        bool grounded = groundCheckData.Item1;
        if(grounded)
        {
            rb.SetRotation(Quaternion.identity);
        }
        Vector3 localScale = originalScale;
        if (lastGroundedValue != grounded)
        {
            if (!lastGroundedValue && grounded)
            {
                landScale.Reset();
            }
            lastGroundedValue = grounded;
        }
        if (grounded && Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            groundMoveScale.Update(Time.deltaTime * Mathf.Abs(rb.velocity.x));
            localScale += (Vector3)groundMoveScale.output;
        }
        landScale.Update(Time.deltaTime);
        localScale += (Vector3)landScale.output;
        graphic.transform.localScale = localScale;
    }
    void FixedUpdate()
    {
        Vector2 finalVelocity = Vector2.zero;
        foreach (MovementComponent movementComponent in movementComponents.Values)
        {
            if(movementComponent.curveData.nullifyWhileActive != null && movementComponent.curveData.state == MovementData.State.InProgress)
            {
                foreach (string componentToNullify in movementComponent.curveData.nullifyWhileActive)
                {
                    if (movementComponents.ContainsKey(componentToNullify))
                    {
                        finalVelocity += -movementComponents[componentToNullify].componentOutput;
                    }
                }
            }
            finalVelocity += movementComponent.componentOutput;
        }
        lastVelocity = currentVelocity;
        currentVelocity = finalVelocity;
        velocityDelta = currentVelocity - lastVelocity;
        rb.velocity = finalVelocity;
    }
    public bool TryAddComponent(MovementData movementData)
    {
        if(movementComponents.ContainsKey(movementData.movementName))
        {
            return false;
        }
        MovementComponent component = new();
        component.curveData = movementData;
        movementComponents.TryAdd(movementData.movementName, component);
        return true;
    }
}
public class MovementComponent
{
    public MovementData curveData;
    public Vector2 direction => curveData.direction;
    public float magnitudeClamp => curveData.maxOutput;
    public Vector2 componentOutput => Vector2.ClampMagnitude(direction * curveData.output, magnitudeClamp);
}