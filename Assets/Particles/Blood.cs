using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    ParticleSystem ps;
    public bool play;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if(play && !ps.isPlaying)
        {
            ps.Play();
            play = false;
            SetUseTexture(0);
        }
    }
    void OnParticleCollision(GameObject other)
    {
        SetUseTexture(1);
    }
    void SetUseTexture(float value)
    {
        var psCustomData = ps.customData;
        psCustomData.SetVector(ParticleSystemCustomData.Custom1, 0, value);
    }
}
