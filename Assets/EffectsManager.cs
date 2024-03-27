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
    private void Awake()
    {
        ins = this;
        materialSwaps = backgroundParent.GetComponentsInChildren<MaterialSwap>();
        cameraManager = GetComponent<CameraManager>();

        for (int i = 0; i < bloodSystemMax; i++)
        {
            GameObject clone = Instantiate(bloodFXPrefab, transform);
            bloodSystems.Add(clone.GetComponent<ParticleSystem>());
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
            CameraShake(amplitude);
        }
        else
        {
            Time.timeScale = 1f;
            HitFXMaterialSwap(MaterialSwap.SwapState.Default);
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
    public void HitFXMaterialSwap(MaterialSwap.SwapState state)
    {
        foreach (var materialSwap in materialSwaps)
        {
            materialSwap.SwapMaterial(state);
        }
    }
    public void RequestHitStunFX(Collider2D hitCollider)
    {
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
