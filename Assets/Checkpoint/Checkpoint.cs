using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Checkpoint : MonoBehaviour
{
    public int checkpointNumber;
    public MeshRenderer bigCubeRenderer;
    public List<MeshRenderer> renderers;
    public List<Transform> rotateObjects;
    public List<Transform> scaleObjects;
    Vector3[] originalScales;
    Transform parent;
    bool checkpointActivated;
    float angle;
    public float checkpointRotateSpeed = 20f;
    public Material transparentMaterial;
    public Color colorA;
    public Color colorB;
    float t;
    float bigCubeAngle;
    public float scaleFactor;
    public UnityEvent onCheckpointGet;
    void Start()
    {
        foreach (MeshRenderer r in renderers)
        {
            r.material.color = Color.clear;
        }
        originalScales = new Vector3[scaleObjects.Count];
        for (int i = 0; i < scaleObjects.Count; i++) 
        {
            originalScales[i] = scaleObjects[i].transform.localScale;
        }
        parent = transform.parent;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        Player player = col.GetComponent<Player>();
        if (player != null)
        {
            if(checkpointNumber > player.currentCheckpointNumber)
            {
                player.checkpointWorldPos = transform.position;
                player.currentCheckpointNumber = checkpointNumber;
                checkpointActivated = true;
                onCheckpointGet?.Invoke();
            }
        }
    }
    void Update()
    {
        if(checkpointActivated)
        {
            if(bigCubeRenderer.material.color.a > 0)
            {
                bigCubeAngle += Time.deltaTime * 360f * angle;
                bigCubeRenderer.transform.rotation = Quaternion.Euler(0f, bigCubeAngle, 45f);
                bigCubeRenderer.material.color -= new Color(0f,0f,0f, Time.deltaTime * angle);
            }

            angle += Time.deltaTime * checkpointRotateSpeed;
            t = Mathf.Sin(Time.time);
            for (int i = 0; i < rotateObjects.Count; i++)
            {
                float rotSign = Mathf.Pow(-1, i);
                rotateObjects[i].rotation = Quaternion.Euler(0f, rotSign*angle, 45f);
            }
            for (int i = 0; i < scaleObjects.Count; i++)
            {
                float rotSign = Mathf.Pow(-1, i);
                scaleObjects[i].transform.localScale = originalScales[i] + rotSign * originalScales[i] * scaleFactor * Mathf.Sin(Time.time);
            }
            for (int i = 0; i < renderers.Count; i++)
            {
                if(i % 2 == 0)
                {
                    renderers[i].material.color = Color.Lerp(colorA, colorB, t);
                }
                else
                {
                    renderers[i].material.color = Color.Lerp(colorB, colorA, t);
                }
            }
        }
    }
}
