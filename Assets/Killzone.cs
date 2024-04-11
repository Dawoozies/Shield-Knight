using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    Player player => GameManager.GetActivePlayer();
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            player.ApplyForce(Vector2.one*1000f);
        }
    }
}
