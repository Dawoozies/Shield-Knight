using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallSystem : ShieldSystem
{
    public void UpdateRecallParameters(Vector3 hitPointEnd)
    {
        this.hitPointEnd = hitPointEnd;
        hitTimeEnd = Vector3.Distance(hitPointStart, hitPointEnd)/systemVelocity;
    }
}