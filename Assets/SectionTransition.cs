using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SectionTransition : MonoBehaviour
{
    public RawImage rawImage;
    private Material instancedMaterial;
    public float transitionSpeed;
    private float _transition;
    const float minValue = 0.99f;
    const float maxValue = 2f;
    private static readonly int Transition = Shader.PropertyToID("_transition");
    bool transitionActive;
    float s,t;
    public float midpointWaitTime;
    float w;
    Action transitionMidpointCallback;
    public void RegisterTransitionMidpointCallback(Action a)
    { transitionMidpointCallback = a; }
    void Start()
    {
        instancedMaterial = new Material(rawImage.material);
        rawImage.material = instancedMaterial;
        _transition = instancedMaterial.GetFloat(Transition);
    }
    void Update()
    {
        if (transitionActive)
        {
            if(s < 1)
            {
                s += Time.unscaledDeltaTime* transitionSpeed;
            }
            else
            {
                if(w <= 0)
                {
                    transitionMidpointCallback();
                }
                w += Time.unscaledDeltaTime;
                if(w > midpointWaitTime)
                {
                    t += Time.unscaledDeltaTime * transitionSpeed;
                    if (t > 1)
                    {
                        s = 0;
                        t = 0;
                        w = 0;
                        transitionActive = false;
                    }
                }
            }
        }
        _transition = Mathf.Lerp(minValue, maxValue, s - t);
        instancedMaterial.SetFloat(Transition, _transition);
    }
    public void StartTransition()
    {
        transitionActive = true;
    }
}
