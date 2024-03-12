using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class VelocitySystem : MonoBehaviour
{
    public List<OneShotVelocity> oneShotComponents = new();
    public List<ContinuousVelocity> continuousComponents = new();

    Rigidbody2D rb;
    public Vector2 finalVelocity;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        finalVelocity = Vector2.zero;
        foreach  (var component in oneShotComponents)
        {
            component.Update(Time.fixedDeltaTime, ref finalVelocity);
        }
        foreach (var component in continuousComponents)
        {
            component.Update(Time.fixedDeltaTime, ref finalVelocity);
        }
        rb.velocity = finalVelocity;
    }
    public void SetupData(VelocityData velocityData, out VelocityComponent component)
    {
        switch (velocityData.applicationType)
        {
            case ApplicationType.OneShot:
                OneShotVelocity oneShotVelocity = new OneShotVelocity(velocityData);
                component = oneShotVelocity;
                oneShotComponents.Add(oneShotVelocity);
                break;
            case ApplicationType.Continuous:
                ContinuousVelocity continuousVelocity = new ContinuousVelocity(velocityData);
                component = continuousVelocity;
                continuousComponents.Add(continuousVelocity);
                break;
            default:
                component = null;
                break;
        }
    }
}
