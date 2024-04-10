using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager ins;
    public Transform backgroundParent;
    MaterialSwap[] materialSwaps;

    CameraManager cameraManager;
    public Vector2 effectAxis;
    public float amplitude;

    public GameObject bloodFXPrefab;
    public int bloodSystemMax;
    List<ParticleSystem> bloodSystems = new();

    HitStunColorSwap[] hitStunColorsSwaps;
    private float cameraShakeRequest;

    public GameObject spiritProjectileHit;
    public int spiritProjectileHitMax;
    private List<ParticleSystem> spiritProjectileHitSystems = new();
    
    private void Awake()
    {
        ins = this;
        if (backgroundParent != null)
        {
            materialSwaps = backgroundParent.GetComponentsInChildren<MaterialSwap>();
        }
        cameraManager = GetComponent<CameraManager>();

        for (int i = 0; i < bloodSystemMax; i++)
        {
            GameObject clone = Instantiate(bloodFXPrefab, transform);
            bloodSystems.Add(clone.GetComponent<ParticleSystem>());
        }
        
        for (int i = 0; i < spiritProjectileHitMax; i++)
        {
            GameObject clone = Instantiate(spiritProjectileHit, transform);
            spiritProjectileHitSystems.Add(clone.GetComponent<ParticleSystem>());
        }

        hitStunColorsSwaps = FindObjectsOfType<HitStunColorSwap>();
    }
    float timeStopRequest;
    private void Update()
    {
        if (timeStopRequest > 0)
        {
            timeStopRequest -= Time.unscaledDeltaTime;
            Time.timeScale = 0f;

            HitFXMaterialSwap(MaterialSwap.SwapState.Mono);
        }
        else
        {
            Time.timeScale = 1f;
            HitFXMaterialSwap(MaterialSwap.SwapState.Default);
        }

        if (cameraShakeRequest > 0)
        {
            cameraShakeRequest -= Time.unscaledDeltaTime;
            CameraShake(amplitude);
        }
        else
        {
            CameraShake(0f);
        }
    }
    public void RequestTimeStop(float requestTime)
    {
        if(timeStopRequest > 0)
        {
            return;
        }
        timeStopRequest = requestTime;
    }
    public void RequestBloodFX(Vector2 pos)
    {
        foreach (var ps in bloodSystems)
        {
            if(ps.isPlaying)
            {
                continue;
            }
            ps.transform.position = pos;
            ps.Play();
            break;
        }
    }

    public void RequestSpiritProjectileHit(Vector2 pos, Vector2 hitNormal)
    {
        foreach (var ps in spiritProjectileHitSystems)
        {
            if(ps.isPlaying)
            {
                continue;
            }
            ps.transform.position = pos;
            ps.transform.forward = hitNormal;
            ps.Play();
            break;
        }          
    }
    public void RequestCameraShake(float shakeTime)
    {
        this.cameraShakeRequest = shakeTime;
    }
    public void HitFXMaterialSwap(MaterialSwap.SwapState state)
    {
        if (backgroundParent == null)
            return;
        foreach (var materialSwap in materialSwaps)
        {
            materialSwap.SwapMaterial(state);
        }
    }
    public void RequestHitStunFX(Collider2D hitCollider)
    {
        if (backgroundParent == null)
            return;
        foreach (var hitStunColorSwap in hitStunColorsSwaps)
        {
            if(hitStunColorSwap.gameObject == hitCollider.gameObject)
            {
                hitStunColorSwap.DoSwap();
                break;
            }
        }
    }
    public void CameraShake(float shakeAmplitude)
    {
        float xRandom = Random.Range(0f, 2 * Mathf.PI);
        float yRandom = Random.Range(0f, 2 * Mathf.PI);
        cameraManager.cameraOffset = new Vector2(
                Mathf.Cos(xRandom),
                Mathf.Sin(yRandom)
            ) * shakeAmplitude;
        cameraManager.cameraOffset = Vector2.Scale(effectAxis, cameraManager.cameraOffset);
    }
}
