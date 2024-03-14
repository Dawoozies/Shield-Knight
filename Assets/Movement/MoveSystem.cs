using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SystemMode
{
    Position, Velocity, LocalPosition
}
public class MoveSystem : MonoBehaviour
{
    public SystemMode mode;
    public List<FreeMove> freeMoveComponents = new();
    public List<MoveToEnd> moveToEndComponents = new();
    public List<MoveBetweenOriginEnd> moveBetweenOriginEndComponents = new();

    Rigidbody2D rb;
    Transform systemAnchor;
    Rigidbody systemAnchorRigidbody;
    public Vector3 finalVector;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector3 systemOrigin = Vector3.zero;
        Vector3 systemVelocity = Vector3.zero;
        if(systemAnchor != null)
        {
            systemOrigin = systemAnchor.position;
            if(systemAnchorRigidbody != null)
            {
                systemVelocity = systemAnchorRigidbody.velocity;
            }
        }
        Vector3 currentVector = Vector3.zero;
        switch (mode)
        {
            case SystemMode.Position:
                finalVector = transform.position;
                currentVector = transform.position;
                break;
            case SystemMode.Velocity:
                finalVector = Vector3.zero;
                currentVector = rb.velocity;
                break;
            case SystemMode.LocalPosition:
                finalVector = transform.localPosition;
                currentVector = transform.localPosition;
                break;
        }
        foreach (var component in freeMoveComponents)
        {
            component.Update(Time.fixedDeltaTime, ref finalVector, currentVector);
        }
        foreach (var component in moveToEndComponents)
        {
            component.Update(Time.fixedDeltaTime, ref finalVector, currentVector);
        }
        foreach (var component in moveBetweenOriginEndComponents)
        {
            component.Update(Time.fixedDeltaTime, ref finalVector, currentVector);
        }
        switch (mode)
        {
            case SystemMode.Position:
                transform.position = systemOrigin + finalVector;
                break;
            case SystemMode.Velocity:
                rb.velocity = systemVelocity + finalVector;
                break;
            case SystemMode.LocalPosition:
                //this may give incorrect results since i havent tested it
                transform.localPosition = transform.worldToLocalMatrix.MultiplyPoint(systemOrigin) + finalVector;
                break;
        }
    }
    public void SetupData(MoveData data, out MoveComponent baseComponent)
    {
        switch (data.moveType)
        {
            case MoveType.Free:
                FreeMove freeMoveComponent = new FreeMove(data);
                baseComponent = freeMoveComponent;
                freeMoveComponents.Add(freeMoveComponent);
                break;
            case MoveType.MoveToEnd:
                MoveToEnd moveToEndComponent = new MoveToEnd(data);
                baseComponent = moveToEndComponent;
                moveToEndComponents.Add(moveToEndComponent);
                break;
            case MoveType.MoveBetweenOriginEnd:
                MoveBetweenOriginEnd moveBetweenOriginEndComponent = new MoveBetweenOriginEnd(data);
                baseComponent = moveBetweenOriginEndComponent;
                moveBetweenOriginEndComponents.Add(moveBetweenOriginEndComponent);
                break;
            default:
                baseComponent = null;
                break;
        }
    }
    public void SetSystemAnchor(Transform systemAnchor)
    {
        this.systemAnchor = systemAnchor;
        systemAnchorRigidbody = systemAnchor.GetComponent<Rigidbody>();
    }
}