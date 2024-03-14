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
    void Update()
    {
        finalVector = Vector3.zero;

        switch (mode)
        {
            case SystemMode.Position:
                rb.transform.position = finalVector;
                break;
            case SystemMode.Velocity:
                rb.velocity = finalVector;
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
}