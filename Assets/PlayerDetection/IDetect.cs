using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetect
{
    public CastTransform castTransform { get; set; }
    public void SetCastDirection(Vector2 direction);
    public RaycastHit2D[] AllResults();
    public (bool, Vector2) FirstResultPosition(string tag); //Enemy only really needs to get back the player nothing else for example
}
