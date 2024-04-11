using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResetManager : MonoBehaviour, Manager
{
    public Transform enemyParent;
    public IManagedEnemy[] enemies;
    public void ManagedStart()
    {
        enemies = enemyParent.GetComponentsInChildren<IManagedEnemy>();
    }

    public void ManagedUpdate()
    {
    }

    public void RegisterPlayer(Player player)
    {
    }
    public void PlayerDeathComplete()
    {
        if(!enemyParent.gameObject.activeSelf)
        {
            return;
        }
        foreach (var enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }
    public void PlayerDeath()
    {
    }
}
