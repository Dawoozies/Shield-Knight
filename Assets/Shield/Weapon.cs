using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    BoxCollider2D weaponBoundingBox;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
public interface IWeapon
{
    //Ready
    //Fire
    //ReturnToIdle
    //
    //RegisterReadyEndCallback
    //RegisterFireEndCallback
    //RegisterReturnToIdleEndCallback
    //
    //SetDamageMultiplier
    //
    //RegisterOnHitCallback
}
//OnHit Components will be another system linked to weapon
//Weapon does animation and registers the collisions
//OnHit actually does something with the collision results