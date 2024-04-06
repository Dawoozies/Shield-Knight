using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHitReceiver : MonoBehaviour, IOnHit
{
    public Collider2D col => _col;
    BoxCollider2D _col;
    public void Hit(ShieldSystemType systemType, Vector2 shieldDir, float shieldVelocity)
    {
    }
    void Start()
    {
        _col = GetComponent<BoxCollider2D>();
    }
}
