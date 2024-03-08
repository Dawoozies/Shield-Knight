using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    public Transform shieldGizmoTransform;
    public Transform shieldParent;
    public Shield shield;
    public Character player;
    public float distanceFromPlayer;
    Vector2 pointOnCircle;
    public LayerMask shieldNonIntersecting;
    float leftClickHeld, rightClickHeld, middleClickHeld;
    bool leftClickDown, rightClickDown, middleClickDown;

    float angle;
    float distance;
    public LayerMask enemyLayerMask;
    RaycastHit2D[] enemyHitResults;

    public int selectedAttack;
    public List<ShieldAttackData> leftClickAttacks = new();


    void Start()
    {
        InputManager.RegisterMouseInputCallback(HandleMouseInput);
        InputManager.RegisterMouseClickCallback(HandleMouseClick);
        InputManager.RegisterMouseDownCallback(HandleMouseDown);
        shieldParent.position = player.transform.position;
    }
    void Update()
    {
        //Go to point on circle which intersects
        //Do raycast to push shield closer to player if pointing at walls
        shieldParent.position = player.transform.position;
        shieldParent.rotation = Quaternion.identity;
        shield.transform.localPosition = (Vector3)pointOnCircle;
        shield.transform.right = pointOnCircle;

        shieldGizmoTransform.position = player.transform.position;
        shieldGizmoTransform.rotation = shield.transform.rotation;
        //for now directly do the shield movement here to test
        //then offset it to the shield itself when ShieldAttackData is good enough

        if (leftClickHeld > 0)
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

        shield.transform.localPosition += leftClickAttacks[selectedAttack].output * shield.transform.right;


        shield.hitForceMultiplier = leftClickAttacks[selectedAttack].hitForceMultiplier;
        shield.hitForce = leftClickAttacks[selectedAttack].hitForce;

        if(shield.hitForce > 0.1f)
        {
            angle = Vector2.Angle(Vector2.right, shield.transform.right);
            distance = Vector2.Distance(player.transform.position, shield.transform.position) + shield.boxCollider.size.x;
            enemyHitResults = Physics2D.BoxCastAll(player.transform.position, shield.boxCollider.size, angle, shield.transform.right, distance, enemyLayerMask);

            if(enemyHitResults != null && enemyHitResults.Length > 0)
            {
                foreach (RaycastHit2D hit in enemyHitResults)
                {
                    Vector2 shiftedHit = hit.point;
                    shiftedHit.y = Mathf.Max(shiftedHit.y, hit.collider.transform.position.y);
                    
                    Vector2 force = (shiftedHit - (Vector2)player.transform.position)*shield.hitForce;

                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    enemy.ApplyHit(force);
                }
                shield.ScaleLerp(Vector2.one * 1.45f, 3f);
            }
        }
        else
        {
            enemyHitResults = null;
        }
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

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Gizmos.matrix = shieldGizmoTransform.localToWorldMatrix;

        if (enemyHitResults != null && enemyHitResults.Length > 0)
        {
            Gizmos.color = Color.green * 0.75f;
        }
        else
        {
            Gizmos.color = Color.clear;
        }

        float castLength = 0;
        int loopBreaker = 0;
        while (castLength < distance)
        {
            Vector2 pos = Vector2.zero + Vector2.right * castLength;
            Gizmos.DrawCube(pos, shield.boxCollider.size);
            castLength += shield.boxCollider.size.x;
            loopBreaker++;
            if (loopBreaker >= 1000)
                break;
        }

        Gizmos.matrix = Matrix4x4.identity;
        if (enemyHitResults != null && enemyHitResults.Length > 0)
        {
            foreach (RaycastHit2D hit in enemyHitResults)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(shieldGizmoTransform.position, hit.point);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(shieldGizmoTransform.position, hit.collider.transform.position);
                Gizmos.color = Color.magenta;
                Vector2 shiftedHit = hit.point;
                shiftedHit.y = Mathf.Max(shiftedHit.y, hit.collider.transform.position.y);
                Gizmos.DrawLine(shieldGizmoTransform.position, shiftedHit);
            }
        }
    }
}
