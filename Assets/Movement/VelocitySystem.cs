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
    private const float checkTime = 0.2f;

    public VelocityData externalMove;
    private VelocityComponent externalMoveComponent;

    public VelocityData externalFollowMove;
    private VelocityComponent externalFollowMoveComponent;

    GroundCheck groundCheck;
    Transform centroidTransform;
    Vector3 lastCentroidPos;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetupData(externalMove, out externalMoveComponent);
        SetupData(externalFollowMove, out externalFollowMoveComponent);
        externalFollowMoveComponent.PlayFromStart();

        centroidTransform = new GameObject("Player_CentroidTransform").transform;
        groundCheck = GetComponent<GroundCheck>();
        groundCheck.onGroundEnterDetailed.Add(GroundEnterDetailedHandler);
        groundCheck.onGroundExit.Add(GroundExitHandler);
    }
    void GroundEnterDetailedHandler(Vector3 centroid, Collider2D col)
    {
        centroidTransform.parent = col.transform;
        centroidTransform.position = centroid;
        lastCentroidPos = centroid;
        rb.transform.position = centroid;
    }
    void GroundExitHandler()
    {
        externalFollowMoveComponent.SetMagnitude(0f);
        centroidTransform.parent = null;
    }
    private void FixedUpdate()
    {
        finalVelocity = Vector2.zero;

        Vector2 ds = centroidTransform.position - lastCentroidPos;
        externalFollowMoveComponent.SetDirection(ds.normalized);
        externalFollowMoveComponent.SetMagnitude(ds.magnitude / Time.fixedDeltaTime);
        lastCentroidPos = centroidTransform.position;


        foreach  (var component in oneShotComponents)
        {
            component.Update(Time.fixedUnscaledDeltaTime, ref finalVelocity);
        }
        foreach (var component in continuousComponents)
        {
            component.Update(Time.fixedUnscaledDeltaTime, ref finalVelocity);
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
    public void ExternalMove(Vector2 force)
    {
        externalMoveComponent.SetMagnitude(force.magnitude);
        externalMoveComponent.SetDirection(force.normalized);
        externalMoveComponent.PlayFromStart();
    }
}
