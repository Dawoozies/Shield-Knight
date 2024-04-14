using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEndGame : MonoBehaviour
{
    public GameObject[] elementsToRemoveOnEnd;
    int END;
    void Start()
    {
        END = PlayerPrefs.GetInt("END");
        if(END == 1)
        {
            foreach (GameObject element in elementsToRemoveOnEnd)
            {
                element.SetActive(false);
            }
        }
    }
}
