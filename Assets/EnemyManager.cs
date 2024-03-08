using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Character player;
    public Transform enemySpawnParent;
    List<Transform> enemySpawns = new();
    public GameObject enemyPrefab;
    List<Enemy> alive = new();
    List<Enemy> dead = new();
    Camera mainCamera;
    public GameObject castTransformPrefab;
    void Start()
    {
        int spawns = enemySpawnParent.childCount;
        for (int i = 0; i < spawns; i++)
        {
            enemySpawns.Add(enemySpawnParent.GetChild(i));
        }
        foreach (Transform spawn in enemySpawns)
        {
            GameObject newEnemyObject = Instantiate(enemyPrefab, spawn.position, spawn.rotation, transform);
            Enemy newEnemy = newEnemyObject.GetComponent<Enemy>();
            newEnemy.health = 1;
            newEnemy.onDeathActions.Add(EnemyDeathHandler);
            newEnemy.spawn = spawn;
            newEnemy.player = player;

            GameObject newCastTransformObject = Instantiate(castTransformPrefab);
            CastTransform castTransform = newCastTransformObject.GetComponent<CastTransform>();
            castTransform.character = newEnemy.GetComponent<Character>();
            IDetect detectionInterface = newEnemy.GetComponent<IDetect>();
            detectionInterface.castTransform = castTransform;

            alive.Add(newEnemy);
        }
        mainCamera = Camera.main;
    }
    void Update()
    {
        if(dead.Count > 0)
        {
            List<Enemy> newDeadList = new();
            foreach (Enemy enemy in dead)
            {
                if (Mathf.Abs(enemy.transform.position.y - mainCamera.transform.position.y) > 100f)
                {
                    enemy.Reset();
                    alive.Add(enemy);
                }
                else
                {
                    newDeadList.Add(enemy);
                }
            }
            dead = newDeadList;
        }
    }
    void EnemyDeathHandler(Enemy deadEnemy, Vector2 force)
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
}
