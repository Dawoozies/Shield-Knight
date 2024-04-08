using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyeball : MonoBehaviour
{
    Player player;
    public float viewRadius;
    public LayerMask castHitLayers;
    public float chargeTime;
    public float beamFiringTime;
    public float cooldownTime;
    float charge;
    float beamFiringTimeLeft;
    float cooldown;
    public float zLookDepth;
    public bool debug;
    void Update()
    {
        if(player == null)
        {
            player = GameManager.GetActivePlayer();
            return;
        }
        transform.forward = (player.transform.position - transform.position).normalized + Vector3.forward * zLookDepth;
    }
    private void OnDrawGizmosSelected()
    {
        if(!debug)
        {
            return;
        }
        Gizmos.color = Color.green*0.75f;
        Gizmos.DrawSphere(transform.position, viewRadius);
    }
}
