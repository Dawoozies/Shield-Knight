using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class WeaponSystem : MonoBehaviour
{
    [Serializable]
    public class WeaponDriver
    {
        public Vector2 pos;
        Vector2 startPos;
        Vector2 endPos;
        public float t = 0;
        public AnimationCurve lerpCurve;
        public float lerpCurveTime;
        float lerpCurveTimeMin => lerpCurve.keys[0].time;
        float lerpCurveTimeMax => lerpCurve.keys[lerpCurve.keys.Length - 1].time;
        public bool inProgress;
        public bool completed;
        public Func<bool, bool> endCondition; 
        public List<Action> onEndDriverActions = new();
        public List<Action> onStartDriverActions = new();
        public float speedMul;
        public void SetPositions(Vector2 startPos, Vector2 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
        }
        public void SetEndPosition(Vector2 endPos)
        {
            this.endPos = endPos;
        }
        public void DriverUpdate(float timeDelta)
        {
            completed = lerpCurveTime >= lerpCurveTimeMax;

            if (endCondition(completed))
            {
                EndDriver();
            }

            if (!inProgress)
            {
                return;
            }
            pos = Vector2.LerpUnclamped(startPos, endPos, t);


            lerpCurveTime += timeDelta * speedMul;
            lerpCurveTime = Mathf.Clamp(lerpCurveTime, lerpCurveTimeMin, lerpCurveTimeMax);

            t = lerpCurve.Evaluate(lerpCurveTime);

        }
        public void EndDriver()
        {
            inProgress = false;
            t = lerpCurveTimeMin;
            lerpCurveTime = 0f;
            completed = false;
            speedMul = 1f;
            foreach (var action in onEndDriverActions)
            {
                action();
            }
        }
        public void RegisterOnEndCallback(Action a)
        {
            onEndDriverActions.Add(a);
        }
        public void RegisterEndCondition(Func<bool, bool> f)
        {
            endCondition = f;
        }
        public void StartDriver()
        {
            inProgress = true;
            foreach (var action in onStartDriverActions)
            {
                action();
            }
        }
        public void ForceStopDriver()
        {
            inProgress = false;
            t = lerpCurveTimeMin;
            lerpCurveTime = 0f;
            completed = false;
            speedMul = 1f;
        }
    }
    public enum ActiveTransform
    {
        Held, Thrown, Embedded
    }
    public ActiveTransform activeTransform;
    public Transform[] weaponTransforms = new Transform[3];
    public Transform owner;
    Vector2 lookAtPos;
    [Range(0f, 2f)]
    public float holdDistance;

    bool leftClickDown, rightClickDown;
    //held charge state
    public float chargeShakeAmplitude;
    public float chargeShakeSpeed;
    public WeaponDriver chargeDriver;
    //held attack state
    public float heldAttackRange;
    public WeaponDriver attackDriver;


    //Thrown
    public LayerMask enemyLayerMask;
    public LayerMask embeddableLayerMask;
    public float throwDistance;
    public WeaponDriver throwDriver;
    List<RaycastHit2D> enemyHitsAlongThrowPath = new();
    Vector2 throwDirection;
    Vector2 castPos;
    Vector2 throwPathEnd;
    public WeaponDriver recallDriver;
    Transform embedTransform;
    void Start()
    {
        embedTransform = new GameObject("embedTransform").transform;
        InputManager.RegisterMouseInputCallback((Vector2 mousePos) => lookAtPos = mousePos);
        InputManager.RegisterMouseClickCallback(MouseClickHandler);
        InputManager.RegisterMouseDownCallback(MouseDownHandler);

        chargeDriver.RegisterEndCondition(
                (bool driverCompleted) =>
                {
                    return driverCompleted && !leftClickDown;
                }
            );
        chargeDriver.RegisterOnEndCallback(
                () => 
                {
                    //Play attack
                    attackDriver.StartDriver();
                }
            );

        attackDriver.RegisterEndCondition((bool driverCompleted) => driverCompleted);
        throwDriver.RegisterEndCondition((bool driverCompleted) => driverCompleted);
        throwDriver.RegisterOnEndCallback(
                () => 
                {
                    if(enemyHitsAlongThrowPath != null && enemyHitsAlongThrowPath.Count > 0)
                    {
                        Transform thrown = weaponTransforms[(int)ActiveTransform.Thrown];
                        //we have been tracking the 0th enemy
                        EffectsManager.ins.RequestBloodFX(enemyHitsAlongThrowPath[0].point);
                        EffectsManager.ins.RequestHitStunFX(enemyHitsAlongThrowPath[0].collider);
                        enemyHitsAlongThrowPath.RemoveAt(0);
                        EffectsManager.ins.RequestTimeStop(0.05f);
                        if (enemyHitsAlongThrowPath.Count > 0)
                        {
                            Vector2 start = thrown.position;
                            Vector2 end = castPos + throwDirection * enemyHitsAlongThrowPath[0].distance;
                            throwDriver.ForceStopDriver();

                            float travelDist = Vector2.Distance(start, end);
                            if (travelDist < thrown.localScale.x * 3f)
                            {
                                throwDriver.speedMul = 100f;
                            }
                            if (travelDist < thrown.localScale.x)
                            {
                                throwDriver.speedMul = 10f;
                            }

                            throwDriver.SetPositions(start, end);
                            throwDriver.StartDriver();
                        }
                        else
                        {
                            //then all enemies have been completed
                            Vector2 start = thrown.position;
                            Vector2 end = throwPathEnd;
                            throwDriver.ForceStopDriver();

                            float travelDist = Vector2.Distance(start, end);
                            if (travelDist < thrown.localScale.x * 3f)
                            {
                                throwDriver.speedMul = 100f;
                            }
                            if (travelDist < thrown.localScale.x)
                            {
                                throwDriver.speedMul = 1000f;
                            }

                            throwDriver.SetPositions(start, end);
                            throwDriver.StartDriver();

                            //Then do similar cast
                            RaycastHit2D secondaryEmbedCast = embeddingCast(thrown);
                            if(secondaryEmbedCast.collider != null)
                            {
                                embedTransform.parent = secondaryEmbedCast.collider.transform;
                                embedTransform.position = secondaryEmbedCast.point - throwDirection * thrown.localScale.x/2f;
                            }
                        }
                    }
                    else
                    {
                        activeTransform = ActiveTransform.Embedded;
                    }
                }
            );

        recallDriver.RegisterEndCondition((bool driverCompleted) => driverCompleted);
        recallDriver.RegisterOnEndCallback(
                () => 
                {
                    activeTransform = ActiveTransform.Held;
                }
            );
    }
    RaycastHit2D embeddingCast(Transform castingTransform)
    {
        return Physics2D.BoxCast(castPos, (Vector2)castingTransform.localScale, Vector2.Angle(Vector2.right, throwDirection), throwDirection, throwDistance, embeddableLayerMask);
    }
    void MouseDownHandler(Vector3Int input)
    {
        if(input.x > 0f)
        {
            if(activeTransform == ActiveTransform.Held)
            {
                if (!chargeDriver.inProgress && !attackDriver.inProgress)
                {
                    chargeDriver.StartDriver();
                }
            }
            if (activeTransform == ActiveTransform.Thrown)
            {
                if (!throwDriver.inProgress)
                {
                    Transform thrown = weaponTransforms[(int)ActiveTransform.Thrown];
                    Transform embedded = weaponTransforms[(int)ActiveTransform.Embedded];
                    throwDirection = (lookAtPos - (Vector2)owner.position).normalized;
                    thrown.right = throwDirection;
                    Vector2 shift = throwDirection * embedded.localScale.x/2f;
                    castPos = (Vector2)owner.position + shift;

                    throwPathEnd = castPos + throwDirection * throwDistance;
                    Vector2 firstPoint = throwPathEnd;

                    RaycastHit2D embeddingHit = embeddingCast(embedded);
                    if (embeddingHit.collider != null)
                    {
                        embedded.transform.right = throwDirection;
                        throwPathEnd = castPos + throwDirection * (embeddingHit.distance);
                        firstPoint = throwPathEnd;

                        embedded.transform.position = throwPathEnd;

                        embedTransform.parent = embeddingHit.collider.transform;
                        embedTransform.position = embeddingHit.point - throwDirection * thrown.localScale.x / 2f;
                    }

                    RaycastHit2D[] enemyHits = Physics2D.BoxCastAll(castPos, (Vector2)embedded.localScale, Vector2.Angle(Vector2.right, throwDirection), throwDirection, throwDistance, enemyLayerMask);
                    enemyHitsAlongThrowPath.Clear();
                    if(enemyHits != null && enemyHits.Length > 0)
                    {
                        for (int i = 0; i < enemyHits.Length; i++)
                        {
                            RaycastHit2D enemyHit = enemyHits[i];
                            if(embeddingHit.collider != null)
                            {
                                if(enemyHit.distance > embeddingHit.distance)
                                {
                                    continue;
                                }
                            }
                            if(i==0)
                            {
                                firstPoint = castPos + throwDirection * enemyHit.distance;
                            }
                            enemyHitsAlongThrowPath.Add(enemyHit);
                        }
                    }

                    float travelDist = Vector2.Distance(owner.position, firstPoint);

                    if (travelDist < thrown.localScale.x * 3f)
                    {
                        throwDriver.speedMul = 100f;
                    }
                    if (travelDist < thrown.localScale.x)
                    {
                        throwDriver.speedMul = 1000f;
                    }

                    throwDriver.SetPositions(owner.position, firstPoint);
                    throwDriver.StartDriver();
                }
            }
            if(activeTransform == ActiveTransform.Embedded)
            {
                if(!throwDriver.inProgress && !recallDriver.inProgress)
                {
                    Transform embedded = weaponTransforms[(int)ActiveTransform.Embedded];
                    recallDriver.SetPositions(embedded.position, owner.position);
                    recallDriver.StartDriver();
                }
            }
        }
    }
    void MouseClickHandler(Vector3 input)
    {
        leftClickDown = input.x > 0f;
        rightClickDown = input.y > 0f;
        if (rightClickDown && activeTransform == ActiveTransform.Held)
        {
            activeTransform = ActiveTransform.Thrown;
        }
        if(!rightClickDown && activeTransform == ActiveTransform.Thrown && !throwDriver.inProgress)
        {
            activeTransform = ActiveTransform.Held;
        }
    }
    void Update()
    {
        for (int i = 0; i < weaponTransforms.Length; i++)
        {
            if (i != (int)activeTransform)
            {
                weaponTransforms[i].gameObject.SetActive(false);
            }
            else
            {
                weaponTransforms[i].gameObject.SetActive(true);
            }
        }
        if(owner == null)
        {
            owner = FindObjectOfType<Player>().transform;
            return;
        }
        HeldUpdate();
        ThrowUpdate();
        EmbeddedUpdate();
    }
    void HeldUpdate()
    {
        Transform held = weaponTransforms[(int)ActiveTransform.Held];
        if(!held.gameObject.activeSelf)
        {
            chargeDriver.ForceStopDriver();
            attackDriver.ForceStopDriver();
            return;
        }
        held.right = lookAtPos - (Vector2)owner.position;
        held.position = (Vector2)owner.position + (Vector2)(held.right * holdDistance);
        if (!attackDriver.inProgress)
        {
            
        }

        if (chargeDriver.inProgress)
        {
            Vector2 start = (Vector2)owner.position;
            Vector2 end = (Vector2)owner.position + (Vector2)(-held.right * (holdDistance + chargeShakeAmplitude * Mathf.Sin(Time.time * chargeShakeSpeed)));
            chargeDriver.SetPositions(start, end);
            held.position = chargeDriver.pos;
        }
        chargeDriver.DriverUpdate(Time.deltaTime);

        if (attackDriver.inProgress)
        {
            Vector2 start = (Vector2)owner.position;
            Vector2 end = (Vector2)owner.position + (Vector2)(held.right * heldAttackRange);
            attackDriver.SetPositions(start, end);
            
            RaycastHit2D attackHit = Physics2D.BoxCast(owner.position, (Vector2)held.localScale, Vector2.Angle(Vector2.right, held.right), held.right, heldAttackRange, embeddableLayerMask);
            Vector3 heldPos = attackDriver.pos;
            if (attackHit.collider != null)
            {
                float distToHeldPos = Vector2.Distance(owner.position, heldPos);
                float distToCentroid = Vector2.Distance(owner.position, attackHit.centroid);
                if (distToCentroid < distToHeldPos)
                {
                    heldPos = (Vector2)owner.position + (Vector2)(held.right * distToCentroid);
                }
            }
            held.position = heldPos;
        }
        attackDriver.DriverUpdate(Time.deltaTime);
    }
    void ThrowUpdate()
    {
        Transform thrown = weaponTransforms[(int)ActiveTransform.Thrown];
        if(!thrown.gameObject.activeSelf)
        {
            throwDriver.ForceStopDriver();
            return;
        }

        if(!throwDriver.inProgress)
        {
            thrown.right = lookAtPos - (Vector2)owner.position;
            thrown.position = (Vector2)owner.position;
        }

        if (throwDriver.inProgress)
        {
            thrown.position = throwDriver.pos;

        }
        throwDriver.DriverUpdate(Time.deltaTime);
    }
    void EmbeddedUpdate()
    {
        Transform embedded = weaponTransforms[(int)ActiveTransform.Embedded];
        if(!embedded.gameObject.activeSelf)
        {
            recallDriver.ForceStopDriver();

            return;
        }
        
        if(recallDriver.inProgress)
        {
            recallDriver.SetEndPosition(owner.position);
            embedded.position = recallDriver.pos;
        }
        else
        {
            if (embedTransform != null)
            {
                embedded.position = embedTransform.position;
            }
        }
        recallDriver.DriverUpdate(Time.deltaTime);
    }

    public bool isRecalling()
    {
        return recallDriver.inProgress;
    }
}