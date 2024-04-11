using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingContraption : MonoBehaviour
{
    public Transform[] points;
    public int startIndex;
    public int endIndex;
    public Transform contraptionGear;
    public Transform objectToMove;
    public float speed;
    public float waitTime;
    public float t;
    public float w;
    public bool reverseMove;
    public Vector3 offset;
    void Start()
    {
        Next();
    }
    void Update()
    {
        if (t < 1)
        {
            t += Time.deltaTime * speed;
            contraptionGear.localRotation *= Quaternion.AngleAxis(speed*Time.deltaTime, Vector3.forward);
            Vector3 newPos = Vector3.Lerp(points[startIndex].position, points[endIndex].position, t);
            Vector3 gearDir = (newPos - contraptionGear.position).normalized;
            contraptionGear.Rotate(Vector3.forward, -Mathf.Sign(Vector2.Dot(Vector2.right, gearDir))*360*speed*Time.deltaTime);
            objectToMove.position = new Vector3(newPos.x, newPos.y, objectToMove.position.z) + offset;
            contraptionGear.position = new Vector3(newPos.x, newPos.y, contraptionGear.position.z);
        }
        else
        {
            w += Time.deltaTime;
            if(w > waitTime)
            {
                //move to nextPoints
                Next();
                w = 0;
                t = 0;
            }
        }
    }
    void Next()
    {
        if(reverseMove)
        {
            startIndex--;
            endIndex = startIndex - 1;
            if(endIndex <= -1)
            {
                reverseMove = false;
                startIndex = 0;
                endIndex = 1;
            }
        }
        else
        {
            startIndex++;
            endIndex = startIndex + 1;
            if(endIndex >= points.Length)
            {
                reverseMove = true;
                //move start to end
                startIndex = points.Length - 1;
                endIndex = startIndex - 1;
            }
        }
    }
}
