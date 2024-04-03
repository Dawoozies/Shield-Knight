using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class ImageEffectCamera : MonoBehaviour
{
    private Camera imageEffectCamera;
    private Camera mainCamera;
    public RenderTexture imageMaterialEffect;
    private Vector2Int currentDimensions;
    void Start()
    {
        imageEffectCamera = GetComponent<Camera>();
        mainCamera = Camera.main;
        SceneManager.activeSceneChanged += ChangedActiveScene;
        DontDestroyOnLoad(gameObject);
        currentDimensions = new Vector2Int(mainCamera.pixelWidth, mainCamera.pixelHeight);
        imageMaterialEffect.width = currentDimensions.x;
        imageMaterialEffect.height = currentDimensions.y;
    }

    private void Update()
    {
        if (currentDimensions.x != mainCamera.pixelWidth || currentDimensions.y != mainCamera.pixelHeight)
        {
            currentDimensions.x = mainCamera.pixelWidth;
            currentDimensions.y = mainCamera.pixelHeight;
            imageMaterialEffect.Release();
            imageMaterialEffect.width = currentDimensions.x;
            imageMaterialEffect.height = currentDimensions.y;
            imageMaterialEffect.Create();
            mainCamera.targetTexture = imageMaterialEffect;
        }
        transform.position = mainCamera.transform.position;
    }
    private void ChangedActiveScene(Scene current, Scene next)
    {
        mainCamera = Camera.main;
        if (next.name == "CastleEnter")
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
