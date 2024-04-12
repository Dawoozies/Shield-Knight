using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCycle : MonoBehaviour
{
    Image image;
    public Sprite[] sprites;
    public float cycleTime;
    float t;
    int index;
    void Start()
    {
        image = GetComponent<Image>();
    }
    void Update()
    {
        t += Time.deltaTime;
        if(t > cycleTime)
        {
            index++;
            index %= sprites.Length;
            image.sprite = sprites[index];
            t = 0;
        }
    }
}
