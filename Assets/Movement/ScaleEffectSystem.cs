using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffectSystem : MonoBehaviour
{
    public List<OneShotScaleEffect> oneShotComponents = new();
    public List<ContinuousScaleEffect> continuousComponents = new();

    SpriteRenderer spriteRenderer;
    Transform graphic;
    public Vector3 originalScale;
    public Vector3 finalScale;
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        graphic = spriteRenderer.transform;
    }
    void Update()
    {
        finalScale = originalScale;
        foreach (var component in oneShotComponents)
        {
            component.Update(Time.deltaTime, ref finalScale);
        }
        foreach (var component in continuousComponents)
        {
            component.Update(Time.deltaTime, ref finalScale);
        }
        graphic.localScale = finalScale;
    }
    public void SetupData(ScaleData data, out ScaleComponent component)
    {
        switch (data.applicationType)
        {
            case ApplicationType.OneShot:
                OneShotScaleEffect oneShot = new OneShotScaleEffect(data);
                component = oneShot;
                oneShotComponents.Add(oneShot);
                break;
            case ApplicationType.Continuous:
                ContinuousScaleEffect continuous = new ContinuousScaleEffect(data);
                component = continuous;
                continuousComponents.Add(continuous);
                break;
            default:
                component = null;
                break;
        }
    }
}
