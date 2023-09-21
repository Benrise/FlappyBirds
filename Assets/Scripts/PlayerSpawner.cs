using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject _playerBirdPrefab;

    private void Awake()
    {
        if (PlayerConfigurationManager.Instance != null)
        {
            foreach (var playerConfig in PlayerConfigurationManager.Instance.playerConfigs)
            {
                var scheme = "KB" + (playerConfig.PlayerIndex + 1);      
                if (playerConfig.PlayerIndex != 3) {
                    var player = PlayerInput.Instantiate(
                        _playerBirdPrefab, 
                        controlScheme: scheme,
                        pairWithDevices: Keyboard.current
                    ); 

                    float randomX = Random.Range(-30f, -28f);

                    player.transform.position = new Vector3(randomX, player.transform.position.y, player.transform.position.z);

                    player.GetComponent<SpriteRenderer>().sprite = playerConfig.PlayerBirdSprite;
                    player.GetComponent<Animator>().runtimeAnimatorController = playerConfig.PlayerBirdSpriteAnimation;
                }
                else {
                    var player = PlayerInput.Instantiate(
                        _playerBirdPrefab, 
                        controlScheme: scheme,
                        pairWithDevices: Mouse.current
                    ); 

                    float randomX = Random.Range(-30f, -28f);

                    player.transform.position = new Vector3(randomX, player.transform.position.y, player.transform.position.z);

                    player.GetComponent<SpriteRenderer>().sprite = playerConfig.PlayerBirdSprite;
                    player.GetComponent<Animator>().runtimeAnimatorController = playerConfig.PlayerBirdSpriteAnimation;
                }
            }
        }
    }


    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player " + pi.playerIndex + " joined, using " + pi.currentControlScheme);
    }
}