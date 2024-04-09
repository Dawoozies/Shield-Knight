using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEyeShake : MonoBehaviour
{
    public bool shake;
    public float shakeRadius;
    private Vector3 origin;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        shake = false;
        origin = transform.localPosition;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (!shake)
        {
            transform.localPosition = origin;
            spriteRenderer.color = Color.clear;
            return;
        }
        float xRand = Random.Range(0f, 1f);
        float yRand = Random.Range(0f, 1f);
        transform.localPosition = origin + shakeRadius * Mathf.Cos(Random.Range(0f, 2*Mathf.PI)) * Vector3.right + shakeRadius*Mathf.Sin(Random.Range(0f, 2*Mathf.PI))*Vector3.up;
    }

    public void StartShake()
    {
        spriteRenderer.color = Color.white;
        shake = true;
    }

    public void Reset()
    {
        shake = false;
        spriteRenderer.color = Color.clear;
    }
}
