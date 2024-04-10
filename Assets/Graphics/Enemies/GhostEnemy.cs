using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorCalculations;

public class GhostEnemy : MonoBehaviour, IOnHit
{
    public float detectionRadius;
    private Rigidbody2D rb;
    private BoxCollider2D _col;
    private Player player;
    public Vector2 velocity;
    public LayerMask hardSurfaceLayers;
    public float distDelta;
    public float bounceFactor;
    public float timeTillDeathAfterBounce;
    private float deathTime;
    public bool heldHit;
    public bool throwHit;
    private bool startDeath;
    private SpriteRenderer[] spriteRenderers;
    private GhostEyeShake eyeShake;
    public ParticleSystem spiritProjectileSystem;
    public float fireSpeed;
    public int maxProjectiles;
    float fireTime;
    public ParticleSystem spiritProjectileHitSystem;
    void Start()
    {
        deathTime = timeTillDeathAfterBounce;
        rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        eyeShake = GetComponentInChildren<GhostEyeShake>();
    }
    void Update()
    {
        if (player == null)
        {
            player = GameManager.GetActivePlayer();
            return;
        }

        if (startDeath)
        {
            deathTime -= Time.deltaTime;
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = Color.Lerp(Color.clear, Color.red, deathTime/timeTillDeathAfterBounce);
            }
            if (deathTime <= 0)
            {
                //death
                gameObject.SetActive(false);
            }
        }

        fireTime -= Time.deltaTime * fireSpeed;
        if(fireTime < 0)
        {
            fireTime = 1;
            spiritProjectileSystem.Play();
        }
    }

    private void FixedUpdate()
    {
        
        Vector2 dv = velocity * Time.fixedDeltaTime;
        Vector3 nextPos = rb.position + dv;
        float angle = Angle.AngleFromXAxis(dv.normalized);
        RaycastHit2D moveHit = Physics2D.BoxCast(rb.position,_col.size,angle,dv.normalized,dv.magnitude, hardSurfaceLayers);
        if (moveHit)
        {
            nextPos = moveHit.centroid + moveHit.normal*distDelta;
            velocity = Vector2.Reflect(velocity, moveHit.normal)*bounceFactor;
            if (heldHit)
            {
                startDeath = true;
            }
        }
        rb.MovePosition(nextPos);
    }

    public Collider2D col { get => _col; }
    public void Hit(ShieldSystemType systemType, Vector2 shieldDir, float shieldVelocity)
    {
        if (systemType == ShieldSystemType.Held)
        {
            //then push ghost in direction
            velocity = shieldDir * shieldVelocity;
            Debug.LogWarning($"velocity = {velocity}");
            heldHit = true;
            EffectsManager.ins.RequestCameraShake(0.5f);
            EffectsManager.ins.RequestBloodFX(rb.position);
            eyeShake.StartShake();
        }

        if (systemType == ShieldSystemType.Throw)
        {
            //dont push ghost just kill
            throwHit = true;
            startDeath = true;
            EffectsManager.ins.RequestCameraShake(0.2f);
            EffectsManager.ins.RequestBloodFX(rb.position);
            eyeShake.StartShake();
        }
    }

    public void ResetEnemy()
    {
        deathTime = timeTillDeathAfterBounce;
        heldHit = false;
        throwHit = false;
        startDeath = false;
        velocity = Vector2.zero;
        gameObject.SetActive(true);
        spriteRenderers[0].color = Color.white;
        spriteRenderers[1].color = Color.white;
        eyeShake.Reset();
    }
}
