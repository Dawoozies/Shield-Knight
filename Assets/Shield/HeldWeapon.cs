using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeldWeapon : Weapon, IEquip
{
    Transform owner;
    List<Action> onEquipEndActions = new();
    public void Equip(Transform owner)
    {
        throw new NotImplementedException();
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
    public void Equip(Transform owner);
    public void RegisterEquipEndCallback(Action a);
}