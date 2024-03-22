using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Button : MonoBehaviour, IEmbeddable
{
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;
    public Vector3 defaultScale;
    public Vector3 pressedScale;
    public Color defaultColor;
    public Color pressedColor;
    ShieldThrow shieldRef;
    SpriteRenderer spriteRenderer;
    public LayerMask canPressButtonLayer;
    BoxCollider2D boxCollider;
    bool isPressed;
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    public void TryEmbed(ShieldThrow shieldThrow, Vector3 embeddingVelocity)
    {
        //float dotResult = Vector3.Dot(embeddingVelocity, transform.right);
        //if(dotResult < 0)
        //{
        //    //Debug.LogError($"Button hit with dotResult = {dotResult}");
        //    shieldRef = shieldThrow;
        //}
    }
    public void TryRemoveEmbed(ShieldThrow shieldThrow, Vector3 recallVelocity)
    {
        //if(shieldRef == shieldThrow)
        //{
        //    shieldRef = null;
        //}
    }
    void Update()
    {
        if(shieldRef != null && !shieldRef.gameObject.activeSelf)
        {
            shieldRef = null;
        }
        Vector2 point = (Vector2)transform.position + boxCollider.offset;
        Collider2D[] results = Physics2D.OverlapBoxAll(point, boxCollider.size + Vector2.one*0.05f, 0f, canPressButtonLayer);
        bool properResults = false;
        foreach (var col in results)
        {
            Vector2 dirToCol = (Vector2)col.transform.position - point;
            float dotResult = Vector2.Dot(dirToCol, transform.right);
            if(dotResult > 0)
            {
                properResults = true;
                break;
            }
        }
        bool pressedDown = (shieldRef != null) || properResults;
        if(pressedDown)
        {
            if(!isPressed)
            {
                PressedDown();
            }
            isPressed = true;
        }
        else
        {
            if(isPressed)
            {
                Released();
            }
            isPressed = false;
        }
    }
    void PressedDown()
    {
        spriteRenderer.transform.localScale = pressedScale;
        spriteRenderer.color = pressedColor;
        onButtonPressed.Invoke();
    }
    void Released()
    {
        spriteRenderer.transform.localScale = defaultScale;
        spriteRenderer.color = defaultColor;
        onButtonReleased.Invoke();
    }
    //either something falls on this with weight good enough to press down
    //or shield embeds at right direction

}
