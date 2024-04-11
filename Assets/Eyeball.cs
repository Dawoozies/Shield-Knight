using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics2DFunctions;
public class Eyeball : MonoBehaviour, IOnHit
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
    public Light beamLight;
    public AnimationCurve beamLightRangeCurve;
    public AnimationCurve beamLightIntensityCurve;
    public Light beamEndLight;
    public AnimationCurve beamEndLightRangeCurve;
    public AnimationCurve beamEndLightIntensityCurve;
    float lightCharge;
    public float lookSmoothTime;
    private Vector3 lookVelocity;
    private bool isDead;
    public Transform lightShaftParent;
    private MeshRenderer[] lightShafts;
    private List<Material> instancedMaterials = new();
    private BoxCastInfo lightShaftCast = new();
    public LayerMask lightShaftLayers;
    public enum BeamFiringState
    {
        Idle,Charging, Firing, Cooldown
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.colorGradient = beamGradient;
        lightShafts = lightShaftParent.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in lightShafts)
        {
            Material instancedMaterial = new Material(meshRenderer.material);
            meshRenderer.material = instancedMaterial;
            instancedMaterials.Add(instancedMaterial);
        }
    }

    void DeadUpdate()
    {
        
    }
    void Update()
    {
        if(player == null)
        {
            player = GameManager.GetActivePlayer();
            return;
        }

        if (isDead)
        {
            return;
        }



        Vector3 playerDistVector = player.transform.position - transform.position;
        Vector3 lookTarget = playerDistVector.normalized + Vector3.forward * zLookDepth;
        transform.forward = Vector3.SmoothDamp(transform.forward, lookTarget, ref lookVelocity, lookSmoothTime);
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
        //update light shaft cast info
        lightShaftCast.Origin = shiftedOrigin;
        lightShaftCast.Direction = fwdProjOntoRight;
        lightShaftCast.Distance = 1000f;
        lightShaftCast.Size = castSize;
        lightShaftCast.Layers = lightShaftLayers;
        lightShaftCast.Cast(false);
        float maxLightShaftDistance = 250f;
        if (lightShaftCast.Hit)
        {
            maxLightShaftDistance = lightShaftCast.Hit.distance;
        }

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
            lineRenderer.enabled = false;
        }
        
        UpdateChargeState();
        UpdateFiringState();
        UpdateCooldownState();
        
        foreach (var material in instancedMaterials)
        {
            material.SetFloat("_originPosX", transform.position.x);
            material.SetFloat("_originPosY", transform.position.y);
            material.SetFloat("_originPosZ", transform.position.z);

            material.SetFloat("_distanceFromOriginCutoff", maxLightShaftDistance);      
        }
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
        lightCharge += Time.deltaTime;
        lightCharge = Mathf.Clamp01(lightCharge);

        beamLength += Time.deltaTime*beamLengthIncreaseSpeed;
        beamLength = Mathf.Clamp01(beamLength);
        
        lineRenderer.SetPosition(0, transform.position);
        beamCastInfo.Cast(false);
        //hit = Physics2D.BoxCast(beamCastInfo.Origin, beamCastInfo.Size, beamCastInfo.Angle, beamCastInfo, distance, castHitLayers);
        if (hit)
        {
            lineRenderer.SetPosition(1, hit.centroid);
            UpdateLights(hit.centroid);
            IHitReceiver hitReceiver = hit.collider.GetComponent<IHitReceiver>();
            hitReceiver?.ApplyForce(beamCastInfo.Direction*100f);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward*beamCastInfo.Distance);
            UpdateLights(transform.position + transform.forward * beamCastInfo.Distance);
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
        lightCharge -= Time.deltaTime*10f;
        lightCharge = Mathf.Clamp01(lightCharge);
        beamLength -= Time.deltaTime * beamLengthDecreaseSpeed;
        float zeroPosDistance = 100f * (1-beamLength);
        float endPosDistance = 100f;
        if (hit)
        {
            zeroPosDistance = hit.distance * (1 - beamLength);
            endPosDistance = hit.distance;

            UpdateLights(hit.centroid);
        }
        else
        {
            UpdateLights(transform.position + transform.forward * beamCastInfo.Distance);
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
        lineRenderer.enabled = true;
        beamState = BeamFiringState.Firing;
        beamFiring = 0;
        firingFx.Play(true);
        chargeFx.Stop();
        beamLength = 0;
    }

    void BeamStateStartCooldown()
    {
        lineRenderer.enabled = false;
        beamState = BeamFiringState.Cooldown;
        cooldown = 0;
        firingEndFx.Play(true);
        firingFx.Stop();
        beamLength = 1;
    }
    void UpdateLights(Vector3 hitPoint)
    {
        beamLight.range = beamLightRangeCurve.Evaluate(lightCharge);
        beamLight.intensity = beamLightIntensityCurve.Evaluate(lightCharge);
        beamEndLight.range = beamEndLightRangeCurve.Evaluate(lightCharge);
        beamEndLight.intensity = beamEndLightIntensityCurve.Evaluate(lightCharge);
        beamEndLight.transform.position = hitPoint;
    }

    public Collider2D col { get; }
    public void Hit(ShieldSystemType systemType, Vector2 shieldDir, float shieldVelocity)
    {
        isDead = true;
    }
}
