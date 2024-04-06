using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform playerSpawn;
    public GameObject playerPrefab;
    static Player player;
    Manager[] managers;
    void Start()
    {
        GameObject playerObject = Instantiate(playerPrefab, playerSpawn);
        player = playerObject.GetComponent<Player>();
        player.transform.localPosition = Vector3.zero;
        player.RegisterOnDeathCallback(PlayerDeathHandler);
        managers = GetComponents<Manager>();
        foreach (Manager manager in managers)
        {
            manager.RegisterPlayer(player);
            manager.ManagedStart();
        }

        ShieldManager shieldManager = GetComponent<ShieldManager>();
        shieldManager.InitializeShieldManager(player);
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
            manager.PlayerDied();
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
    public void PlayerDied();
}