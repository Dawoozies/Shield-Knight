using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTrailingEffect : MonoBehaviour
{
    [Serializable]
    public class MagnitudeBuffer
    {
        public float value;
        public float bufferTime;
        public float bufferWriteSpeed;
        public float timeLeft;
        public void Write(float currentValue)
        {
            if(currentValue >= value)
            {
                value = currentValue;
                timeLeft = bufferTime;
                return;
            }
            if(currentValue < value)
            {
                if(timeLeft <= 0)
                {
                    value = currentValue;
                }
            }
        }
        public void Update(float timeDelta)
        {
            timeLeft -= timeDelta * bufferWriteSpeed;
        }
    }

    public float minVelocityMagnitude;
    public float maxVelocityMagnitude;

    public MagnitudeBuffer magnitudeBuffer;

    public int totalPoolSize;
    public int trailLengthDefault;
    public int trailLength;
    public float emitTime;
    float emitTimer;
    public float trailFadeSpeed;
    GameObject trailObject;
    SpriteRenderer spriteRenderer;
    Transform trailParent;
    List<Trail> readyToUse = new();
    List<Trail> activeTrails = new();
    Rigidbody2D parentRigidbody;
    public class Trail
    {
        public SpriteRenderer renderer;
        public float activeTime;
        public Trail(SpriteRenderer renderer)
        {
            this.renderer = renderer;
            activeTime = 0;
        }
        public void Activate(float activeTime, Sprite sprite, Color baseColor, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            renderer.color = baseColor;
            renderer.sprite = sprite;
            renderer.transform.position = pos;
            renderer.transform.rotation = rot;
            renderer.transform.localScale = scale;
            this.activeTime = activeTime;
        }
        public void Deactivate()
        {
            renderer.color = Color.clear;
            activeTime = 0;
        }
        public void UpdateColor(Color baseColor)
        {
            renderer.color = Color.Lerp(Color.clear, baseColor, activeTime);
        }
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailObject = new GameObject("TrailObject", typeof(SpriteRenderer));
        SpriteRenderer trailObjectSpriteRenderer = trailObject.GetComponent<SpriteRenderer>();
        trailObjectSpriteRenderer.color = Color.clear;

        trailParent = new GameObject().transform;
        for (int i = 0; i < totalPoolSize; i++)
        {
            GameObject trailObjectClone = Instantiate(trailObject, trailParent);
            SpriteRenderer trailRenderer = trailObjectClone.GetComponent<SpriteRenderer>();
            trailRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
            Trail trail = new Trail(trailRenderer);
            readyToUse.Add(trail);
        }

        parentRigidbody = GetComponentInParent<Rigidbody2D>();
        trailLength = trailLengthDefault;
    }
    void Update()
    {
        magnitudeBuffer.Write(parentRigidbody.velocity.magnitude);
        magnitudeBuffer.Update(Time.deltaTime);

        float clampedVelocityMagnitude = Mathf.Clamp(magnitudeBuffer.value, minVelocityMagnitude, maxVelocityMagnitude);
        float lengthMultiplier = Mathf.InverseLerp(minVelocityMagnitude, maxVelocityMagnitude, clampedVelocityMagnitude);
        trailLength = trailLengthDefault + Mathf.FloorToInt(totalPoolSize * lengthMultiplier);
        trailLength = Mathf.Clamp(trailLength, trailLengthDefault, totalPoolSize);

        if (emitTimer < emitTime)
        {
            emitTimer += Time.deltaTime * (1 + lengthMultiplier);
        }
        else
        {
            emitTimer = 0 + lengthMultiplier*emitTime;
            if(readyToUse.Count > 0 && activeTrails.Count < trailLength)
            {
                UseFromPool();
            }
        }
        List<Trail> toRemoveFromActive = new();
        foreach (Trail trail in activeTrails)
        {
            trail.activeTime -= Time.deltaTime * trailFadeSpeed * (1 + lengthMultiplier);
            trail.UpdateColor(spriteRenderer.color);
            if(trail.activeTime <= 0)
            {
                trail.Deactivate();
                readyToUse.Add(trail);
                toRemoveFromActive.Add(trail);
            }
        }
        foreach (Trail toRemove in toRemoveFromActive)
        {
            activeTrails.Remove(toRemove);
        }
    }
    void UseFromPool()
    {
        Trail poolObject = readyToUse[0];
        poolObject.Activate(1f, spriteRenderer.sprite, spriteRenderer.color, parentRigidbody.transform.position, parentRigidbody.transform.rotation, spriteRenderer.transform.localScale);

        activeTrails.Add(poolObject);
        readyToUse.RemoveAt(0);
    }
}
