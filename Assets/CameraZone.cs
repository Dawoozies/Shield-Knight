using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    BoxCollider2D boxCollider;
    public enum ZoneType
    {
        Default, Offset, OffsetAndFOV, OffsetAndFOVClamp
    }
    public ZoneType zoneType;
    public Vector2 offset;
    public float fieldOfView = 0f;
    public Transform[] heightClamps;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            if (fieldOfView > 0)
            {
                CameraManager.CameraEnterOffsetAndFOVZone(boxCollider, zoneType, offset, fieldOfView, heightClamps);
            }
            else
            {
                CameraManager.CameraEnterLockedZone(boxCollider, zoneType, offset);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (fieldOfView > 0)
            {
                CameraManager.CameraEnterOffsetAndFOVZone(boxCollider, zoneType, offset, fieldOfView, heightClamps);
            }
            else
            {
                CameraManager.CameraEnterLockedZone(boxCollider, zoneType, offset);
            }
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
