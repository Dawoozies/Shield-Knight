using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamDamage : MonoBehaviour
{
    BeamCast[] beamCasts;
    public float beamDamage;
    public float damageCooldown;
    float cooldownTimer;
    public List<string> entityTags;
    void Start()
    {
        beamCasts = GetComponentsInChildren<BeamCast>();
        foreach (var beam in beamCasts)
        {
            beam.RegisterCastColliderHitCallback(BeamColliderHit);
        }
    }
    void Update()
    {
        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    void BeamColliderHit(Vector2 beamDir, Collider2D col)
    {
        if(cooldownTimer <= 0)
        {
            IHitReceiver hitReceiver = col.GetComponent<IHitReceiver>();
            if(hitReceiver != null && entityTags.Contains(col.tag))
            {
                hitReceiver.ApplyForce(beamDir * beamDamage);
                cooldownTimer = damageCooldown;
            }
        }
    }
}
