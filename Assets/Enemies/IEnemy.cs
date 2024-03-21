using System;
using UnityEngine;
public interface IEnemy
{
    public void SetSpawn(Vector3 spawn);
    public void Kill();
    public void RegisterEnemyDeathCallback(Action<IEnemy> action);
    public void Respawn();
    public void RegisterEnemyRespawnCallback(Action<IEnemy> action);
    public void Reset();
    public void RegisterMainCamera(Camera mainCamera);
}
