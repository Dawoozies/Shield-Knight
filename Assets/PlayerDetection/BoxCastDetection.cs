using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxCastDetection : MonoBehaviour, IDetect
{
    [Serializable]
    public class BoxCastData
    {
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
        hits = Physics2D.BoxCastAll(transform.position, boxCastData.size, boxCastData.castAngle, castTransform.right, boxCastData.castDistance, boxCastData.castLayerMask);
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.matrix = castTransform.localToWorldMatrix;
        if(hits == null)
        {
            return;
        }
        Gizmos.color = Color.red * 0.75f;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Gizmos.color = Color.green * 0.75f;
                break;
            }
        }

        float castLength = 0;
        int loopBreaker = 0;
        while (castLength < boxCastData.castDistance)
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

    public (bool, Vector2) FirstResultPosition()
    {
        if(hits == null)
        {
            return (false, Vector2.zero);
        }

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider != null)
            {
                return (true, hit.collider.transform.position);
            }
        }

        return (false, Vector2.zero);
    }
}
