using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocitySystem : MonoBehaviour
{
    public List<OneShotVelocity> oneShotComponents = new();
    public List<ContinuousVelocity> continuousComponents = new();

    Rigidbody2D rb;
    public Vector2 finalVelocity;
    private Transform parentTransform;
    private Vector2 lastParentPos;
    private bool checkParentExit;
    private float t;
    private const float checkTime = 0.06f;
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
        
        if (parentTransform != null)
        {
            Vector2 dv = (Vector2)parentTransform.position - lastParentPos;
            rb.position = rb.position + dv;
            lastParentPos = parentTransform.position;
            if (checkParentExit)
            {
                if (t > checkTime)
                {
                    //we remove parent
                    parentTransform = null;
                    checkParentExit = false;
                }
                t += Time.deltaTime;
            }
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
    public void SetParent(Transform parent)
    {
        if (checkParentExit)
        {
            if (parentTransform != null && parentTransform == parent)
            {
                checkParentExit = false;
            }
        }
        parentTransform = parent;
        if (parent != null)
        {
            lastParentPos = parent.position;
        }
    }

    public void CheckParentExit()
    {
        if (parentTransform == null)
            return;
        checkParentExit = true;
        t = 0;
    }

    public bool CheckParent()
    {
        return parentTransform != null;
    }
}
