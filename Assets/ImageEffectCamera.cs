using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class ImageEffectCamera : MonoBehaviour
{
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
        SceneManager.activeSceneChanged += ChangedActiveScene;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        transform.position = mainCamera.transform.position;
    }
    private void ChangedActiveScene(Scene current, Scene next)
    {
        mainCamera = Camera.main;
    }
}
