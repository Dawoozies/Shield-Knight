using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearHitTarget : MonoBehaviour, IOnHit
{
    public Collider2D col => throw new System.NotImplementedException();
    BoxCollider2D _col;
    Action<ShieldSystemType, Vector2, float, float> hitCallback;
    public void RegisterHitCallback(Action<ShieldSystemType, Vector2, float, float> a)
    { hitCallback = a; }
    void Start()
    {
        _col = GetComponent<BoxCollider2D>();
    }
    public void Hit(ShieldSystemType systemType, Vector2 shieldDir, float shieldVelocity)
    {
        float hitDotProduct = Vector2.Dot(shieldDir, transform.up);
        hitCallback?.Invoke(systemType, shieldDir, shieldVelocity, hitDotProduct);
    }
}
