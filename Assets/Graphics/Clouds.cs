using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public enum WindDirection
    {
        Left = -1, Right = 1
    }
    public WindDirection windDirection;
    public float windSpeed;
    public float offScreenTime;
    public int onScreenTotal = 10;
    public float hSpace;
    [Tooltip("As a percentage, where along the screen height are we allowed to spawn clouds")]
    public Vector2 verticalSpawnBounds;
    public float ySpawnMin, ySpawnMax;
    List<SpriteRenderer> clouds = new();

    public Sprite[] sprites;
    public GameObject cloudPrefab;
    Camera mainCamera;

    public Vector2 bottomLeft, bottomRight, topLeft, topRight;
    float width, height;
    private void Start()
    {
        mainCamera = Camera.main;

        //transforms are wrong
        bottomLeft = mainCamera.ScreenToWorldPoint(Vector2.zero);
        bottomRight = mainCamera.ScreenToWorldPoint(Vector2.right * mainCamera.pixelWidth);
        topLeft = mainCamera.ScreenToWorldPoint(Vector2.up*mainCamera.pixelHeight);
        topRight = mainCamera.ScreenToWorldPoint((Vector2.right * mainCamera.pixelWidth + Vector2.up * mainCamera.pixelHeight));

        width = Vector2.Distance(bottomLeft, bottomRight);
        height = Vector2.Distance(bottomLeft, topLeft);

        ySpawnMin = topLeft.y * verticalSpawnBounds.x;
        ySpawnMax = topLeft.y * verticalSpawnBounds.y;
        float driftShift = windSpeed * hSpace; 
        for (int i = 0; i < onScreenTotal; i++)
        {
            GameObject clone = Instantiate(cloudPrefab, transform);
            SpriteRenderer spriteRenderer = clone.GetComponent<SpriteRenderer>();
            clouds.Add(spriteRenderer);

            int randomSprite = Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[randomSprite];

            float xSpawnValue = bottomLeft.x;
            if(windDirection == WindDirection.Right)
            {
                xSpawnValue = bottomLeft.x - driftShift * (i+1);
            }
            else
            {
                xSpawnValue = bottomRight.x + driftShift * (i+1);
            }

            float ySpawnValue = Random.Range(ySpawnMin, ySpawnMax);
            clone.transform.position = new Vector2(xSpawnValue, ySpawnValue);
        }
    }
    private void Update()
    {
        bottomLeft = mainCamera.ScreenToWorldPoint(Vector2.zero);
        bottomRight = mainCamera.ScreenToWorldPoint(Vector2.right * mainCamera.pixelWidth);
        topLeft = mainCamera.ScreenToWorldPoint(Vector2.up * mainCamera.pixelHeight);
        topRight = mainCamera.ScreenToWorldPoint((Vector2.right * mainCamera.pixelWidth + Vector2.up * mainCamera.pixelHeight));
        foreach (var item in clouds)
        {
            if(windDirection == WindDirection.Right)
            {
                item.transform.position += Time.deltaTime * windSpeed * Vector3.right;
                if (item.transform.position.x > bottomRight.x + windSpeed * hSpace)
                {
                    float xNew = bottomLeft.x - (windSpeed * hSpace);
                    float yNew = Random.Range(ySpawnMin, ySpawnMax);
                    item.transform.position = new Vector2(xNew, yNew);
                    item.sprite = sprites[Random.Range(0, sprites.Length)];
                }
            }
            else
            {
                item.transform.position -= Time.deltaTime * windSpeed * Vector3.right;
            }


        }
    }
    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawCube(bottomLeft, Vector3.one);
        Gizmos.DrawCube(bottomRight, Vector3.one);
        Gizmos.DrawCube(topLeft, Vector3.one);
        Gizmos.DrawCube(topRight, Vector3.one);

    }
}
