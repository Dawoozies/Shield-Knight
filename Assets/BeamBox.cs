using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BeamBox : MonoBehaviour
{
    public float rotSpeed;
    public enum RotationDirection
    {
        Clockwise, AntiClockwise
    }
    public RotationDirection rotDir;
    float angle;
    public bool rotate;
    void Update()
    {
        if(!rotate)
        {
            return;
        }
        if (rotDir == RotationDirection.Clockwise)
        {
            angle += Time.deltaTime * rotSpeed;
        }
        else
        {
            angle -= Time.deltaTime * rotSpeed;
        }
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public void SwitchOnRotation()
    {
        rotate = true;
    }
    public void SwitchOffRotation()
    {
        rotate = false;
    }
    public void ChangeToClockwise()
    {
        rotDir = RotationDirection.Clockwise;
    }
    public void ChangeToAntiClockwise()
    {
        rotDir = RotationDirection.AntiClockwise;
    }
}
