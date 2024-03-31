using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class UIEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;
    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
    }
}
