using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IEnemy
{
    public void ApplyDamage(Vector2 force);
    public void SetSpawn(Vector3 spawn);
}
