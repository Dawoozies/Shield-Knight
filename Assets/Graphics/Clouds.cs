using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
public class Clouds : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject cloudPrefab;
    public Vector2 yBounds;
    public float minSpeed, maxSpeed;
    public float spawnTime;
    public int cloudCount;
    private List<Cloud> clouds = new();
    private Camera mainCamera;
    private Vector2 cornerA, cornerB;
    private float timer;
    public Transform backgroundParent;
    public float zPos;
    private class Cloud
    {
        public SpriteRenderer spriteRenderer;
        public float speed;
        public Transform transform => spriteRenderer.transform;
        public float _getWidth(Transform backgroundParent)
        {
            return spriteRenderer.sprite.bounds.size.x * backgroundParent.localScale.x;
        }
    }
    private void Start()
    {
        mainCamera = Camera.main;
        //Spawn all clouds across screen
        float distAcrossScreen = 0f;
        cornerA = mainCamera.ScreenToWorldPoint(Vector2.zero);
        cornerB = mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight));
        for (int i = 0; i < cloudCount; i++)
        {
            Cloud cloudSpawned = StartSpawnCloud();
            distAcrossScreen += cloudSpawned._getWidth(backgroundParent);
            cloudSpawned.spriteRenderer.transform.position += Vector3.right*distAcrossScreen;
            if (distAcrossScreen > cornerB.x - cornerA.x)
            {
                break;
            }
        }
    }

    private void Update()
    {
        cornerA = mainCamera.ScreenToWorldPoint(Vector2.zero);
        cornerB = mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight));
        if (clouds.Count < cloudCount)
        {
            if (timer < spawnTime)
            {
                timer += Time.unscaledDeltaTime;
                if (timer >= spawnTime)
                {
                    SpawnCloud();
                    timer = 0f;
                }
            }
        }

        foreach (var cloud in clouds)
        {
            cloud.transform.position += Vector3.right*(cloud.speed*Time.unscaledDeltaTime);
            if (cloud.transform.position.x > cornerB.x + cloud.spriteRenderer.sprite.bounds.size.x * backgroundParent.localScale.x)
            {
                ResetCloud(cloud);
            }
            cloud.transform.localPosition = new Vector3(cloud.transform.localPosition.x,cloud.transform.localPosition.y, zPos);
        }
    }
    void SpawnCloud()
    {
        GameObject clone = Instantiate(cloudPrefab, transform);
        SpriteRenderer spriteRenderer = clone.GetComponent<SpriteRenderer>();
        Cloud cloud = new Cloud();
        cloud.spriteRenderer = spriteRenderer;
        clouds.Add(cloud);
        ResetCloud(cloud);
    }
    Cloud StartSpawnCloud()
    {
        GameObject clone = Instantiate(cloudPrefab, transform);
        SpriteRenderer spriteRenderer = clone.GetComponent<SpriteRenderer>();
        Cloud cloud = new Cloud();
        cloud.spriteRenderer = spriteRenderer;
        clouds.Add(cloud);
        ResetCloud(cloud);
        return cloud;
    }
    void ResetCloud(Cloud cloud)
    {
        cloud.spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        float yRand = Random.Range(yBounds.x, yBounds.y);
        cloud.transform.position = new Vector2(
                cornerA.x - cloud.spriteRenderer.sprite.bounds.size.x * backgroundParent.localScale.x,
                Mathf.Lerp(cornerA.y, cornerB.y, yRand)
            );
        float yPos = Mathf.Clamp(cloud.transform.position.y, 0f, cornerB.y - (cloud.spriteRenderer.sprite.bounds.size.y/2f)*backgroundParent.localScale.y);
        cloud.transform.position = new Vector3(cloud.transform.position.x, yPos, zPos);
        cloud.speed = Random.Range(minSpeed, maxSpeed);
    }
}
