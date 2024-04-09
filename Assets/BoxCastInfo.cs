using System;
using UnityEngine;
[Serializable]
public class BoxCastInfo
{
    private Vector2 origin;
    private Vector2 size;
    private Vector2 direction;
    private float angle => VectorCalculations.Angle.AngleFromXAxis(direction);
    private float distance;
    private LayerMask layers;
    private RaycastHit2D hit;
    private RaycastHit2D[] hits;
    public BoxCastInfo()
    {
        this.origin = Vector2.zero;
        this.size = Vector2.zero;
        this.direction =Vector2.right;
        this.distance = 0f;
        this.layers = 0;
    }
    public BoxCastInfo(Vector2 origin, Vector2 size, Vector2 direction, float distance, LayerMask layers)
    {
        this.origin = origin;
        this.size = size;
        this.direction = direction;
        this.distance = distance;
        this.layers = layers;
    }

    public Vector2 Origin
    {
        get => origin;
        set => origin = value;
    }

    public Vector2 Size
    {
        get => size;
        set => size = value;
    }

    public Vector2 Direction
    {
        get => direction;
        set => direction = value;
    }

    public float Distance
    {
        get => distance;
        set => distance = value;
    }

    public LayerMask Layers
    {
        get => layers;
        set => layers = value;
    }

    public float Angle
    {
        get => angle;
    }

    public void Cast(bool all)
    {
        if (all)
        {
            hits = Physics2DFunctions.Cast.BoxCastAll(this);
            return;
        }

        hit = Physics2DFunctions.Cast.BoxCast(this);
    }

    public RaycastHit2D Hit => hit;

    public RaycastHit2D[] Hits => hits;
}