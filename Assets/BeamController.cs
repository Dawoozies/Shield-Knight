using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    BeamCast[] beamCasts;
    public float beamDistance;
    public Color beamColor;
    public bool on;
    void Start()
    {
        beamCasts = GetComponentsInChildren<BeamCast>();
    }
    void Update()
    {
        foreach (var beam in beamCasts)
        {
            if(on)
            {
                beam.distance = beamDistance;
            }
            else
            {
                beam.distance = 0f;
            }
            beam.lineRenderer.startColor = beamColor;
            beam.lineRenderer.endColor = beamColor;
        }
    }
    public void SwitchOn() { on = true; }
    public void SwitchOff() { on = false; }
}
