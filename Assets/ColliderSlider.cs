using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSlider : MonoBehaviour
{
    public Transform objectToSlide;
    public Transform top;
    public Transform bottom;
    public BoxCollider2D box;
    public enum Mode
    {
        OpenClose, Incremental, Decremental
    }
    public Mode mode;
    public bool active;
    [Range(0f,1f)]
    public float sliderParameter;
    public float speed;
    void Update()
    {
        if(active)
        {
            if(mode == Mode.OpenClose)
            {
                sliderParameter -= Time.deltaTime * speed;
            }
            if(mode == Mode.Incremental)
            {
                sliderParameter += Time.deltaTime * speed;
            }
            if(mode == Mode.Decremental)
            {
                sliderParameter -= Time.deltaTime * speed;
            }
        }
        else
        {
            if(mode == Mode.OpenClose)
            {
                sliderParameter += Time.deltaTime * speed;
            }
        }
        sliderParameter = Mathf.Clamp01(sliderParameter);

        Vector2 topToBottom = bottom.position - top.position;
        float sliderLengthMax = topToBottom.magnitude;
        Vector2 sliderPos = Vector2.Lerp(top.position, bottom.position, sliderParameter);
        Vector2 sliderMidPos = Vector2.Lerp(top.position, sliderPos, 0.5f);
        box.transform.position = sliderMidPos;
        Vector3 sliderScale = new Vector3(1f, 1f, 1f);
        sliderScale.y = Mathf.Lerp(0f, sliderLengthMax, sliderParameter);
        box.transform.localScale = sliderScale;

        if(objectToSlide != null)
        {
            objectToSlide.position = sliderPos;
        }
    }
    public void Activate()
    {
        active = true;
    }
    public void Deactivate()
    {
        active = false;
    }
    public void Increment(float delta)
    {
        sliderParameter += delta;
        sliderParameter = Mathf.Clamp01(sliderParameter);
    }
    public void Decrement(float delta)
    {
        sliderParameter -= delta;
        sliderParameter = Mathf.Clamp01(sliderParameter);
    }
}
