using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallSystem : ShieldSystem
{
    //if we holding down recall then end path is player always and nothing will stop shield
    //if we let go at any point we restart the system
    public bool recallButtonHeldDown;
    const float catchRadius = 0.75f;
    Vector2 recallSystemStartingPosition;
    public bool systemActivatedPreviously;
    Player player => GameManager.GetActivePlayer();
    public override bool SystemComplete()
    {
        return ShieldCaught();
    }
    protected override void FixedUpdate()
    {
        //Move towards player at systemVelocity
        //cast at each step and do IOnHit effects etc
        //but do not stop
        Vector2 v = player.transform.position - transform.position;
        float distToPlayer = v.magnitude;
        Vector2 dv = v.normalized*systemVelocity*Time.fixedUnscaledDeltaTime;
        if(distToPlayer < catchRadius*4f)
        {
            dv = Vector2.ClampMagnitude(dv, distToPlayer);
        }
        Vector2 nextPos = rb.position + dv;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position, col.size, Angle.AngleFromXAxis(dv), dv.normalized, dv.magnitude, validCastLayers);
        foreach (var hit in hits)
        {
            IOnHit onHit = hit.collider.GetComponent<IOnHit>();
            onHit?.Hit(ShieldSystemType.Recall, transform.right, systemVelocity);
        }
        rb.MovePosition(nextPos);
        transform.right = dv;
    }
    public bool ShieldCaught()
    {
        return Vector3.Distance(player.transform.position, transform.position) < catchRadius;
    }
    public void ActivateRecall()
    {
        if(!systemActivatedPreviously)
        {
            transform.position = recallSystemStartingPosition;
            systemActivatedPreviously = true;
        }
        transform.right = player.transform.position-transform.position;
    }
    public void SetUpRecallSystem(Vector2 throwPosition)
    {
        recallSystemStartingPosition = throwPosition;
        systemActivatedPreviously = false;
    }
}