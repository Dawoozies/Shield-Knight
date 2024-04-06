using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingRangedEnemy : MonoBehaviour, IOnHit
{
    public Collider2D col { get => boxCollider; }
    BoxCollider2D boxCollider;
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    public void Hit(ShieldSystemType systemType, Vector2 shieldDir, float shieldVelocity)
    {
    }
}
