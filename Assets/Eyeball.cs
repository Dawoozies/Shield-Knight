using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics2DFunctions;
public class Eyeball : MonoBehaviour
{
    Player player;
    public float viewRadius;
    public LayerMask castHitLayers;
    public float chargeTime;
    public float beamFiringTime;
    public float cooldownTime;
    float charge;
    float beamFiring;
    float cooldown;
    public float zLookDepth;
    public bool debug;
    private bool playerInRange;
    public ParticleSystem chargeFx;
    public ParticleSystem firingFx;
    public ParticleSystem firingEndFx;
    public BeamFiringState beamState;
    private LineRenderer lineRenderer;
    public Vector2 castSize;
    private RaycastHit2D hit => beamCastInfo.Hit;
    private RaycastHit2D viewHit => viewCastInfo.Hit;
    private float beamLength;
    public float beamLengthIncreaseSpeed;
    public float beamLengthDecreaseSpeed;
    private BoxCastInfo beamCastInfo = new ();
    private BoxCastInfo viewCastInfo = new ();
    public LayerMask viewCastLayers;
    public Gradient beamGradient;
    public ParticleSystem beamHitSystem;
    public enum BeamFiringState
    {
        Idle,Charging, Firing, Cooldown
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if(player == null)
        {
            player = GameManager.GetActivePlayer();
            return;
        }
        Vector3 playerDistVector = player.transform.position - transform.position;
        transform.forward = playerDistVector.normalized + Vector3.forward * zLookDepth;
        Vector2 projDistVector = playerDistVector;
        if (projDistVector.magnitude < viewRadius)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        Vector2 shiftedOrigin = transform.position - Vector3.forward * transform.localScale.z / 2f;
        Vector2 fwdProjOntoRight =Vector2.Dot(transform.forward, Vector2.right)*Vector2.right + Vector2.Dot(transform.forward, Vector2.up)*Vector2.up;
        //UpdateViewCastInfo
        viewCastInfo.Origin = shiftedOrigin;
        viewCastInfo.Direction = fwdProjOntoRight;
        viewCastInfo.Distance = viewRadius;
        viewCastInfo.Size = castSize;
        viewCastInfo.Layers = viewCastLayers;
        //UpdateBeamCastInfo
        beamCastInfo.Origin = shiftedOrigin;
        beamCastInfo.Direction = Vector2.Dot(transform.forward, Vector2.right)*Vector2.right + Vector2.Dot(transform.forward, Vector2.up)*Vector2.up;
        beamCastInfo.Distance = 100f * beamLength;
        beamCastInfo.Size = castSize;
        beamCastInfo.Layers = castHitLayers;

        if (playerInRange && beamState == BeamFiringState.Idle)
        {
            
            viewCastInfo.Cast(false);
            if (viewHit)
            {
                if (viewHit.collider.CompareTag("Player"))
                {
                    BeamStateStartCharge();           
                }
            }
        }

        if (beamState == BeamFiringState.Idle)
        {
            lineRenderer.colorGradient.alphaKeys[0].alpha = 0;
            lineRenderer.colorGradient.alphaKeys[1].alpha = 0;
        }
        
        UpdateChargeState();
        UpdateFiringState();
        UpdateCooldownState();


    }

    void UpdateChargeState()
    {
        if (beamState != BeamFiringState.Charging)
            return;
        charge += Time.deltaTime;
        if (charge > chargeTime)
        {
            BeamStateStartFiring();
        }
    }

    void UpdateFiringState()
    {
        if (beamState != BeamFiringState.Firing)
            return;
        lineRenderer.colorGradient = beamGradient;
        beamLength += Time.deltaTime*beamLengthIncreaseSpeed;
        beamLength = Mathf.Clamp01(beamLength);
        
        lineRenderer.SetPosition(0, transform.position);
        beamCastInfo.Cast(false);
        //hit = Physics2D.BoxCast(beamCastInfo.Origin, beamCastInfo.Size, beamCastInfo.Angle, beamCastInfo, distance, castHitLayers);
        if (hit)
        {
            lineRenderer.SetPosition(1, hit.centroid);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward*beamCastInfo.Distance);
        }
        beamFiring += Time.deltaTime;
        if (beamFiring > beamFiringTime)
        {
            BeamStateStartCooldown();
        }
        
        //Beam Hit FX
        if (hit)
        {
            beamHitSystem.transform.position = hit.centroid;
            beamHitSystem.transform.forward = hit.normal;
            if (!beamHitSystem.isPlaying)
            {
                beamHitSystem.Play();
            }
        }
    }

    void UpdateCooldownState()
    {
        if (beamState != BeamFiringState.Cooldown)
            return;
        if (beamHitSystem.isPlaying)
        {
            beamHitSystem.Stop();
        }
        beamLength -= Time.deltaTime * beamLengthDecreaseSpeed;
        float zeroPosDistance = 100f * (1-beamLength);
        float endPosDistance = 100f;
        if (hit)
        {
            zeroPosDistance = hit.distance * (1 - beamLength);
            endPosDistance = hit.distance;
        }

        beamLength = Mathf.Clamp01(beamLength);
        lineRenderer.SetPosition(0, transform.position + transform.forward*zeroPosDistance);
        lineRenderer.SetPosition(1, transform.position + transform.forward*endPosDistance);
        cooldown += Time.deltaTime;
        if (cooldown > cooldownTime)
        {
            beamState = BeamFiringState.Idle;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if(!debug)
        {
            return;
        }

        if (playerInRange)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.color = Color.white;
        if (hit)
        {
            if (hit.collider != null)
            {
                BoxCast2DGizmo.BoxCast(beamCastInfo);
            }
        }

        if (viewHit)
        {
            if (viewHit.collider.CompareTag("Player"))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }
        }
        BoxCast2DGizmo.BoxCast(viewCastInfo);
    }

    void BeamStateStartCharge()
    {
        beamState = BeamFiringState.Charging;
        charge = 0;
        chargeFx.Play(true);
    }

    void BeamStateStartFiring()
    {
        beamState = BeamFiringState.Firing;
        beamFiring = 0;
        firingFx.Play(true);
        chargeFx.Stop();
        beamLength = 0;
    }

    void BeamStateStartCooldown()
    {
        beamState = BeamFiringState.Cooldown;
        cooldown = 0;
        firingEndFx.Play(true);
        firingFx.Stop();
        beamLength = 1;
    }
}
