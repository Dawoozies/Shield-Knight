using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : HeldWeapon
{
    MoveSystem moveSystem;
    protected override void Start()
    {
        moveSystem = GetComponent<MoveSystem>();
    }
}
