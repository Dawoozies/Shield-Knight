using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    public Transform shieldParent;
    public Shield shield;
    public Character player;
    public float distanceFromPlayer;
    Vector2 pointOnCircle;
    public LayerMask shieldNonIntersecting;
    float leftClickHeld, rightClickHeld, middleClickHeld;
    bool leftClickDown, rightClickDown, middleClickDown;
    public int selectedAttack;
    List<ShieldAttackData> leftClickAttacks = new();
    public ShieldAttackData shieldBash;
    public ShieldAttackData shieldSlam;
    void Start()
    {
        InputManager.RegisterMouseInputCallback(HandleMouseInput);
        InputManager.RegisterMouseClickCallback(HandleMouseClick);
        InputManager.RegisterMouseDownCallback(HandleMouseDown);
        shieldParent.position = player.transform.position;
        leftClickAttacks.Add(shieldBash);
        leftClickAttacks.Add(shieldSlam);
    }
    void Update()
    {
        //Go to point on circle which intersects
        //Do raycast to push shield closer to player if pointing at walls
        shieldParent.position = player.transform.position;
        shieldParent.rotation = Quaternion.identity;
        shield.transform.localPosition = (Vector3)pointOnCircle;
        shield.transform.right = pointOnCircle;
        //for now directly do the shield movement here to test
        //then offset it to the shield itself when ShieldAttackData is good enough

        if(leftClickDown)
        {
            if (leftClickAttacks[selectedAttack].state == ShieldAttackData.State.Completed)
            {
                selectedAttack++;
                selectedAttack = selectedAttack % leftClickAttacks.Count;
            }
            leftClickAttacks[selectedAttack].Start(() => Time.deltaTime);
        }
        foreach (ShieldAttackData attackData in leftClickAttacks)
        {
            attackData.Update();
        }

        shield.transform.localPosition += leftClickAttacks[selectedAttack].output.x * shield.transform.right;

        float parentRotationAngle = leftClickAttacks[selectedAttack].output.y;
        shieldParent.rotation *= Quaternion.AngleAxis(parentRotationAngle, Vector3.forward);

        float shieldRotationAngle = leftClickAttacks[selectedAttack].output.z;
        shield.transform.rotation *= Quaternion.AngleAxis(shieldRotationAngle, Vector3.forward);
    }
    void HandleMouseInput(Vector2 mouseWorldPos)
    {
        pointOnCircle = distanceFromPlayer*((Vector3)mouseWorldPos - player.transform.position).normalized;
    }
    void HandleMouseClick(Vector3 mouseClickInput)
    {
        leftClickHeld = mouseClickInput.x;
        rightClickHeld = mouseClickInput.y;
        middleClickHeld = mouseClickInput.z;
    }
    void HandleMouseDown(Vector3Int mouseDownInput)
    {
        leftClickDown = mouseDownInput.x > 0;
        rightClickDown = mouseDownInput.y > 0;
        middleClickDown = mouseDownInput.z > 0;
    }
}
