using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    BeamCast[] beamCasts;
    public float beamDistance;
    public Color beamColor;
    void Start()
    {
        beamCasts = GetComponentsInChildren<BeamCast>();
    }
    void Update()
    {
        foreach (var beam in beamCasts)
        {
            beam.distance = beamDistance;
            beam.lineRenderer.startColor = beamColor;
            beam.lineRenderer.endColor = beamColor;
        }
    }
}
