using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : HeldWeapon
{
    MoveSystem moveSystem;
    bool equipped;
    public MoveData equip;
    MoveComponent equipComponent;
    protected override void Start()
    {
        moveSystem = GetComponent<MoveSystem>();
        moveSystem.SetupData(equip, out equipComponent);
    }
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(!equipped)
        {
            if (!equipComponent.isPlaying && col.tag == "Player")
            {
                equipComponent.SetTarget(col.transform);
                equipComponent.PlayFromStart();
                equipComponent.endActions.Add(
                    (MoveEndType endType) =>
                    {
                        equipped = true;
                        moveSystem.SetSystemAnchor(col.transform);
                        Debug.Log("Player equipped shield");
                    }
                );
            }
            return;
        }
    }
}
