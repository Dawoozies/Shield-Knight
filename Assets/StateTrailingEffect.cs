using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTrailingEffect : MonoBehaviour
{
    [Serializable]
    public class TrailState
    {
        public int trailLength;
        public float emissionDelta;
        public float lerpIncrement;
        public float fadeSpeed;
    }
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
    public int totalPoolSize;
    public int state;
    public List<TrailState> states;
    int trailLength;
    float emissionDelta;
    float lerpIncrement;
    float fadeSpeed;
    GameObject trailObject;
    SpriteRenderer spriteRenderer;
    BoxCollider2D parentCollider;
    Transform trailParent;
    List<Trail> readyToUse = new();
    List<Trail> activeTrails = new();
    Vector3 lastPos;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentCollider = GetComponentInParent<BoxCollider2D>();
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
    }
    private void Update()
    {
        UpdateStateVariables();
        trailLength = Mathf.Clamp(trailLength, 0, totalPoolSize);
        float delta = Vector3.Distance(parentCollider.transform.position, lastPos);

        if(delta > emissionDelta)
        {
            float lerpParameter = 0;
            if(lerpIncrement <= 0)
            {
                Debug.LogError("Lerp increment <= 0, while loop will not end. Returning...");
                return;
            }
            while(lerpParameter <= 1)
            {
                Vector3 positionLerp = Vector3.Lerp(lastPos, parentCollider.transform.position, lerpParameter);
                if(readyToUse.Count > 0 && activeTrails.Count < trailLength)
                {
                    UseFromPool(positionLerp, lerpParameter);
                }
                lerpParameter += lerpIncrement;
            }
        }
        List<Trail> toRemoveFromActive = new();
        foreach (Trail trail in activeTrails)
        {
            trail.activeTime -= Time.deltaTime * fadeSpeed;
            trail.UpdateColor(spriteRenderer.color);
            if (trail.activeTime <= 0)
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
        lastPos = parentCollider.transform.position;
    }
    void UseFromPool(Vector3 pos, float activeTime)
    {
        Trail poolObject = readyToUse[0];
        poolObject.Activate(activeTime, spriteRenderer.sprite, spriteRenderer.color, pos, parentCollider.transform.rotation, spriteRenderer.transform.localScale);
        activeTrails.Add(poolObject);
        readyToUse.RemoveAt(0);
    }
    void UpdateStateVariables()
    {
        trailLength = states[state].trailLength;
        emissionDelta = states[state].emissionDelta;
        lerpIncrement = states[state].lerpIncrement;
        fadeSpeed = states[state].fadeSpeed;
    }
}
