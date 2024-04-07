using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShieldManager : MonoBehaviour
{
    bool _initialized;
    HeldSystem _heldSystem;
    ThrowSystem _throwSystem;
    RecallSystem _recallSystem;
    public GameObject heldPrefab, throwPrefab, recallPrefab;
    Player player;
    ShieldSystemType activeShieldSystem;
    Vector3 mouseClickInput;
    bool _LMB => mouseClickInput.x > 0;
    bool _RMB => mouseClickInput.y > 0;
    bool _MMB => mouseClickInput.z > 0;
    Vector3Int mouseDownInput;
    bool _LMB_DOWN => mouseDownInput.x > 0;
    bool _RMB_DOWN => mouseDownInput.y > 0;
    bool _MMB_DOWN => mouseDownInput.z > 0;
    Vector3Int mouseUpInput;
    bool _LMB_UP => mouseUpInput.x > 0;
    bool shieldThrown, shieldRecalled;
    Transform embeddingTransform;
    Vector2 mousePos;
    public Vector3 throwOffset;
    static List<Action> onShieldMovingInAir = new();
    static List<Action> onShieldMovingInAirCompleted = new();
    public float heldDistance;
    public void InitializeShieldManager(Player player)
    {
        GameObject heldObj = Instantiate(heldPrefab);
        _heldSystem = heldObj.GetComponent<HeldSystem>();
        GameObject throwObj = Instantiate(throwPrefab);
        _throwSystem = throwObj.GetComponent<ThrowSystem>();
        GameObject recallObj = Instantiate(recallPrefab);
        _recallSystem = recallObj.GetComponent<RecallSystem>();

        embeddingTransform = new GameObject("ShieldThrowEmbeddingTransform").transform;
        _throwSystem.earlyStopCallback = ThrowEarlyStopHandler;

        this.player = player;
        InputManager.RegisterMouseInputCallback((Vector2 mousePos) => this.mousePos = mousePos);
        InputManager.RegisterMouseClickCallback((Vector3 mouseClickInput) => this.mouseClickInput = mouseClickInput);
        InputManager.RegisterMouseDownCallback((Vector3Int mouseDownInput) => this.mouseDownInput = mouseDownInput);
        InputManager.RegisterMouseUpCallback((Vector3Int mouseUpInput) => this.mouseUpInput = mouseUpInput);
        _initialized = true;
    }
    private void Update()
    {
        if (!_initialized) { return; }

        switch (activeShieldSystem)
        {
            case ShieldSystemType.Held:
                _heldSystem.gameObject.SetActive(true);
                _throwSystem.gameObject.SetActive(false);
                _recallSystem.gameObject.SetActive(false);
                break;
            case ShieldSystemType.Throw:
                _heldSystem.gameObject.SetActive(false);
                _throwSystem.gameObject.SetActive(true);
                _recallSystem.gameObject.SetActive(false);
                break;
            case ShieldSystemType.Recall:
                _heldSystem.gameObject.SetActive(false);
                _throwSystem.gameObject.SetActive(false);
                _recallSystem.gameObject.SetActive(true);
                break;
        }

        if(_heldSystem.gameObject.activeSelf)
        {
            DoHeldAiming();
            if (_RMB)
            {
                activeShieldSystem = ShieldSystemType.Throw;
            }
        }
        if(_throwSystem.gameObject.activeSelf)
        {
            if (!shieldThrown)
            {
                DoAiming();
                _throwSystem.SetColliderActive(false);
                if (!_RMB)
                {
                    activeShieldSystem = ShieldSystemType.Held;
                }
                if (_LMB)
                {
                    embeddingTransform.parent = null;
                    _throwSystem.SystemCast();
                    shieldThrown = true;
                    foreach (var action in onShieldMovingInAir)
                    {
                        action();
                    }
                }
            }
            else
            {
                if (_LMB_DOWN)
                {
                    //_throwSystem.transform.position = player.transform.position;
                    activeShieldSystem = ShieldSystemType.Recall;
                    _recallSystem.SetUpRecallSystem(_throwSystem.transform.position);
                    shieldThrown = false;
                }
                else
                {
                    if (_throwSystem.SystemComplete())
                    {
                        foreach (var action in onShieldMovingInAirCompleted)
                        {
                            action();
                        }
                    }
                    if (embeddingTransform.parent != null)
                    {
                        _throwSystem.SetColliderActive(true);
                        _throwSystem.transform.position = embeddingTransform.position;
                    }
                }
            }
        }
        if(_recallSystem.gameObject.activeSelf)
        {
            if (!_recallSystem.systemActivatedPreviously)
            {
                //if not activated previously last frame we must have started the recall
                _recallSystem.ActivateRecall();
            }
            if(_recallSystem.ShieldCaught())
            {
                //Then finally we can go to holding the shield again
                activeShieldSystem = ShieldSystemType.Held;
            }
        }
    }
    void ThrowEarlyStopHandler(Vector3 throwPos, Collider2D col)
    {
        embeddingTransform.parent = col.transform;
        embeddingTransform.position = throwPos;
    }
    void DoAiming()
    {
        _throwSystem.transform.position = player.transform.position;
        _throwSystem.transform.right = mousePos - (Vector2)_throwSystem.transform.position;
    }
    public static void RegisterOnShieldMovingInAirCallback(Action a)
    {
        onShieldMovingInAir.Add(a);
    }
    public static void RegisterOnShieldMovingInAirCompleteCallback(Action a)
    {
        onShieldMovingInAirCompleted.Add(a);
    }
    void DoHeldAiming()
    {
        _heldSystem.transform.right = mousePos - (Vector2)player.transform.position;
        _heldSystem.transform.position = player.transform.position + _heldSystem.transform.right * heldDistance;
    }
}