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
                if (_RMB)
                {
                    activeShieldSystem = ShieldSystemType.Throw;
                }
                if(!_RMB)
                {
                    DoHeldAiming();
                }
                break;
            case ShieldSystemType.Throw:
                _heldSystem.gameObject.SetActive(false);
                _throwSystem.gameObject.SetActive(true);
                _recallSystem.gameObject.SetActive(false);
                if(!shieldThrown)
                {
                    DoAiming();
                    _throwSystem.SetColliderActive(false);
                    if (!_RMB)
                    {
                        activeShieldSystem = ShieldSystemType.Held;
                        break;
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
                    if(_LMB_DOWN)
                    {
                        //_throwSystem.transform.position = player.transform.position;
                        activeShieldSystem = ShieldSystemType.Recall;
                        shieldThrown = false;
                    }
                    else
                    {
                        if(_throwSystem.SystemComplete())
                        {
                            foreach (var action in onShieldMovingInAirCompleted)
                            {
                                action();
                            }
                        }
                        if(embeddingTransform.parent != null)
                        {
                            _throwSystem.SetColliderActive(true);
                            _throwSystem.transform.position = embeddingTransform.position;
                        }
                    }
                }
                break;
            case ShieldSystemType.Recall:
                _heldSystem.gameObject.SetActive(false);
                _throwSystem.gameObject.SetActive(false);
                _recallSystem.gameObject.SetActive(true);

                if(!shieldRecalled)
                {
                    _recallSystem.transform.position = _throwSystem.transform.position;
                    _recallSystem.transform.right = -_throwSystem.transform.right;
                    _recallSystem.SystemCast();
                    _recallSystem.UpdateRecallParameters(player.transform.position);
                    shieldRecalled = true;
                    foreach (var action in onShieldMovingInAir)
                    {
                        action();
                    }
                }
                else
                {
                    _recallSystem.UpdateRecallParameters(player.transform.position);
                    if (_recallSystem.SystemComplete())
                    {
                        foreach (var action in onShieldMovingInAirCompleted)
                        {
                            action();
                        }
                        activeShieldSystem = ShieldSystemType.Held;
                        shieldRecalled = false;
                    }
                }
                break;
        }
    }
    void ThrowEarlyStopHandler(Vector3 throwPos, Collider2D col)
    {
        embeddingTransform.parent = col.transform;
        embeddingTransform.position = throwPos;
    }
    void DoAiming()
    {
        _throwSystem.transform.position = player.transform.position + throwOffset;
        _throwSystem.transform.right = mousePos - (Vector2)player.transform.position;
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