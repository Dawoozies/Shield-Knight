using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform playerSpawn;
    public GameObject playerPrefab;
    Player player;
    Manager[] managers;
    void Start()
    {
        GameObject playerObject = Instantiate(playerPrefab, playerSpawn);
        player = playerObject.GetComponent<Player>();
        managers = GetComponents<Manager>();
        foreach (Manager manager in managers)
        {
            manager.RegisterPlayer(player);
            manager.ManagedStart();
        }
    }
    void Update()
    {
        
    }
}
public interface Manager
{
    public void ManagedStart();
    public void RegisterPlayer(Player player);
}