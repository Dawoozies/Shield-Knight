using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : HeldWeapon
{
    MoveSystem moveSystem;
    bool equipped;
    public MoveData equip, idle;
    MoveComponent equipComponent, idleComponent;
    Vector3 mouseWorldPos;

    protected override void Start()
    {
        moveSystem = GetComponent<MoveSystem>();
        moveSystem.SetupData(equip, out equipComponent);
        moveSystem.SetupData(idle, out idleComponent);
        InputManager.RegisterMouseInputCallback((Vector2 mouseWorldPos) => this.mouseWorldPos = mouseWorldPos);
    }
    protected override void Update()
    {
        if(owner != null)
        {
            idleComponent.SetEndPoint(mouseWorldPos);
            transform.right = (mouseWorldPos - owner.transform.position).normalized;
        }
    }
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(!equipped)
        {
            if (!equipComponent.isPlaying && col.tag == "Player")
            {
                equipComponent.SetEndTransform(col.transform);
                equipComponent.PlayFromStart();
                equipComponent.endActions.Add(
                    (MoveEndType endType) =>
                    {
                        equipped = true;
                        idleComponent.SetOriginTransform(col.transform);
                        idleComponent.Play();
                        SetOwner(col.transform);
                    }
                );
            }
            return;
        }
    }
}
