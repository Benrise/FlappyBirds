using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerConfigurationManager : MonoBehaviour
{

    GameObject playerPrefab;

    private List<PlayerConfiguration> playerConfigs;
    
    public static PlayerConfigurationManager Instance { get; private set; }
    
    private int maxPlayers;
    
    private void Awake(){

        if(Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
            maxPlayers = GetComponent<PlayerInputManager>().maxPlayerCount;
            PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

            if (playerInputManager != null){
                playerPrefab = playerInputManager.playerPrefab;
            }
        }
    }

     private void Start(){

        //Здесь нужно циклом проходиться
        var p1 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "KB1", pairWithDevices: Keyboard.current);
        var p2 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "KB2", pairWithDevices: Keyboard.current);
        var p3 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "KB3", pairWithDevices: Keyboard.current);
        var p4 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "KB4", pairWithDevices: Mouse.current);
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player " + pi.playerIndex + " joined, using " + pi.currentControlScheme);

        if(!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            playerConfigs.Add(new PlayerConfiguration(pi));
        }
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].isReady = true;
        if (playerConfigs.Count == maxPlayers && playerConfigs.All(p => p.isReady == true))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }

    public PlayerInput Input { get; private set; }
    public int PlayerIndex { get; private set; }
    public bool isReady { get; set; }
    public Sprite Bird { get; set; }
}
