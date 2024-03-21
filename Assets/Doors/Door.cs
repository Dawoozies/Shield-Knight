using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform top;
    public Transform bottom;
    public BoxCollider2D door;
    public bool open;
    [Range(0f,1f)]
    public float doorParameter;
    public float doorSpeed;
    void Update()
    {
        if (open)
        {
            doorParameter -= Time.deltaTime * doorSpeed;
        }
        else
        {
            doorParameter += Time.deltaTime * doorSpeed;
        }
        doorParameter = Mathf.Clamp01(doorParameter);

        Vector2 topToBottom = bottom.position - top.position;
        float doorLengthMax = topToBottom.magnitude;
        Vector2 doorPos = Vector2.Lerp(top.position, bottom.position, doorParameter);
        Vector2 doorMidPos = Vector2.Lerp(top.position, doorPos, 0.5f);
        door.transform.position = doorMidPos;
        Vector3 doorScale = new Vector3(1f, 1f, 1f);
        doorScale.y = Mathf.Lerp(0f, doorLengthMax, doorParameter);
        door.transform.localScale = doorScale;
    }
    public void Open()
    {
        open = true;
    }
    public void Close()
    {
        open = false;
    }
}
