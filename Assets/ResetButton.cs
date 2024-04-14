using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public GameObject resetButton;
    int END;
    private void Start()
    {
        END = PlayerPrefs.GetInt("END");
    }
    private void Update()
    {
        if(END == 1)
        {
            resetButton.SetActive(true);
        }
        else
        {
            resetButton.SetActive(false);
        }
    }
    public void ResetGame()
    {
        PlayerPrefs.SetInt("END", 0);
        Application.Quit();
    }
}
