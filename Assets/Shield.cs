using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public BoxCollider2D boxCollider { get; set; }
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        
    }
}
