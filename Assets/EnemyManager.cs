using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OldSystems;
public class EnemyManager : MonoBehaviour, Manager
{
    public Player player;
    public Transform enemySpawnParent;
    List<Transform> enemySpawns = new();
    public GameObject enemyPrefab;
    List<IEnemy> alive = new();
    List<IEnemy> dead = new();
    Camera mainCamera;
    public bool respawnDeadEnemies;
    public void ManagedStart()
    {
        mainCamera = Camera.main;
        int spawns = enemySpawnParent.childCount;
        for (int i = 0; i < spawns; i++)
        {
            enemySpawns.Add(enemySpawnParent.GetChild(i));
        }
        foreach (Transform spawn in enemySpawns)
        {
            GameObject newEnemyObject = Instantiate(enemyPrefab, spawn.position, spawn.rotation, transform);
            IEnemy newEnemy = newEnemyObject.GetComponent<IEnemy>();
            newEnemy.SetSpawn(spawn.position);
            newEnemy.RegisterEnemyDeathCallback(EnemyDeathHandler);
            newEnemy.RegisterMainCamera(mainCamera);
            alive.Add(newEnemy);
        }
    }
    public void RegisterPlayer(Player player)
    {
        this.player = player;
    }
    public void ManagedUpdate()
    {
        if(respawnDeadEnemies)
        {
            RespawnAllEnemies();
            respawnDeadEnemies = false;
        }
    }
    void EnemyDeathHandler(IEnemy deadEnemy)
    {
        if(alive.Contains(deadEnemy))
        {
            alive.Remove(deadEnemy);
        }
        if(!dead.Contains(deadEnemy))
        {
            dead.Add(deadEnemy);
        }
    }
    void ResetAllEnemies()
    {
        foreach (IEnemy aliveEnemy in alive)
        {
            aliveEnemy.Reset();
        }
        RespawnAllEnemies();
    }
    void RespawnAllEnemies()
    {
        foreach (IEnemy deadEnemy in dead)
        {
            deadEnemy.Respawn();
        }
    }
    public void PlayerDied()
    {
        ResetAllEnemies();
    }
}
