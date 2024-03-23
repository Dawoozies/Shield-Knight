using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager ins;
    private void Awake()
    {
        ins = this;
    }
    float timeStopRequest;
    private void Update()
    {
        if (timeStopRequest > 0)
        {
            timeStopRequest -= Time.unscaledDeltaTime;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    public void RequestTimeStop(float requestTime)
    {
        if(timeStopRequest > 0)
        {
            return;
        }
        timeStopRequest = requestTime;
    }
}
