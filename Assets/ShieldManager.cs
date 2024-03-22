using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OldSystems;
namespace OldSystems
{
    public class ShieldManager : MonoBehaviour, Manager
    {
        public Transform shieldGizmoTransform;
        Transform shieldParent;
        public GameObject shieldBasePrefab;
        public Color shieldHighForceColor;
        public Color shieldDefaultColor;
        public LayerMask shieldLayerMask;
        Shield shield;
        [Header("Shield Throw Data")]
        public VelocityData shieldThrown;
        public VelocityData shieldRecall;
        public LayerMask shieldThrowLayerMask;
        ShieldThrow shieldThrow;
        public float shieldThrowForce;
        Player player;
        public float distanceFromPlayer;
        Vector2 pointOnCircle;
        public LayerMask shieldNonIntersecting;
        float leftClickHeld, rightClickHeld, middleClickHeld;
        bool leftClickDown, rightClickDown, middleClickDown;

        float angle;
        float distance;
        public LayerMask shieldBashLayer;
        RaycastHit2D[] shieldBashResults;

        public int selectedAttack;
        public List<ShieldAttackData> leftClickAttacks = new();
        Vector3 mouseWorldPos;
        public void ManagedStart()
        {
            GameObject shieldParentObject = new GameObject("ShieldParent");
            shieldParent = shieldParentObject.transform;

            GameObject shieldObject = Instantiate(shieldBasePrefab, shieldParent);
            shield = shieldObject.AddComponent<Shield>();
            shield.highForceColor = shieldHighForceColor;
            shield.defaultColor = shieldDefaultColor;
            shield.layerMask = shieldLayerMask;

            GameObject shieldThrowObject = Instantiate(shieldBasePrefab);
            Rigidbody2D shieldThrowRb = shieldThrowObject.AddComponent<Rigidbody2D>();
            shieldThrowRb.angularDrag = 0f;
            shieldThrowRb.drag = 0f;
            shieldThrowRb.gravityScale = 0f;
            shieldThrowObject.AddComponent<VelocitySystem>();
            shieldThrow = shieldThrowObject.AddComponent<ShieldThrow>();
            shieldThrow.shieldThrown = shieldThrown;
            shieldThrow.shieldRecall = shieldRecall;
            shieldThrow.highForceColor = shieldHighForceColor;
            shieldThrow.defaultColor = shieldDefaultColor;
            shieldThrow.hittableLayers = shieldThrowLayerMask;

            InputManager.RegisterMouseDownCallback(
                    (Vector3Int mouseDownInput) =>
                    {
                        if (mouseDownInput.x > 0)
                        {
                            if (!shieldThrow.thrown && player.aiming)
                            {
                                shieldThrow.Throw(shieldThrowForce);
                            }
                            else if (shieldThrow.thrown)
                            {
                                shieldThrow.Recall(player.transform);
                            }
                        }
                    }
                );

            InputManager.RegisterMouseInputCallback(HandleMouseInput);
            InputManager.RegisterMouseClickCallback(HandleMouseClick);
            InputManager.RegisterMouseDownCallback(HandleMouseDown);
            shieldParent.position = player.transform.position;
        }

        public void RegisterPlayer(Player player)
        {
            this.player = player;
        }
        public void ManagedUpdate()
        {
            if (shieldThrow.thrown)
            {
                shieldParent.gameObject.SetActive(false);
                return;
            }

            if (player.aiming)
            {
                shieldParent.gameObject.SetActive(false);
                shieldThrow.gameObject.SetActive(true);
                shieldThrow.WhileNotThrown(player.transform.position, pointOnCircle);
                return;
            }
            else
            {
                shieldParent.gameObject.SetActive(true);
                shieldThrow.gameObject.SetActive(false);
            }
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

            if (shield.hitForce > 0.1f)
            {
                angle = Vector2.Angle(Vector2.right, shield.transform.right);
                distance = Vector2.Distance(player.transform.position, shield.transform.position) + shield.boxCollider.size.x;
                shieldBashResults = Physics2D.BoxCastAll(player.transform.position, shield.boxCollider.size, angle, shield.transform.right, distance, shieldBashLayer);

                if (shieldBashResults != null && shieldBashResults.Length > 0)
                {
                    foreach (RaycastHit2D hit in shieldBashResults)
                    {
                        //Vector2 shiftedHit = hit.point;
                        //shiftedHit.y = Mathf.Max(shiftedHit.y, hit.collider.transform.position.y);

                        //Vector2 force = (shiftedHit - (Vector2)player.transform.position) * shield.hitForce;
                        //Vector2 force = (hit.point - (Vector2)player.transform.position).normalized * shield.hitForce;
                        Vector2 force = (mouseWorldPos - player.transform.position).normalized * shield.hitForce;

                        IHitReceiver hitReceiver = hit.collider.GetComponent<IHitReceiver>();
                        hitReceiver.ApplyForce(force);
                    }
                    shield.ScaleLerp(Vector2.one * 1.45f, 3f);
                }
            }
            else
            {
                shieldBashResults = null;
            }
        }
        void HandleMouseInput(Vector2 mouseWorldPos)
        {
            this.mouseWorldPos = mouseWorldPos;
            pointOnCircle = distanceFromPlayer * ((Vector3)mouseWorldPos - player.transform.position).normalized;
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

            if (shieldBashResults != null && shieldBashResults.Length > 0)
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
            if (shieldBashResults != null && shieldBashResults.Length > 0)
            {
                foreach (RaycastHit2D hit in shieldBashResults)
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

        public void PlayerDied()
        {
        }
    }
}