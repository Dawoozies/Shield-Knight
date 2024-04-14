using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EndGameText : MonoBehaviour
{
    public string endGameText;
    TextMeshProUGUI textMesh;
    int indexToAdd;
    public float indexIncreaseTime;
    float t;
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        t += Time.deltaTime;
        if(t > indexIncreaseTime)
        {
            textMesh.text += endGameText[indexToAdd % endGameText.Length];
            indexToAdd++;
            t = 0;
        }
    }
}
