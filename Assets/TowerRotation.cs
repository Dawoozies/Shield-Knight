using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRotation : MonoBehaviour
{
    public float rotationSpeed;
    void Update()
    {
        transform.Rotate(rotationSpeed*Time.deltaTime*Vector3.forward);        
    }
}
