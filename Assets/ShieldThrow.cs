using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldThrow : MonoBehaviour
{
    VelocitySystem velocitySystem;
    public VelocityData shieldThrown, shieldRecall;
    VelocityComponent shieldThrownComponent, shieldRecallComponent;
    public Color highForceColor;
    public Color defaultColor;
    public LayerMask hittableLayers;
    public bool thrown => shieldThrownComponent.isPlaying || shieldRecallComponent.isPlaying || embedded;
    Transform recallTransform;
    bool embedded;
    float leftClickHeld;
    const string nonEmbeddedLayer = "Shield";
    const string embeddedLayer = "ShieldThrow";
    void Start()
    {
        velocitySystem = GetComponent<VelocitySystem>();
        velocitySystem.SetupData(shieldThrown, out shieldThrownComponent);
        velocitySystem.SetupData(shieldRecall, out shieldRecallComponent);

        InputManager.RegisterMouseClickCallback((Vector3 mouseClickInput) => leftClickHeld = mouseClickInput.x);
    }
    public void WhileNotThrown(Vector3 pos, Vector3 lookDirection)
    {
        if(shieldThrownComponent.isPlaying)
        {
            return;
        }
        transform.position = pos - (lookDirection)*0.35f;
        transform.up = -lookDirection;
    }
    public void Throw(float throwForce)
    {
        if (shieldThrownComponent.isPlaying)
        {
            return;
        }
        shieldThrownComponent.SetMagnitude(throwForce);
        shieldThrownComponent.SetDirection(-transform.up);
        shieldThrownComponent.PlayFromStart();
    }
    public void Recall(Transform recallTransform)
    {
        this.recallTransform = recallTransform;
        if (!shieldRecallComponent.isPlaying)
        {
            shieldRecallComponent.PlayFromStart();
        }
        shieldThrownComponent.Stop();
    }
    void Update()
    {
        if(embedded)
        {
            gameObject.layer = LayerMask.NameToLayer(embeddedLayer);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer(nonEmbeddedLayer);
        }
        if(recallTransform != null)
        {
            if(shieldRecallComponent.isPlaying)
            {
                Vector3 returnDir = (recallTransform.position - transform.position).normalized;
                float dist = Vector3.Distance(recallTransform.position, transform.position);
                shieldRecallComponent.SetMagnitude(30f + dist * dist);
                shieldRecallComponent.SetDirection(returnDir);
                if(dist < 1.2f)
                {
                    shieldRecallComponent.Stop();
                    embedded = false;
                }
                transform.up = -velocitySystem.finalVelocity;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!shieldThrownComponent.isPlaying && !shieldRecallComponent.isPlaying)
        {
            return;
        }
        if(col.tag == "Enemy")
        {
            //Hit enemy and bounce back
            IEnemy enemy = col.GetComponent<IEnemy>();
            enemy.ApplyDamage(velocitySystem.finalVelocity);
        }
        if (col.tag == "HardSurface")
        {
            if(shieldThrownComponent.isPlaying)
            {
                Embed();
            }
            if(!shieldRecallComponent.isPlaying)
            {
                if(leftClickHeld == 0)
                {
                    Embed();
                }
            }
        }
    }
    void Embed()
    {
        embedded = true;
        shieldThrownComponent.Stop();
        shieldRecallComponent.Stop();
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag == "HardSurface")
        {
            if(embedded)
            {
                embedded = false;
                if(!shieldRecallComponent.isPlaying)
                {
                    shieldRecallComponent.PlayFromStart();
                }
            }
        }
    }
}