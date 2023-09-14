using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerSpawner : MonoBehaviour
{

    private void Awake()
    {
        if (PlayerConfigurationManager.Instance != null)
        {
            foreach (var playerConfig in PlayerConfigurationManager.Instance.playerConfigs)
            {
                var prefab = playerConfig.PlayerBirdPrefab;
                var scheme = "KB" + (playerConfig.PlayerIndex + 1); 
                
                if (playerConfig.PlayerIndex != 3) {
                    PlayerInput.Instantiate(
                    prefab, 
                    controlScheme: scheme,
                    pairWithDevices: Keyboard.current
                    ); 
                }
                else {
                    PlayerInput.Instantiate(
                    prefab, 
                    controlScheme: scheme,
                    pairWithDevices: Mouse.current
                    ); 
                }
            }
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player " + pi.playerIndex + " joined, using " + pi.currentControlScheme);
    }
}