using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour, Manager
{
    AudioSource audioSource;
    public AudioClip[] takingDamage;
    public AudioClip[] death;
    public AudioClip recallCatch;
    bool recallPlaying;
    Player player;
    ShieldManager shieldManager;
    void ShieldRecallHandler(float distanceFromPlayer, float recallVelocity)
    {
        audioSource.PlayOneShot(recallCatch);
    }
    public void ManagedStart()
    {
        audioSource = player.GetComponent<AudioSource>();
        shieldManager = GetComponent<ShieldManager>();
        shieldManager.shieldRecallCallback = ShieldRecallHandler;
    }
    public void ManagedUpdate()
    {
    }

    public void RegisterPlayer(Player player)
    {
        this.player = player;
    }

    public void PlayerDeathComplete()
    {
    }

    public void PlayerDeath()
    {
    }
}
