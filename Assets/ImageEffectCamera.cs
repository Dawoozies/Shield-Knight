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
        if (mainCamera != null)
        {
            imageEffectCamera.orthographic = mainCamera.orthographic;
            imageEffectCamera.fieldOfView = mainCamera.fieldOfView;
            imageEffectCamera.transform.forward = mainCamera.transform.forward;
            imageEffectCamera.backgroundColor = mainCamera.backgroundColor;
        }
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
        imageEffectCamera.transform.position = mainCamera.transform.position;
    }
    private void ChangedActiveScene(Scene current, Scene next)
    {
        mainCamera = Camera.main;

        if (next.name == "CastleEnter" || next.name == "Level1Gears")
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
