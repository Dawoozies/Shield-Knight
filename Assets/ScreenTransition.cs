using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenTransition : MonoBehaviour
{
    private RawImage rawImage;
    private Material instancedMaterial;
    public float transitionAcceleration;
    public float transitionSpeed;
    public enum TransitionDirection
    {
        None, Forward, Backward
    }
    private TransitionDirection direction = TransitionDirection.None;
    private float transitionParameter;
    private static readonly int Transition = Shader.PropertyToID("_transition");
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        instancedMaterial = new Material(rawImage.material);
        rawImage.material = instancedMaterial;
        transitionParameter = instancedMaterial.GetFloat(Transition);
    }
    void Update()
    {
        switch (direction)
        {
            case TransitionDirection.None:
                break;
            case TransitionDirection.Forward:
                transitionParameter += Time.unscaledDeltaTime*transitionSpeed;
                if (transitionParameter > 0.15)
                {
                    transitionSpeed += Time.unscaledDeltaTime*transitionAcceleration;
                }
                break;
            case TransitionDirection.Backward:
                transitionParameter -= Time.unscaledDeltaTime*transitionSpeed;
                if (transitionParameter > 0.15)
                {
                    transitionSpeed += Time.unscaledDeltaTime*transitionAcceleration;
                }
                break;
        }
        instancedMaterial.SetFloat(Transition, transitionParameter);
    }
    public void TransitionForward()
    {
        direction = TransitionDirection.Forward;
    }
    public void TransitionBackward()
    {
        direction = TransitionDirection.Backward;
    }
}
