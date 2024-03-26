using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    BoxCollider2D boxCollider;
    public enum ZoneType
    {
        Default, Offset
    }
    public ZoneType zoneType;
    public Vector2 offset;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            CameraManager.CameraEnterLockedZone(boxCollider, zoneType, offset);
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            CameraManager.CameraEnterLockedZone(boxCollider, zoneType, offset);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            CameraManager.CameraExitLockedZone(boxCollider);
        }
    }
}
