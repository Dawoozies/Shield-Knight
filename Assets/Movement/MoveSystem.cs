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
    public List<OneShotMove> oneShotComponents = new();
    public List<ContinuousMove> continuousComponents = new();

    Rigidbody2D rb;
    Transform systemAnchor;
    public Vector3 finalVector;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
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
        foreach (OneShotMove component in oneShotComponents)
        {
            component.Update(Time.fixedDeltaTime, ref finalVector, currentVector);
        }
        foreach (ContinuousMove component in continuousComponents)
        {
            component.Update(Time.fixedDeltaTime, ref finalVector, currentVector);
        }
        switch (mode)
        {
            case SystemMode.Position:
                transform.position = finalVector;
                break;
            case SystemMode.Velocity:
                rb.velocity = finalVector;
                break;
            case SystemMode.LocalPosition:
                transform.localPosition = finalVector;
                break;
        }
    }
    public void SetupData(MoveData data, out MoveComponent component)
    {
        switch (data.applicationType)
        {
            case ApplicationType.OneShot:
                OneShotMove oneShotMove = new OneShotMove(data);
                component = oneShotMove;
                oneShotComponents.Add(oneShotMove);
                break;
            case ApplicationType.Continuous:
                ContinuousMove continuousMove = new ContinuousMove(data);
                component = continuousMove;
                continuousComponents.Add(continuousMove);
                break;
            default:
                component = null;
                break;
        }
    }
    public void SetSystemAnchor(Transform systemAnchor)
    {
        this.systemAnchor = systemAnchor;
    }
}