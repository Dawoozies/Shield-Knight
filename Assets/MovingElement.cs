using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingElement : MonoBehaviour
{
    public float frequency;
    public Vector2 moveAxis;
    Vector2 origin;
    void Start()
    {
        origin = transform.position;
    }
    void Update()
    {
        transform.position = origin + moveAxis * Mathf.Sin(frequency*Time.time);
    }
}
