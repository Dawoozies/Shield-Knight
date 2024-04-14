using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public Transform toMove;
    public float rotValue;
    public float rotValueTarget;
    public Vector3 originalLocalPos;
    public Vector3 targetLocalPos;
    Player player;
    private void Start()
    {
        player = GameManager.GetActivePlayer();
        player.RegisterOnDeathCompleteCallback(() => { rotValue = 0; });
    }
    public void InputGearRotation(float rotValue)
    {
        if(this.rotValue < rotValueTarget)
        {
            this.rotValue += rotValue;
            if(this.rotValue > rotValueTarget)
            {
                this.rotValue = rotValueTarget;
            }
        }
    }
    public void NotRotatingHandler(float dragValue)
    {
        if(rotValue > 0)
        {
            rotValue -= dragValue * Time.deltaTime;
            if(rotValue <= 0)
            {
                rotValue = 0;
            }
        }
    }
    private void Update()
    {
        toMove.localPosition = Vector3.Lerp(originalLocalPos, targetLocalPos, rotValue/rotValueTarget);
    }
}
