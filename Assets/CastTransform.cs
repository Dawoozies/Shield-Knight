using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastTransform : MonoBehaviour
{
    public Character character;
    void Update()
    {
        transform.position = character.transform.position;
    }
}
