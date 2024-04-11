using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform playerSpawn;
    public GameObject playerPrefab;
    static Player player;
    Manager[] managers;
    void Awake()
    {
        GameObject playerObject = Instantiate(playerPrefab, playerSpawn);
        player = playerObject.GetComponent<Player>();
        player.transform.localPosition = Vector3.zero;
        player.RegisterOnDeathCallback(PlayerDeathHandler);
        player.RegisterOnDeathCompleteCallback(PlayerDeathCompleteHandler);
        managers = GetComponents<Manager>();
        foreach (Manager manager in managers)
        {
            manager.RegisterPlayer(player);
            manager.ManagedStart();
        }

        ShieldManager shieldManager = GetComponent<ShieldManager>();
        if (shieldManager != null)
        {
            shieldManager.InitializeShieldManager(player);
        }
    }
    void Update()
    {
        foreach (Manager manager in managers)
        {
            manager.ManagedUpdate();
        }
    }
    void PlayerDeathHandler()
    {
        foreach(Manager manager in managers)
        {
            manager.PlayerDeath();
        }
    }

    void PlayerDeathCompleteHandler()
    {
        foreach (Manager manager in managers)
        {
            manager.PlayerDeathComplete();
        }
    }
    public static Player GetActivePlayer()
    {
        return player;
    }
}
public interface Manager
{
    public void ManagedStart();
    public void ManagedUpdate();
    public void RegisterPlayer(Player player);
    public void PlayerDeathComplete();
    public void PlayerDeath();
}