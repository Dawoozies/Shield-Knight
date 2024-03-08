using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxCastDetection : MonoBehaviour, IDetect
{
    [Serializable]
    public class BoxCastData
    {
        public bool castAll;
        public Vector2 size;
        public float castAngle;
        public Vector2 castDirection;
        public float castDistance;
        public LayerMask castLayerMask;
    }
    public BoxCastData boxCastData;
    public RaycastHit2D[] hits;

    public Transform castTransform;
    CastTransform IDetect.castTransform { 
        get => CastTransform;
        set { 
            CastTransform = value;
            castTransform = value.transform;
        }
    }
    CastTransform CastTransform;
    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        castTransform.right = boxCastData.castDirection;
        boxCastData.castAngle = Vector2.Angle(Vector2.right, castTransform.right);
        if(boxCastData.castAll)
        {
            hits = Physics2D.BoxCastAll(transform.position, boxCastData.size, boxCastData.castAngle, castTransform.right, boxCastData.castDistance, boxCastData.castLayerMask);
        }
        else
        {
            hits = new RaycastHit2D[1];
            hits[0] = Physics2D.BoxCast(transform.position, boxCastData.size, boxCastData.castAngle, castTransform.right, boxCastData.castDistance, boxCastData.castLayerMask);
        }
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.matrix = castTransform.localToWorldMatrix;
        Gizmos.color = Color.red * 0.75f;
        float hitDistance = boxCastData.castDistance;
        if (hits == null)
        {
            return;
        }
        if (boxCastData.castAll)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    Gizmos.color = Color.green * 0.75f;
                    break;
                }
            }
        }
        else
        {
            if (hits[0].collider != null)
            {
                Gizmos.color = Color.green * 0.75f;
                hitDistance = hits[0].distance;
            }
        }

        float castLength = 0;
        int loopBreaker = 0;
        while (castLength < hitDistance)
        {
            Vector2 pos = Vector2.zero + Vector2.right * castLength;
            Gizmos.DrawCube(pos, boxCastData.size);
            castLength += boxCastData.size.x;
            loopBreaker++;
            if (loopBreaker >= 1000)
                break;
        }
    }

    public void SetCastDirection(Vector2 direction)
    {
        boxCastData.castDirection = direction;
    }

    public RaycastHit2D[] AllResults()
    {
        return hits;
    }

    public (bool, Vector2) FirstResultPosition(string tag)
    {
        if (hits == null)
        {
            return (false, Vector2.zero);
        }
        if (tag.Length == 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    return (true, hit.collider.transform.position);
                }
            }
        }
        else
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    if(hit.collider.tag != tag)
                    {
                        return (false, Vector2.zero);
                    }
                    return (true, hit.collider.transform.position);
                }
            }
        }


        return (false, Vector2.zero);
    }
}
