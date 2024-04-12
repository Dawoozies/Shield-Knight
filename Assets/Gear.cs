 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[Serializable]
public class GearEvent : UnityEvent<float>
{

}
public class Gear : MonoBehaviour, IHitReceiver
{
    GearHitTarget[] hitTargets;
    public float angularVelocity;
    public float angularDrag;
    public GearEvent onGearHit;
    public GearEvent onGearRotating;
    public bool returnMagnitude;
    public bool constantOutput;
    public float constantOutputValue;
    private void Start()
    {
        hitTargets = GetComponentsInChildren<GearHitTarget>();
        foreach (GearHitTarget target in hitTargets)
        {
            target.RegisterHitCallback(GearHitHandler);
        }
        GameManager.GetActivePlayer().RegisterOnDeathCompleteCallback(() => { angularVelocity = 0; });
    }
    void GearHitHandler(ShieldSystemType shieldSystemType, Vector2 shieldDir, float shieldVelocity, float hitDotProduct)
    {
        Debug.Log("Gear hit" + shieldSystemType);
        angularVelocity = -hitDotProduct * shieldVelocity;
        onGearHit?.Invoke(shieldVelocity);
    }
    private void Update()
    {
        if(Mathf.Abs(angularVelocity) > 0)
        {
            Debug.Log("Gear rotating");
            if(returnMagnitude)
            {
                if(constantOutput)
                {
                    onGearRotating?.Invoke(constantOutputValue*Time.deltaTime);
                }
                else
                {
                    onGearRotating?.Invoke(Mathf.Abs(angularVelocity) * Time.deltaTime);
                }
            }
            else
            {
                if (constantOutput)
                {
                    onGearRotating?.Invoke(constantOutputValue * Time.deltaTime);
                }
                else
                {
                    onGearRotating?.Invoke(angularVelocity * Time.deltaTime);
                }
            }
            transform.Rotate(Vector3.forward, -angularVelocity*Time.deltaTime);
            float drag = Mathf.Sign(angularVelocity) *angularDrag*Time.deltaTime;
            angularVelocity -= drag;
            if(Mathf.Abs(angularVelocity) < 0.2)
            {
                angularVelocity = 0f;
            }
        }
    }

    public void ApplyForce(Vector2 force)
    {
        Debug.LogError("Force = " + force);
        angularVelocity += force.magnitude*Time.deltaTime;
    }
}
