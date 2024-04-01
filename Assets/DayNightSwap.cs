using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSwap : MonoBehaviour
{
    public Material[] paletteSwapMaterials;
    [Serializable]
    public enum State
    {
        Day,Night
    }
    public State state;
    private static readonly int recolorPropID = Shader.PropertyToID("_RecolorActive");
    private void Awake()
    {
        switch (state)
        {
            case State.Day:
                foreach (var material in paletteSwapMaterials)
                {
                    material.SetFloat(recolorPropID, 0f);
                }
                break;
            case State.Night:
                foreach (var material in paletteSwapMaterials)
                {
                    material.SetFloat(recolorPropID, 1f);
                }
                break;
        }
    }
}
