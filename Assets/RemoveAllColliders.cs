using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAllColliders : MonoBehaviour
{
    void Start()
    {
        Collider2D[] collidersToRemove = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in collidersToRemove)
        {
            Destroy(col);
        }
    }

}
