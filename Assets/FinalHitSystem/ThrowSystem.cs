using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSystem : ShieldSystem
{
    public void SetColliderActive(bool value)
    {
        col.enabled = value;
    }
}