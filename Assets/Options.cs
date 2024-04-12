using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Options : MonoBehaviour
{
    public Image optionsImage;
    public void OpenOptions()
    {
        optionsImage.gameObject.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsImage.gameObject.SetActive(false);
    }
}
