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

    [SerializeField]
    private Transform _spawnPointsContainer;

    private List<Transform> _spawnPoints = new List<Transform>();

    private void Awake()
    {

        _spawnPoints = _spawnPointsContainer.GetComponentsInChildren<Transform>().ToList();

        _spawnPoints.RemoveAt(0);

        if (PlayerConfigurationManager.Instance != null)
        {
            foreach (var playerConfig in PlayerConfigurationManager.Instance.playerConfigs)
            {
                var scheme = "KB" + (playerConfig.PlayerIndex + 1);     

                Transform spawnPoint = GetRandomAvailableSpawnPoint();

                if (spawnPoint != null){
                    var player = PlayerInput.Instantiate(
                    _playerBirdPrefab, 
                    controlScheme: scheme,
                    pairWithDevices: playerConfig.PlayerIndex != 3 ? Keyboard.current : Mouse.current
                    ); 

                    MarkSpawnPointAsOccupied(spawnPoint);

                    player.transform.position = spawnPoint.position;

                    player.GetComponent<SpriteRenderer>().sprite = playerConfig.PlayerBirdSprite;
                    player.GetComponent<Animator>().runtimeAnimatorController = playerConfig.PlayerBirdSpriteAnimation;
                }

            }
        }
    }

    private Transform GetRandomAvailableSpawnPoint()
    {
        List<Transform> availableSpawnPoints = _spawnPoints.Where(point => !point.GetComponent<SpawnPoint>().IsOccupied).ToList();

        if (availableSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            return availableSpawnPoints[randomIndex];
        }

        return null;
    }

    private void MarkSpawnPointAsOccupied(Transform spawnPoint)
    {
        spawnPoint.GetComponent<SpawnPoint>().IsOccupied = true;
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player " + pi.playerIndex + " joined, using " + pi.currentControlScheme);
    }
}