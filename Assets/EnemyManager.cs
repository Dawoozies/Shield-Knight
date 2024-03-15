using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            alive.Add(newEnemy);
        }
        mainCamera = Camera.main;
    }
    void Update()
    {

    }
}
