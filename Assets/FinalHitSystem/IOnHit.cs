using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnHit
{
    public Collider2D col { get; }
    public void Hit(ShieldSystemType systemType, Vector2 shieldDir, float shieldVelocity);
}