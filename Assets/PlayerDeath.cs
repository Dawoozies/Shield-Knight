using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerDeath : MonoBehaviour
{
    public ParticleSystem system;
    private Action systemCompleteCallback;
    private bool started;
    public SpriteRenderer spriteRenderer;
    public void StartSystem(Action a)
    {
        if (!started)
        {
            systemCompleteCallback = a;
            system.Play();
            started = true;
        }
    }

    private void Update()
    {
        if (started)
        {
            spriteRenderer.color = Color.clear;
            if (!system.isPlaying)
            {
                systemCompleteCallback?.Invoke();
                started = false;
                system.Stop();
            }
        }
    }
}
