using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerCheck
{
    public void RegisterOnHitCallback(Action<List<RaycastHit2D>> action);
}
