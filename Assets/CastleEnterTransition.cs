using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleEnterTransition : MonoBehaviour
{
    public Transform hallwayEndTarget;
    public Transform lookUpTarget;
    public float lookParameter;
    public float lookTime;
    public float lookSpeed;
    public float headBobFrequency;
    public float headBobAmplitude;
    public float walkSpeed;
    private float headBob;
    public float originalFogDensity;
    public float endFogDensity;
    public float fogLerpTime;
    private float fogLerp;
    public string sceneToTransitionTo;
    private void Start()
    {
        RenderSettings.fogDensity = originalFogDensity;
    }

    void Update()
    {
        Vector3 targetPos = Vector3.Slerp(lookUpTarget.position, hallwayEndTarget.position, lookParameter);
        transform.forward = (targetPos - transform.position).normalized;
        if (lookParameter < 1f)
        {
            lookParameter += Time.unscaledDeltaTime*lookSpeed;
        }
        else
        {
            WalkForwards();
        }

        if (fogLerp >= 1)
        {
            if (sceneToTransitionTo.Length >= 0)
            {
                SceneManager.LoadScene(sceneToTransitionTo, LoadSceneMode.Single);
            }
        }
    }
    void WalkForwards()
    {
        Vector3 walkVector = Vector3.forward * walkSpeed * Time.unscaledDeltaTime;
        headBob += walkSpeed * Time.unscaledDeltaTime;
        Vector3 headBobVector = Vector3.up * Mathf.Sin(headBob * headBobFrequency) * headBobAmplitude * Time.unscaledDeltaTime;
        transform.Translate(walkVector + headBobVector);

        RenderSettings.fogDensity = Mathf.Lerp(originalFogDensity, endFogDensity, easeInExp(fogLerp));
        if (fogLerp < 1)
        {
            fogLerp += Time.unscaledDeltaTime * 1/fogLerpTime;
        }
    }

    float easeInExp(float t)
    {
        return t == 0 ? 0 : Mathf.Pow(2, 10f * t - 10f);
    }
}
