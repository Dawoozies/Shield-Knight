using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class TextEffect : ScriptableObject
{
    public AnimationCurve lerpCurve;
    public TextEffectFlags flags;
    public Vector2 positionA,positionB;
    public Vector3 eulerAnglesA, eulerAnglesB;
    public Vector3 scaleA, scaleB;
    public Color colorA, colorB;
}

[Flags]
public enum TextEffectFlags
{
    None = 0,
    Translation = 1,
    Rotation = 1 << 1,
    Scale = 1 << 2,
    Color = 1 << 3,
}