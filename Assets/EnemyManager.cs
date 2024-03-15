using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OldSystems;
public class EnemyManager : MonoBehaviour
{
    public Player player;
    public Transform enemySpawnParent;
    List<Transform> enemySpawns = new();
    public GameObject enemyPrefab;
    List<IEnemy> alive = new();
    List<IEnemy> dead = new();
    Camera mainCamera;
    public bool respawnDeadEnemies;
    void Start()
    {
        if(player == null)
        {
            player = FindAnyObjectByType<Player>();
        }
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
            alive.Add(newEnemy);
        }
        mainCamera = Camera.main;
    }
    void Update()
    {
        if(respawnDeadEnemies)
        {
            foreach (IEnemy deadEnemy in dead)
            {
                deadEnemy.Respawn();
             }
            respawnDeadEnemies = false;
        }
    }
    void EnemyDeathHandler(IEnemy deadEnemy)
    {
        if(alive.Contains(deadEnemy))
        {
            alive.Remove(deadEnemy);
            deadEnemy.Kill();
        }
        if(!dead.Contains(deadEnemy))
        {
            dead.Add(deadEnemy);
        }
    }
}
