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
    private RaycastHit2D reflectHit => beamReflectCastInfo.Hit;
    private float beamLength;
    public float beamLengthIncreaseSpeed;
    public float beamLengthDecreaseSpeed;
    private BoxCastInfo beamCastInfo = new ();
    private BoxCastInfo beamReflectCastInfo = new();
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
    public Transform lightShaftParent;
    private MeshRenderer[] lightShafts;
    private List<Material> instancedMaterials = new();
    private BoxCastInfo lightShaftCast = new();
    public LayerMask lightShaftLayers;
    public Vector2 beamWidthStartEnd = Vector2.one;
    public bool useBeamEndPoint;
    public Transform beamEndPoint;
    public float beamDistance;
    public int beamReflects = 2;
    public LayerMask castReflectLayers;
    public bool hittingReflectiveSurface;
    public bool canBeDamaged;
    public bool isDying;
    public enum BeamFiringState
    {
        Idle,Charging, Firing, Cooldown
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.colorGradient = beamGradient;
        lineRenderer.startWidth = beamWidthStartEnd.x;
        lineRenderer.endWidth = beamWidthStartEnd.y;
        lightShafts = lightShaftParent.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in lightShafts)
        {
            Material instancedMaterial = new Material(meshRenderer.material);
            meshRenderer.material = instancedMaterial;
            instancedMaterials.Add(instancedMaterial);
        }
    }
    float velocity;
    void Update()
    {
        if(player == null)
        {
            player = GameManager.GetActivePlayer();
            return;
        }

        if(isDying)
        {
            if(transform.position.y < -300f)
            {
                return;
            }
            transform.position += Vector3.up * velocity;
            velocity -= Time.deltaTime;
            return;
        }

        Vector3 playerDistVector = player.transform.position - transform.position;
        Vector3 lookTarget = playerDistVector.normalized + Vector3.forward * zLookDepth;
        if(useBeamEndPoint)
        {
            lookTarget = beamEndPoint.position - transform.position;
        }
        Vector2 projDistVector = playerDistVector;
        playerInRange = projDistVector.magnitude < viewRadius;
        if(playerInRange || useBeamEndPoint)
        {
            transform.forward = Vector3.SmoothDamp(transform.forward, lookTarget, ref lookVelocity, lookSmoothTime);
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
        beamCastInfo.Direction = fwdProjOntoRight;
        beamCastInfo.Distance = beamDistance * beamLength;
        beamCastInfo.Size = castSize;
        beamCastInfo.Layers = castHitLayers;

        //update light shaft cast info
        lightShaftCast.Origin = shiftedOrigin;
        lightShaftCast.Direction = fwdProjOntoRight;
        lightShaftCast.Distance = 1000f;
        lightShaftCast.Size = castSize;
        lightShaftCast.Layers = lightShaftLayers;
        lightShaftCast.Cast(false);
        float maxLightShaftDistance = 750f;
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
        Vector3 reflectEndPoint = transform.position;
        //hit = Physics2D.BoxCast(beamCastInfo.Origin, beamCastInfo.Size, beamCastInfo.Angle, beamCastInfo, distance, castHitLayers);
        if (hit)
        {
            lineRenderer.SetPosition(1, hit.centroid);
            UpdateLights(hit.centroid);
            IHitReceiver hitReceiver = hit.collider.GetComponent<IHitReceiver>();
            hitReceiver?.ApplyForce(beamCastInfo.Direction* beamDistance);

            //Only cast the reflection if we hit something
            if (hit.collider.CompareTag("Shield"))
            {
                hittingReflectiveSurface = true;
                lineRenderer.positionCount = 3;
                lineRenderer.SetPosition(2, beamCastInfo.Hit.centroid + beamCastInfo.Hit.normal * beamDistance);

                beamReflectCastInfo.Origin = beamCastInfo.Hit.centroid;
                beamReflectCastInfo.Direction = beamCastInfo.Hit.normal;
                beamReflectCastInfo.Distance = beamDistance;
                beamReflectCastInfo.Size = castSize;
                beamReflectCastInfo.Layers = castReflectLayers;
                beamReflectCastInfo.Cast(false);

                //Debug.LogWarning($"Reflect Surface Hit name = {beamCastInfo.Hit.collider.name} Normal = {beamCastInfo.Hit.normal} Dir = {beamCastInfo.Direction} ReflectVec = {Vector2.Reflect(beamCastInfo.Hit.normal, beamCastInfo.Direction)}");
                if(beamReflectCastInfo.Hit)
                {
                    lineRenderer.SetPosition(2, beamReflectCastInfo.Hit.centroid);
                    IHitReceiver reflectHitReceiver = beamReflectCastInfo.Hit.collider.GetComponentInParent<IHitReceiver>();
                    reflectHitReceiver?.ApplyForce(beamReflectCastInfo.Direction*beamDistance);
                }
                //if(beamReflectCastInfo.Hit)
                //{
                //    Debug.LogWarning("Reflecting beam off shield! Reflection Object = " + beamReflectCastInfo.Hit.collider.name + " normal = ");
                //}

            }
            else
            {
                hittingReflectiveSurface = false;
                lineRenderer.positionCount = 2;
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward*beamCastInfo.Distance);
            UpdateLights(transform.position + transform.forward * beamCastInfo.Distance);
            hittingReflectiveSurface = false;
        }
        if(hittingReflectiveSurface)
        {
            ////then add line renderer point
            //if(reflectHit)
            //{
            //    Debug.Log("THIS RUNNING");
            //    lineRenderer.SetPosition(2, reflectHit.centroid);
            //    IHitReceiver hitReceiver = reflectHit.collider.GetComponentInParent<IHitReceiver>();
            //    hitReceiver?.ApplyForce(beamReflectCastInfo.Direction*beamDistance);
            //}
            //else
            //{
            //    Vector2 reflectedDir = Vector2.Reflect(beamCastInfo.Direction, hit.normal);
            //    lineRenderer.SetPosition(2, hit.centroid + reflectedDir * beamDistance);
            //}
        }
        else
        {
            //if(hit)
            //{
            //    lineRenderer.SetPosition(2, hit.centroid);
            //}
            //else
            //{
            //    lineRenderer.SetPosition(2,transform.position + transform.forward*beamCastInfo.Distance);
            //}
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
        float zeroPosDistance = beamDistance * (1-beamLength);
        float endPosDistance = beamDistance;
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


    private void OnDrawGizmos()
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

        if(useBeamEndPoint)
        {
            Gizmos.color = beamGradient.colorKeys[0].color;
            Gizmos.DrawLine(transform.position, beamEndPoint.position);
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

        if(reflectHit)
        {
            Gizmos.color = Color.cyan;
        }
        else
        {
            Gizmos.color = Color.magenta;
        }
        BoxCast2DGizmo.BoxCast(beamReflectCastInfo);
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
        if(canBeDamaged && !isDying)
        {
            if(systemType == ShieldSystemType.Held)
            {
                EffectsManager.ins.RequestBloodFX(transform.position);
                EffectsManager.ins.RequestBloodFX(transform.position);
                EffectsManager.ins.RequestBloodFX(transform.position);
                EffectsManager.ins.RequestCameraShake(2f);
                isDying = true;
            }
        }
    }
}