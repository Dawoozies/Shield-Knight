using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeldWeapon : Weapon, IEquip
{
    protected Transform owner;
    List<Action> onEquipEndActions = new();
    public void SetOwner(Transform owner)
    {
        this.owner = owner;
        foreach (var action in onEquipEndActions)
        {
            action();
        }
    }

    public void RegisterEquipEndCallback(Action a)
    {
        onEquipEndActions.Add(a);
    }

    protected override void Start()
    {
        
    }
    protected override void Update()
    {
        
    }
}
public interface IEquip
{
    public void SetOwner(Transform owner);
    public void RegisterEquipEndCallback(Action a);
}