using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSystem : ShieldSystem
{
    protected override void Start()
    {
        base.Start();
        debug = true;
    }
    public void SetColliderActive(bool value)
    {
        col.enabled = value;
    }
}