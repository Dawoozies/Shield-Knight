using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class Weapon : MonoBehaviour, IWeapon
{
    BoxCollider2D weaponBoundingBox;
    List<Action> onReadyEndActions = new();
    List<Action> onFireEndActions = new();
    List<Action> onReturnToIdleEndActions = new();
    public class OnHitInfo
    {
        public string tag;
        public Collider2D collider;
        public OnHitInfo(Collider2D collider)
        {
            tag = collider.tag;
            this.collider = collider;
        }
    }
    public List<Action<OnHitInfo>> onHitActions = new();
    protected virtual void Start()
    {
        weaponBoundingBox = GetComponent<BoxCollider2D>();
    }
    protected virtual void Update()
    {
        
    }
    public virtual void Use()
    {
        throw new NotImplementedException();
    }

    public void RegisterReadyEndCallback(Action a)
    {
        onReadyEndActions.Add(a);
    }

    public void RegisterFireEndCallback(Action a)
    {
        onFireEndActions.Add(a);
    }

    public void RegisterReturnToIdleEndCallback(Action a)
    {
        onReturnToIdleEndActions.Add(a);
    }

    public void RegisterOnHitCallback(Action<OnHitInfo> a)
    {
        onHitActions.Add(a);
    }
}
public interface IWeapon
{
    public void Use();
    public void RegisterReadyEndCallback(Action a);
    public void RegisterFireEndCallback(Action a);
    public void RegisterReturnToIdleEndCallback(Action a);
    public void RegisterOnHitCallback(Action<Weapon.OnHitInfo> a);
}
//OnHit Components will be another system linked to weapon
//Weapon does animation and registers the collisions
//OnHit actually does something with the collision results